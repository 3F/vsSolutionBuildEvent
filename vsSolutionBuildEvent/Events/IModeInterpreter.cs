/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Events.Commands;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with streaming tools
    /// </summary>
    [Guid("DEF30B20-9A0B-41FC-97DD-E95BA770FDB0")]
    public interface IModeInterpreter: ICommand
    {
        /// <summary>
        /// Stream handler
        /// </summary>
        string Handler { get; set; }

        /// <summary>
        /// Treat newline as
        /// </summary>
        string Newline { get; set; }

        /// <summary>
        /// Symbol/s for wrapping of commands
        /// </summary>
        string Wrapper { get; set; }
    }
}
