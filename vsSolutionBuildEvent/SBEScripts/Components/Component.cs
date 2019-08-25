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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using net.r_eg.EvMSBuild;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    public abstract class Component: IComponent
    {
        /// <summary>
        /// For evaluation with SBE-Scripts
        /// </summary>
        protected ISBEScript script;

        /// <summary>
        /// For evaluation with MSBuild
        /// </summary>
        protected IEvMSBuild msbuild;

        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Current container of user-variables
        /// </summary>
        protected IUVars uvariable;

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

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public Component(IEnvironment env, IUVars uvariable)
            : this()
        {
            init(env, uvariable);
            script = new Script(env, uvariable);
        }

        /// <param name="loader">Initialization with loader</param>
        public Component(IBootloader loader)
            : this()
        {
            init(loader.Env, loader.UVariable);
            script = new Script(loader);
        }

        /// <param name="script">Instance of SBE-Scripts core</param>
        /// <param name="msbuild">Instance of MSBuild core</param>
        [Obsolete]
        public Component(ISBEScript script, IEvMSBuild msbuild)
            //: this(msbuild)
        {
            this.script = script;

            env             = script.Bootloader.Env;
            uvariable       = msbuild.Variables;
            this.msbuild    = msbuild;
        }

        ///// <param name="msbuild">Instance of MSBuild core</param>
        //public Component(IMSBuild msbuild)
        //    : this()
        //{
        //    env             = msbuild.Env;        // TODO: E-MSBuild
        //    uvariable       = msbuild.UVariable;
        //    this.msbuild    = msbuild;
        //}

        /// <param name="env">Used environment</param>
        public Component(IEnvironment env)
            : this()
        {
            this.env = env;
        }

        public Component()
        {
            Log.Trace("init: '{0}'", GetType().FullName);
        }

        protected void init(IEnvironment env, IUVars uvariable)
        {
            this.env        = env;
            this.uvariable  = uvariable;
            msbuild         = MSBuild.MakeEvaluator(env, uvariable);
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
                                    String.Format(@"^\[{0}
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
                throw new SyntaxIncorrectException("Failed {0} - `{1}`", GetType().FullName, data);
            }

            return new KeyValuePair<string, string>(m.Groups["type"].Value, m.Groups["request"].Value);
        }

        protected virtual string evaluate(string data)
        {
            Log.Trace($"'{Condition}'-evaluate: started with `{data}`");

            if(script != null) {
                data = script.parse(data);
                Log.Trace($"'{Condition}'-evaluate: evaluated data: `{data}` :: ISBEScript");
            }

            if(msbuild != null) {
                //if(PostProcessingMSBuild) {
                    data = msbuild.Eval(data);
                    Log.Trace($"'{Condition}'-evaluate: evaluated data: `{data}` :: IMSBuild");
                //}
            }

            return data;
        }
    }
}
