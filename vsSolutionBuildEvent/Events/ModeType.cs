/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Type of available processing modes
    /// </summary>
    [Guid("F7E15CE6-F7B1-44A6-AB55-62F1C8BEDE26")]
    public enum ModeType
    {
        /// <summary>
        /// Unspecified mode.
        /// </summary>
        Common = 0x10,

        /// <summary>
        /// Alias to Common.
        /// </summary>
        General = Common,

        /// <summary>
        /// External handling with files.
        /// </summary>
        File = 0x100,

        /// <summary>
        /// Processing with external interpreter.
        /// generally, it's a stream processor etc.
        /// </summary>
        Interpreter = 0x101,

        /// <summary>
        /// DTE-commands - operations with EnvDTE.
        /// </summary>
        Operation = 0x102,

        /// <summary>
        /// Script processing.
        /// generally, it's internal handling with MSBuild / SBE-Scripts cores, and similar
        /// </summary>
        Script = 0x103,

        /// <summary>
        /// MSBuild targets
        /// </summary>
        Targets = 0x104,

        /// <summary>
        /// C# code
        /// </summary>
        CSharp = 0x105,
        
        /// <summary>
        /// Environment Commands
        /// </summary>
        EnvCommand = 0x106,
    }
}
