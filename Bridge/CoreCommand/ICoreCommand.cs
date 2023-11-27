/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge.CoreCommand
{
    /// <summary>
    /// Specifies work for commands from core library
    /// </summary>
    [Guid("9DD00E57-3ED9-4561-84B3-895ACEA3166C")]
    public interface ICoreCommand
    {
        /// <summary>
        /// Standard available commands.
        /// </summary>
        CoreCommandType Type { get; set; }

        /// <summary>
        /// Arguments for current command.
        /// </summary>
        object[] Args { get; set; }
    }
}
