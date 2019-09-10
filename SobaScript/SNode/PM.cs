/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Extensions;
using net.r_eg.Varhead;

namespace net.r_eg.SobaScript.SNode
{
    public class PM: IPM
    {
        protected IEvMSBuild emsbuild;

        protected EvalType teval = EvalType.None;

        protected IList<ILevel> levels = new List<ILevel>(7);

        private readonly Lazy<Regex> _pmAnalyzer = new Lazy<Regex>(() => new Regex
        (@"
           ^\s*\.?\s*
            (?:
               ([A-Za-z_0-9]+)\s*    #1 - method
               \((.*?)\)             #2 - arguments
             |
               ([A-Za-z_0-9]+)       #3 - property
            )
            \s*(.*)                  #4 - operation
            ",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline | RegexOptions.Compiled
        ));

        /// <summary>
        /// Access to found levels.
        /// </summary>
        public RLevels Levels => levels.AsRLevels();

        /// <summary>
        /// Access to first level.
        /// </summary>
        public ILevel FirstLevel
        {
            get
            {
                if(levels.Count < 1) {
                    throw new ArgumentException("PM: The first level is not initialized or not exists anymore.");
                }
                return levels[0];
            }
            set
            {
                if(levels.Count < 1) {
                    throw new ArgumentException("PM: Allowed only updating. Initialize first.");
                }
                levels[0] = value;
            }
        }

        /// <summary>
        /// Compiled rules of nodes.
        /// </summary>
        protected Regex Rcon => _pmAnalyzer.Value;

        /// <summary>
        /// Checks equality for level.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool Is(int level, LevelType type, string data = null)
        {
            if(level < 0 || level >= levels.Count) {
                return false;
            }

            ILevel lvl = levels[level];

            return lvl.Type == type && lvl.Data == data;
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

            if(IsLastLevel(levels[level])) {
                return true; // if current is also the last
            }

            // check next level
            if(IsLastLevel(levels[level + 1])) {
                return true; // 'as is' if next level is final.
            }
            
            // the next level is not latest
            throw new NotSupportedOperationException($"PM - FinalIs: the level '{levels[level].Data}'({levels[level].Type}) is not final.");
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

            if(levels[level].Type == LevelType.RightOperandEmpty) {
                return true; // if current is also the last
            }

            // check next level
            if(levels[level + 1].Type == LevelType.RightOperandEmpty) {
                return true; // 'as is' if next level is final and is RightOperandEmpty.
            }

            // the next level is not latest or RightOperandEmpty 
            throw new NotSupportedOperationException($"PM - FinalEmptyIs: the level '{levels[level].Data}'({levels[level].Type}) is not final.");
        }
        
        /// <summary>
        /// Slicing of current levels to selected.
        /// </summary>
        /// <param name="level">New start position.</param>
        /// <returns>Self reference.</returns>
        public IPM PinTo(int level)
        {
            levels = SliceLevels(level);
            return this;
        }

        /// <summary>
        /// Get all levels from selected.
        /// </summary>
        /// <param name="level">Start position.</param>
        /// <returns>New instance of IPM.</returns>
        public IPM GetFrom(int level)
        {
            return new PM(SliceLevels(level));
        }

        /// <summary>
        /// The string of diagnostic information about level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public string TraceLevel(int level = 0)
        {
            if(level < 0 || level >= levels.Count) {
                return string.Format("Level '{0}' is not exists. /{1}", level, levels.Count);
            }
            ILevel l = levels[level];
            return string.Format("Data({0}), Type({1}), LevelType({2}), Args = {3}, Level({4}/{5})", 
                                    l.Data, l.DataType, l.Type, (l.Args == null) ? 0 : l.Args.Count, level, levels.Count);
        }

        /// <summary>
        /// Throws error for level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ident">Custom id of place where occurred.</param>
        /// <exception cref="IncorrectNodeException"></exception>
        public void Fail(int level = 0, string ident = null)
        {
            string stMethod = ident ?? (new StackTrace()).GetFrame(1).GetMethod().Name;
            throw new IncorrectNodeException($"`{stMethod}` Node - {TraceLevel(level)} is not correct for this way.");
        }

        /// <summary>
        /// Checks equality for zero level.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool Is(LevelType type, string data = null)
        {
            return Is(0, type, data);
        }

        /// <summary>
        /// Checks equality for zero level with additional checking of finalization in levels chain.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool FinalIs(LevelType type, string data = null)
        {
            return FinalIs(0, type, data);
        }

        /// <summary>
        /// Checks equality for zero level with additional checking of finalization as RightOperandEmpty in levels chain.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool FinalEmptyIs(LevelType type, string data = null)
        {
            return FinalEmptyIs(0, type, data);
        }

        /// <summary>
        /// Checks equality for specific level and move to next level if it is equal to this data.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool It(int level, LevelType type, string data = null)
        {
            if(!Is(level, type, data)) {
                return false;
            }

            if(levels.Count == 1) {
                levels.Clear(); // current level is already checked
            }
            else {
                PinTo(level + 1); // move to next
            }
            return true;
        }

        /// <summary>
        /// Checks equality for zero level and move to next level if it is equal to this data.
        /// </summary>
        /// <param name="type">Level should be with type.</param>
        /// <param name="data">Level should be with data.</param>
        /// <returns>true value if selected level is equal to selected type and data, otherwise false.</returns>
        public bool It(LevelType type, string data = null)
        {
            return It(0, type, data);
        }

        /// <summary>
        /// Checks equality of method for specific level.
        /// </summary>
        /// <param name="level">Selected level.</param>
        /// <param name="name">Method name.</param>
        /// <param name="types">The arguments that should be.</param>
        /// <returns></returns>
        public bool IsMethodWithArgs(int level, string name, params ArgumentType[] types)
        {
            return Is(level, LevelType.Method, name) && levels[level].Is(types);
        }

        /// <summary>
        /// Checks equality of method for zero level.
        /// </summary>
        /// <param name="name">Method name.</param>
        /// <param name="types">The arguments that should be.</param>
        /// <returns></returns>
        public bool IsMethodWithArgs(string name, params ArgumentType[] types)
        {
            return IsMethodWithArgs(0, name, types);
        }

        /// <summary>
        /// Checks type of right operand for zero level.
        /// </summary>
        /// <param name="type">The right operand should be with level type.</param>
        /// <returns>true value if the right operand is equal to selected level type, otherwise false.</returns>
        public bool IsRight(LevelType type)
        {
            return levels.Count > 0 && levels[0].Type == type;
        }

        /// <summary>
        /// Checks equality of data for zero level.
        /// </summary>
        /// <param name="data">Level should be with data.</param>
        /// <param name="variants">Alternative variants that can be.</param>
        /// <returns>true value if selected level is equal to selected data, otherwise false.</returns>
        public bool IsData(string data, params string[] variants)
        {
            if(levels.Count < 1) {
                return false;
            }
            string ldata = levels[0].Data;

            if(ldata == data) {
                return true;
            }

            if(variants.Any(v => ldata == v)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Extracts all arguments from raw data.
        /// </summary>
        /// <param name="raw">Raw data of arguments.</param>
        /// <param name="splitter">A character that delimits arguments.</param>
        /// <returns>List of parsed arguments or null value if data is empty or null.</returns>
        /// <exception cref="IncorrectSyntaxException">If incorrect data.</exception>
        public RArgs GetArguments(string raw, char splitter = ',')
        {
            if(string.IsNullOrWhiteSpace(raw)) {
                return null;
            }
            return ExtractArgs(raw, splitter);
        }

        /// <param name="raw">Initial raw data.</param>
        /// <param name="msbuild">To evaluate data with MSBuild engine where it's allowed.</param>
        /// <param name="type">Allowed types of evaluation with MSBuild.</param>
        public PM(string raw, IEvMSBuild msbuild = null, EvalType type = EvalType.ArgStringD /*| EvalType.RightOperandStd*/)
            : this(msbuild, type)
        {
            Detect(raw);
        }

        /// <param name="levels">predefined levels.</param>
        public PM(IList<ILevel> levels)
        {
            this.levels = levels;
        }

        /// <param name="emsbuild">To evaluate data with MSBuild engine where it's allowed.</param>
        /// <param name="type">Allowed types of evaluation with MSBuild.</param>
        public PM(IEvMSBuild emsbuild = null, EvalType type = EvalType.ArgStringD)
        {
            //if(msbuild == null) {
            //    throw new InvalidArgumentException("PM: The `msbuild` argument cannot be null");
            //}
            this.emsbuild   = emsbuild;
            teval           = type;
        }

        /// <summary>
        /// Entry point of analyser.
        /// </summary>
        /// <param name="data">mixed data</param>
        protected void Detect(string data)
        {
            LSender.Send(this, $"PM-detect: entered with '{data}'", MsgLevel.Trace);

            var h   = new StringHandler();
            data    = h.ProtectMixedQuotes(data);

            Match m = Rcon.Match(data);
            if(!m.Success) {
                levels.Add(GetRightOperand(data, h));
                return;
            }

            string method       = (m.Groups[1].Success)? m.Groups[1].Value : null;
            string arguments    = (m.Groups[2].Success)? m.Groups[2].Value : null;
            string property     = (m.Groups[3].Success)? m.Groups[3].Value : null;
            string operation    = m.Groups[4].Value;
            LSender.Send(this, $"PM-detect: found '{property}', '{method}', '{arguments}', '{operation}'", MsgLevel.Trace);
            
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
                    Args = ExtractArgs(h.Recovery(arguments)),
                });
            }

            Detect(h.Recovery(operation));
        }

        /// <summary>
        /// Extracts all arguments from line.
        /// </summary>
        /// <param name="data">Raw line with user arguments.</param>
        /// <param name="splitter">A character that delimits arguments.</param>
        /// <returns>List of parsed arguments or null value if data is empty.</returns>
        /// <exception cref="IncorrectSyntaxException">If incorrect arguments line.</exception>
        protected RArgs ExtractArgs(string data, char splitter = ',')
        {
            if(string.IsNullOrWhiteSpace(data)) {
                return new RArgs(new Argument[0]);
            }
            
            StringHandler h = new StringHandler();
            string[] raw    = h.ProtectArguments(data).Split(splitter);

            Argument[] ret = new Argument[raw.Length];
            for(int i = 0; i < raw.Length; ++i)
            {
                string arg = h.Recovery(raw[i]).Trim();
                if(arg.Length < 1 && splitter == ',') { // std: p1, p2, p3
                    throw new IncorrectSyntaxException($"PM - extractArgs: incorrect arguments line '{data}'");
                }
                ret[i] = DetectArgument(arg);
            }

            return new RArgs(ret);
        }

        /// <summary>
        /// Parse of argument from raw line.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns>Prepared struct.</returns>
        protected Argument DetectArgument(string raw)
        {
            // Object - { "p1", true, 12 }

            Match m = Regex.Match(raw, string.Format("^{0}$", Pattern.ObjectContent), RegexOptions.IgnorePatternWhitespace);
            if(m.Success)
            {
                return new Argument() {
                    type = ArgumentType.Object,
                    data = ExtractArgs(m.Groups[1].Value.Trim())
                };
            }

            // Char

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.CharContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Char,
                    data = Value.ToChar(m.Groups[1].Value)
                };
            }


            // Strings

            m = Regex.Match(raw, 
                             string.Format(@"^(?:
                                                {0}   #1  - Content from double quotes
                                              |
                                                {1}   #2  - Content from single quotes
                                             )$", Pattern.DoubleQuotesContent, Pattern.SingleQuotesContent
                             ),
                             RegexOptions.IgnorePatternWhitespace);
            if(m.Success)
            {
                if(m.Groups[1].Success) {
                    return new Argument() { type = ArgumentType.StringDouble,
                                            data = Eval(EvalType.ArgStringD, Tokens.UnescapeQuotes('"', m.Groups[1].Value)) };
                }

                return new Argument() { type = ArgumentType.StringSingle,
                                        data = Eval(EvalType.ArgStringS, Tokens.UnescapeQuotes('\'', m.Groups[2].Value)) };
            }

            // Integer

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.IntegerContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Integer,
                    data = Value.ToInt32(m.Groups[1].Value)
                };
            }

            // Float

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.FloatContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Float,
                    data = Value.ToFloat(m.Groups[1].Value)
                };
            }

            // Double

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.DoubleContent));
            if(m.Success)
            {
                return new Argument() {
                    type = ArgumentType.Double,
                    data = Value.ToDouble(m.Groups[1].Value)
                };
            }

            // Boolean

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.BooleanContent));
            if(m.Success)
            {
                return new Argument() { 
                    type = ArgumentType.Boolean,
                    data = Value.ToBoolean(m.Groups[1].Value)
                };
            }

            // Enum or Const

            m = Regex.Match(raw, string.Format("^{0}$", Pattern.EnumOrConstContent));
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
        protected ILevel GetRightOperand(string data, StringHandler handler = null)
        {
            if(string.IsNullOrWhiteSpace(data)) {
                return new Level() { Type = LevelType.RightOperandEmpty };
            }

            Match m = Regex.Match(data, @"^\s*(=|:)(.*)$", RegexOptions.Singleline);
            if(!m.Success) {
                throw new IncorrectSyntaxException($"PM - getRightOperand: incorrect data '{data}'");
            }

            string type = m.Groups[1].Value;
            string raw  = m.Groups[2].Value;

            string ldata = (handler == null)? raw : handler.Recovery(raw);

            if(type == ":") {
                return new Level() { Type = LevelType.RightOperandColon, Data = Eval(EvalType.RightOperandColon, ldata) };
            }
            return new Level() { Type = LevelType.RightOperandStd, Data = Eval(EvalType.RightOperandStd, ldata) };
        }

        protected string Eval(EvalType type, string raw)
        {
            if(type == EvalType.None || emsbuild == null) {
                return raw;
            }

            return ((teval & type) == type)? emsbuild.Eval(raw) : raw;
        }

        /// <summary>
        /// Checks last level.
        /// </summary>
        /// <param name="level">Level for checking.</param>
        /// <returns>true value if selected is latest.</returns>
        protected bool IsLastLevel(ILevel level)
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

        /// <param name="level">Start position of slicing.</param>
        /// <returns></returns>
        protected IList<ILevel> SliceLevels(int level)
        {
            if(level < 0 || level >= levels.Count) {
                throw new ArgumentException($"PM: The level '{level}' should be >= 0 && < Levels({levels.Count})");
            }
            return new List<ILevel>(levels.Skip(level));
        }
    }
}
