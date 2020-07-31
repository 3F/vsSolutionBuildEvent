/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.WForms.Controls;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
    using CEAfterEventHandler = EnvDTE._dispCommandEvents_AfterExecuteEventHandler;
    using CEBeforeEventHandler = EnvDTE._dispCommandEvents_BeforeExecuteEventHandler;

    internal class Events
    {
        /// <summary>
        /// Prefix for new action by default.
        /// </summary>
        public const string ACTION_PREFIX       = "Act";

        /// <summary>
        /// Prefix for cloned action by default.
        /// </summary>
        public const string ACTION_PREFIX_CLONE = "CopyOf";

        /// <summary>
        /// Mapper of the available components.
        /// </summary>
        protected IInspector inspector;

        /// <summary>
        /// Registered used SBE-events
        /// </summary>
        protected List<SBEWrap> events = new List<SBEWrap>();

        /// <summary>
        /// Selected event
        /// </summary>
        protected volatile int currentEventIndex = 0;

        /// <summary>
        /// Selected item of event
        /// </summary>
        protected volatile int currentEventItem = 0;

        /// <summary>
        /// List of available types of the build
        /// </summary>
        protected List<BuildType> buildType = new List<BuildType>();

        /// <summary>
        /// Backup of settings.
        /// </summary>
        protected RestoreData backup = new RestoreData();

        /// <summary>
        /// Information by existing components
        /// </summary>
        protected Dictionary<string, List<INodeInfo>> cInfo = new Dictionary<string, List<INodeInfo>>();

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        protected EnvDTE.CommandEvents cmdEvents;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Provides operations with environment
        /// </summary>
        public IEnvironment Env
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used loader
        /// </summary>
        public Bootloader Loader
        {
            get;
            protected set;
        }

        /// <summary>
        /// Manager of configurations.
        /// </summary>
        public Configuration.IManager CfgManager
        {
            get {
                return Settings.CfgManager;
            }
        }

        /// <summary>
        /// Current SBE-event
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public SBEWrap SBE
        {
            get { return events[currentEventIndex]; }
        }

        /// <summary>
        /// Current item of SBE
        /// </summary>
        public ISolutionEvent SBEItem
        {
            get {
                if(SBE.evt.Count < 1) {
                    return null;
                }
                return SBE.evt[Math.Max(0, Math.Min(currentEventItem, SBE.evt.Count - 1))];
            }
        }

        /// <summary>
        /// Initialize the mode with end-type
        /// </summary>
        public virtual IMode DefaultMode
        {
            get { return new ModeFile(); }
        }

        /// <summary>
        /// Access to available events.
        /// </summary>
        public ISolutionEvents SlnEvents
        {
            get { return Settings.Cfg; }
        }

        /// <summary>
        /// Next unique name for action
        /// </summary>
        public string UniqueNameForAction
        {
            get {
                return genUniqueName(ACTION_PREFIX, SBE.evt);
            }
        }

        /// <summary>
        /// Predefined operations
        /// TODO:
        /// </summary>
        public List<ModeOperation> DefOperations
        {
            get { return defOperations; }
        }
        protected List<ModeOperation> defOperations = DefCommandsDTE.operations();

        /// <param name="evt"></param>
        public void addEvent(SBEWrap evt)
        {
            events.Add(evt);
        }

        /// <param name="index">Selected event</param>
        /// <param name="item">Selected item of event</param>
        public void setEventIndexes(int index, int item)
        {
            currentEventIndex   = Math.Max(0, Math.Min(index, events.Count - 1));
            currentEventItem    = Math.Max(0, Math.Min(item, SBE.evt.Count - 1));
        }

        public void updateInfo(int index, string name, bool enabled)
        {
            SBE.evt[index].Name = name;
            SBE.evt[index].Enabled = enabled;
        }

        public void updateInfo(int index, SBEEvent evt)
        {
            SBE.evt[index] = evt;
        }

        /// <summary>
        /// Initialize the new Mode by type
        /// </summary>
        /// <param name="type">Available Modes</param>
        /// <returns>Mode with default values</returns>
        public IMode initMode(ModeType type)
        {
            switch(type) {
                case ModeType.Interpreter: {
                    return new ModeInterpreter();
                }
                case ModeType.File: {
                    return new ModeFile();
                }
                case ModeType.Script: {
                    return new ModeScript();
                }
                case ModeType.Targets: {
                    return new ModeTargets();
                }
                case ModeType.CSharp: {
                    return new ModeCSharp();
                }
                case ModeType.Operation: {
                    return new ModeOperation();
                }
            }
            return DefaultMode;
        }

        public bool isDefOperation(string caption)
        {
            foreach(ModeOperation operation in DefOperations) {
                if(operation.Caption == caption) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Getting the operations as array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string[] splitOperations(string data)
        {
            return data.Replace("\r\n", "\n").Split('\n');
        }

        /// <summary>
        /// Getting the operations as string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string joinOperations(string[] data)
        {
            return String.Join("\n", data);
        }

        public virtual string formatMSBuildProperty(string name, string project = null)
        {
            if(project == null) {
                return String.Format("$({0})", name);
            }
            return String.Format("$({0}:{1})", name, project);
        }

        public string genUniqueName(string prefix, List<ISolutionEvent> scope)
        {
            int id = getUniqueId(prefix, scope);
            return String.Format("{0}{1}", prefix, (id < 1) ? "" : id.ToString());
        }

        public virtual string validateName(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                return UniqueNameForAction;
            }

            name = Regex.Replace(name, 
                                    @"(?:
                                            ^([^a-z]+)
                                        |
                                            ([^a-z_0-9]+)
                                        )", 
                                    delegate(Match m) { return String.Empty; }, 
                                    RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            
            return String.IsNullOrEmpty(name)? UniqueNameForAction : name;
        }

        public void saveData()
        {
            CfgManager.UserConfig.save();
            CfgManager.Config.save(); // all changes has been passed by reference
            backupUpdate();
        }

        public void restoreData()
        {
            if(Settings.CfgUser != null) {
                Settings.CfgUser.avoidRemovingFromCache();
            }
            backupRestore();
        }

        /// <summary>
        /// Clone configuration from specific context into current.
        /// </summary>
        /// <param name="from">Clone from this context.</param>
        public void cloneCfg(ContextType from)
        {
            CfgManager.Config.load(CfgManager.getConfigFor(from).Data.CloneBySerializationWithType<ISolutionEvents, SolutionEvents>());
            CfgManager.UserConfig.load(CfgManager.getUserConfigFor(from).Data.CloneBySerializationWithType<IData, Data>());
        }

        /// <summary>
        /// Update of backup copies of user config for actual context.
        /// </summary>
        public void updateUserCfg()
        {
            backup.update(CfgManager.UserConfig.Data.CloneBySerializationWithType<IData, Data>(), CfgManager.Context);
        }

        public void fillEvents(ComboBox combo)
        {
            events.Clear();
            combo.Items.Clear();

            addEvent(new SBEWrap(SolutionEventType.Pre));
            combo.Items.Add(":: Pre-Build :: Before build");

            addEvent(new SBEWrap(SolutionEventType.Post));
            combo.Items.Add(":: Post-Build :: After build");

            addEvent(new SBEWrap(SolutionEventType.Cancel));
            combo.Items.Add(":: Cancel-Build :: by user or when occurs error");

            addEvent(new SBEWrap(SolutionEventType.CommandEvent));
            combo.Items.Add(":: CommandEvent (DTE) :: The Command Events from EnvDTE");

            addEvent(new SBEWrap(SolutionEventType.Warnings));
            combo.Items.Add(":: Warnings-Build :: Warnings during assembly processing");

            addEvent(new SBEWrap(SolutionEventType.Errors));
            combo.Items.Add(":: Errors-Build :: Errors during assembly processing");

            addEvent(new SBEWrap(SolutionEventType.OWP));
            combo.Items.Add(":: Output-Build :: Customization and full control by using listener");

            addEvent(new SBEWrap(SolutionEventType.SlnOpened));
            combo.Items.Add(":: Sln-Opened :: When solution has been opened");

            addEvent(new SBEWrap(SolutionEventType.SlnClosed));
            combo.Items.Add(":: Sln-Closed :: When solution has been closed");

            addEvent(new SBEWrap(SolutionEventType.Transmitter));
            combo.Items.Add(":: Transmitter :: Transmission of the build-data to outer handler");

            addEvent(new SBEWrap(SolutionEventType.Logging));
            combo.Items.Add(":: Logging :: All processes with internal logging");

            combo.SelectedIndex = 0;
        }

        public void fillBuildTypes(ComboBox combo)
        {
            buildType.Clear();
            combo.Items.Clear();

            buildType.Add(BuildType.Common);
            combo.Items.Add("");

            buildType.Add(BuildType.Build);
            combo.Items.Add("Build");

            buildType.Add(BuildType.Rebuild);
            combo.Items.Add("Rebuild");

            buildType.Add(BuildType.Clean);
            combo.Items.Add("Clean");

            buildType.Add(BuildType.Deploy);
            combo.Items.Add("Deploy");

            buildType.Add(BuildType.Before);
            combo.Items.Add("Before");

            buildType.Add(BuildType.After);
            combo.Items.Add("After");

            buildType.Add(BuildType.BeforeAndAfter);
            combo.Items.Add("Before & After");

            buildType.Add(BuildType.Start);
            combo.Items.Add("Start Debugging");

            buildType.Add(BuildType.StartNoDebug);
            combo.Items.Add("Start Without Debugging");

            buildType.Add(BuildType.Publish);
            combo.Items.Add("Publish");

            buildType.Add(BuildType.BuildSelection);
            combo.Items.Add("Build Selection");

            buildType.Add(BuildType.RebuildSelection);
            combo.Items.Add("Rebuild Selection");

            buildType.Add(BuildType.CleanSelection);
            combo.Items.Add("Clean Selection");

            buildType.Add(BuildType.DeploySelection);
            combo.Items.Add("Deploy Selection");

            buildType.Add(BuildType.PublishSelection);
            combo.Items.Add("Publish Selection");

            buildType.Add(BuildType.BuildOnlyProject);
            combo.Items.Add("Build Project");

            buildType.Add(BuildType.RebuildOnlyProject);
            combo.Items.Add("Rebuild Project");

            buildType.Add(BuildType.CleanOnlyProject);
            combo.Items.Add("Clean Project");

            buildType.Add(BuildType.Compile);
            combo.Items.Add("Compile");

            buildType.Add(BuildType.LinkOnly);
            combo.Items.Add("Link Only");

            buildType.Add(BuildType.BuildCtx);
            combo.Items.Add("BuildCtx");

            buildType.Add(BuildType.RebuildCtx);
            combo.Items.Add("RebuildCtx");

            buildType.Add(BuildType.CleanCtx);
            combo.Items.Add("CleanCtx");

            buildType.Add(BuildType.DeployCtx);
            combo.Items.Add("DeployCtx");

            buildType.Add(BuildType.PublishCtx);
            combo.Items.Add("PublishCtx");

            combo.SelectedIndex = 0;
        }

        public int getBuildTypeIndex(BuildType type)
        {
            return buildType.IndexOf(type);
        }

        public BuildType getBuildTypeBy(int index)
        {
            Debug.Assert(index != -1);
            return buildType[index];
        }

        public void fillComponents(DataGridView grid)
        {
            grid.Rows.Clear();
            foreach(IComponent c in Loader.Soba.Registered)
            {
                Type type = c.GetType();
                if(!Inspector.IsComponent(type)) {
                    continue;
                }

                bool enabled        = c.Enabled;
                string className    = c.GetType().Name;

                Component v = SlnEvents.Components?.FirstOrDefault(p => p.ClassName == className);
                if(v != null) {
                    enabled = v.Enabled;
                }

                cInfo[className] = new List<INodeInfo>();
                bool withoutAttr = true;

                foreach(Attribute attr in type.GetCustomAttributes(true))
                {
                    if(attr.GetType() == typeof(ComponentAttribute) || attr.GetType() == typeof(DefinitionAttribute)) {
                        withoutAttr = false;
                    }

                    if(attr.GetType() == typeof(ComponentAttribute) && ((ComponentAttribute)attr).Parent == null)
                    {
                        fillComponents((ComponentAttribute)attr, enabled, className, grid);
                    }
                    else if(attr.GetType() == typeof(DefinitionAttribute) && ((DefinitionAttribute)attr).Parent == null)
                    {
                        DefinitionAttribute def = (DefinitionAttribute)attr;
                        grid.Rows.Add(DomIcon.definition, enabled, def.Name, className, def.Description);
                    }
                    else if(((DefinitionAttribute)attr).Parent != null)
                    {
                        cInfo[className].Add(new NodeInfo((DefinitionAttribute)attr));
                    }
                    else if(((ComponentAttribute)attr).Parent != null)
                    {
                        cInfo[className].Add(new NodeInfo((ComponentAttribute)attr));
                    }
                }

                if(withoutAttr) {
                    grid.Rows.Add(DomIcon.package, enabled, String.Empty, className, String.Empty);
                }
                cInfo[className].AddRange(new List<INodeInfo>(domElemsBy(className)));
            }
            grid.Sort(grid.Columns[2], System.ComponentModel.ListSortDirection.Descending);
        }

        public void updateComponents(Configuration.Component[] components)
        {
            SlnEvents.Components = components.Where(c => !c.Enabled).ToArray(); // L-585

            foreach(IComponent c in Loader.Soba.Registered) {
                Component found = components.FirstOrDefault(p => p.ClassName == c.GetType().Name);
                if(found != null) {
                    c.Enabled = found.Enabled;
                }
            }
        }

        public IEnumerable<INodeInfo> infoByComponent(string className)
        {
            foreach(INodeInfo info in cInfo[className]) {
                yield return info;
            }
        }

        /// <param name="copy">Cloning the event-item at the specified index</param>
        /// <returns>added item</returns>
        public ISolutionEvent addEventItem(int copy = -1)
        {
            ISolutionEvent added;
            bool isNew = (copy >= SBE.evt.Count || copy < 0);

            switch(SBE.type)
            {
                case SolutionEventType.Pre: {
                    var evt = (isNew)? new SBEEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEvent>();
                    SlnEvents.PreBuild = SlnEvents.PreBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Post: {
                    var evt = (isNew)? new SBEEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEvent>();
                    SlnEvents.PostBuild = SlnEvents.PostBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Cancel: {
                    var evt = (isNew) ? new SBEEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEvent>();
                    SlnEvents.CancelBuild = SlnEvents.CancelBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.OWP: {
                    var evt = (isNew)? new SBEEventOWP() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEventOWP>();
                    SlnEvents.OWPBuild = SlnEvents.OWPBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Warnings: {
                    var evt = (isNew)? new SBEEventEW() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEventEW>();
                    SlnEvents.WarningsBuild = SlnEvents.WarningsBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Errors: {
                    var evt = (isNew)? new SBEEventEW() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEventEW>();
                    SlnEvents.ErrorsBuild = SlnEvents.ErrorsBuild.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Transmitter: {
                    var evt = (isNew)? new SBETransmitter() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBETransmitter>();
                    SlnEvents.Transmitter = SlnEvents.Transmitter.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.CommandEvent: {
                    var evt = (isNew)? new CommandEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, CommandEvent>();
                    SlnEvents.CommandEvent = SlnEvents.CommandEvent.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.SlnOpened: {
                    var evt = (isNew)? new SBEEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEvent>();
                    SlnEvents.SlnOpened = SlnEvents.SlnOpened.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.SlnClosed: {
                    var evt = (isNew)? new SBEEvent() : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, SBEEvent>();
                    SlnEvents.SlnClosed = SlnEvents.SlnClosed.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                case SolutionEventType.Logging:
                {
                    var evt = (isNew)? new LoggingEvent() {
                                                Process = new EventProcess() {
                                                    Waiting = false // is better for performance
                                                }
                                            } 
                                     : SBE.evt[copy].CloneBySerializationWithType<ISolutionEvent, LoggingEvent>();

                    SlnEvents.Logging = SlnEvents.Logging.GetWithAdded(evt);
                    added = evt;
                    break;
                }
                default: {
                    throw new ArgumentException($"Unsupported SolutionEventType: '{SBE.type}'");
                }
            }
            SBE.update();
            
            // fix new data

            if(isNew) {
                added.Name = UniqueNameForAction;
                return added;
            }

            added.Caption   = String.Format("Copy of '{0}' - {1}", added.Name, added.Caption);
            added.Name      = genUniqueName(ACTION_PREFIX_CLONE + added.Name, SBE.evt);
            cacheUnlink(added.Mode);
            
            return added;
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void moveEventItem(int from, int to)
        {
            if(from == to) {
                return;
            }

            switch(SBE.type) {
                case SolutionEventType.Pre: {
                    SlnEvents.PreBuild = SlnEvents.PreBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Post: {
                    SlnEvents.PostBuild = SlnEvents.PostBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Cancel: {
                    SlnEvents.CancelBuild = SlnEvents.CancelBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.OWP: {
                    SlnEvents.OWPBuild = SlnEvents.OWPBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Warnings: {
                    SlnEvents.WarningsBuild = SlnEvents.WarningsBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Errors: {
                    SlnEvents.ErrorsBuild = SlnEvents.ErrorsBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Transmitter: {
                    SlnEvents.Transmitter = SlnEvents.Transmitter.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.CommandEvent: {
                    SlnEvents.CommandEvent = SlnEvents.CommandEvent.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.SlnOpened: {
                    SlnEvents.SlnOpened = SlnEvents.SlnOpened.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.SlnClosed: {
                    SlnEvents.SlnClosed = SlnEvents.SlnClosed.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Logging: {
                    SlnEvents.Logging = SlnEvents.Logging.GetWithMoved(from, to);
                    break;
                }
            }
            SBE.update();
            setEventIndexes(currentEventIndex, to);
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void removeEventItem(int index)
        {
            cacheToRemove(SBE.evt[index].Mode);

            switch(SBE.type) {
                case SolutionEventType.Pre: {
                    SlnEvents.PreBuild = SlnEvents.PreBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Post: {
                    SlnEvents.PostBuild = SlnEvents.PostBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Cancel: {
                    SlnEvents.CancelBuild = SlnEvents.CancelBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.OWP: {
                    SlnEvents.OWPBuild = SlnEvents.OWPBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Warnings: {
                    SlnEvents.WarningsBuild = SlnEvents.WarningsBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Errors: {
                    SlnEvents.ErrorsBuild = SlnEvents.ErrorsBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Transmitter: {
                    SlnEvents.Transmitter = SlnEvents.Transmitter.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.CommandEvent: {
                    SlnEvents.CommandEvent = SlnEvents.CommandEvent.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.SlnOpened: {
                    SlnEvents.SlnOpened = SlnEvents.SlnOpened.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.SlnClosed: {
                    SlnEvents.SlnClosed = SlnEvents.SlnClosed.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Logging: {
                    SlnEvents.Logging = SlnEvents.Logging.GetWithRemoved(index);
                    break;
                }
            }
            SBE.update();
        }

        /// <summary>
        /// Prepare data to removing from cache.
        /// </summary>
        /// <param name="mode">Data from used mode.</param>
        public void cacheToRemove(IMode mode)
        {
            if(mode.Type == ModeType.CSharp)
            {
                IModeCSharp cfg = (IModeCSharp)mode;
                if(cfg.CacheData == null) {
                    return;
                }

                Settings.CfgUser.toRemoveFromCache(cfg.CacheData);
                cacheUnlink(mode);
            }
        }

        /// <summary>
        /// Unlink data from cache container.
        /// </summary>
        /// <param name="mode">Data from used mode.</param>
        public void cacheUnlink(IMode mode)
        {
            if(mode.Type == ModeType.CSharp) {
                ((IModeCSharp)mode).CacheData = null;
            }
        }

        /// <summary>
        /// To reset cache data.
        /// </summary>
        /// <param name="mode"></param>
        public void cacheReset(IMode mode)
        {
            if(mode.Type == ModeType.CSharp)
            {
                IModeCSharp cfg = (IModeCSharp)mode;
                if(cfg.CacheData != null) {
                    cfg.CacheData.Manager.reset();
                }
            }
        }

        /// <summary>
        /// Gets current ICommon configuration.
        /// </summary>
        /// <returns></returns>
        public ICommon getCommonCfg(ModeType type)
        {
            var data    = Settings.CfgUser.Common;
            Route route = new Route() { Event = SBE.type, Mode = type };

            if(!data.ContainsKey(route)) {
                data[route] = new Common();
            }
            return data[route];
        }

        /// <summary>
        /// Gets index from defined events
        /// </summary>
        /// <param name="type"></param>
        /// <returns>current position in list of definition</returns>
        public int getDefIndexByEventType(SolutionEventType type)
        {
            int idx = 0;
            foreach(SBEWrap evt in events)
            {
                if(evt.type == type) {
                    return idx;
                }
                ++idx;
            }
            return -1;
        }

        public void attachCommandEvents(CEBeforeEventHandler before, CEAfterEventHandler after)
        {
            cmdEvents = Env.Events?.CommandEvents;
            if(cmdEvents == null) {
                return;
            }

            lock(_lock) {
                cmdEvents.BeforeExecute -= before;
                cmdEvents.BeforeExecute += before;
                cmdEvents.AfterExecute  -= after;
                cmdEvents.AfterExecute  += after;
            }
        }

        public void detachCommandEvents(CEBeforeEventHandler before, CEAfterEventHandler after)
        {
            if(cmdEvents == null) {
                return;
            }
            lock(_lock) {
                cmdEvents.BeforeExecute -= before;
                cmdEvents.AfterExecute  -= after;
            }
        }

        /// <summary>
        /// Execution by user.
        /// </summary>
        public void execAction()
        {
            if(SBEItem == null) {
                Log.Info("No actions to execution. Add new, then try again.");
                return;
            }
            Actions.ICommand cmd = new Actions.Command
            (
                Loader.Env,
                Loader.Soba,
                Loader.Soba.EvMSBuild
            );

            ISolutionEvent evt      = SBEItem;
            SolutionEventType type  = SBE.type;
            Log.Info($"Execute an action '{evt.Name}' manually. Emulate '{type}' event. {evt.Caption}");

            cmd.Env.BuildType = BuildType.Common; //TODO: IBuild.updateBuildType
            try {
                bool res = cmd.exec(evt, type);
                Log.Info($"Completed an action '{evt.Name}': {res}");
            }
            catch(Exception ex) {
                Log.Error($"Failed an action '{evt.Name}': {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        public Events(Bootloader loader, IInspector inspector = null)
        {
            Loader          = loader;
            this.inspector  = inspector;
            Env             = loader.Env;

            backupUpdate();
        }

        /// <summary>
        /// Updating of deep copies from configuration data using all available contexts.
        /// </summary>
        protected void backupUpdate()
        {
            //backupUpdate(ContextType.Common);
            //if(CfgManager.IsExistCfg(ContextType.Solution)) {
            //    backupUpdate(ContextType.Solution);
            //}
            backupUpdate(ContextType.Static);
        }

        /// <summary>
        /// Updating of deep copies from configuration data using specific context.
        /// </summary>
        protected void backupUpdate(ContextType context)
        {
            backup.update(CfgManager.getConfigFor(context).Data.CloneBySerializationWithType<ISolutionEvents, SolutionEvents>(), context);
            backup.update(CfgManager.getUserConfigFor(context).Data.CloneBySerializationWithType<IData, Data>(), context);
        }

        /// <summary>
        /// Restore configuration data from backup using all available contexts.
        /// </summary>
        protected void backupRestore()
        {
            backupUpdate(ContextType.Static);
            //if(CfgManager.IsExistCfg(ContextType.Solution)) {
            //    backupRestore(ContextType.Solution);
            //}
            //backupRestore(ContextType.Common);
        }

        /// <summary>
        /// Restore configuration data from backup using specific context.
        /// </summary>
        protected void backupRestore(ContextType context)
        {
            CfgManager.getConfigFor(context).load(backup.getConfig(context).CloneBySerializationWithType<ISolutionEvents, SolutionEvents>());
            CfgManager.getUserConfigFor(context).load(backup.getUserConfig(context).CloneBySerializationWithType<IData, Data>());
        }

        /// <summary>
        /// Generating id for present scope
        /// </summary>
        /// <param name="prefix">only for specific prefix</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        protected virtual int getUniqueId(string prefix, List<ISolutionEvent> scope)
        {
            int maxId = 0;
            foreach(ISolutionEvent item in scope)
            {
                if(String.IsNullOrEmpty(item.Name)) {
                    continue;
                }

                try
                {
                    Match m = Regex.Match(item.Name, String.Format(@"^{0}(\d*)$", prefix), RegexOptions.IgnoreCase);
                    if(!m.Success) {
                        continue;
                    }
                    string num = m.Groups[1].Value;

                    maxId = Math.Max(maxId, (num.Length > 0)? Int32.Parse(num) + 1 : 1);
                }
                catch(Exception ex) {
                    Log.Debug("getUniqueId: {0} ::'{1}'", ex.ToString(), prefix);
                }
            }
            return maxId;
        }

        protected void fillComponents(ComponentAttribute attr, bool enabled, string className, DataGridView grid)
        {
            grid.Rows.Add(DomIcon.package, enabled, attr.Name, className, attr.Description);

            if(attr.Aliases == null) {
                return;
            }
            foreach(string alias in attr.Aliases)
            {
                int idx = grid.Rows.Add(DomIcon.alias, enabled, alias, className, String.Format("Alias to '{0}' Component", attr.Name));

                grid.Rows[idx].ReadOnly = true;
                grid.Rows[idx].Cells[1] = new DataGridViewCheckBoxCell() { Style = { 
                                                                               ForeColor = System.Drawing.Color.Transparent, 
                                                                               SelectionForeColor = System.Drawing.Color.Transparent }};
            }
        }

        protected IEnumerable<INodeInfo> domElemsBy(string className)
        {
            if(inspector == null) {
                Log.Debug("domElemsBy: Inspector is null");
                yield break;
            }

            List<INodeInfo> ret = new List<INodeInfo>();
            foreach(IComponent c in Loader.Soba.Registered)
            {
                if(c.GetType().Name != className) {
                    continue;
                }

                foreach(INodeInfo info in inspector.GetBy(c.GetType())) {
                    ret.Add(info);
                    ret.AddRange(domElemsBy(info.Link));
                }
            }

            // TODO:
            foreach(INodeInfo info in ret.Distinct()) {
                yield return info;
            }
        }

        protected IEnumerable<INodeInfo> domElemsBy(NodeIdent ident)
        {
            foreach(INodeInfo info in inspector.GetBy(ident))
            {
                if(!String.IsNullOrEmpty(info.Name)) {
                    yield return info;
                }

                foreach(INodeInfo child in domElemsBy(info.Link)) {
                    yield return child;
                }
            }
        }
    }
}