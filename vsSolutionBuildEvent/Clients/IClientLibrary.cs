/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
