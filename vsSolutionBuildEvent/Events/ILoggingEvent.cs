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
    /// Public events of internal logging
    /// </summary>
    [Guid("E01CD0C6-E9C1-4FAB-A9F0-B6AF4F159C38")]
    public interface ILoggingEvent: ISolutionEvent
    {

    }
}