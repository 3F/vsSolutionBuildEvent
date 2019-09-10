/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.VS contributors: https://github.com/3F/Varhead/graphs/contributors
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
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.VS.Owp;

namespace net.r_eg.SobaScript.Z.VS
{
    [Component("OWP", "For work with OWP (Output Window Pane)")]
    public class OwpComponent: ComponentAbstract, IComponent, ILogInfo
    {
        protected IOwpEnv env;

        private LogInfo logcopy;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "OWP ";

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data, RegexOptions.Singleline);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            switch(subtype)
            {
                case "out": {
                    return StOut(new PM(request));
                }
                case "log": {
                    return StLog(new PM(request));
                }
                case "item": {
                    return StItem(new PM(request));
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        /// <param name="message"></param>
        /// <param name="level"></param>
        public void UpdateLogInfo(string message, string level)
        {
            logcopy = new LogInfo() {
                Message = message,
                Level   = level
            };
        }

        public OwpComponent(ISobaScript soba, IOwpEnv env)
            : base(soba)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        /// <summary>
        ///     `log.Message`
        ///     `log.Level`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("log", "Provides data from events of logging.")]
        [Property("Message", "Current message from log.", "log", nameof(StLog), CValType.String)]
        [Property("Level", "The Level of current Message.", "log", nameof(StLog), CValType.String)]
        protected string StLog(IPM pm)
        {
            if(pm.It(LevelType.Property, "log"))
            {
                if(pm.It(LevelType.Property, "Message")) {
                    return Value.From(logcopy.Message);
                }

                if(pm.It(LevelType.Property, "Level")) {
                    return Value.From(logcopy.Level);
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
        [Method("item",
                "Access to item of the Output window by name.",
                new[] { "name" },
                new[] { "Name of item" },
                CValType.Void,
                CValType.String)]
        protected string StItem(IPM pm)
        {
            if(!pm.Is(LevelType.Method, "item")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel; // level of the item() method

            if(!level.Is(ArgumentType.StringDouble)) {
                throw new PMLevelException(level, "item(string name)");
            }
            string name = (string)level.Args[0].data;

            if(string.IsNullOrWhiteSpace(name) 
                /*|| name.Trim().Equals(Settings.OWP_ITEM_VSSBE, StringComparison.OrdinalIgnoreCase)*/)
            {
                throw new NotSupportedOperationException($"The OW pane '{name}' is not supported for current operation.");
            }

            pm.PinTo(1);
            switch(pm.FirstLevel.Data)
            {
                case "write": {
                    return StItemWrite(name, false, pm);
                }
                case "writeLine": {
                    return StItemWrite(name, true, pm);
                }
                case "delete": {
                    return StItemDelete(name, pm);
                }
                case "clear": {
                    return StItemClear(name, pm);
                }
                case "activate": {
                    return StItemActivate(name, pm);
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
        [Method("write",
                "Writes data into selected pane.",
                "item",
                nameof(StItem),
                new[] { "force", "In" },
                new[] { "Creates selected item if it does not exist for true value.", "Content" },
                CValType.Void,
                CValType.Boolean, CValType.Input)]
        [Method("writeLine",
                "Writes data with the newline char into selected pane.",
                "item",
                nameof(StItem),
                new[] { "force", "In" },
                new[] { "Creates selected item if it does not exist for true value.", "Content" },
                CValType.Void,
                CValType.Boolean, CValType.Input)]
        protected string StItemWrite(string name, bool newline, IPM pm)
        {
            if(!pm.IsMethodWithArgs("write", ArgumentType.Boolean) 
                && !pm.IsMethodWithArgs("writeLine", ArgumentType.Boolean))
            {
                throw new IncorrectNodeException(pm);
            }

            bool createIfNo = (bool)pm.FirstLevel.Args[0].data;

            if(pm.Levels[1].Type != LevelType.RightOperandColon) {
                throw new IncorrectNodeException(pm);
            }

            string content = pm.Levels[1].Data;

            env.Write(content, newline, name, createIfNo);
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
            "StItem",
            CValType.Boolean,
            CValType.Boolean)]
        protected string StItemDelete(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "delete") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.ToBoolean(pm.FirstLevel.Data))
            {
#if DEBUG
                LSender.Send(this, $"skip removing '{name}'", MsgLevel.Trace);
#endif
                return Value.From(false);
            }

            LSender.Send(this, $"removing the item '{name}'");
            return Value.From(env.Delete(name));
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
            nameof(StItem),
            CValType.Boolean,
            CValType.Boolean)]
        protected string StItemClear(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "clear") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.ToBoolean(pm.FirstLevel.Data))
            {
#if DEBUG
                LSender.Send(this, $"skip clearing '{name}'", MsgLevel.Trace);
#endif
                return Value.From(false);
            }

            LSender.Send(this, $"Clearing the item '{name}'");
            return Value.From(env.Clear(name));
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
            nameof(StItem),
            CValType.Boolean,
            CValType.Boolean)]
        protected string StItemActivate(string name, IPM pm)
        {
            if(!pm.It(LevelType.Property, "activate") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(!Value.ToBoolean(pm.FirstLevel.Data))
            {
#if DEBUG
                LSender.Send(this, $"skip activation of pane '{name}'", MsgLevel.Trace);
#endif
                return Value.From(false);
            }

            LSender.Send(this, $"Activation the item '{name}'");
            return Value.From(env.Activate(name));
        }

        /// <summary>
        /// Gets data from the output pane.
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("out",
                "Streaming of getting the mixed data from selected pane. Returns: partial raw data.", 
                new[] { "name" }, 
                new[] { "Name of pane" }, 
                CValType.String, 
                CValType.String)]
        [Method("out",
                "Streaming of getting the mixed data from selected pane. Returns: partial raw data.",
                new[] { "ident", "isGuid" },
                new[] { "Identifier of pane", "Use Guid as identifier or as name of item" },
                CValType.String,
                CValType.String, CValType.Boolean)]
        [Property("out", "Alias for: out(\"Build\").", CValType.String)]
        [Property("All", "Get raw data if exists.", "out", nameof(StOut), CValType.String)]
        [Property("Warnings", "For work with warnings from received data. Also used as short alias for: Warnings.Raw", "out", nameof(StOut), CValType.String)]
        [Property("Raw", "Return the partial raw data with warning/s if an exists.", "Warnings", nameof(StOut), CValType.String)]
        [Property("Count", "Count of warnings from data", "Warnings", nameof(StOut), CValType.Integer)]
        [Property("Codes", "List of warnings from data as C4702,4505, ...", "Warnings", nameof(StOut), CValType.List)]
        [Property("Errors", "For work with errors from received data. Also used as short alias for: Errors.Raw:", "out", nameof(StOut), CValType.String)]
        [Property("Raw", "Return the partial raw data with errors if an exists", "Errors", nameof(StOut), CValType.String)]
        [Property("Count", "Count of errors from data", "Errors", nameof(StOut), CValType.Integer)]
        [Property("Codes", "List of errors from data as C4702,4505, ...", "Errors", nameof(StOut), CValType.List)]
        protected string StOut(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "out") && !pm.Is(LevelType.Method, "out")) {
                throw new IncorrectNodeException(pm);
            }

            string item = env.DefaultItem; // by default for all
            bool isGuid = false;

            if(pm.Is(LevelType.Method, "out"))
            {
                ILevel lvlOut = pm.FirstLevel;

                if(!lvlOut.Is(ArgumentType.StringDouble)
                    && !lvlOut.Is(ArgumentType.StringDouble, ArgumentType.Boolean))
                {
                    throw new PMLevelException(lvlOut, "`out(string ident [, boolean isGuid])`");
                }

                var args = lvlOut.Args;

                item    = (string)args[0].data;
                isGuid  = args.Length == 2 ? (bool)args[1].data : false; // optional isGuid param
            }

            LSender.Send(this, $"StOut: out = item('{item}'), isGuid('{isGuid}')", MsgLevel.Trace);

            IEWData ew  = env.GetEWData(item, isGuid);
            string raw  = StringHandler.EscapeQuotes(ew.Raw);

            // #[OWP out.All] / #[OWP out] / #[OWP out("Build").All] / #[OWP out("Build")] ...
            if(pm.FinalEmptyIs(1, LevelType.Property, "All") || pm.FinalEmptyIs(1, LevelType.RightOperandEmpty)) {
                return raw;
            }

            // #[OWP out.Warnings.Count] ...
            if(pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Count")) {
                return Value.From(ew.Warnings.Count);
            }

            // #[OWP out.Warnings.Codes] ...
            if(pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Codes")) {
                return Value.From(ew.Warnings);
            }

            // #[OWP out.Warnings.Raw] / #[OWP out.Warnings] ...
            if((pm.Is(1, LevelType.Property, "Warnings") && pm.FinalEmptyIs(2, LevelType.Property, "Raw"))
                || pm.FinalEmptyIs(1, LevelType.Property, "Warnings"))
            {
                return ew.Warnings.Count > 0 ? raw : Value.Empty;
            }

            // #[OWP out.Errors.Count] ...
            if(pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Count")) {
                return Value.From(ew.Errors.Count);
            }

            // #[OWP out.Errors.Codes] ...
            if(pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Codes")) {
                return Value.From(ew.Errors);
            }

            // #[OWP out.Errors.Raw] / #[OWP out.Errors] ...
            if((pm.Is(1, LevelType.Property, "Errors") && pm.FinalEmptyIs(2, LevelType.Property, "Raw"))
                || pm.FinalEmptyIs(1, LevelType.Property, "Errors"))
            {
                return ew.Errors.Count > 0 ? raw : Value.Empty;
            }

            throw new IncorrectNodeException(pm, 1);
        }
    }
}
