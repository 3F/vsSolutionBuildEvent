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
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components.Condition
{
    /// <summary>
    /// Core of conditional expressions.
    /// </summary>
    public abstract class Expression
    {
        /// <summary>
        /// Maximum of nesting level for brackets
        /// </summary>
        protected const int DEPTH_BRACKETS_LIMIT = 40;

        /// <summary>
        /// For evaluation with SBE-Scripts
        /// </summary>
        protected ISBEScript script;

        /// <summary>
        /// For evaluation with MSBuild
        /// </summary>
        protected IMSBuild msbuild;

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

        /// <param name="data">mixed</param>
        protected abstract string evaluate(string data);

        /// <summary>
        /// To parse of raw expression and to check the is it true.
        /// </summary>
        /// <param name="exp">Raw conditional expressions.</param>
        /// <returns></returns>
        public bool isTrue(string exp)
        {
            lock(_lock) {
                hString = new StringHandler();
                _depthBracketsLevel = 0;
            }

            exp = hString.protectCores(
                hString.protectMixedQuotes(exp)
            );

            Log.Trace("Expression-parse: started with - '{0}' :: '{1}' :: '{2}'", exp);
            return (disclosure(exp) == Value.VTRUE);
        }

        public Expression(ISBEScript script, IMSBuild msbuild)
        {
            this.script     = script;
            this.msbuild    = msbuild;
        }

        protected bool calculate(string data)
        {
            Log.Trace("Condition->calculate: started with - '{0}'", data);
            data = hString.recovery(data);
            Log.Trace("Condition->calculate: after recovery - '{0}'", data);

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
