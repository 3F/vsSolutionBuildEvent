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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using EnvDTE;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Forwards messages from VS component (OutputWindow) for own subscribers.
    /// </summary>
    public class OWP
    {
        /// Max length of envelope.
        /// </summary>
        public const int ENVELOPE_LIMIT = 32768;

        /// <summary>
        /// When is receiving data from pane
        /// </summary>
        public event EventHandler<PaneArgs> Receiving = delegate(object sender, PaneArgs e) { };

        /// <summary>
        /// Events of selected pane/s.
        /// </summary>
        protected OutputWindowEvents evt;

        //TODO: fix me. Prevents Duplicate Data / bug with OutputWindowPane
        protected Dictionary<string, ConcurrentQueue<string>> dataList = new Dictionary<string, ConcurrentQueue<string>>();
        protected System.Threading.Thread tUpdated;

        /// <summary>
        /// List of previous count of lines for EditPoint::GetLines
        /// </summary>
        private Dictionary<string, int> _prevCountLines = new Dictionary<string, int>();

        /// <summary>
        /// obj synch.
        /// </summary>
        private Object _eLock = new Object();

        public void attachEvents()
        {
            if(evt == null) {
                Log.Warn("OWP: Disabled for current Environment.");
                return;
            }

            lock(_eLock) {
                detachEvents();
                evt.PaneUpdated     += onPaneUpdated;
                evt.PaneAdded       += onPaneAdded;
                evt.PaneClearing    += onPaneClearing;
            }
        }

        public void detachEvents()
        {
            if(evt == null) {
                return;
            }

            lock(_eLock) {
                evt.PaneUpdated     -= onPaneUpdated;
                evt.PaneAdded       -= onPaneAdded;
                evt.PaneClearing    -= onPaneClearing;
            }
        }

        public OWP(IEnvironment env)
        {
            if(env.Events != null) {
                evt = env.Events.OutputWindowEvents;
            }
        }

        /// <summary>
        /// All collection should receive raw data.
        /// The envelope should avoid duplicate Data.
        /// </summary>
        /// <param name="guid">Guid string of item pane</param>
        /// <param name="item">Name of item pane</param>
        protected void notifyRaw(string guid, string item)
        {
            if(dataList.Count < 1) {
                return;
            }

            if(!dataList.ContainsKey(guid)) {
                Log.Debug("notifyRaw is called for undefined guid: '{0}':'{1}'", guid, item);
                return;
            }

            if(dataList[guid].Count < 1) {
                return;
            }

            lock(_eLock)
            {
                StringBuilder envelope = new StringBuilder();
                while(dataList[guid].Count > 0)
                {
                    string msg;
                    if(!dataList[guid].TryDequeue(out msg)) {
                        continue;
                    }

                    envelope.Append(msg);
                    if(envelope.Length > ENVELOPE_LIMIT) {
                        break;
                    }
                }

                Receiving(this, new PaneArgs() { Raw = envelope.ToString(), Guid = guid, Item = item });
            }

            if(dataList[guid].Count > 0) {
                notifyRaw(guid, item);
            }
        }

        protected void onPaneUpdated(OutputWindowPane pane)
        {
            if(pane == null || pane.Guid == null) {
                return;
            }

            TextDocument textD;
            try {
                textD = pane.TextDocument;
            }
            catch(System.Runtime.InteropServices.COMException ex) {
                Log.Debug("notifyRaw: COMException - '{0}'", ex.Message);
                return;
            }

            int countLines = textD.EndPoint.Line;
            if(countLines <= 1 || countLines - getPrevCountLines(pane.Guid) < 1) {
                return;
            }

            if(!dataList.ContainsKey(pane.Guid)) {
                dataList[pane.Guid] = new ConcurrentQueue<string>();
            }

            EditPoint point = textD.StartPoint.CreateEditPoint();

            // text between Start (inclusive) and ExclusiveEnd (exclusive)
            dataList[pane.Guid].Enqueue(point.GetLines(getPrevCountLines(pane.Guid), countLines)); // e.g. first line: 1, 2
            setPrevCountLines(countLines, pane.Guid);

            //TODO: fix me. Prevent Duplicate Data / bug with OutputWindowPane
            if(tUpdated == null || tUpdated.ThreadState == ThreadState.Unstarted || tUpdated.ThreadState == ThreadState.Stopped)
            {
                tUpdated = new System.Threading.Thread(() =>
                {
                    if(pane == null) {
                        return;
                    }

                    try {
                        notifyRaw(pane.Guid, pane.Name);
                    }
                    catch(Exception ex) {
                        Log.Debug("notifyRaw: failed '{0}'", ex.Message);
                    }
                });

                try {
                    tUpdated.Start();
                }
                catch(Exception ex) {
                    // ThreadStateException, OutOfMemoryException
                    Log.Debug("notifyRaw: can't start '{0}'", ex.Message);
                }
            }
        }

        /// <param name="guid">Guid of pane.</param>
        /// <returns></returns>
        protected int getPrevCountLines(string guid)
        {
            if(!_prevCountLines.ContainsKey(guid)) {
                _prevCountLines[guid] = 1;
            }
            return _prevCountLines[guid];
        }

        /// <param name="val">New value.</param>
        /// <param name="guid">Guid of pane.</param>
        protected void setPrevCountLines(int val, string guid)
        {
            _prevCountLines[guid] = val;
        }

        /// <param name="guid">Guid of pane.</param>
        protected void resetPrevCountLines(string guid)
        {
            setPrevCountLines(1, guid);
        }

        private void onPaneAdded(OutputWindowPane pane)
        {
            resetPrevCountLines(pane.Guid);
            dataList.Clear();
        }

        private void onPaneClearing(OutputWindowPane pane)
        {
            resetPrevCountLines(pane.Guid);
            dataList.Clear();
        }
    }
}
