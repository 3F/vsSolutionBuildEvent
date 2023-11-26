/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Specifies work with client library
    /// </summary>
    [Guid("7586B777-5104-4BE4-9CA3-E0F7A5E2CE7A")]
    public interface IEntryPointClient
    {
        /// <summary>
        /// Type of implementation.
        /// </summary>
        ClientType Type { get; }

        /// <summary>
        /// Entry point of core library.
        /// Use this for additional work with core library.
        /// </summary>
        IEntryPointCore Core { set; }

        /// <summary>
        /// Version of core library.
        /// Use this for internal settings in client if needed.
        /// </summary>
        IVersion Version { set; }

        /// <summary>
        /// Should provide instance for handling IEvent2 by client from core library.
        /// </summary>
        IEvent2 Event { get; }

        /// <summary>
        /// Should provide instance for handling IBuild by client from core library.
        /// </summary>
        IBuild Build { get; }

        /// <summary>
        /// Load with DTE2 context.
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        void load(object dte2);

        /// <summary>
        /// Load with isolated environment.
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        void load(string sln, Dictionary<string, string> properties);

        /// <summary>
        /// Load with empty environment.
        /// </summary>
        void load();
    }
}
