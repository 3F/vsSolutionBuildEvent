/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("D9C520D8-E053-4EE3-AA82-1A2400B4F64E")]
    public interface IRoute
    {
        /// <summary>
        /// Identifier of Event.
        /// </summary>
        SolutionEventType Event { get; set; }

        /// <summary>
        /// Identifier of Mode.
        /// </summary>
        ModeType Mode { get; set; }
    }
}
