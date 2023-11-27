/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Provider
{
    [Guid("29223824-6176-47F4-A185-502B04AA3017")]
    public interface ISettings
    {
        /// <summary>
        /// Additional details for all provider elements
        /// </summary>
        bool DebugMode { get; set; }

        /// <summary>
        /// Settings for library
        /// </summary>
        Bridge.ISettings LibSettings { get; set; }
    }
}
