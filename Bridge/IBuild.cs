/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("15A35810-27BB-4D8F-9CD1-1EAE12C6A9DA")]
    public interface IBuild
    {
        /// <summary>
        /// During assembly.
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        void onBuildRaw(string data);

        /// <summary>
        /// Sets current type of the build
        /// </summary>
        /// <param name="type"></param>
        void updateBuildType(BuildType type);
    }
}
