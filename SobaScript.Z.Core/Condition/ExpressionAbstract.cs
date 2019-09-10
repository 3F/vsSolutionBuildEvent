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

using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.SobaScript.Z.Core.Condition
{
    /// <summary>
    /// Core of conditional expressions.
    /// </summary>
    public abstract class ExpressionAbstract
    {
        /// <summary>
        /// Maximum of nesting level for brackets
        /// </summary>
        protected const int DEPTH_BRACKETS_LIMIT = 40;

        /// <summary>
        /// For evaluation with SBE-Scripts
        /// </summary>
        protected ISobaScript soba;

        /// <summary>
        /// For evaluation with MSBuild
        /// </summary>
        protected IEvMSBuild emsbuild;

        /// <summary>
        /// Current level of nesting brackets.
        /// Aborting if reached limit
        /// </summary>
        private volatile uint depthBracketsLevel = 0;

        /// <summary>
        /// Protecting from incorrect symbols.
        /// </summary>
        private StringHandler hString = new StringHandler();

        private readonly object sync = new object();

        /// <param name="data">mixed</param>
        protected abstract string Evaluate(string data);

        /// <summary>
        /// To Parse of raw expression and to check the is it true.
        /// </summary>
        /// <param name="exp">Raw conditional expressions.</param>
        /// <returns></returns>
        public bool IsTrue(string exp)
        {
            lock(sync) {
                hString = new StringHandler();
                depthBracketsLevel = 0;
            }

            exp = hString.ProtectCores(
                hString.ProtectMixedQuotes(exp)
            );

            LSender.Send(this, $"Expression-Parse: started with - '{exp}'", MsgLevel.Trace);
            return Disclosure(exp) == Value.TRUE;
        }

        public ExpressionAbstract(ISobaScript soba, IEvMSBuild emsbuild)
        {
            this.soba       = soba;
            this.emsbuild   = emsbuild;
        }

        protected bool Calculate(string data)
        {
            LSender.Send(this, $"Condition->Calculate: started with - '{data}'", MsgLevel.Trace);
            data = hString.Recovery(data);
            LSender.Send(this, $"Condition->Calculate: after recovery - '{data}'", MsgLevel.Trace);

            Match m = Regex.Match
            (
                data.Trim(), 
                @"^\s*
                 (!)?           #1 - flag of inversion (optional)
                 ([^\^=!~<>]+)  #2 - left operand - boolean type if as a single
                 (?:
                     (
                        ===
                      |
                        !==
                      |
                        ~=
                      |
                        ==
                      |
                        !=
                      |
                        \^=
                      |
                        =\^
                      |
                        >=
                      |
                        <=
                      |
                        >
                      |
                        <
                     )          #3 - operator      (optional with #4)
                     (.+)       #4 - right operand (optional with #3)
                 )?$", 
                 RegexOptions.IgnorePatternWhitespace
            );

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed ConditionComponent->Calculate - '{data}'");
            }

            bool invert         = m.Groups[1].Success;
            string left         = Spaces(m.Groups[2].Value);
            string coperator    = null;
            string right        = null;
            bool result         = false;

            if(m.Groups[3].Success)
            {
                coperator   = m.Groups[3].Value;
                right       = m.Groups[4].Value;

                switch(coperator + right) {
                    case "===":
                    case "!==":
                    case ">=":
                    case "<=": {
                        throw new IncorrectSyntaxException($"Failed ConditionComponent: reserved combination- '{data}'");
                    }
                }
                right = Spaces(right);
            }
            LSender.Send(this, $"Condition->Calculate: left: '{left}', right: '{right}', operator: '{coperator}', invert: {invert}");

            left = Evaluate(left);

            if(right != null) {
                result = Value.Cmp(left, Evaluate(right), coperator);
            }
            else {
                result = Value.Cmp(Alias4SingleOperand(left));
            }
            LSender.Send(this, $"Condition->Calculate: result is: '{result}'");
            return invert ? !result : result;
        }

        protected virtual string Alias4SingleOperand(string op)
        {
            if(op == "0") {
                return Value.FALSE;
            }

            if(op == "1") {
                return Value.TRUE;
            }

            if(string.IsNullOrWhiteSpace(op) || op == EvMSBuilder.UNDEF_VAL) {
                return Value.FALSE;
            }

            return op;
        }

        /// <summary>
        /// Handling spaces
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string Spaces(string data)
        {
            Match m = Regex.Match
            (
                data.Trim(), // // ->" data  "<- or ->data<-
                string.Format
                (
                    @"(?:
                        {0}   #1 - with space protection
                    |
                        (.*)  #2 - without
                    )?",
                    Pattern.DoubleQuotesContent
                ),
                RegexOptions.IgnorePatternWhitespace
            );

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed ConditionComponent->spaces - '{data}'");
            }

            if(m.Groups[1].Success) {
                return StringHandler.Normalize(m.Groups[1].Value);
            }
            return m.Groups[2].Value.Trim();
        }

        /// <summary>
        /// TODO: this is really fastest variant of implementation. 
        ///       However, for productivity and Short-circuit Evaluation we need an additional implementation as left serial movement.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string Disclosure(string data)
        {
            if(depthBracketsLevel >= DEPTH_BRACKETS_LIMIT) {
                depthBracketsLevel = 0;
                throw new LimitException($"Condition-Disclosure: Nesting level of '{DEPTH_BRACKETS_LIMIT}' reached. Aborted.", DEPTH_BRACKETS_LIMIT);
            }

            string ret = Regex.Replace
            (
                data,
                @"
                  \(
                     (
                       [^()]*
                     )         #1 - expression
                  \)
                ", 
                (Match m) =>
                {
                    string exp = m.Groups[1].Value.Trim();
                    LSender.Send(this, $"Condition-Disclosure: expression - '{exp}' /level: {depthBracketsLevel}", MsgLevel.Trace);

                    if(string.IsNullOrEmpty(exp)) {
                        throw new IncorrectSyntaxException("Condition-Disclosure: empty brackets are not allowed.");
                    }
                    return Composite(exp);
                },
                RegexOptions.IgnorePatternWhitespace
            );

            if(ret.IndexOf('(') != -1)
            {
                LSender.Send(this, $"Condition-Disclosure: found a new bracket - '{ret}'", MsgLevel.Trace);

                ++depthBracketsLevel;
                string dret = Disclosure(ret); // not all disclosed
                --depthBracketsLevel;

                return dret;
            }
            return Composite(ret);
        }

        /// <summary>
        /// Composite Conditions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string Composite(string data)
        {
            LSender.Send(this, $"Condition-Composite: started with - '{data}'", MsgLevel.Trace);

            //if(data.IndexOfAny(new char[] { '=', '>', '<', '!', '|', '&' }) == -1) {
            //    //TODO: without expression e.g.: 1 > (7) -> 1 > 7
            //}
            //Log.Trace("Condition-composite: finding operators..");

            int left = 0;
            for(int i = 0, len = data.Length - 1; i < len; )
            {
                char curr   = data[i];
                char next   = data[i + 1];
                if((curr == '|' && next != '|')
                    ||
                    (curr == '&' && next != '&'))
                {
                    throw new IncorrectSyntaxException("Condition-Composite: allowed only logical operators - '&&' and '||'");
                }

                string exp = null;
                if(curr == '|' || curr == '&') {
                    exp = data.Substring(left, i - left);
                    left = i += 2;
                }

                if(curr == '|')
                {
                    if(Calculate(exp)) {
                        return Value.TRUE;
                    }

                    if(left >= len) {
                        return Value.FALSE;
                    }

                    continue;
                }

                if(curr == '&')
                {
                    if(Calculate(exp)) {
                        continue;
                    }

                    int orpos = data.IndexOf("||", left);
                    if(orpos == -1) {
                        return Value.FALSE;
                    }

                    left = i = (orpos + 2);
                    continue; // try with new block _x_|| -> ?
                }

                ++i;
            }
            return Calculate(data.Substring(left)) ? Value.TRUE : Value.FALSE; // -> ??? EOL
        }
    }
}
