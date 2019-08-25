﻿/*
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
using System.Text.RegularExpressions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// try/catch for user scripts
    /// </summary>
    [Definition("try", "try/catch")]
    [Definition("{ }catch{ }", "try/catch\n\nProtects from errors in try{...} block and handles it in catch{...}", "try")]
    [Definition("{ }catch(err, msg){ }", "try/catch with error type and its message.", "try")]
    public class TryComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "try"; }
        }

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        public override bool PostParse
        {
            get { return true; }
        }

        /// <summary>
        /// Should be located before deepening
        /// </summary>
        public override bool BeforeDeepen
        {
            get { return true; }
        }

        /// <summary>
        /// Main rule of container.
        /// </summary>
        protected string Rule
        {
            get
            {
                return String.Format(
                                @"^\[\s*
                                      try
                                      \s*{0}\s*             #1      - try
                                      catch
                                      (?:\s*
                                        \((?'args'.*?)\)    #args   - optional arguments
                                      \s*)?
                                      \s*{0}\s*             #2      - catch
                                   \]",
                                   RPattern.CurlyBracketsContent
                );
            }
        }

        /// <summary>
        /// Compiled rule.
        /// </summary>
        protected Regex CRule
        {
            get
            {
                if(crule == null) {
                    crule = new Regex(Rule,
                                        RegexOptions.IgnorePatternWhitespace |
                                        RegexOptions.Singleline |
                                        RegexOptions.Compiled);
                }
                return crule;
            }
        }
        private Regex crule;

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public TryComponent(IEnvironment env, IUVars uvariable)
            : base(env, uvariable)
        {

        }

        /// <param name="loader">Initialization with loader</param>
        public TryComponent(IBootloader loader)
            : base(loader)
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var hString = new StringHandler();

            Match m = CRule.Match(hString.ProtectMixedQuotes(data.Trim()));
            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed TryComponent - '{0}'", data);
            }

            string blockTry     = hString.Recovery(m.Groups[1].Value);
            string blockCatch   = hString.Recovery(m.Groups[2].Value);
            string args         = (m.Groups["args"].Success)? hString.Recovery(m.Groups["args"].Value) : null;

            try {
                return evaluate(blockTry);
            }
            catch(Exception ex) {
                return doCatch(blockCatch, ex, (new PM()).arguments(args));
            }
        }

        protected string doCatch(string cmd, Exception ex, Argument[] args)
        {
            Log.Info($"Catched error `{ex.Message}`");

            if(args == null) {
                return evaluate(cmd);
            }

            if(args.Length == 2
                && args[0].type == ArgumentType.EnumOrConst
                && args[1].type == ArgumentType.EnumOrConst)
            {
                // try{ }catch(err, msg){ }
                return doCatch(cmd, ex, args[0].data.ToString(), args[1].data.ToString());
            }

            throw new NotSupportedOperationException("the format of the catch block is incorrect or not supported yet.");
        }

        protected string doCatch(string cmd, Exception ex, string err, string msg)
        {
            try {
                setvar(err, ex.GetType().FullName);
                setvar(msg, ex.Message);

                return evaluate(cmd);
            }
            finally {
                delvar(err, msg);
            }
        }

        private void setvar(string name, string value)
        {
            uvariable.SetVariable(name, null, value);
            uvariable.Evaluate(name, null, new EvaluatorBlank(), true);
        }

        private void delvar(params string[] names)
        {
            if(names == null) {
                return;
            }

            foreach(string name in names) {
                uvariable.Unset(name, null);
            }
        }
    }
}
