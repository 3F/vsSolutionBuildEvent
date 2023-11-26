/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Specification of status for all actions
    /// </summary>
    public interface IStatus
    {
        /// <summary>
        /// New status for Event type
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="tstatus">Execution status</param>
        void add(SolutionEventType tevent, StatusType tstatus);

        /// <summary>
        /// Flushing of all execution statuses by Event type
        /// </summary>
        /// <param name="tevent"></param>
        void flush(SolutionEventType tevent);

        /// <summary>
        /// Flushing of all execution statuses
        /// </summary>
        void flush();

        /// <summary>
        /// Getting the Execution status by Event type and position in list
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="index">Position in list</param>
        /// <returns>Executed status</returns>
        StatusType get(SolutionEventType tevent, int index);

        //// <summary>
        //// Getting the Execution statuses by Event type
        //// </summary>
        //// <param name="tevent">Event type</param>
        //// <returns>List of Execution statuses</returns>
        //SynchronizedCollection<StatusType> get(SolutionEventType tevent);

        /// <summary>
        /// Checking existence of StatusType in the current statuses
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="type"></param>
        /// <returns>true value if contains</returns>
        bool contains(SolutionEventType tevent, StatusType type);

        /// <summary>
        /// Updating status for Event type
        /// </summary>
        /// <param name="tevent">Event type</param>
        /// <param name="index">Position in list</param>
        /// <param name="tstatus">new status</param>
        void update(SolutionEventType tevent, int index, StatusType tstatus);
    }

    /// <summary>
    /// Type of available statuses
    /// </summary>
    public enum StatusType
    {
        /// <summary>
        /// By default, for erroneous access
        /// </summary>
        NotFound,
        /// <summary>
        /// 
        /// </summary>
        Success,
        /// <summary>
        /// 
        /// </summary>
        Fail,
        /// <summary>
        ///  Support for deferred events
        /// </summary>
        Deferred
    }
}
