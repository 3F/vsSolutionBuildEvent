/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
