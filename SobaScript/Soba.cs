/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.SobaScript
{
    public class Soba: ISobaScript, IEvaluator, ISobaCLoader
    {
        /// <summary>
        /// Maximum of nesting level
        /// </summary>
        const int DEPTH_LIMIT = 70;

        protected ConcurrentDictionary<Type, IComponent> components = new ConcurrentDictionary<Type, IComponent>();

        /// <summary>
        /// Flag of post-processing with MSBuild core.
        /// In general, some components can require immediate processing with evaluation before passing control to next level.
        /// This flag allows processing if needed.
        /// </summary>
        protected bool postMSBuild;

        /// <summary>
        /// Current level of nesting data.
        /// Aborting if reached limit
        /// </summary>
        private volatile int _depthLevel = 0;

        /// <summary>
        /// object synch.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Only enabled components from `Registered`.
        /// </summary>
        public IEnumerable<IComponent> Components
            => components.Where(c => c.Value.Enabled).Select(c => c.Value);

        /// <summary>
        /// All registered components.
        /// </summary>
        public IEnumerable<IComponent> Registered => components.Select(c => c.Value);

        /// <summary>
        /// Used instance of the E-MSBuild engine.
        /// </summary>
        public IEvMSBuild EvMSBuild
        {
            get;
            protected set;
        }

        /// <summary>
        /// Varhead container for user-variables.
        /// </summary>
        public IUVars UVars
        {
            get;
            protected set;
        }

        /// <param name="data">mixed data</param>
        /// <param name="allowMSBuild">Allows post-processing with MSBuild or not.
        /// Some components can require immediate processing with evaluation, before passing control to next level.
        /// </param>
        /// <returns>prepared and evaluated data</returns>
        public string parse(string data, bool allowMSBuild)
        {
            lock(sync)
            {
                _depthLevel = 0;
                postMSBuild = allowMSBuild;
                StringHandler hString = new StringHandler();
                return hString.Recovery(parse(hString.protect(data), _depthLevel, hString));
            }
        }

        public string parse(string data) => parse(data, false);

        /// <summary>
        /// Evaluates mixed data through some engine like E-MSBuild, SobaScript, etc.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public string Evaluate(string data) => parse(data);

        /// <summary>
        /// Get component for specified type.
        /// </summary>
        /// <param name="type">The type of registered component.</param>
        /// <returns>Found instance or null value if this type is not registered.</returns>
        public IComponent GetComponent(Type type)
        {
            if(components.ContainsKey(type)) {
                return components[type];
            }
            return null;
        }

        /// <summary>
        /// To register new component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Success of</returns>
        /// <exception cref="ComponentException"></exception>
        public bool Register(IComponent component)
        {
            if(string.IsNullOrEmpty(component.Condition)) {
                throw new ComponentException($"Invalid component. {nameof(component.Condition)} property is null or empty. `{component.ToString()}`");
            }

            Type ident = component.GetType();

            if(components.ContainsKey(ident)) {
                LSender.Send(this, $"IComponent '{ident}:{component.ToString()}' is already registered.");
                return false;
            }

            components[ident] = component;
            return true;
        }

        /// <summary>
        /// To unregister specific component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool Unregister(IComponent component)
            => components.TryRemove(component.GetType(), out IComponent v);

        /// <summary>
        /// Unregister all available components.
        /// </summary>
        public void Unregister() => components.Clear();

        public Soba()
            : this(new UVars())
        {

        }

        public Soba(IUVars uvars)
            : this(new EvMSBuilder(uvars), uvars)
        {

        }

        public Soba(IEvMSBuildMaker evmaker, IUVars uvars)
            : this(evmaker?.MakeEvaluator(uvars), uvars)
        {

        }

        public Soba(IEvMSBuild evm, IUVars uvars)
        {
            EvMSBuild   = evm ?? throw new ArgumentNullException(nameof(evm));
            UVars       = uvars ?? throw new ArgumentNullException(nameof(uvars));
        }

        /// <param name="data">Mixed data</param>
        /// <param name="level">Nesting level</param>
        /// <param name="hString">Handler of strings if exists</param>
        /// <returns>Prepared and evaluated data</returns>
        protected string parse(string data, int level, StringHandler hString = null)
        {
            if(level >= DEPTH_LIMIT) {
                _depthLevel = 0;
                throw new LimitException($"Nesting level of '{DEPTH_LIMIT}' reached. Aborted.", DEPTH_LIMIT);
            }
            var rcon = RPattern.Container;

            return rcon.Replace
            (
                data, 
                (Match m) =>
                {
                    string escape   = m.Groups[1].Value;
                    string raw      = m.Groups[2].Value;

                    if(escape.Length > 1) {
                        LSender.Send(this, $"SBEScripts-Container: escape `{((raw.Length > 40) ? raw.Substring(0, 40) + "..." : raw)}`");
                        return "#" + escapeMSBuildData(raw, true);
                    }

                    return selector((hString != null)? hString.Recovery(raw) : raw);
                }
            );
        }

        /// <summary>
        /// Parse data for specific component
        /// </summary>
        /// <param name="data">Mixed data</param>
        /// <param name="c">Component</param>
        /// <returns>Prepared + evaluated data by component</returns>
        protected string parse(string data, IComponent c)
        {
            string ret = c.parse(data);

            if(c.PostParse)
            {
                ++_depthLevel;
                ret = parse(ret, _depthLevel);
                --_depthLevel;
            }
            return ret;
        }

        /// <param name="data"></param>
        /// <param name="force">only $(..) -> $$(..) if false / and $$(..) -> $$$(..), etc. if true</param>
        /// <returns></returns>
        protected virtual string escapeMSBuildData(string data, bool force)
        {
            string pattern = string.Format
            (
                @"{0}{1}", 
                force ? string.Empty : @"(?<!\$)",
                net.r_eg.EvMSBuild.RPattern.ContainerOuter
            );
            return Regex.Replace(data, pattern, (Match m) => "$" + m.Value, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Checking ability to parse the data for specific component
        /// </summary>
        /// <param name="data">Mixed data</param>
        /// <param name="c">Component</param>
        /// <returns>ready to parse or not</returns>
        protected bool isReadyToParse(string data, IComponent c)
        {
            if(!c.CRegex) {
                return data.StartsWith(string.Format("[{0}", c.Condition));
            }
            return Regex.IsMatch(data, string.Format("^\\[{0}", c.Condition), RegexOptions.IgnorePatternWhitespace);
        }

        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        protected string selector(string data)
        {
            LSender.Send(this, $"Selector: started with `{data}`", MsgLevel.Trace);

            foreach(IComponent c in Components)
            {
                c.PostProcessingMSBuild = postMSBuild;

                if(!c.BeforeDeepen) {
                    continue;
                }

                if(isReadyToParse(data, c)) {
                    return parse(data, c);
                }
            }

            if(deepen(ref data)) {
                ++_depthLevel;
                data = parse(data, _depthLevel);
                --_depthLevel;
            }

            foreach(IComponent c in Components)
            {
                if(c.BeforeDeepen) {
                    continue; // should already parsed above
                }

                if(isReadyToParse(data, c)) {
                    return parse(data, c);
                }
            }

            throw new MismatchException(
                $"Selector: cannot find component. {Components.Count()}/{Registered.Count()} :: `{data}`"
            );
        }

        protected bool deepen(ref string data)
        {
            return RPattern.Container.IsMatch(data);
        }
    }
}
