/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
        string DefaultOWPItem { get; set; }

        /// <summary>
        /// Updates working path for library.
        /// </summary>
        /// <param name="path">New path.</param>
        void setWorkPath(string path);
    }
}
