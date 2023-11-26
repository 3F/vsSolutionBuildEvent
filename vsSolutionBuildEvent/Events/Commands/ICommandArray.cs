/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events.Commands
{
    /// <summary>
    /// Specifies basic fields for command list.
    /// </summary>
    [Guid("2229F68B-0FC7-46E3-953A-AFC53650DA63")]
    public interface ICommandArray<T>
    {
        /// <summary>
        /// Atomic commands for handling.
        /// </summary>
        T[] Command { get; set; }
    }
}
