/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Configuration;

namespace net.r_eg.vsSBE
{
    [Guid("06CBD536-36E8-4EB7-B51B-9767A315BE58")]
    internal interface IAppSettings
    {
        /// <summary>
        /// When <see cref="DebugMode"/> is updated.
        /// </summary>
        /// <remarks>
        /// Useful for clients, for example, using <see cref="Bridge.IEntryPointClient"/>.
        /// </remarks>
        event EventHandler<DataArgs<bool>> DebugModeUpdated;

        /// <summary>
        /// When <see cref="IAppSettings.WorkPath"/> is updated.
        /// </summary>
        event EventHandler<DataArgs<string>> WorkPathUpdated;

        /// <summary>
        /// Debug mode for application.
        /// </summary>
        bool DebugMode { get; set; }

        /// <summary>
        /// Ignores all actions if value set as true.
        /// </summary>
        /// <remarks>
        /// To have control like PRE -&gt; POST [recursive DTE: PRE -&gt; POST] -&gt; etc.
        /// </remarks>
        bool IgnoreActions { get; set; }

        /// <summary>
        /// Full path to the shared (between instances) directory.
        /// </summary>
        string CommonPath { get; }

        /// <summary>
        /// Full path to the library.
        /// </summary>
        string LibPath { get; }

        /// <summary>
        /// Full path to the current working directory.
        /// </summary>
        string WorkPath { get; }

        /// <summary>
        /// Active configuration.
        /// </summary>
        ConfManager Config { get; }

        /// <summary>
        /// Updates <see cref="WorkPath"/>.
        /// </summary>
        /// <param name="path">New path.</param>
        string setWorkPath(string path);
    }
}
