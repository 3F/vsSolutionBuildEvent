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
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Conditional statement for scripts
    /// </summary>
    [Definition("(true) { }", "Conditionals statements\n\n(1 > 2) {\n ... \n}")]
    public class ConditionComponent: Component, IComponent
    {
        /// <summary>
        /// Maximum of nesting level for brackets
        /// </summary>
        protected const int DEPTH_BRACKETS_LIMIT = 40;

        /// <summary>
        /// Current level of nesting brackets.
        /// Aborting if reached limit
        /// </summary>
        private volatile uint _depthBracketsLevel = 0;

        /// <summary>
        /// Protecting from incorrect symbols.
        /// </summary>
        private StringHandler hString = new StringHandler();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "("; }
        }

        /// <summary>
        /// Forced post-analysis.
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

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public ConditionComponent(IEnvironment env, IUserVariable uvariable)
            : base(env, uvariable)
        {

        }

        /// <param name="loader">Initialization with loader</param>
        public ConditionComponent(IBootloader loader)
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
            lock(_lock) {
                hString = new StringHandler();
                _depthBracketsLevel = 0;
            }

            Match m = Regex.Match(hString.protectMixedQuotes(data.Trim()),
                                    String.Format(@"^\[\s*
                                                    {0}            #1 - Condition
                                                    \s*
                                                    {1}            #2 - Body if true
                                                    (?:
                                                      \s*else\s*
                                                      {1}          #3 - Body if false (optional)
                                                    )?\s*\]",
                                                    RPattern.RoundBracketsContent,
                                                    RPattern.CurlyBracketsContent
                                                 ), 
                                                 RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent - '{0}'", data);
            }

            string condition    = m.Groups[1].Value;
            string bodyIfTrue   = hString.recovery(m.Groups[2].Value);
            string bodyIfFalse  = (m.Groups[3].Success)? hString.recovery(m.Groups[3].Value) : Value.Empty;

            return parse(hString.protectCores(condition), bodyIfTrue, bodyIfFalse);
        }

        protected string parse(string condition, string ifTrue, string ifFalse)
        {
            Log.Debug("Condition-parse: started with - '{0}' :: '{1}' :: '{2}'", condition, ifTrue, ifFalse);
            return (disclosure(condition) == Value.VTRUE)? ifTrue : ifFalse;
        }

        protected bool calculate(string data)
        {
            Log.Debug("Condition->calculate: started with - '{0}'", data);
            data = hString.recovery(data);
            Log.Debug("Condition->calculate: after recovery - '{0}'", data);

            Match m = Regex.Match(data.Trim(), 
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
                                               RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent->calculate - '{0}'", data);
            }

            bool invert         = m.Groups[1].Success;
            string left         = spaces(m.Groups[2].Value);
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
                        throw new SyntaxIncorrectException("Failed ConditionComponent: reserved combination- '{0}'", data);
                    }
                }
                right = spaces(right);
            }
            Log.Debug("Condition->calculate: left: '{0}', right: '{1}', operator: '{2}', invert: {3}", left, right, coperator, invert);

            left = evaluate(left);

            if(right != null) {
                result = Value.cmp(left, evaluate(right), coperator);
            }
            else {
                result = Value.cmp(alias4SingleOperand(left));
            }
            Log.Debug("Condition->calculate: result is: '{0}'", result);
            return ((invert)? !result : result);
        }

        protected virtual string alias4SingleOperand(string op)
        {
            if(op == "0") {
                return Value.VFALSE;
            }

            if(op == "1") {
                return Value.VTRUE;
            }

            if(String.IsNullOrWhiteSpace(op) || op == MSBuild.Parser.PROP_VALUE_DEFAULT) {
                return Value.VFALSE;
            }

            return op;
        }

        /// <summary>
        /// Handling spaces
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string spaces(string data)
        {
            Match m = Regex.Match(data.Trim(), // // ->" data  "<- or ->data<-
                                    String.Format(@"(?:
                                                      {0}   #1 - with space protection
                                                    |
                                                      (.*)  #2 - without
                                                    )?",
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent->spaces - '{0}'", data);
            }

            if(m.Groups[1].Success) {
                return StringHandler.normalize(m.Groups[1].Value);
            }
            return m.Groups[2].Value.Trim();
        }

        /// <param name="data">mixed</param>
        protected string evaluate(string data)
        {
            Log.Trace("Condition-evaluate: started with '{0}'", data);

            data = script.parse(data);
            Log.Trace("Condition-evaluate: evaluated data: '{0}' :: ISBEScript", data);

            if(PostProcessingMSBuild) {
                data = msbuild.parse(data);
                Log.Trace("Condition-evaluate: evaluated data: '{0}' :: IMSBuild", data);
            }
            return data;
        }

        /// <summary>
        /// TODO: this realy fastest variant of implementation. 
        ///       However, for productivity and Short-circuit Evaluation! needed additional implementation as left serial movement..
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string disclosure(string data)
        {
            if(_depthBracketsLevel >= DEPTH_BRACKETS_LIMIT) {
                _depthBracketsLevel = 0;
                throw new LimitException("Condition-disclosure: Nesting level of '{0}' reached. Aborted.", DEPTH_BRACKETS_LIMIT);
            }

            string ret = Regex.Replace(data, @"
                                                \(
                                                   (
                                                     [^()]*
                                                   )         #1 - expression
                                                \)
                                              ", 
            delegate(Match m)
            {
                string exp = m.Groups[1].Value.Trim();
                Log.Trace("Condition-disclosure: expression - '{0}' /level: {1}", exp, _depthBracketsLevel);

                if(String.IsNullOrEmpty(exp)) {
                    throw new SyntaxIncorrectException("Condition-disclosure: empty brackets are not allowed.");
                }
                return composite(exp);
            },
            RegexOptions.IgnorePatternWhitespace);

            if(ret.IndexOf('(') != -1)
            {
                Log.Trace("Condition-disclosure: found a new bracket - '{0}'", ret);

                ++_depthBracketsLevel;
                string dret = disclosure(ret); // not all disclosed
                --_depthBracketsLevel;

                return dret;
            }
            return composite(ret);
        }

        /// <summary>
        /// Composite Conditions
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string composite(string data)
        {
            Log.Trace("Condition-composite: started with - '{0}'", data);

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
                    throw new SyntaxIncorrectException("Condition-composite: allowed only logical operators - '&&' and '||'");
                }

                string exp = null;
                if(curr == '|' || curr == '&') {
                    exp = data.Substring(left, i - left);
                    left = i += 2;
                }

                if(curr == '|')
                {
                    if(calculate(exp)) {
                        return Value.VTRUE;
                    }

                    if(left >= len) {
                        return Value.VFALSE;
                    }

                    continue;
                }

                if(curr == '&')
                {
                    if(calculate(exp)) {
                        continue;
                    }

                    int orpos = data.IndexOf("||", left);
                    if(orpos == -1) {
                        return Value.VFALSE;
                    }

                    left = i = (orpos + 2);
                    continue; // try with new block _x_|| -> ?
                }

                ++i;
            }
            return calculate(data.Substring(left))? Value.VTRUE : Value.VFALSE; // -> ??? EOL
        }
    }
}
