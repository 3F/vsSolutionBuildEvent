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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    public class Status: IStatus
    {
        /// <summary>
        /// Thread-safe getting the instance of Status class
        /// </summary>
        public static IStatus _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Status> _lazy = new Lazy<Status>(() => new Status());

        /// <summary>
        /// Contains the all execution status by event type
        /// </summary>
        protected ConcurrentDictionary<SolutionEventType, SynchronizedCollection<StatusType>> states;

        /// <summary>
        /// New status for Event type
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="tstatus">Execution status</param>
        public void add(SolutionEventType tevent, StatusType tstatus)
        {
            if(!states.ContainsKey(tevent)) {
                states[tevent] = new SynchronizedCollection<StatusType>();
            }
            states[tevent].Add(tstatus);
        }

        /// <summary>
        /// Flushing of all execution statuses by Event type
        /// </summary>
        /// <param name="tevent"></param>
        public void flush(SolutionEventType tevent)
        {
            states[tevent] = new SynchronizedCollection<StatusType>();
        }

        /// <summary>
        /// Flushing of all execution statuses
        /// </summary>
        public void flush()
        {
            states = new ConcurrentDictionary<SolutionEventType, SynchronizedCollection<StatusType>>();
        }

        /// <summary>
        /// Updating status for Event type
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="index">Position in list</param>
        /// <param name="tstatus">new status</param>
        public void update(SolutionEventType tevent, int index, StatusType tstatus)
        {
            try {
                states[tevent][index] = tstatus;
            }
            catch(Exception ex) {
                Log.Debug("Updating status: '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Getting the Execution status by Event type and position in list
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="index">Position in list</param>
        /// <returns>Executed status</returns>
        public StatusType get(SolutionEventType tevent, int index)
        {
            Debug.Assert(states != null);
            try {
                return states[tevent][index];
            }
            catch(Exception) {
                return StatusType.NotFound;
            }
        }

        /// <summary>
        /// Checking existence of StatusType in the current statuses
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="type"></param>
        /// <returns>true value if contains</returns>
        public bool contains(SolutionEventType tevent, StatusType type)
        {
            Debug.Assert(states != null);
            if(!states.ContainsKey(tevent)) {
                return false;
            }
            return states[tevent].Contains(type);
        }

        private Status()
        {
            flush();
        }
    }
}
