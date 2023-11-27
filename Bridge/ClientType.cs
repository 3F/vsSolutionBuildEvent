/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Available types for client libraries
    /// </summary>
    [Guid("AF93FF29-E336-4202-9681-443C5573365E")]
    public enum ClientType
    {
        /// <summary>
        /// Use DTE2 context by default.
        /// </summary>
        Default = Dte2,

        /// <summary>
        /// Full with DTE2 context.
        /// </summary>
        Dte2 = 0x100,

        /// <summary>
        /// Isolated environment.
        /// </summary>
        Isolated = 0x101,

        /// <summary>
        /// Empty environment.
        /// </summary>
        Empty = 0x102,
    }
}
