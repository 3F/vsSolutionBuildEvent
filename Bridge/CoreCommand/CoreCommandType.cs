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
    /// Represents available commands for core library
    /// </summary>
    [Guid("52F17F05-7097-4E5E-8263-0696C9EA4205")]
    public enum CoreCommandType
    {
        /// <summary>
        /// Command by default
        /// </summary>
        Default = Nop,

        /// <summary>
        /// No Operation
        /// </summary>
        Nop = 0x90,

        /// <summary>
        /// Returns latest pushed command
        /// </summary>
        LastCommand = 0x100,

        /// <summary>
        /// To abort latest command if it's possible
        /// </summary>
        AbortCommand = 0x101,

        /// <summary>
        /// Unspecified raw command
        /// </summary>
        RawCommand = 0x110,

        /// <summary>
        /// Cancel build operation if it's available for abort
        /// </summary>
        BuildCancel = 0x200,
    }
}
