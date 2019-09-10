/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Core contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.Z.Core.Condition;

namespace net.r_eg.SobaScript.Z.Core
{
    [Definition("(true) { }", "Conditionals statements\n\n(1 > 2) {\n ... \n}")]
    public class ConditionComponent: ComponentAbstract, IComponent
    {
        private readonly Lazy<Regex> _crule;

        /// <summary>
        /// Core of conditional expressions.
        /// </summary>
        protected ExpressionAbstract expression;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "(";

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        public override bool PostParse => true;

        /// <summary>
        /// Will be located before deepening if true.
        /// </summary>
        public override bool BeforeDeepening => true;

        private class ConditionalExpression: ExpressionAbstract
        {
            private ConditionComponent cond;

            protected override string Evaluate(string data)
            {
                return cond.Evaluate(data);
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
                  {1}            #3 - Body if false (optional)
                )?\s*\]",
                Pattern.RoundBracketsContent,
                Pattern.CurlyBracketsContent
        );

        /// <summary>
        /// Compiled rule.
        /// </summary>
        protected Regex CRule => _crule.Value;

        /// <summary>
        /// Prepare, parse, and Evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var hString = new StringHandler();

            Match m = CRule.Match(hString.ProtectMixedQuotes(data.Trim()));
            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed ConditionComponent - '{data}'");
            }

            string condition    = hString.Recovery(m.Groups[1].Value);
            string bodyIfTrue   = hString.Recovery(m.Groups[2].Value);
            string bodyIfFalse  = m.Groups[3].Success ? hString.Recovery(m.Groups[3].Value) : Value.Empty;

            return Parse(condition, bodyIfTrue, bodyIfFalse);
        }

        public ConditionComponent(ISobaScript soba)
            : base(soba)
        {
            expression = new ConditionalExpression(this, soba, emsbuild);

            _crule = new Lazy<Regex>(() => new Regex
            (
                Rule,
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Compiled                
            ));
        }

        protected string Parse(string condition, string ifTrue, string ifFalse)
        {
            LSender.Send(this, $"Condition-Parse: started with - '{condition}' :: '{ifTrue}' :: '{ifFalse}'", MsgLevel.Trace);
            return expression.IsTrue(condition) ? ifTrue : ifFalse;
        }
    }
}
