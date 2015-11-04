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
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Receiver.Output;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;
using OWPIdent = net.r_eg.vsSBE.Receiver.Output.Ident;
using OWPItems = net.r_eg.vsSBE.Receiver.Output.Items;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    [Component("OWP", "For work with OWP (Output Window Pane)")]
    public class OWPComponent: Component, IComponent, ILogData
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "OWP "; }
        }

        /// <summary>
        /// atomic unit
        /// </summary>
        protected struct LogData
        {
            public string Message { get; set; }
            public string Level { get; set; }
        }
        protected LogData logcopy;


        /// <param name="env">Used environment</param>
        public OWPComponent(IEnvironment env)
            : base(env)
        {

        }

        /// <summary>
        /// Updating copies of the log data for work with event of logging
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public void updateLogData(string message, string level)
        {
            logcopy = new LogData() {
                Message = message,
                Level   = level
            };
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[OWP
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$",
                                           RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed OWPComponent - '{0}'", data);
            }

            string ident    = m.Groups[1].Value;
            string subtype  = m.Groups[2].Value;

            switch(subtype) {
                case "out": {
                    Log.Debug("OWPComponent: use stOut");
                    return stOut(ident);
                }
                case "log": {
                    Log.Debug("OWPComponent: use stLog");
                    return stLog(ident);
                }
                case "item": {
                    Log.Debug("OWPComponent: use stItem");
                    return stItem(ident);
                }
            }
            throw new SubtypeNotFoundException("OWPComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// For work with events of logging.
        /// 
        /// * #[OWP log.Message]
        /// * #[OWP log.Level]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Property("log", "Provides data from events of logging.")]
        [Property("Message", "Current message from log.", "log", "stLog", CValueType.String)]
        [Property("Level", "Level for current property the Message.", "log", "stLog", CValueType.String)]
        protected string stLog(string data)
        {
            Match m = Regex.Match(data, @"log
                                          \s*\.\s*
                                          ([A-Za-z_0-9]+)", // #1 - Property name
                                          RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new OperationNotFoundException("Failed stLog - '{0}'", data);
            }

            string property = m.Groups[1].Value;

            switch(property) {
                case "Message": {
                    return Value.from(logcopy.Message);
                }
                case "Level": {
                    return Value.from(logcopy.Level);
                }
            }
            throw new OperationNotFoundException("OWPComponent-stLog: not found property - '{0}'", property);
        }

        /// <summary>
        /// Access to items of the OWP.
        /// 
        /// * #[OWP item("name")]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [
            Method
            (
                "item",
                "Access to item of the Output window by name.",
                new string[] { "name" },
                new string[] { "Name of item" },
                CValueType.Void,
                CValueType.String
            )
        ]
        protected string stItem(string data)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"
                                                    item
                                                    \s*
                                                    \(
                                                       {0}            #1 - name
                                                    \)
                                                    \s*\.\s*
                                                    ([A-Za-z_0-9]+)   #2 - operation
                                                    \s*(.*)           #3 - raw data for operation (optional)", 
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stItem - '{0}'", data);
            }

            string name         = m.Groups[1].Value;            
            string operation    = m.Groups[2].Value;
            string raw          = (m.Groups[3].Success)? m.Groups[3].Value : null;

            Log.Debug("stItem: '{0}', '{1}', '{2}'", name, operation, raw);

            if(String.IsNullOrEmpty(name) 
                || name.Trim().Equals(Settings.OWP_ITEM_VSSBE, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedOperationException("OWPComponent: The name '{0}' is not available to using for current operation.", name);
            }

            switch(operation)
            {
                case "write": {
                    return stItemWrite(name, false, raw);
                }
                case "writeLine": {
                    return stItemWrite(name, true, raw);
                }
                case "delete": {
                    return stItemDelete(name, raw);
                }
                case "clear": {
                    return stItemClear(name, raw);
                }
                case "activate": {
                    return stItemActivate(name, raw);
                }
            }
            throw new OperandNotFoundException("OWPComponent-stItem: not supported operation - '{0}'", operation);
        }

        /// <summary>
        /// Sends data to the OutputWindowPane window.
        /// 
        /// * #[OWP item("name").write(true): content]
        /// * #[OWP item("name").writeLine(true): content]
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="newline">Flag of new line symbol</param>
        /// <param name="pane">Selected pane</param>
        /// <returns></returns>
        [
            Method
            (
                "write",
                "Writes data into selected pane.",
                "item",
                "stItem",
                new string[] { "createIfNotExist", "In" },
                new string[] { "Flag of creating. If this value as true: Creates if this item does not exist.", "Content" },
                CValueType.Void,
                CValueType.Boolean, CValueType.Input
            )
        ]
        [
            Method
            (
                "writeLine",
                "Writes data with the newline char into selected pane.",
                "item",
                "stItem",
                new string[] { "createIfNotExist", "In" },
                new string[] { "Flag of creating. If this value as true: Creates if this item does not exist.", "Content" },
                CValueType.Void,
                CValueType.Boolean, CValueType.Input
            )
        ]
        protected string stItemWrite(string name, bool newline, string raw)
        {
            Match m = Regex.Match(raw, 
                                    String.Format(@"
                                                    \(
                                                       {0}   #1 - flag ( createIfNotExist ) - if true value: creates if the item does not exist.
                                                    \)
                                                    \s*:(.*) #2 - content",
                                                    RPattern.BooleanContent
                                                 ), RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stItemWrite - '{0}'", raw);
            }

            bool createIfNotExist   = Value.toBoolean(m.Groups[1].Value);
            string content          = m.Groups[2].Value;

            EnvDTE.OutputWindowPane pane;
            if(!createIfNotExist)
            {
                try {
                    pane = env.OutputWindowPane.getByName(name, false);
                }
                catch(ArgumentException) {
                    throw new NotFoundException("OWPComponent: Item '{0}' does not exist. Note: use the 'createIfNotExist' flag for automatically creation.", name);
                }
            }
            else {
                pane = env.OutputWindowPane.getByName(name, true);
            }

            if(newline) {
                content += System.Environment.NewLine;
            }
            pane.OutputString(content);

            return String.Empty;
        }

        /// <summary>
        /// Removes selected pane.
        /// 
        /// * #[OWP item("name").delete = true]
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="name">Name of item</param>
        /// <returns></returns>
        [Property(
            "delete",
            "Removes pane. Returns false if this item not exist, and true value if is successfully deleted.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemDelete(string name, string raw)
        {
            Log.Trace("stItemDelete: started");
            if(!_booleanValueFromRaw(ref raw)) {
                return Value.from(false);
            }
            
            Log.Debug("stItemDelete: removing the item '{0}'", name);
            try {
                env.OutputWindowPane.deleteByName(name);
                return Value.from(true);
            }
            catch(ArgumentException) {
                return Value.from(false);
            }
        }

        /// <summary>
        /// Clear contents of item by name.
        /// 
        /// * #[OWP item("name").clear = true]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        [Property(
            "clear",
            "Clear contents of item. Returns false if this item not exist, and true value if is clean.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemClear(string name, string raw)
        {
            Log.Trace("stItemClear: started");
            if(!_booleanValueFromRaw(ref raw)) {
                return Value.from(false);
            }

            Log.Debug("stItemClear: clearing the item '{0}'", name);
            try {
                env.OutputWindowPane.getByName(name, false).Clear();
                return Value.from(true);
            }
            catch(ArgumentException) {
                return Value.from(false);
            }
        }

        /// <summary>
        /// Activate item by name.
        /// 
        /// * #[OWP item("name").activate = true]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="raw"></param>
        /// <returns></returns>
        [Property(
            "activate",
            "Activate(Display) item.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemActivate(string name, string raw)
        {
            Log.Trace("stItemActivate: started");
            if(!_booleanValueFromRaw(ref raw)) {
                return Value.from(false);
            }

            Log.Debug("stItemActivate: activation the item '{0}'", name);
            try {
                env.OutputWindowPane.getByName(name, false).Activate();
                return Value.from(true);
            }
            catch(ArgumentException) {
                return Value.from(false);
            }
        }

        /// <summary>
        /// Gets data from the output pane.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Method(
                "out",
                "Streaming of getting the mixed data from selected pane. Returns: partial raw data.", 
                new string[] { "name" }, 
                new string[] { "Name of pane" }, 
                CValueType.String, 
                CValueType.String
        )]
        [Method(
                "out",
                "Streaming of getting the mixed data from selected pane. Returns: partial raw data.",
                new string[] { "ident", "isGuid" },
                new string[] { "Identifier of pane", "Use Guid as identifier or as name of item" },
                CValueType.String,
                CValueType.String, CValueType.Boolean
        )]
        [Property("out", "Alias for: out(\"Build\").", CValueType.String)]
        [Property("All", "Get raw data if exists.", "out", "stOut", CValueType.String)]
        [Property("Warnings", "For work with warnings from received data. Also used as short alias for: Warnings.Raw", "out", "stOut", CValueType.String)]
        [Property("Raw", "Return the partial raw data with warning/s if an exists.", "Warnings", "stOut", CValueType.String)]
        [Property("Count", "Count of warnings from data", "Warnings", "stOut", CValueType.Integer)]
        [Property("Codes", "List of warnings from data as C4702,4505, ...", "Warnings", "stOut", CValueType.List)]
        [Property("Errors", "For work with errors from received data. Also used as short alias for: Errors.Raw:", "out", "stOut", CValueType.String)]
        [Property("Raw", "Return the partial raw data with errors if an exists", "Errors", "stOut", CValueType.String)]
        [Property("Count", "Count of errors from data", "Errors", "stOut", CValueType.Integer)]
        [Property("Codes", "List of errors from data as C4702,4505, ...", "Errors", "stOut", CValueType.List)]
        protected string stOut(string data)
        {
            IPM pm = new PM(data);

            if(!pm.Is(0, LevelType.Property, "out") && !pm.Is(0, LevelType.Method, "out")) {
                throw new SyntaxIncorrectException("Failed stOut - '{0}'", data);
            }

            string item = Settings._.DefaultOWPItem; // by default for all
            bool isGuid = false;

            if(pm.Is(0, LevelType.Method, "out"))
            {
                Argument[] args = pm.Levels[0].Args;
                if((args.Length < 1 || args.Length > 2)
                    || args[0].type != ArgumentType.StringDouble
                    || (args.Length == 2 && args[1].type != ArgumentType.Boolean))
                {
                    throw new InvalidArgumentException("stOut: incorrect arguments to `out(string ident [, boolean isGuid])`");
                }

                item    = (string)args[0].data;
                isGuid  = (args.Length == 2)? (bool)args[1].data : false; // optional isGuid param
            }

            Log.Trace("stOut: out = item('{0}'), isGuid('{1}')", item, isGuid);

            IItemEW ew = OWPItems._.getEW((isGuid)? new OWPIdent() { guid = item } : new OWPIdent() { item = item });
            string raw = StringHandler.escapeQuotes(ew.Raw);

            // #[OWP out.All] / #[OWP out] / #[OWP out("Build").All] / #[OWP out("Build")] ...
            if(pm.FinalEmptyIs(1, LevelType.Property, "All") || pm.FinalEmptyIs(1, LevelType.RightOperandEmpty)) {
                return raw;
            }

            // #[OWP out.Warnings.Count] ...
            if(pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Count")) {
                return Value.from(ew.WarningsCount);
            }

            // #[OWP out.Warnings.Codes] ...
            if(pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Codes")) {
                return Value.from(ew.Warnings);
            }

            // #[OWP out.Warnings.Raw] / #[OWP out.Warnings] ...
            if((pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Raw"))
                || pm.FinalEmptyIs(1, LevelType.Property, "Warnings"))
            {
                return (ew.IsWarnings)? raw : String.Empty;
            }

            // #[OWP out.Errors.Count] ...
            if(pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Count")) {
                return Value.from(ew.ErrorsCount);
            }

            // #[OWP out.Errors.Codes] ...
            if(pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Codes")) {
                return Value.from(ew.Errors);
            }

            // #[OWP out.Errors.Raw] / #[OWP out.Errors] ...
            if((pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Raw"))
                || pm.FinalEmptyIs(1, LevelType.Property, "Errors"))
            {
                return (ew.IsErrors)? raw : String.Empty;
            }
            
            throw new OperationNotFoundException("stOut: not found - '{0}' /'{1}'", pm.Levels[1].Data, pm.Levels[1].Type);
        }

        private bool _booleanValueFromRaw(ref string raw)
        {
            Match m = Regex.Match(raw, @"=\s*(false|true|1|0)\s*$", RegexOptions.IgnoreCase);
            if(!m.Success) {
                throw new OperationNotFoundException("Failed getting boolean value - '{0}'", raw);
            }
            string val = m.Groups[1].Value;

            Log.Debug("Extracted boolean value: is '{0}'", val);
            return Value.toBoolean(val);
        }
    }
}
