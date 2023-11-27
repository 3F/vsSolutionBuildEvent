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
    /// Processing with environment of Visual Studio
    /// TODO: string type is obsolete. See variant in vsCE
    /// </summary>
    [Guid("3860EC60-0206-422F-A16E-4228DCEF2B30")]
    public interface IModeOperation: ICommandArray<string>
    {
        /// <summary>
        /// Caption for atomic commands.
        /// TODO: obsolete and should be removed
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Abort operations on first error.
        /// </summary>
        bool AbortOnFirstError { get; set; }
    }
}
