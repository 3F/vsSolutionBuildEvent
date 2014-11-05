/*
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
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
        public Environment Env
        {
            get { return env; }
        }
        protected Environment env;

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
        /// Used for restoring settings
        /// </summary>
        protected SolutionEvents toRestoring;

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
            combo.Items.Add(":: Transmitter :: Transmission building-data to outer handler");

            combo.SelectedIndex = 0;
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
            }
            SBE.update();
        }

        public Events(Environment env)
        {
            this.env = env;
            toRestoring = Config._.Data.CloneBySerialization();
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

        protected virtual string genUniqueName(string prefix, List<ISolutionEvent> scope)
        {
            return String.Format("{0}{1}", prefix, getUniqueId(prefix, scope));
        }
    }
}