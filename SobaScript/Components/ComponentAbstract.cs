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
using net.r_eg.EvMSBuild;
using net.r_eg.Varhead;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Components;

namespace net.r_eg.SobaScript.Components
{
    public abstract class ComponentAbstract: IComponent
    {
        /// <summary>
        /// For evaluation with SBE-Scripts
        /// </summary>
        protected ISobaScript soba;

        /// <summary>
        /// For evaluation with MSBuild
        /// </summary>
        protected IEvMSBuild msbuild;

        /// <summary>
        /// Current container of user-variables
        /// </summary>
        protected IUVars uvars;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public abstract string Condition { get; }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public abstract string parse(string data);

        /// <summary>
        /// Allows post-processing with MSBuild core.
        /// In general, some components can require immediate processing with evaluation, before passing control to next level
        /// </summary>
        public virtual bool PostProcessingMSBuild
        {
            get;
            set;
        }

        /// <summary>
        /// Activation status
        /// </summary>
        public virtual bool Enabled
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Sets location "as is" - after deepening
        /// </summary>
        public virtual bool BeforeDeepen
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
        /// Using of the regex engine for property - Condition
        /// </summary>
        public virtual bool CRegex
        {
            get;
            protected set;
        }

        public ComponentAbstract(ISobaScript soba)
            : this(soba, soba?.EvMSBuild, soba?.UVars)
        {

        }

        public ComponentAbstract(ISobaScript soba, IEvMSBuildMaker evmaker, IUVars uvars)
            : this(soba, evmaker?.MakeEvaluator(uvars), uvars)
        {

        }

        public ComponentAbstract(ISobaScript soba, IEvMSBuild evm, IUVars uvars)
            : this()
        {
            this.soba       = soba ?? throw new ArgumentNullException(nameof(soba));
            this.msbuild    = evm ?? throw new ArgumentNullException(nameof(evm));
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
        protected virtual KeyValuePair<string, string> entryPoint(string data, RegexOptions opt = RegexOptions.None)
        {
            Match m = Regex.Match(data, 
                                    string.Format(@"^\[{0}
                                                        \s*
                                                        (?'request'
                                                           (?'type'
                                                              [A-Za-z_0-9]+
                                                           )
                                                           .*
                                                        )
                                                     \]$", 
                                                     (CRegex)? Condition : Condition.Replace(" ", @"\s")
                                    ),
                                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | opt);

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed {GetType().FullName} - `{data}`");
            }

            return new KeyValuePair<string, string>(m.Groups["type"].Value, m.Groups["request"].Value);
        }

        protected virtual string evaluate(string data)
        {
            LSender.Send(this, $"'{Condition}'-evaluate: started with `{data}`", MsgLevel.Trace);

            if(soba != null) {
                data = soba.parse(data);
                LSender.Send(this, $"'{Condition}'-evaluate: evaluated data: `{data}` :: ISBEScript", MsgLevel.Trace);
            }

            if(msbuild != null) {
                //if(PostProcessingMSBuild) {
                    data = msbuild.Eval(data);
                    LSender.Send(this, $"'{Condition}'-evaluate: evaluated data: `{data}` :: IMSBuild", MsgLevel.Trace);
                //}
            }

            return data;
        }
    }
}
