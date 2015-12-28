/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

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
        protected IMSBuild msbuild;

        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Current container of user-variables
        /// </summary>
        protected IUserVariable uvariable;

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
            get { return postProcessingMSBuild; }
            set { postProcessingMSBuild = value; }
        }
        protected bool postProcessingMSBuild = false;

        /// <summary>
        /// Activation status
        /// </summary>
        public virtual bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        protected bool enabled = true;

        /// <summary>
        /// Sets location "as is" - after deepening
        /// </summary>
        public virtual bool BeforeDeepen
        {
            get { return beforeDeepen; }
        }
        protected bool beforeDeepen = false;

        /// <summary>
        /// Disabled the forced post analysis
        /// </summary>
        public virtual bool PostParse
        {
            get { return postParse; }
        }
        protected bool postParse = false;

        /// <summary>
        /// Disabled regex engine for property - condition
        /// </summary>
        public virtual bool CRegex
        {
            get { return cregex; }
        }
        protected bool cregex = false;

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public Component(IEnvironment env, IUserVariable uvariable)
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
        public Component(ISBEScript script, IMSBuild msbuild)
            : this()
        {
            env             = script.Bootloader.Env;
            uvariable       = script.Bootloader.UVariable;
            this.script     = script;
            this.msbuild    = msbuild;
        }

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

        protected void init(IEnvironment env, IUserVariable uvariable)
        {
            this.env        = env;
            this.uvariable  = uvariable;
            msbuild         = new MSBuild.Parser(env, uvariable);
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
                                                     \]$", Condition
                                    ),
                                    RegexOptions.IgnorePatternWhitespace | opt);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed {0} - `{1}`", GetType().FullName, data);
            }

            return new KeyValuePair<string, string>(m.Groups["type"].Value, m.Groups["request"].Value);
        }
    }
}
