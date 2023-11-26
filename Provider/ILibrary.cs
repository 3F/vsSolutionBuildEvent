/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Provider
{
    [Guid("536FFC0D-8BC1-4347-B4B4-694308F9E396")]
    public interface ILibrary
    {
        /// <summary>
        /// Absolute path to used library
        /// </summary>
        string Dllpath { get; }

        /// <summary>
        /// Name of used library with full path
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Version of used library
        /// </summary>
        Bridge.IVersion Version { get; }

        /// <summary>
        /// All public events of used library
        /// </summary>
        IEvent Event { get; }

        /// <summary>
        /// The Build operations of used library
        /// </summary>
        IBuild Build { get; }

        /// <summary>
        /// Settings of used library
        /// </summary>
        Bridge.ISettings Settings { get; }

        /// <summary>
        /// Entry point for core library
        /// </summary>
        IEntryPointCore EntryPoint { get; }
    }
}
