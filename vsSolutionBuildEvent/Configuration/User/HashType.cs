/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("CD46BF40-05AF-40F1-9569-F0A1981D4D93")]
    public enum HashType
    {
        /// <summary>
        /// ~335 MiB/Second	with CPU frequency ~2.194e+09 Hz
        /// </summary>
        MD5,

        /// <summary>
        /// Tiger tree.
        /// ~328 MiB/Second	with CPU frequency ~2.194e+09 Hz
        /// </summary>
        TTH,

        /// <summary>
        /// SHA-1
        /// ~192 MiB/Second	with CPU frequency ~2.194e+09 Hz
        /// </summary>
        SHA1,
    }
}
