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
    /// Support of the OutputWindowPane
    /// </summary>
    [Guid("8DA8E950-40F4-49A2-8018-E9AC666AD752")]
    public interface ISolutionEventOWP: ISolutionEvent
    {
        /// <summary>
        /// List of statements from OWP.
        /// TODO: obsolete. use OWP.IMatching /-> C27A1E8C-7808-4529-BAC4-E8322D4F11CD
        /// </summary>
        IMatchWords[] Match { get; set; }
    }
}