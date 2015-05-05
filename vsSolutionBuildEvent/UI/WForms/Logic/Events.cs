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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Dom;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
    using DomIcon = net.r_eg.vsSBE.SBEScripts.Dom.Icon;

    public class Events
    {
        /// <summary>
        /// For naming actions
        /// </summary>
        public const string ACTION_PREFIX       = "Act";
        public const string ACTION_PREFIX_CLONE = "CopyOf";

        public class SBEWrap
        {
            /// <summary>
            /// Wrapped event
            /// </summary>
            public List<ISolutionEvent> evt;
            /// <summary>
            /// Specific type
            /// </summary>
            public SolutionEventType type;

            public SBEWrap(SolutionEventType type)
            {
                this.type = type;
                update();
            }

            /// <summary>
            /// Updating list from used array data
            /// </summary>
            public void update()
            {
                evt = new List<ISolutionEvent>(Config._.Data.getEvent(type));
                if(evt == null) {
                    Log.nlog.Debug("SBEWrap: evt is null for type '{0}'", type);
                    evt = new List<ISolutionEvent>();
                }
            }
        }

        /// <summary>
        /// Provides operations with environment
        /// </summary>
        public IEnvironment Env
        {
            get;
            protected set;
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
                if(currentEventItem == -1) {
                    return new SBEEvent();
                }
                return SBE.evt[currentEventItem];
            }
        }

        /// <summary>
        /// Current Mode from SBE-item
        /// </summary>
        public IMode SBEItemMode
        {
            get {
                return (SBEItem.Mode == null) ? DefaultMode : SBEItem.Mode;
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
        /// Predefined operations
        /// TODO:
        /// </summary>
        public List<ModeOperation> DefOperations
        {
            get { return defOperations; }
        }
        protected List<ModeOperation> defOperations = DefCommandsDTE.operations();

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
        /// Used for restoring settings
        /// </summary>
        protected SolutionEvents toRestoring;

        /// <summary>
        /// Information by existing components
        /// </summary>
        protected Dictionary<string, List<INodeInfo>> cInfo = new Dictionary<string, List<INodeInfo>>();

        /// <summary>
        /// Used loader
        /// </summary>
        protected IBootloader bootloader;

        /// Mapper of the available components
        /// </summary>
        protected IInspector inspector;

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
        /// Protection, if not exist the all items for some reason.
        /// 
        /// This probably can be if the configuration is:
        ///   * corrupted or incorrectly modified manually or there is an incompatibility, etc.
        /// Therefore, we should have the some fixes for current problem
        /// </summary>
        public virtual void protectMinEventItems()
        {
            if(SBE.evt == null || SBE.evt.Count < 1) {
                Log.nlog.Warn("Event-item < 1 for type '{0}'", SBE.type);
                addEventItem(); // simply to work with new container
            }
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

        public virtual string validateName(string name)
        {
            if(String.IsNullOrEmpty(name)){
                return String.Empty;
            }

            return Regex.Replace(name, @"(?:
                                            ^([^a-z]+)
                                          |
                                            ([^a-z_0-9]+)
                                         )", delegate(Match m)
            {
                return String.Empty;
            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        public void saveData()
        {
            Config._.save(); // all changes have been passed by reference
            toRestoring = Config._.Data.CloneBySerialization(); // update the deep copy
        }

        public void restoreData()
        {
            Config._.load(toRestoring.CloneBySerialization());
        }

        public void fillEvents(ComboBox combo)
        {
            events.Clear();
            combo.Items.Clear();

            addEvent(new SBEWrap(SolutionEventType.Pre));
            combo.Items.Add(":: Pre-Build :: Before assembling");

            addEvent(new SBEWrap(SolutionEventType.Post));
            combo.Items.Add(":: Post-Build :: After assembling");

            addEvent(new SBEWrap(SolutionEventType.Cancel));
            combo.Items.Add(":: Cancel-Build :: by user or when an error occurs");

            addEvent(new SBEWrap(SolutionEventType.Warnings));
            combo.Items.Add(":: Warnings-Build :: Warnings during assembly processing");

            addEvent(new SBEWrap(SolutionEventType.Errors));
            combo.Items.Add(":: Errors-Build :: Errors during assembly processing");

            addEvent(new SBEWrap(SolutionEventType.OWP));
            combo.Items.Add(":: Output-Build customization :: Full control");

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

            buildType.Add(BuildType.Start);
            combo.Items.Add("Start Debugging");

            buildType.Add(BuildType.StartNoDebug);
            combo.Items.Add("Start Without Debugging");

            buildType.Add(BuildType.BuildSelection);
            combo.Items.Add("Build Selection");

            buildType.Add(BuildType.RebuildSelection);
            combo.Items.Add("Rebuild Selection");

            buildType.Add(BuildType.CleanSelection);
            combo.Items.Add("Clean Selection");

            buildType.Add(BuildType.DeploySelection);
            combo.Items.Add("Deploy Selection");

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

            buildType.Add(BuildType.PublishSelection);
            combo.Items.Add("Publish Selection");

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
            foreach(IComponent c in bootloader.ComponentsAll)
            {
                Type type = c.GetType();
                if(!Inspector.isComponent(type)) {
                    continue;
                }

                bool enabled        = c.Enabled;
                string className    = c.GetType().Name;

                Configuration.Component[] cfg = Config._.Data.Components;
                if(cfg != null && cfg.Length > 0) {
                    Configuration.Component v = cfg.Where(p => p.ClassName == className).FirstOrDefault();
                    if(v != null) {
                        enabled = v.Enabled;
                    }
                }

                bool withoutAttr = true;
                foreach(Attribute attr in type.GetCustomAttributes(true))
                {
                    if(attr.GetType() == typeof(ComponentAttribute))
                    {
                        withoutAttr = false;
                        grid.Rows.Add(
                                    DomIcon.package,
                                    enabled, 
                                    ((ComponentAttribute)attr).Name,
                                    className,
                                    ((ComponentAttribute)attr).Description);
                    }

                    if(attr.GetType() == typeof(DefinitionAttribute))
                    {
                        withoutAttr = false;
                        grid.Rows.Add(
                                    DomIcon.definition,
                                    enabled,
                                    ((DefinitionAttribute)attr).Name,
                                    className,
                                    ((DefinitionAttribute)attr).Description);
                    }
                }

                if(withoutAttr) {
                    grid.Rows.Add(DomIcon.package, enabled, String.Empty, className, String.Empty);
                }

                cInfo[className] = new List<INodeInfo>(domElemsBy(className));
            }
            grid.Sort(grid.Columns[2], System.ComponentModel.ListSortDirection.Descending);
        }

        public void updateComponents(Configuration.Component[] components)
        {
            Config._.Data.Components = components;
            foreach(IComponent c in bootloader.ComponentsAll) {
                Configuration.Component found = components.Where(p => p.ClassName == c.GetType().Name).FirstOrDefault();
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

        /// <param name="copyFrom">Cloning the event-item at the specified index</param>
        /// <returns>added item</returns>
        public ISolutionEvent addEventItem(int copyFrom = -1)
        {
            switch(SBE.type) {
                case SolutionEventType.Pre: {
                    Config._.Data.PreBuild = Config._.Data.PreBuild.GetWithAdded(new SBEEvent());
                    break;
                }
                case SolutionEventType.Post: {
                    Config._.Data.PostBuild = Config._.Data.PostBuild.GetWithAdded(new SBEEvent());
                    break;
                }
                case SolutionEventType.Cancel: {
                    Config._.Data.CancelBuild = Config._.Data.CancelBuild.GetWithAdded(new SBEEvent());
                    break;
                }
                case SolutionEventType.OWP: {
                    Config._.Data.OWPBuild = Config._.Data.OWPBuild.GetWithAdded(new SBEEventOWP());
                    break;
                }
                case SolutionEventType.Warnings: {
                    Config._.Data.WarningsBuild = Config._.Data.WarningsBuild.GetWithAdded(new SBEEventEW());
                    break;
                }
                case SolutionEventType.Errors: {
                    Config._.Data.ErrorsBuild = Config._.Data.ErrorsBuild.GetWithAdded(new SBEEventEW());
                    break;
                }
                case SolutionEventType.Transmitter: {
                    Config._.Data.Transmitter = Config._.Data.Transmitter.GetWithAdded(new SBETransmitter());
                    break;
                }
                case SolutionEventType.Logging: {
                    Config._.Data.Logging = Config._.Data.Logging.GetWithAdded(new LoggingEvent());
                    break;
                }
            }
            SBE.update();

            // we already have added item in main storage, 
            // and we can working with any updating by shallow copy

            ISolutionEvent added = SBE.evt[SBE.evt.Count - 1];

            if(copyFrom >= SBE.evt.Count || copyFrom < 0) {
                added.Name = genUniqueName(ACTION_PREFIX, SBE.evt);
            }
            else {
                SBE.evt[copyFrom].CloneByReflectionInto(added);
                added.Caption   = String.Format("Copy of '{0}' - {1}", added.Name, added.Caption);
                added.Name      = genUniqueName(ACTION_PREFIX_CLONE + added.Name, SBE.evt);
            }
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
                    Config._.Data.PreBuild = Config._.Data.PreBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Post: {
                    Config._.Data.PostBuild = Config._.Data.PostBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Cancel: {
                    Config._.Data.CancelBuild = Config._.Data.CancelBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.OWP: {
                    Config._.Data.OWPBuild = Config._.Data.OWPBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Warnings: {
                    Config._.Data.WarningsBuild = Config._.Data.WarningsBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Errors: {
                    Config._.Data.ErrorsBuild = Config._.Data.ErrorsBuild.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Transmitter: {
                    Config._.Data.Transmitter = Config._.Data.Transmitter.GetWithMoved(from, to);
                    break;
                }
                case SolutionEventType.Logging: {
                    Config._.Data.Logging = Config._.Data.Logging.GetWithMoved(from, to);
                    break;
                }
            }
            SBE.update();
            setEventIndexes(currentEventIndex, to);
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void removeEventItem(int index)
        {
            switch(SBE.type) {
                case SolutionEventType.Pre: {
                    Config._.Data.PreBuild = Config._.Data.PreBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Post: {
                    Config._.Data.PostBuild = Config._.Data.PostBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Cancel: {
                    Config._.Data.CancelBuild = Config._.Data.CancelBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.OWP: {
                    Config._.Data.OWPBuild = Config._.Data.OWPBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Warnings: {
                    Config._.Data.WarningsBuild = Config._.Data.WarningsBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Errors: {
                    Config._.Data.ErrorsBuild = Config._.Data.ErrorsBuild.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Transmitter: {
                    Config._.Data.Transmitter = Config._.Data.Transmitter.GetWithRemoved(index);
                    break;
                }
                case SolutionEventType.Logging: {
                    Config._.Data.Logging = Config._.Data.Logging.GetWithRemoved(index);
                    break;
                }
            }
            SBE.update();
        }

        public Events(IBootloader bootloader, IInspector inspector = null)
        {
            this.bootloader = bootloader;
            this.inspector  = inspector;
            Env             = bootloader.Env;
            toRestoring     = Config._.Data.CloneBySerialization();
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

                try {
                    Match m = Regex.Match(item.Name, String.Format("^{0}(\\d+)", prefix), RegexOptions.IgnoreCase);
                    if(m.Groups[1].Success) {
                        maxId = Math.Max(maxId, Int32.Parse(m.Groups[1].Value));
                    }
                }
                catch(Exception ex) {
                    Log.nlog.Debug("{0} ::'{1}'", ex.ToString(), prefix);
                }
            }
            return ++maxId;
        }

        protected IEnumerable<INodeInfo> domElemsBy(string className)
        {
            if(inspector == null) {
                Log.nlog.Debug("domElemsBy: Inspector is null");
                yield break;
            }

            List<INodeInfo> ret = new List<INodeInfo>();
            foreach(IComponent c in bootloader.ComponentsAll)
            {
                if(c.GetType().Name != className) {
                    continue;
                }

                foreach(INodeInfo info in inspector.getBy(c.GetType())) {
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
            foreach(INodeInfo info in inspector.getBy(ident))
            {
                if(!String.IsNullOrEmpty(info.Name)) {
                    yield return info;
                }

                foreach(INodeInfo child in domElemsBy(info.Link)) {
                    yield return child;
                }
            }
        }

        protected virtual string genUniqueName(string prefix, List<ISolutionEvent> scope)
        {
            return String.Format("{0}{1}", prefix, getUniqueId(prefix, scope));
        }
    }
}