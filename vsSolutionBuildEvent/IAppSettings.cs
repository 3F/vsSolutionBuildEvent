/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Configuration;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE
{
    [Guid("06CBD536-36E8-4EB7-B51B-9767A315BE58")]
    internal interface IAppSettings
    {
        /// <summary>
        /// When DebugMode is updated.
        /// Useful for clients, for example with IEntryPointClient.
        /// </summary>
        event EventHandler<DataArgs<bool>> DebugModeUpdated;

        /// <summary>
        /// When IAppSettings.WorkPath was updated.
        /// </summary>
        event EventHandler<DataArgs<string>> WorkPathUpdated;

        /// <summary>
        /// Debug mode for application.
        /// </summary>
        bool DebugMode { get; set; }

        /// <summary>
        /// Ignores all actions if value set as true.
        /// To support of cycle control, e.g.: PRE -> POST [recursive DTE: PRE -> POST] -> etc.
        /// </summary>
        bool IgnoreActions { get; set; }

        /// <summary>
        /// Checks availability data for used configurations.
        /// </summary>
        bool IsCfgExists { get; }

        /// <summary>
        /// Common path of library.
        /// </summary>
        string CommonPath { get; }

        /// <summary>
        /// Full path to library.
        /// </summary>
        string LibPath { get; }

        /// <summary>
        /// Working path for library.
        /// </summary>
        string WorkPath { get; }

        /// <summary>
        /// Manager of configurations.
        /// </summary>
        IManager ConfigManager { get; }

        /// <summary>
        /// Main configuration data.
        /// </summary>
        ISolutionEvents Config { get; }

        /// <summary>
        /// User configuration data.
        /// </summary>
        IUserData UserConfig { get; }

        /// <summary>
        /// Global configuration data.
        /// </summary>
        IUserData GlobalConfig { get; }

        /// <summary>
        /// OWP item name by default.
        /// </summary>
        string DefaultOWPItem { get; }

        /// <summary>
        /// Updates working path for library.
        /// </summary>
        /// <param name="path">New path.</param>
        void setWorkPath(string path);
    }
}
