/*
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.UI.Xaml.Logic
{
    public class StatusTool
    {
        /// <summary>
        /// Message counter of received warnings
        /// </summary>
        public int Warnings
        {
            get { return warnings; }
        }
        protected volatile int warnings = 0;

        /// <summary>
        /// Current the Enabled statuses for all used events
        /// </summary>
        protected Dictionary<SolutionEventType, bool[]> status = new Dictionary<SolutionEventType, bool[]>();

        /// <summary>
        /// Updating status for used event type
        /// </summary>
        /// <param name="type"></param>
        public void update(SolutionEventType type)
        {
            if(!isDisabledAll(type)) {
                status[type] = getEvent(type).Select(i => i.Enabled).ToArray();
            }
        }

        /// <summary>
        /// Adding information about of new warning
        /// </summary>
        /// <returns>Count of received warnings</returns>
        public int addWarning()
        {
            return ++warnings;
        }

        /// <summary>
        /// Resetting counter on all warnings
        /// </summary>
        public void resetWarnings()
        {
            warnings = 0;
        }

        /// <summary>
        /// Provides captions by event type
        /// </summary>
        /// <param name="type"></param>
        public string caption(SolutionEventType type)
        {
            switch(type) {
                case SolutionEventType.OWP: {
                    return "Output";
                }
            }
            return type.ToString();
        }

        /// <summary>
        /// Provides captions by event type and selection mode
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selected"></param>
        /// <exception cref="*"></exception>
        public string caption(SolutionEventType type, bool selected)
        {
            ISolutionEvent[] evt = getEvent(type);
            int enabled = (evt == null)? 0 : evt.Where(i => i.Enabled).Count();
            if(selected) {
                return String.Format("({0} /{1})", enabled, evt.Length);
            }

            if(enabled < 1) {
                return caption(type);
            }
            return String.Format("{0} ({1})", caption(type), enabled);
        }

        /// <summary>
        /// Changing the Enabled staus for all event-items
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        public void enabled(SolutionEventType type, bool status)
        {
            ISolutionEvent[] evt = getEvent(type);
            foreach(ISolutionEvent item in evt) {
                item.Enabled = status;
            }
        }

        public void restore(SolutionEventType type)
        {
            if(status.ContainsKey(type) && status[type] != null)
            {
                ISolutionEvent[] evt = getEvent(type);
                for(int i = 0; i < evt.Length; ++i) {
                    evt[i].Enabled = status[type][i];
                }
            }

            if(isDisabledAll(type)) {
                enabled(type, true);
            }
        }

        /// <param name="type">Event type</param>
        /// <returns>true value if all event are disabled for present type</returns>
        public bool isDisabledAll(SolutionEventType type)
        {
            return getEvent(type).All(x => !x.Enabled);
        }

        /// <exception cref="*"></exception>
        public void executeCommand(EnvDTE80.DTE2 dte2, string cmd)
        {
            dte2.ExecuteCommand(cmd);
        }

        protected virtual ISolutionEvent[] getEvent(SolutionEventType type)
        {
            return Config._.Data.getEvent(type);
        }
    }
}
