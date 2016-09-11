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

        /// <summary>
        /// OutputWindowPane
        /// </summary>
        protected virtual IOW OWP
        {
            get {
                if(env.OutputWindowPane == null) {
                    throw new NotSupportedException("The OW pane is not available for current environment.");
                }
                return env.OutputWindowPane;
            }
        }

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
            var point       = entryPoint(data, RegexOptions.Singleline);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            switch(subtype) {
                case "out": {
                    return stOut(new PM(request));
                }
                case "log": {
                    return stLog(new PM(request));
                }
                case "item": {
                    return stItem(new PM(request));
                }
            }
            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        /// <summary>
        /// For work with events of logging.
        ///     `log.Message`
        ///     `log.Level`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("log", "Provides data from events of logging.")]
        [Property("Message", "Current message from log.", "log", "stLog", CValueType.String)]
        [Property("Level", "The Level of current Message.", "log", "stLog", CValueType.String)]
        protected string stLog(IPM pm)
        {
            if(pm.It(LevelType.Property, "log"))
            {
                if(pm.It(LevelType.Property, "Message")) {
                    return Value.from(logcopy.Message);
                }

                if(pm.It(LevelType.Property, "Level")) {
                    return Value.from(logcopy.Level);
                }

                throw new IncorrectNodeException(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Access to OW items.
        ///     `item("name")`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method(
                "item",
                "Access to item of the Output window by name.",
                new string[] { "name" },
                new string[] { "Name of item" },
                CValueType.Void,
                CValueType.String
        )]
        protected string stItem(IPM pm)
        {
            if(!pm.Is(LevelType.Method, "item")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel; // level of the item() method

            if(!level.Is(ArgumentType.StringDouble)) {
                throw new ArgumentPMException(level, "item(string name)");
            }
            string name = (string)level.Args[0].data;

            if(String.IsNullOrWhiteSpace(name) 
                /*|| name.Trim().Equals(Settings.OWP_ITEM_VSSBE, StringComparison.OrdinalIgnoreCase)*/)
            {
                throw new NotSupportedOperationException("The OW pane '{0}' is not available for current operation.", name);
            }

            pm.pinTo(1);
            switch(pm.FirstLevel.Data)
            {
                case "write": {
                    return stItemWrite(name, false, pm);
                }
                case "writeLine": {
                    return stItemWrite(name, true, pm);
                }
                case "delete": {
                    return stItemDelete(name, pm);
                }
                case "clear": {
                    return stItemClear(name, pm);
                }
                case "activate": {
                    return stItemActivate(name, pm);
                }
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// To send data into OutputWindowPane window.
        ///     `item("name").write(true): content`
        ///     `item("name").writeLine(true): content`
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newline">Flag of new line symbol</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method(
                "write",
                "Writes data into selected pane.",
                "item",
                "stItem",
                new string[] { "force", "In" },
                new string[] { "Creates selected item if it does not exist for true value.", "Content" },
                CValueType.Void,
                CValueType.Boolean, CValueType.Input
        )]
        [Method(
                "writeLine",
                "Writes data with the newline char into selected pane.",
                "item",
                "stItem",
                new string[] { "force", "In" },
                new string[] { "Creates selected item if it does not exist for true value.", "Content" },
                CValueType.Void,
                CValueType.Boolean, CValueType.Input
        )]
        protected string stItemWrite(string name, bool newline, IPM pm)
        {
            if(!pm.IsMethodWithArgs("write", ArgumentType.Boolean) 
                && !pm.IsMethodWithArgs("writeLine", ArgumentType.Boolean))
            {
                throw new IncorrectNodeException(pm);
            }
            bool createIfNotExist = (bool)pm.FirstLevel.Args[0].data;

            if(pm.Levels[1].Type != LevelType.RightOperandColon) {
                throw new IncorrectNodeException(pm);
            }

            string content = pm.Levels[1].Data;

            EnvDTE.OutputWindowPane pane;
            if(!createIfNotExist)
            {
                try {
                    pane = OWP.getByName(name, false);
                }
                catch(ArgumentException) {
                    throw new NotFoundException("The item '{0}' does not exist. Use 'force' flag for automatic creation if needed.", name);
                }
            }
            else {
                pane = OWP.getByName(name, true);
            }

            if(newline) {
                content += System.Environment.NewLine;
            }
            pane.OutputString(content);

            return Value.Empty;
        }

        /// <summary>
        /// Removes selected pane.
        ///     `item("name").delete = true`
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property(
            "delete",
            "Removes pane. Returns false value if item does not exist, otherwise true as a successfully deleted.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemDelete(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "delete") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.toBoolean(pm.FirstLevel.Data)) {
#if DEBUG
                Log.Trace("skip removing '{0}'", name);
#endif
                return Value.from(false);
            }

            Log.Debug("removing the item '{0}'", name);
            try {
                OWP.deleteByName(name);
                return Value.from(true);
            }
            catch(ArgumentException) {
                Log.Debug("Incorrect name of pane item `{0}`", name);
            }

            return Value.from(false);
        }

        /// <summary>
        /// To clear contents from pane.
        ///     `item("name").clear = true`
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property(
            "clear",
            "To clear contents from OW pane. Returns false value if item does not exist, otherwise true as a successfully cleared.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemClear(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "clear") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.toBoolean(pm.FirstLevel.Data)) {
#if DEBUG
                Log.Trace("skip clearing '{0}'", name);
#endif
                return Value.from(false);
            }

            Log.Debug("Clearing the item '{0}'", name);
            try {
                OWP.getByName(name, false).Clear();
                return Value.from(true);
            }
            catch(ArgumentException) {
                Log.Debug("Incorrect name of pane item `{0}`", name);
            }

            return Value.from(false);
        }

        /// <summary>
        /// Activate item by name.
        ///     `item("name").activate = true`
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property(
            "activate",
            "To activate (display) OW pane by item name.",
            "item",
            "stItem",
            CValueType.Boolean,
            CValueType.Boolean
        )]
        protected string stItemActivate(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "activate") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.toBoolean(pm.FirstLevel.Data)) {
#if DEBUG
                Log.Trace("skip activation of pane '{0}'", name);
#endif
                return Value.from(false);
            }

            Log.Debug("Activation the item '{0}'", name);
            try {
                OWP.getByName(name, false).Activate();
                return Value.from(true);
            }
            catch(ArgumentException) {
                Log.Debug("Incorrect name of pane item `{0}`", name);
            }

            return Value.from(false);
        }

        /// <summary>
        /// Gets data from the output pane.
        /// </summary>
        /// <param name="pm"></param>
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
        protected string stOut(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "out") && !pm.Is(LevelType.Method, "out")) {
                throw new IncorrectNodeException(pm);
            }

            string item = Settings._.DefaultOWPItem; // by default for all
            bool isGuid = false;

            if(pm.Is(LevelType.Method, "out"))
            {
                ILevel lvlOut = pm.FirstLevel;
                if(!lvlOut.Is(ArgumentType.StringDouble)
                    && !lvlOut.Is(ArgumentType.StringDouble, ArgumentType.Boolean))
                {
                    throw new InvalidArgumentException("Incorrect arguments to `out(string ident [, boolean isGuid])`");
                }
                Argument[] args = lvlOut.Args;

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
                return (ew.IsWarnings)? raw : Value.Empty;
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
                return (ew.IsErrors)? raw : Value.Empty;
            }

            throw new IncorrectNodeException(pm, 1);
        }
    }
}
