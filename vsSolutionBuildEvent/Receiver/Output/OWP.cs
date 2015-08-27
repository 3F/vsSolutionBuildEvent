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
using System.Threading;
using EnvDTE;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Forwards messages from VS component (OutputWindow) for own subscribers.
    /// </summary>
    public class OWP
    {
        /// <summary>
        /// When is receiving data from pane
        /// </summary>
        public event EventHandler<PaneArgs> Receiving = delegate(object sender, PaneArgs e) { };

        /// <summary>
        /// Events of selected pane/s.
        /// </summary>
        protected OutputWindowEvents evt;

        //TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
        protected SynchronizedCollection<string> dataList = new SynchronizedCollection<string>();
        protected System.Threading.Thread tUpdated;

        /// <summary>
        /// Current item name
        /// </summary>
        protected string item;

        /// <summary>
        /// previous count of lines for EditPoint::GetLines
        /// </summary>
        private int _prevCountLines = 1;

        /// <summary>
        /// obj synch.
        /// </summary>
        private Object _eLock = new Object();

        public void attachEvents()
        {
            if(evt == null) {
                Log.nlog.Warn("OWP: Disabled for current Environment.");
                return;
            }

            lock(_eLock) {
                detachEvents();
                evt.PaneUpdated     += evtPaneUpdated;
                evt.PaneAdded       += evtPaneAdded;
                evt.PaneClearing    += evtPaneClearing;
            }
        }

        public void detachEvents()
        {
            if(evt == null) {
                return;
            }

            lock(_eLock) {
                evt.PaneUpdated     -= evtPaneUpdated;
                evt.PaneAdded       -= evtPaneAdded;
                evt.PaneClearing    -= evtPaneClearing;
            }
        }

        public OWP(IEnvironment env, string item)
        {
            this.item = item;
            if(env.Events != null) {
                evt = env.Events.get_OutputWindowEvents(item);
            }
        }

        /// <summary>
        /// All collection should receive raw data.
        /// The envelope should avoid duplicate Data.
        /// </summary>
        protected void notifyRaw()
        {
            if(dataList.Count < 1) {
                return;
            }

            //TODO: timer for splitting a long messages
            lock(_eLock)
            {
                string envelope = String.Empty;
                while(dataList.Count > 0) {
                    envelope += dataList[0];
                    dataList.RemoveAt(0);
                }

                Receiving(this, new PaneArgs() { Raw = envelope });
            }

            if(dataList.Count > 0) {
                notifyRaw();
            }
        }

        protected virtual void evtPaneUpdated(OutputWindowPane pane)
        {
            TextDocument textD  = pane.TextDocument;
            int countLines      = textD.EndPoint.Line;

            if(countLines <= 1 || countLines - _prevCountLines < 1) {
                return;
            }

            EditPoint point = textD.StartPoint.CreateEditPoint();

            // text between Start (inclusive) and ExclusiveEnd (exclusive)
            dataList.Add(point.GetLines(_prevCountLines, countLines)); // e.g. first line: 1, 2
            _prevCountLines = countLines;

            //TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
            if(tUpdated == null || tUpdated.ThreadState == ThreadState.Unstarted || tUpdated.ThreadState == ThreadState.Stopped)
            {
                tUpdated = new System.Threading.Thread(() => { notifyRaw(); });
                try {
                    tUpdated.Start();
                }
                catch(Exception e) {
                    Log.nlog.Warn("notifyRaw() {0}", e.Message);
                }
            }
        }

        protected virtual void evtPaneAdded(OutputWindowPane pane)
        {
            _prevCountLines = 1;
            dataList.Clear();
        }

        protected virtual void evtPaneClearing(OutputWindowPane pane)
        {
            _prevCountLines = 1;
            dataList.Clear();
        }
    }
}
