/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class Script: ISBEScript, IEvaluator
    {
        /// <summary>
        /// Maximum of nesting level
        /// </summary>
        const int DEPTH_LIMIT = 70;

        /// <summary>
        /// Getting instance of used loader.
        /// Default initialization if it still is not used.
        /// </summary>
        public IBootloader Bootloader
        {
            get
            {
                if(bootloader == null)
                {
                    Debug.Assert(env != null);
                    Debug.Assert(uvariable != null);

                    bootloader = new Bootloader(env, uvariable);
                    bootloader.register();
                }
                return bootloader;
            }
        }
        protected IBootloader bootloader;

        /// <summary>
        /// Support of User-variables.
        /// </summary>
        protected IUVars uvariable;

        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

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
        private Object _lock = new Object();

        /// <summary>
        /// Handler of mixed data SBE-Scripts
        /// Format: https://bitbucket.org/3F/vssolutionbuildevent/issue/22/#comment-12739932
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <param name="allowMSBuild">Allows post-processing with MSBuild or not.
        /// Some components can require immediate processing with evaluation, before passing control to next level.
        /// </param>
        /// <returns>prepared and evaluated data</returns>
        public string parse(string data, bool allowMSBuild)
        {
            lock(_lock)
            {
                _depthLevel = 0;
                postMSBuild = allowMSBuild;
                StringHandler hString = new StringHandler();
                return hString.Recovery(parse(hString.protect(data), _depthLevel, hString));
            }
        }

        public string parse(string data)
        {
            return parse(data, false);
        }

        /// <summary>
        /// Evaluates mixed data through some engine like E-MSBuild, SobaScript, etc.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public string Evaluate(string data)
        {
            return parse(data);
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variable</param>
        public Script(IEnvironment env, IUVars uvariable)
        {
            this.env = env;
            this.uvariable = uvariable;
        }

        /// <param name="loader">Initialization with IBootloader</param>
        public Script(IBootloader loader)
        {
            bootloader  = loader;
            env         = loader.Env;
            uvariable   = loader.UVariable;
        }

        /// <param name="data">Mixed data</param>
        /// <param name="level">Nesting level</param>
        /// <param name="hString">Handler of strings if exists</param>
        /// <returns>Prepared and evaluated data</returns>
        protected string parse(string data, int level, StringHandler hString = null)
        {
            if(level >= DEPTH_LIMIT) {
                _depthLevel = 0;
                throw new LimitException("Nesting level of '{0}' reached. Aborted.", DEPTH_LIMIT);
            }
            var rcon = RPattern.Container;

            return rcon.Replace(data, 
                                delegate(Match m)
                                {
                                    string escape   = m.Groups[1].Value;
                                    string raw      = m.Groups[2].Value;

                                    if(escape.Length > 1) {
                                        Log.Trace("SBEScripts-Container: escape `{0}`", (raw.Length > 40)? raw.Substring(0, 40) + "..." : raw);
                                        return "#" + escapeMSBuildData(raw, true);
                                    }

                                    return selector((hString != null)? hString.Recovery(raw) : raw);
                                });
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
            string pattern = String.Format(@"{0}{1}", 
                                            (force)? String.Empty : @"(?<!\$)", 
                                            EvMSBuild.RPattern.ContainerOuter);
            return Regex.Replace(data, pattern, delegate(Match m) { return "$" + m.Value; }, RegexOptions.IgnorePatternWhitespace);
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
                return data.StartsWith(String.Format("[{0}", c.Condition));
            }
            return Regex.IsMatch(data, String.Format("^\\[{0}", c.Condition), RegexOptions.IgnorePatternWhitespace);
        }

        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        protected string selector(string data)
        {
            Log.Trace("Selector: started with `{0}`", data);

            foreach(IComponent c in Bootloader.Components)
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

            foreach(IComponent c in Bootloader.Components)
            {
                if(c.BeforeDeepen) {
                    continue; // should already parsed above
                }

                if(isReadyToParse(data, c)) {
                    return parse(data, c);
                }
            }

            throw new SelectorMismatchException("Selector: cannot find component. {0}/{1} :: `{2}`", 
                                                                    Bootloader.Components.Count(), 
                                                                    Bootloader.Registered.Count(),
                                                                    data);
        }

        protected bool deepen(ref string data)
        {
            return RPattern.Container.IsMatch(data);
        }
    }
}
