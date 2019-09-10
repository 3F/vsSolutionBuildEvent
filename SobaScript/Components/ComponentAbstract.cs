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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;

namespace net.r_eg.SobaScript.Components
{
    public abstract class ComponentAbstract: IComponent
    {
        /// <summary>
        /// Activated SobaScript engine.
        /// </summary>
        protected ISobaScript soba;

        /// <summary>
        /// Activated E-MSBuild engine.
        /// </summary>
        protected IEvMSBuild emsbuild;

        /// <summary>
        /// Container of user-variables through Varhead.
        /// </summary>
        protected IUVars uvars;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public abstract string Activator { get; }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public abstract string Eval(string data);

        /// <summary>
        /// An activation status of this component.
        /// </summary>
        public virtual bool Enabled
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Allows post-processing with MSBuild core.
        /// Some components may require immediate processing with evaluation before passing control to the next level.
        /// </summary>
        public virtual bool PostProcessingMSBuild
        {
            get;
            set;
        }

        /// <summary>
        /// Will be located before deepening if true.
        /// </summary>
        public virtual bool BeforeDeepening
        {
            get;
            protected set;
        }

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        public virtual bool PostParse
        {
            get;
            protected set;
        }

        /// <summary>
        /// Using regex engine in {Activator}.
        /// </summary>
        public virtual bool ARegex
        {
            get;
            protected set;
        }

        /// <param name="soba">Used SobaScript engine.</param>
        public ComponentAbstract(ISobaScript soba)
            : this(soba, soba?.EvMSBuild, soba?.UVars)
        {

        }

        /// <param name="soba">Used SobaScript engine.</param>
        /// <param name="evmaker">Custom maker of the E-MSBuild engine.</param>
        /// <param name="uvars">Varhead container.</param>
        public ComponentAbstract(ISobaScript soba, IEvMSBuildMaker evmaker, IUVars uvars)
            : this(soba, evmaker?.MakeEvaluator(uvars), uvars)
        {

        }

        /// <param name="soba">Used SobaScript engine.</param>
        /// <param name="evm">E-MSBuild engine.</param>
        /// <param name="uvars">Varhead container.</param>
        public ComponentAbstract(ISobaScript soba, IEvMSBuild evm, IUVars uvars)
            : this()
        {
            this.soba       = soba ?? throw new ArgumentNullException(nameof(soba));
            this.emsbuild   = evm ?? throw new ArgumentNullException(nameof(evm));
            this.uvars      = uvars ?? throw new ArgumentNullException(nameof(uvars));
        }

        public ComponentAbstract()
        {
            LSender.Send(this, $"init: '{GetType().FullName}'", MsgLevel.Trace);
        }

        /// <summary>
        /// Default entry point to start analysis.
        /// </summary>
        /// <param name="data">Raw data.</param>
        /// <param name="opt">Additional options to engine.</param>
        /// <returns></returns>
        protected virtual KeyValuePair<string, string> EntryPoint(string data, RegexOptions opt = RegexOptions.None)
        {
            Match m = Regex.Match
            (
                data, 
                string.Format
                (
                    @"^\[{0}
                        \s*
                        (?'request'
                            (?'type'
                                [A-Za-z_0-9]+
                            )
                            .*
                        )
                        \]$", 
                        ARegex ? Activator : Activator.Replace(" ", @"\s")
                ),
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | opt
            );

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed {GetType().FullName} - `{data}`");
            }

            return new KeyValuePair<string, string>(m.Groups["type"].Value, m.Groups["request"].Value);
        }

        protected virtual string Evaluate(string data)
        {
            LSender.Send(this, $"'{Activator}'-evaluate: started with `{data}`", MsgLevel.Trace);

            if(soba != null)
            {
                data = soba.Eval(data);
                LSender.Send(this, $"'{Activator}'-evaluate: evaluated data: `{data}` :: ISBEScript", MsgLevel.Trace);
            }

            if(emsbuild != null)
            {
                //if(PostProcessingMSBuild) {
                    data = emsbuild.Eval(data);
                    LSender.Send(this, $"'{Activator}'-evaluate: evaluated data: `{data}` :: IMSBuild", MsgLevel.Trace);
                //}
            }

            return data;
        }
    }
}
