/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Conditions in scripts
    /// </summary>
    public class ConditionComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "[("; }
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variables</param>
        public ConditionComponent(IEnvironment env, IUserVariable uvariable): base(env, uvariable)
        {
            beforeDeepen    = true; // Should be located before deepening
            postParse       = true; // Forced post analysis
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            StringHandler hString = new StringHandler();

            Match m = Regex.Match(hString.protect(data.Trim()),
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
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent - '{0}'", data);
            }

            string condition    = hString.recovery(m.Groups[1].Value);
            string bodyIfTrue   = hString.recovery(m.Groups[2].Value);
            string bodyIfFalse  = (m.Groups[3].Success)? hString.recovery(m.Groups[3].Value) : String.Empty;

            return parse(condition, bodyIfTrue, bodyIfFalse);
        }

        protected string parse(string condition, string ifTrue, string ifFalse)
        {
            Log.nlog.Debug("Condition-parse: started with - '{0}' :: '{1}' :: '{2}'", condition, ifTrue, ifFalse);

            Match m = Regex.Match(condition.Trim(), 
                                              @"^\s*
                                               (!)?         #1 - flag of inversion (optional)
                                               ([^=!~<>]+) #2 - left operand - boolean type if as a single
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
                                                      >=
                                                    |
                                                      <=
                                                    |
                                                      >
                                                    |
                                                      <
                                                   )        #3 - operator      (optional with #4)
                                                   (.+)     #4 - right operand (optional with #3)
                                               )?$", 
                                               RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed ConditionComponent->parse - '{0}'", condition);
            }

            bool invert         = m.Groups[1].Success;
            string left         = spaces(m.Groups[2].Value);
            string coperator    = null;
            string right        = null;
            bool result         = false;

            if(m.Groups[3].Success) {
                coperator   = m.Groups[3].Value;
                right       = m.Groups[4].Value;

                switch(coperator + right) {
                    case "===":
                    case "!==":
                    case ">=":
                    case "<=": {
                        throw new SyntaxIncorrectException("Failed ConditionComponent: reserved combination- '{0}'", condition);
                    }
                }
                right = spaces(right);
            }
            Log.nlog.Debug("Condition-parse: left: '{0}', right: '{1}', operator: '{2}', invert: {3}", left, right, coperator, invert);

            left = evaluate(left);

            if(right != null) {
                result = Values.cmp(left, evaluate(right), coperator);
            }
            else {
                result = Values.cmp((left == "1")? Values.VTRUE : (left == "0")? Values.VFALSE : left);
            }
            Log.nlog.Debug("Condition-parse: result is: '{0}'", result);
            return ((invert)? !result : result)? ifTrue : ifFalse;
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
            Log.nlog.Trace("Condition-evaluate: started with '{0}'", data);

            data = script.parse(data);
            Log.nlog.Trace("Condition-evaluate: evaluated data: '{0}' :: ISBEScript", data);

            if(PostProcessingMSBuild) {
                data = msbuild.parse(data);
                Log.nlog.Trace("Condition-evaluate: evaluated data: '{0}' :: IMSBuild", data);
            }
            return data;
        }
    }

    /// <summary>
    /// Result type for ConditionComponent
    /// </summary>
    public struct ConditionComponentResult
    {
        /// <summary>
        /// Left operand
        /// </summary>
        public string left;

        /// <summary>
        /// Right operand
        /// </summary>
        public string right;

        /// <summary>
        /// Operator of comparison
        /// </summary>
        public string coperator;

        /// <summary>
        /// Body if true
        /// </summary>
        public string ifTrue;

        /// <summary>
        /// Body if false
        /// </summary>
        public string ifFalse;
    }
}
