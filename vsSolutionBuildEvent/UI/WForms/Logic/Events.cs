/*
 * Copyright (c) 2013-2014 Developed by reg <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.UI
{
    public class Events
    {
        /// <summary>
        /// Used for name of new action
        /// </summary>
        public const string ACTION_PREFIX       = "Act";
        public const string ACTION_PREFIX_CLONE = "CopyOf";

        public class SBEWrap
        {
            public List<SBEEvent> evt;
            public SolutionEventType type;

            public SBEWrap(SBEEvent[] evt, SolutionEventType type)
            {
                setEvent(evt);
                this.type = type;
            }

            public SBEWrap(SBEEvent[] evt)
            {
                setEvent(evt);
                type = SolutionEventType.General;
            }

            protected void setEvent(SBEEvent[] evt)
            {
                this.evt = new List<SBEEvent>(evt);
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
        public SBEEvent SBEItem
        {
            get {
                if(currentEventItem == -1) {
                    return new SBEEvent();
                }
                return SBE.evt[currentEventItem];
            }
        }

        /// <summary>
        /// Predefined operations
        /// TODO:
        /// </summary>
        public List<TOperation> DefOperations
        {
            get { return defOperations; }
        }
        protected List<TOperation> defOperations = DefCommandsDTE.operations();

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

        public void addEvent(SBEWrap evt)
        {
            events.Add(evt);
        }

        /// <param name="copyFrom">Cloning the event-item from present index</param>
        /// <returns>added item</returns>
        public SBEEvent addEventItem(int copyFrom = -1)
        {
            SBEEvent evt;
            if(copyFrom >= SBE.evt.Count || copyFrom < 0)
            {
                evt = new SBEEvent();
                evt.name = genUniqueName(ACTION_PREFIX, SBE.evt);
            }
            else
            {
                evt         = deepCopyFrom(SBE.evt[copyFrom]);
                evt.caption = String.Format("Copy of '{0}' - {1}", evt.name, evt.caption);
                evt.name    = genUniqueName(ACTION_PREFIX_CLONE + evt.name, SBE.evt);
            }

            if(SBE.evt == null) {
                Log.nlog.Debug("evt is null for type '{0}'", SBE.type);
                SBE.evt = new List<SBEEvent>();
            }
            SBE.evt.Add(evt);
            return evt;
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void moveEventItem(int from, int to)
        {
            if(from == to) {
                return;
            }
            SBEEvent moving = SBE.evt[from];
            SBE.evt.RemoveAt(from);
            SBE.evt.Insert(to, moving);
        }

        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void removeEventItem(int index)
        {
            SBE.evt.RemoveAt(index);
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
            SBE.evt[index].name = name;
            SBE.evt[index].enabled = enabled;
        }

        public void updateInfo(int index, SBEEvent evt)
        {
            SBE.evt[index] = evt;
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
            SBEEvent[] data = SBE.evt.ToArray();

            switch(SBE.type) {
                case SolutionEventType.Pre: { 
                    Config._.Data.preBuild = data; 
                    break; 
                }
                case SolutionEventType.Post: { 
                    Config._.Data.postBuild = data; 
                    break; 
                }
                case SolutionEventType.Cancel: { 
                    Config._.Data.cancelBuild = data; 
                    break; 
                }
                case SolutionEventType.Warnings: { 
                    Config._.Data.warningsBuild = (SBEEventEW[])data; 
                    break; 
                }
                case SolutionEventType.Errors: { 
                    Config._.Data.errorsBuild = (SBEEventEW[])data; 
                    break; 
                }
                case SolutionEventType.OWP: { 
                    Config._.Data.outputCustomBuild = (SBEEventOWP[])data; 
                    break; 
                }
                case SolutionEventType.Transmitter: { 
                    Config._.Data.transmitter = (SBETransmitter[])data; 
                    break; 
                }
            }

            Config._.save();
        }

        public void fillEvents(ComboBox combo)
        {
            events.Clear();
            combo.Items.Clear();

            addEvent(new SBEWrap(Config._.Data.preBuild, SolutionEventType.Pre));
            combo.Items.Add(":: Pre-Build :: Before assembling");

            addEvent(new SBEWrap(Config._.Data.postBuild, SolutionEventType.Post));
            combo.Items.Add(":: Post-Build :: After assembling");

            addEvent(new SBEWrap(Config._.Data.cancelBuild, SolutionEventType.Cancel));
            combo.Items.Add(":: Cancel-Build :: by user or when an error occurs");

            addEvent(new SBEWrap(Config._.Data.warningsBuild, SolutionEventType.Warnings));
            combo.Items.Add(":: Warnings-Build :: Warnings during assembly processing");

            addEvent(new SBEWrap(Config._.Data.errorsBuild, SolutionEventType.Errors));
            combo.Items.Add(":: Errors-Build :: Errors during assembly processing");

            addEvent(new SBEWrap(Config._.Data.outputCustomBuild, SolutionEventType.OWP));
            combo.Items.Add(":: Output-Build customization :: Full control");

            addEvent(new SBEWrap(Config._.Data.transmitter, SolutionEventType.Transmitter));
            combo.Items.Add(":: Transmitter :: Transmission building-data to outer handler");

            combo.SelectedIndex = 0;
        }

        public Events(Environment env)
        {
            this.env = env;
        }

        /// <summary>
        /// Generating id for present scope
        /// </summary>
        /// <param name="prefix">only for specific prefix</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        protected virtual int getUniqueId(string prefix, List<SBEEvent> scope)
        {
            int maxId = 0;
            foreach(SBEEvent item in scope)
            {
                if(String.IsNullOrEmpty(item.name)) {
                    continue;
                }

                try {
                    Match m = Regex.Match(item.name, String.Format("^{0}(\\d+)", prefix), RegexOptions.IgnoreCase);
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

        protected virtual string genUniqueName(string prefix, List<SBEEvent> scope)
        {
            return String.Format("{0}{1}", prefix, getUniqueId(prefix, scope));
        }

        protected virtual SBEEvent deepCopyFrom(SBEEvent evt)
        {
            SBEEvent ret = new SBEEvent();
            System.Reflection.PropertyInfo[] properties = evt.GetType().GetProperties();

            foreach(System.Reflection.PropertyInfo property in properties) {
                property.SetValue(ret, property.GetValue(evt, null), null);
            }
            return ret;
        }
    }
}