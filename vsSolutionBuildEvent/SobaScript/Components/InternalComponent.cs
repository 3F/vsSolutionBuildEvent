/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Ext.IO;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.SobaScript.Components
{
    /// <summary>
    /// Internal operations with vsSBE.
    /// </summary>
    [Component("vsSBE", new[] { "Core" }, "Internal operations with vsSBE")]
    public class InternalComponent: ComponentAbstract, IComponent
    {
        protected IEnvironment env;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => @"(?:vsSBE|Core)\s";

        /// <summary>
        /// Use regex engine
        /// </summary>
        public override bool ARegex => true;

        public IExer Exer
        {
            get;
            protected set;
        }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace($"`{ToString()}`: subtype - `{subtype}`, request - `{request}`");

            IPM pm = new PM(request);

            switch(subtype)
            {
                case "StartUpProject": {
                    return stStartUpProject(pm);
                }
                case "events": {
                    return stEvents(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public InternalComponent(ISobaScript soba, IEnvironment env)
            : this(soba, env, null)
        {

        }

        public InternalComponent(ISobaScript soba, IEnvironment env, IExer exer)
            : base(soba)
        {
            this.env    = env ?? throw new ArgumentNullException(nameof(env));
            Exer        = exer;
        }

        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("StartUpProject", "To get/set the project by default or 'StartUp Project'.", CValType.String, CValType.String)]
        protected string stStartUpProject(IPM pm)
        {
            if(!pm.It(LevelType.Property, "StartUpProject")) {
                throw new IncorrectNodeException(pm);
            }

            // get
            if(pm.IsRight(LevelType.RightOperandEmpty)) {
                return env.StartupProjectString;
            }

            // set
            if(!pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            ILevel level    = pm.FirstLevel;
            var val         = new PM().GetArguments(level.Data);

            if(val == null || val.Count < 1)
            {
                env.updateStartupProject(null);
                return Value.Empty;
            }

            Argument pname = val[0];
            if(val.Count > 1 || 
                (pname.type != ArgumentType.StringDouble
                    && pname.type != ArgumentType.EnumOrConst
                    && pname.type != ArgumentType.Mixed))
            {
                throw new PMLevelException(level, "= string name");
            }

            env.updateStartupProject(pname.data.ToString());
            return Value.Empty;
        }

        /// <summary>
        /// Work with the events node - `events.Type`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("events", "Work with events")]
        [Property("Pre", "Pre-Build\nBefore build", "events", nameof(stEvents)), Property("", "Pre", nameof(stEvents))]
        [Property("Post", "Post-Build\nAfter build", "events", nameof(stEvents)), Property("", "Post", nameof(stEvents))]
        [Property("Cancel", "Cancel-Build\nby user or when occurs error", "events", nameof(stEvents)), Property("", "Cancel", nameof(stEvents))]
        [Property("CommandEvent", "CommandEvent (DTE)\nThe Command Events from EnvDTE", "events", nameof(stEvents)), Property("", "CommandEvent", nameof(stEvents))]
        [Property("Warnings", "Warnings-Build\nWarnings during assembly processing", "events", nameof(stEvents)), Property("", "Warnings", nameof(stEvents))]
        [Property("Errors", "Errors-Build\nErrors during assembly processing", "events", nameof(stEvents)), Property("", "Errors", nameof(stEvents))]
        [Property("OWP", "Output-Build\nCustomization and full control by using listener", "events", nameof(stEvents)), Property("", "OWP", nameof(stEvents))]
        [Property("SlnOpened", "Sln-Opened\nWhen solution has been opened", "events", nameof(stEvents)), Property("", "SlnOpened", nameof(stEvents))]
        [Property("SlnClosed", "Sln-Closed\nWhen solution has been closed", "events", nameof(stEvents)), Property("", "SlnClosed", nameof(stEvents))]
        [Property("Transmitter", "Transmitter\nTransmission of the build-data to outer handler", "events", nameof(stEvents)), Property("", "Transmitter", nameof(stEvents))]
        [Property("Logging", "Logging\nAll processes with internal logging", "events", nameof(stEvents)), Property("", "Logging", nameof(stEvents))]
        protected string stEvents(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "events")) {
                throw new IncorrectNodeException(pm);
            }

            ILevel etype = pm.Levels[1];
            if(etype.Type != LevelType.Property) {
                throw new IncorrectNodeException(pm, 1);
            }

            if(pm.Is(2, LevelType.Method, "item"))
            {
                SolutionEventType type;
                try {
                    type = (SolutionEventType)Enum.Parse(typeof(SolutionEventType), etype.Data);
                }
                catch(ArgumentException) {
                    throw new OperandNotFoundException(etype.Data);
                }
                return stEventItem(type, pm.PinTo(2));
            }

            throw new IncorrectNodeException(pm, 2);
        }

        /// <summary>
        /// `events.Type.item(string name | integer index)`
        /// </summary>
        /// <param name="type">Type of available events</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("item",
                "Access to action by name", 
                "", nameof(stEvents), 
                new[] { "name" }, 
                new[] { "Name of the action" }, 
                CValType.Void, 
                CValType.String)]
        [Method("item",
                "Access to action by index", 
                "", 
                nameof(stEvents), 
                new[] { "index" }, 
                new[] { "Index of the action >= 1" },
                CValType.Void, 
                CValType.Integer)]
        protected string stEventItem(SolutionEventType type, IPM pm)
        {
            ILevel level = pm.FirstLevel;

            int index = -1;
            ISolutionEvent evt;

            if(level.Is(ArgumentType.StringDouble)) {
                evt = getEventByName(type, (string)level.Args[0].data, out index);
            }
            else if(level.Is(ArgumentType.Integer))
            {
                index   = (int)level.Args[0].data;
                evt     = getEventByIndex(type, index);
            }
            else {
                throw new PMLevelException(level, "`item( string name | integer index )`");
            }

            // .item(...).

            if(pm.Is(1, LevelType.Property, "Enabled")) {
                return pEnabled(evt, pm.PinTo(2));
            }
            if(pm.Is(1, LevelType.Method, "run")) {
                return mActionRun(type, evt, pm.PinTo(1));
            }
            if(pm.Is(1, LevelType.Property, "Status")) {
                return itemStatus(type, index, pm.PinTo(1));
            }
            if(pm.Is(1, LevelType.Property, "stdout")) {
                return pStdout(evt, pm.PinTo(2));
            }
            if(pm.Is(1, LevelType.Property, "stderr")) {
                return pStderr(evt, pm.PinTo(2));
            }

            throw new IncorrectNodeException(pm, 1);
        }

        /// <summary>
        /// `item(...).Status`
        /// `item(...).Status.HasErrors`
        /// </summary>
        /// <param name="type">Selected event type.</param>
        /// <param name="index">Access to action by index.</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("Status", "Available states for selected event-action.", "item", nameof(stEventItem))]
        [Property("HasErrors", "Checking existence of errors after executed action for selected event-action.", "Status", nameof(itemStatus), CValType.Boolean)]
        protected string itemStatus(SolutionEventType type, int index, IPM pm)
        {
            if(!pm.Is(LevelType.Property, "Status")) {
                throw new IncorrectNodeException(pm);
            }

            if(pm.FinalEmptyIs(1, LevelType.Property, "HasErrors"))
            {
                string status = Value.From(Status._.get(type, index) == StatusType.Fail);
#if DEBUG
                Log.Trace($"pStatus: status - '{status}'");
#endif
                return status;
            }

            throw new IncorrectNodeException(pm, 1);
        }

        [Property("stdout", "Get data from stdout for action which is executed asynchronously.", "item", nameof(stEventItem), CValType.String)]
        protected string pStdout(ISolutionEvent evt, IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.RightOperandEmpty)) {
                return Value.From(Exer?.PullStdOut(evt.Id));
            }

            throw new IncorrectNodeException(pm);
        }

        [Property("stderr", "Get data from stderr for action which is executed asynchronously.", "item", nameof(stEventItem), CValType.String)]
        protected string pStderr(ISolutionEvent evt, IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.RightOperandEmpty)) {
                return Value.From(Exer?.PullStdErr(evt.Id));
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// item(...).Enabled
        /// item(...).Enabled = true|false
        /// </summary>
        /// <param name="evt">Selected event</param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("Enabled", "Gets or Sets Enabled status for selected event-action", "item", nameof(stEventItem), CValType.Boolean, CValType.Boolean)]
        protected string pEnabled(ISolutionEvent evt, IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.RightOperandEmpty)) {
                return Value.From(evt.Enabled);
            }

            evt.Enabled = Value.ToBoolean(pm.FirstLevel.Data);

            Log.Trace($"pEnabled: updated status '{evt.Enabled}' for '{evt.Name}'");
            return Value.Empty;
        }

        [Method("run",
                "Execute Action with specific context. Returns true value if it was handled.",
                "item",
                nameof(stEventItem), 
                new[] { "context" }, 
                new[] { "Specific context." },
                CValType.Boolean, 
                CValType.Enum)]
        [Method("run",
                "Execute Action. Returns true value if it was handled.",
                "item",
                nameof(stEventItem), 
                new[] { "" }, 
                new[] { "" },
                CValType.Boolean, 
                CValType.Void)]
        protected string mActionRun(SolutionEventType type, ISolutionEvent evt, IPM pm)
        {
            if(!pm.FinalEmptyIs(LevelType.Method, "run")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            BuildType buildType;
            if(level.Args == null || level.Args.Count < 1) {
                buildType = BuildType.Common;
            }
            else if(level.Is(ArgumentType.EnumOrConst)) {
                buildType = (BuildType)Enum.Parse(typeof(BuildType), (string)level.Args[0].data);
            }
            else {
                throw new PMLevelException(level, "run([enum context])");
            }

            ICommand cmd = new Actions.Command(env, soba, emsbuild);
            Log.Info($"Execute action by user-script: '{evt.Name}'(context: {buildType}) /as '{type}' event");

            cmd.Env.BuildType = buildType;
            return Value.From(cmd.exec(evt, type));
        }

        protected virtual ISolutionEvent[] getEvent(SolutionEventType type)
        {
            try {
                return Settings.Cfg.getEvent(type); //TODO:
            }
            catch(NotFoundException) {
                throw new NotSupportedOperationException($"The event type '{type}' is not supported yet.");
            }
        }

        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="index">Index of selected type of event by name</param>
        /// <returns></returns>
        private ISolutionEvent getEventByName(SolutionEventType type, string name, out int index)
        {
            if(string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException(nameof(name));
            }

            index = -1;
            foreach(ISolutionEvent item in getEvent(type))
            {
                ++index;
                if(item.Name == name) {
                    return item;
                }
            }

            throw new NotFoundException(name, $"Event type `{type}`.", type);
        }

        private ISolutionEvent getEventByIndex(SolutionEventType type, int index)
        {
            ISolutionEvent[] evt = getEvent(type);
            try {
                return evt[index - 1]; // starts with 1
            }
            catch(IndexOutOfRangeException) {
                throw new NotFoundException(type, $"For index '{index}'  /{evt.Length}", index, evt.Length);
            }
        }
    }
}
