/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Receiver.Output
{
    [Guid("71320C9F-3019-4FF1-B19C-BE2E63713937")]
    public enum EWType
    {
        /// <summary>
        /// Type of common warnings.
        /// </summary>
        Warnings,

        /// <summary>
        /// Type of common errors.
        /// </summary>
        Errors
    }
}
