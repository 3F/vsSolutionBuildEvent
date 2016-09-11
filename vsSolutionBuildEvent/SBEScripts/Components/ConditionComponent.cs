/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Components.Condition;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Conditional statement for user scripts.
    /// </summary>
    [Definition("(true) { }", "Conditionals statements\n\n(1 > 2) {\n ... \n}")]
    public class ConditionComponent: Component, IComponent
    {
        /// <summary>
        /// Core of conditional expressions.
        /// </summary>
        protected Expression expression;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "("; }
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

        protected sealed class ConditionalExpression: Expression
        {
            private ConditionComponent cond;

            protected override string evaluate(string data)
            {
                return cond.evaluate(data);
            }

            public ConditionalExpression(ConditionComponent cond, ISBEScript script, IMSBuild msbuild)
                : base(script, msbuild)
            {
                this.cond = cond;
            }
        }

        /// <summary>
        /// Main rule of container.
        /// </summary>
        protected string Rule
        {
            get
            {
                return String.Format(@"^\[\s*
                                          {0}            #1 - Condition
                                          \s*
                                          {1}            #2 - Body if true
                                          (?:
                                            \s*else\s*
                                            {1}          #3 - Body if false (optional)
                                          )?\s*\]",
                                          RPattern.RoundBracketsContent,
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
                                        RegexOptions.Compiled);
                }
                return crule;
            }
        }
        private Regex crule;

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public ConditionComponent(IEnvironment env, IUserVariable uvariable)
            : base(env, uvariable)
        {
            init();
        }

        /// <param name="loader">Initialization with loader</param>
        public ConditionComponent(IBootloader loader)
            : base(loader)
        {
            init();
        }

        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var hString = new StringHandler();

            Match m = CRule.Match(hString.protectMixedQuotes(data.Trim()));
            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent - '{0}'", data);
            }

            string condition    = hString.recovery(m.Groups[1].Value);
            string bodyIfTrue   = hString.recovery(m.Groups[2].Value);
            string bodyIfFalse  = (m.Groups[3].Success)? hString.recovery(m.Groups[3].Value) : Value.Empty;

            return parse(condition, bodyIfTrue, bodyIfFalse);
        }

        protected string parse(string condition, string ifTrue, string ifFalse)
        {
            Log.Trace("Condition-parse: started with - '{0}' :: '{1}' :: '{2}'", condition, ifTrue, ifFalse);
            return expression.isTrue(condition) ? ifTrue : ifFalse;
        }

        protected void init()
        {
            expression = new ConditionalExpression(this, script, msbuild);
        }
    }
}
