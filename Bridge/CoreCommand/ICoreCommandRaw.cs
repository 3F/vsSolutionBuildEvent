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
    /// Specifies work for raw commands from core library
    /// </summary>
    [Guid("AFCA0A4C-1AAC-4E02-87E5-43B9B26891E7")]
    public interface ICoreCommandRaw: ICoreCommand
    {
        /// <summary>
        /// Unspecified mixed raw command.
        /// </summary>
        object Raw { get; set; }
    }
}
