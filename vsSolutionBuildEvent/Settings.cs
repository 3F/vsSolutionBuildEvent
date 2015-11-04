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
using System.IO;
using System.Reflection;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Extensions;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE
{
    internal sealed class Settings: IAppSettings
    {
        /// <summary>
        /// Name of item in VS OutputWindow
        /// </summary>
        public const string OWP_ITEM_VSSBE = "vsSolutionBuildEvent";

        /// <summary>
        /// Name of application.
        /// </summary>
        public const string APP_NAME = "vsSolutionBuildEvent";

        /// <summary>
        /// Short name of application.
        /// </summary>
        public const string APP_NAME_SHORT = "vsSBE";
        
        /// <summary>
        /// When DebugMode is updated.
        /// Useful for clients, for example with IEntryPointClient.
        /// </summary>
        public event EventHandler<DataArgs<bool>> DebugModeUpdated = delegate(object sender, DataArgs<bool> e) { };

        /// <summary>
        /// Debug mode for application.
        /// </summary>
        public bool DebugMode
        {
            get {
                return debugMode;
            }
            set {
                debugMode = value;
                DebugModeUpdated(this, new DataArgs<bool>() { Data = debugMode });
            }
        }
        private bool debugMode = false;

        /// <summary>
        /// Ignores all actions if value set as true.
        /// To support of cycle control, e.g.: PRE -> POST [recursive DTE: PRE -> POST] -> etc.
        /// </summary>
        public bool IgnoreActions
        {
            get { return ignoreActions; }
            set { ignoreActions = value; }
        }
        private volatile bool ignoreActions = false;

        /// <summary>
        /// Checks availability data for used configurations.
        /// </summary>
        public bool IsCfgExists
        {
            get {
                return (Config != null && UserConfig != null);
            }
        }

        /// <summary>
        /// Common path of library.
        /// </summary>
        public string CommonPath
        {
            get
            {
                if(commonPath != null) {
                    return commonPath;
                }

                string vsdir    = System.Environment.GetEnvironmentVariable("VisualStudioDir");
                string path     = Path.Combine((vsdir)?? Path.GetTempPath(), APP_NAME).PathFormat();

                if(!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                commonPath = path;
                return commonPath;
            }
        }
        private string commonPath;

        /// <summary>
        /// Full path to library.
        /// </summary>
        public string LibPath
        {
            get
            {
                if(String.IsNullOrWhiteSpace(libPath)) {
                    libPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).PathFormat();
                }
                return libPath;
            }
        }
        private string libPath;

        /// <summary>
        /// Working path for library.
        /// </summary>
        public string WorkPath
        {
            get
            {
                if(String.IsNullOrWhiteSpace(workPath)) {
                    workPath = "/";
                    Log.Trace("WorkPath is empty or null, use '{0}' by default.", workPath);
                    //throw new SBEException("WorkPath is empty or null");
                }
                return workPath;
            }
        }
        private string workPath;

        /// <summary>
        /// Manager of configurations.
        /// </summary>
        public IManager ConfigManager
        {
            get
            {
                if(configManager == null) {
                    configManager = new Manager(this);
                }
                return configManager;
            }
        }
        private IManager configManager;

        /// <summary>
        /// Main configuration data.
        /// </summary>
        public ISolutionEvents Config
        {
            get
            {
                if(ConfigManager.Config == null) {
                    return null;
                }
                return ConfigManager.Config.Data;
            }
        }

        /// <summary>
        /// User configuration data.
        /// </summary>
        public IUserData UserConfig
        {
            get
            {
                if(ConfigManager.UserConfig == null) {
                    return null;
                }
                return ConfigManager.UserConfig.Data;
            }
        }

        /// <summary>
        /// Global configuration data.
        /// </summary>
        public IUserData GlobalConfig
        {
            get
            {
                IConfig<IUserData> gcfg = ConfigManager.getUserConfigFor(ContextType.Static);
                if(gcfg == null) {
                    return null;
                }
                return gcfg.Data;
            }
        }

        /// <summary>
        /// Static alias. Manager of configurations.
        /// </summary>
        public static IManager CfgManager
        {
            get { return _.ConfigManager; }
        }

        /// <summary>
        /// Static alias. Main configuration data.
        /// </summary>
        public static ISolutionEvents Cfg
        {
            get { return _.Config; }
        }

        /// <summary>
        /// Static alias. User configuration data.
        /// </summary>
        public static IUserData CfgUser
        {
            get { return _.UserConfig; }
        }

        /// <summary>
        /// Static alias. Global configuration data.
        /// </summary>
        public static IUserData CfgGlobal
        {
            get { return _.GlobalConfig; }
        }

        /// <summary>
        /// Static alias. Working path for library.
        /// </summary>
        public static string WPath
        {
            get { return _.WorkPath; }
        }

        /// <summary>
        /// Static alias. Full path to library.
        /// </summary>
        public static string LPath
        {
            get { return _.LibPath; }
        }

        /// <summary>
        /// Updates working path for library.
        /// </summary>
        /// <param name="path">New path.</param>
        public void setWorkPath(string path)
        {
            workPath = path.PathFormat();
        }

        /// <summary>
        /// Thread-safe getting instance from Settings.
        /// </summary>
        public static IAppSettings _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Settings> _lazy = new Lazy<Settings>(() => new Settings());

        private Settings() { }
    }
}
