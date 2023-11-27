/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Specifies work with core library
    /// </summary>
    [Guid("FF4EA5B6-61F6-43F7-8528-01CF4A482A37")]
    public interface IEntryPointCore
    {
        /// <summary>
        /// Event of core commands.
        /// </summary>
        event CoreCommandHandler CoreCommand;

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="debug">Optional flag of debug mode</param>
        void load(object dte2, bool debug = false);

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="cfg">Specific settings</param>
        void load(object dte2, ISettings cfg);

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="debug">Optional flag of debug mode</param>
        void load(string sln, Dictionary<string, string> properties, bool debug = false);

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="cfg">Specific settings</param>
        void load(string sln, Dictionary<string, string> properties, ISettings cfg);
    }
}
