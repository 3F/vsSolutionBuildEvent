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
using System.Text.RegularExpressions;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components.Condition;
using net.r_eg.vsSBE.SBEScripts.Dom;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Conditional statement for user scripts.
    /// </summary>
    [Definition("(true) { }", "Conditionals statements\n\n(1 > 2) {\n ... \n}")]
    public class ConditionComponent: ComponentAbstract, IComponent
    {
        private readonly Lazy<Regex> _crule;

        /// <summary>
        /// Core of conditional expressions.
        /// </summary>
        protected Expression expression;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition => "(";

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        public override bool PostParse => true;

        /// <summary>
        /// Should be located before deepening
        /// </summary>
        public override bool BeforeDeepen => true;

        protected sealed class ConditionalExpression: Expression
        {
            private ConditionComponent cond;

            protected override string evaluate(string data)
            {
                return cond.evaluate(data);
            }

            public ConditionalExpression(ConditionComponent cond, ISobaScript script, IEvMSBuild msbuild)
                : base(script, msbuild)
            {
                this.cond = cond;
            }
        }

        /// <summary>
        /// Main rule of container.
        /// </summary>
        protected string Rule => string.Format
        (
            @"^\[\s*
                {0}            #1 - Condition
                \s*
                {1}            #2 - Body if true
                (?:
                \s*else\s*
                {1}          #3 - Body if false (optional)
                )?\s*\]",
                SobaScript.RPattern.RoundBracketsContent,
                SobaScript.RPattern.CurlyBracketsContent
        );

        /// <summary>
        /// Compiled rule.
        /// </summary>
        protected Regex CRule => _crule.Value;

        public ConditionComponent(ISobaScript soba)
            : base(soba)
        {
            expression = new ConditionalExpression(this, soba, msbuild);

            _crule = new Lazy<Regex>(() => new Regex
            (
                Rule,
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Compiled                
            ));
        }

        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var hString = new StringHandler();

            Match m = CRule.Match(hString.ProtectMixedQuotes(data.Trim()));
            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed ConditionComponent - '{data}'");
            }

            string condition    = hString.Recovery(m.Groups[1].Value);
            string bodyIfTrue   = hString.Recovery(m.Groups[2].Value);
            string bodyIfFalse  = (m.Groups[3].Success)? hString.Recovery(m.Groups[3].Value) : Value.Empty;

            return parse(condition, bodyIfTrue, bodyIfFalse);
        }

        protected string parse(string condition, string ifTrue, string ifFalse)
        {
            Log.Trace("Condition-parse: started with - '{0}' :: '{1}' :: '{2}'", condition, ifTrue, ifFalse);
            return expression.isTrue(condition) ? ifTrue : ifFalse;
        }
    }
}
