/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Clients
{
    [Guid("D5B1F1B9-4BB7-415E-BFBE-81D0FFE186F9")]
    public interface IClientLibrary
    {
        /// <summary>
        /// Name of client library with full path.
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Absolute path to client library.
        /// </summary>
        string Dllpath { get; }

        /// <summary>
        /// Checking existence of client library.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Access to IEvent2 in client library.
        /// </summary>
        IEvent2 Event { get; }

        /// <summary>
        /// Access to IBuild in client library.
        /// </summary>
        IBuild Build { get; }

        /// <summary>
        /// Trying of loading for DTE2 context
        /// </summary>
        /// <param name="core">Entry point of core library</param>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <returns>true value if library exists and successfully loaded</returns>
        bool tryLoad(IEntryPointCore core, object dte2);

        /// <summary>
        /// Trying of loading for Isolated environment
        /// </summary>
        /// <param name="core">Entry point of core library</param>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <returns>true value if library exists and successfully loaded</returns>
        bool tryLoad(IEntryPointCore core, string sln, Dictionary<string, string> properties);
    }
}
