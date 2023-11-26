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
    [Guid("905CFFE9-1C44-449A-939E-B0ABC4E871C5")]
    public interface IMode
    {
        /// <summary>
        /// Used type from available modes
        /// </summary>
        ModeType Type { get; }
    }
}
