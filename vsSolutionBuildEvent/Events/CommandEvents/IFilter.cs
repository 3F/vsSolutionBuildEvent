/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events.CommandEvents
{
    /// <summary>
    /// Specifies filters for ICommandEvent
    /// </summary>
    [Guid("7119BA06-8F1A-4055-BA13-9ADA5850D1B7")]
    public interface IFilter
    {
        /// <summary>
        /// For work with command ID
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Scope by GUID
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// Filter by Custom input parameters
        /// </summary>
        object CustomIn { get; set; }

        /// <summary>
        /// Filter by Custom output parameters
        /// </summary>
        object CustomOut { get; set; }

        /// <summary>
        /// Cancel command if it's possible
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        /// Use Before executing command
        /// </summary>
        bool Pre { get; set; }

        /// <summary>
        /// Use After executed command
        /// </summary>
        bool Post { get; set; }

        /// <summary>
        /// About filter
        /// </summary>
        string Description { get; set; }
    }
}