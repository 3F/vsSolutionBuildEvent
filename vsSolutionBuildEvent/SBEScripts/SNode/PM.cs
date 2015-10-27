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
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.SNode
{
    /// <summary>
    /// Detector of PM levels.
    /// </summary>
    public class PM: IPM
    {
        /// <summary>
        /// Condition for analyzer.
        /// </summary>
        public string Condition
        {
            get
            {
                return @"^\s*\.?\s*
                         (?:
                            ([A-Za-z_0-9]+)\s*    #1 - method
                            \((.*?)\)             #2 - arguments
                          |
                            ([A-Za-z_0-9]+)       #3 - property
                         )
                         \s*(.*)                  #4 - operation
                         ";
            }
        }

        /// <summary>
        /// Found levels.
        /// </summary>
        public List<ILevel> Levels
        {
            get {
                return levels;
            }
        }
        protected List<ILevel> levels = new List<ILevel>();


        /// <summary>
        /// Checks equality for level.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool Is(int level, LevelType type, string data = null)
        {
            if(level < 0 || level >= Levels.Count) {
                return false;
            }
            ILevel lvl = Levels[level];

            return (lvl.Type == type && lvl.Data == data);
        }

        /// <summary>
        /// Checks equality for level with additional checking of finalization in levels chain.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        /// <exception cref="NotSupportedOperationException">If found level is equal to selected type and data but is not latest in levels chain.</exception>
        public bool FinalIs(int level, LevelType type, string data = null)
        {
            bool ret = Is(level, type, data);
            if(!ret) {
                return false;
            }

            if(isLastLevel(Levels[level])) {
                return true; // if current is also the last
            }

            // check next level
            if(isLastLevel(Levels[level + 1])) {
                return true; // 'as is' if next level is final.
            }
            
            // the next level is not latest
            throw new NotSupportedOperationException("PM - FinalIs: the level '{0}'({1}) is not final.", Levels[level].Data, Levels[level].Type);
        }

        /// <summary>
        /// Checks equality for level with additional checking of finalization as RightOperandEmpty in levels chain.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        /// <exception cref="NotSupportedOperationException">If found level is equal to selected type and data but is not latest or is not RightOperandEmpty in levels chain.</exception>
        public bool FinalEmptyIs(int level, LevelType type, string data = null)
        {
            bool ret = Is(level, type, data);
            if(!ret) {
                return false;
            }

            if(Levels[level].Type == LevelType.RightOperandEmpty) {
                return true; // if current is also the last
            }

            // check next level
            if(Levels[level + 1].Type == LevelType.RightOperandEmpty) {
                return true; // 'as is' if next level is final and is RightOperandEmpty.
            }

            // the next level is not latest or RightOperandEmpty 
            throw new NotSupportedOperationException("PM - FinalEmptyIs: the level '{0}'({1}) is not final.", Levels[level].Data, Levels[level].Type);
        }
        
        /// <summary>
        /// Slicing of current levels to selected.
        /// </summary>
        /// <param name="level">New start position.</param>
        /// <returns>Self reference.</returns>
        public IPM pinTo(int level)
        {
            if(level < 0 || level >= Levels.Count) {
                throw new InvalidArgumentException("pinTo: the level '{0}' should be >= 0 && < Levels({1})", level, Levels.Count);
            }

            Log.Trace("PM-pinTo: '{0}' /Levels: {1}", level, Levels.Count);
            levels = new List<ILevel>(levels.Skip(level));
            return this;
        }

        /// <param name="raw">Initial raw data.</param>
        public PM(string raw)
        {
            detect(raw);
        }

        /// <summary>
        /// Entry point of analyser.
        /// </summary>
        /// <param name="data">mixed data</param>
        protected void detect(string data)
        {
            Log.Trace("PM-detect: entered with '{0}'", data);

            StringHandler h = new StringHandler();
            data            = h.protectMixedQuotes(data);

            Match m = Regex.Match(data, Condition, RegexOptions.IgnorePatternWhitespace);
            if(!m.Success) {
                levels.Add(getRightOperand(data, h));
                return;
            }

            string method       = (m.Groups[1].Success)? m.Groups[1].Value : null;
            string arguments    = (m.Groups[2].Success)? m.Groups[2].Value : null;
            string property     = (m.Groups[3].Success)? m.Groups[3].Value : null;
            string operation    = m.Groups[4].Value;
            Log.Trace("PM-detect: found '{0}', '{1}', '{2}', '{3}'", property, method, arguments, operation);
            
            if(property != null)
            {
                levels.Add(new Level() {
                    Type = LevelType.Property,
                    Data = property,
                });
            }
            else
            {
                levels.Add(new Level() {
                    Type = LevelType.Method,
                    Data = method,
                    Args = extractArgs(h.recovery(arguments)),
                });
            }

            detect(h.recovery(operation));
        }

        /// <summary>
        /// Extracts all arguments from line.
        /// </summary>
        /// <param name="data">Raw line with user arguments.</param>
        /// <returns>List of parsed arguments or null value if data is empty.</returns>
        /// <exception cref="SyntaxIncorrectException">If incorrect arguments line.</exception>
        protected Argument[] extractArgs(string data)
        {
            if(String.IsNullOrWhiteSpace(data)) {
                return new Argument[0];
            }
            
            StringHandler h = new StringHandler();
            string[] raw    = h.protectMixedQuotes(data).Split(',');

            Argument[] ret = new Argument[raw.Length];
            for(int i = 0; i < raw.Length; ++i)
            {
                string arg = h.recovery(raw[i]).Trim();
                if(arg.Length < 1) {
                    throw new SyntaxIncorrectException("PM - extractArgs: incorrect arguments line '{0}'", data);
                }
                ret[i] = detectArgument(arg);
            }
            return ret;
        }

        /// <summary>
        /// Parse of argument from raw line.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns>Prepared struct.</returns>
        protected Argument detectArgument(string raw)
        {
            // Strings

            Match m = Regex.Match(raw, 
                                    String.Format(@"^(?:
                                                       {0}   #1  - Content from double quotes
                                                     |
                                                       {1}   #2  - Content from single quotes
                                                    )$", RPattern.DoubleQuotesContent, RPattern.SingleQuotesContent
                                    ),
                                    RegexOptions.IgnorePatternWhitespace);
            if(m.Success)
            {
                return new Argument() { 
                    type = (m.Groups[1].Success)? ArgumentType.StringDouble : ArgumentType.StringSingle,
                    data = (m.Groups[1].Success)? m.Groups[1].Value : m.Groups[2].Value
                };
            }

            // Integer

            m = Regex.Match(raw, String.Format("^{0}$", RPattern.IntegerContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Integer,
                    data = Value.toInt32(m.Groups[1].Value)
                };
            }

            // Double

            m = Regex.Match(raw, String.Format("^{0}$", RPattern.DoubleContent));
            if(m.Success)
            {
                return new Argument() {
                    type = ArgumentType.Double,
                    data = Value.toDouble(m.Groups[1].Value)
                };
            }

            // Float

            m = Regex.Match(raw, String.Format("^{0}$", RPattern.FloatContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Float,
                    data = Value.toFloat(m.Groups[1].Value)
                };
            }

            // Boolean

            m = Regex.Match(raw, String.Format("^{0}$", RPattern.BooleanContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Boolean,
                    data = Value.toBoolean(m.Groups[1].Value)
                };
            }

            // Enum or Const

            m = Regex.Match(raw, String.Format("^{0}$", RPattern.EnumOrConstContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.EnumOrConst,
                    data = m.Groups[1].Value
                };
            }

            // Mixed

            return new Argument() {
                type = ArgumentType.Mixed,
                data = raw
            };
        }

        /// <summary>
        /// Gets right operand as a Level object.
        /// </summary>
        /// <param name="data">raw data</param>
        /// <param name="handler">Handler of string if used.</param>
        /// <returns></returns>
        protected Level getRightOperand(string data, StringHandler handler = null)
        {
            if(String.IsNullOrWhiteSpace(data)) {
                return new Level() { Type = LevelType.RightOperandEmpty };
            }

            Match m = Regex.Match(data, @"\s*(=|:)(.*)$");
            if(!m.Success) {
                throw new SyntaxIncorrectException("PM - getRightOperand: incorrect data '{0}'", data);
            }

            string type = m.Groups[1].Value;
            string raw  = m.Groups[2].Value;

            return new Level() {
                Type = (type == ":")? LevelType.RightOperandColon : LevelType.RightOperandStd,
                Data = (handler == null)? raw : handler.recovery(raw)
            };
        }

        /// <summary>
        /// Checks last level.
        /// </summary>
        /// <param name="level">Level for checking.</param>
        /// <returns>true value if selected is latest.</returns>
        protected bool isLastLevel(ILevel level)
        {
            switch(level.Type)
            {
                case LevelType.RightOperandColon:
                case LevelType.RightOperandStd:
                case LevelType.RightOperandEmpty: {
                    return true;
                }
            }
            return false;
        }
    }
}
