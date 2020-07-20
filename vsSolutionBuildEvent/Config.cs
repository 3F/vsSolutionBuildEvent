/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Text;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Exceptions;
using SysVersion = System.Version;

namespace net.r_eg.vsSBE
{
    internal class Config: PackerAbstract<ISolutionEvents, SolutionEvents>, IConfig<ISolutionEvents>
    {
        /// <summary>
        /// When data is updated.
        /// </summary>
        public event EventHandler<DataArgs<ISolutionEvents>> Updated = delegate(object sender, DataArgs<ISolutionEvents> e) { };

        /// <summary>
        /// Entity of configuration data.
        /// </summary>
        public struct Entity
        {
            /// <summary>
            /// Config version.
            /// Version of app managed by Package!
            /// </summary>
            public static readonly SysVersion Version = new SysVersion(0, 12, 4);

            /// <summary>
            /// To file system
            /// </summary>
            public const string NAME = ".vssbe";
        }

        /// <summary>
        /// SBE data at runtime.
        /// </summary>
        public ISolutionEvents Data
        {
            get;
            protected set;
        }

        /// <summary>
        /// Loads our data from file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public bool load(string path, string prefix)
        {
            Settings._.setWorkPath(path);
            Link = getLink(Settings.WPath, Entity.NAME, prefix);
            return loadByLink(Link);
        }

        /// <summary>
        /// Settings from other object.
        /// </summary>
        /// <param name="data">Object with configuration.</param>
        public void load(ISolutionEvents data)
        {
            Data = data;
            Updated(this, new DataArgs<ISolutionEvents>() { Data = Data });
        }

        /// <summary>
        /// Use link from other configuration for loading new settings.
        /// </summary>
        /// <param name="link">Link from other configuration.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public bool load(string link)
        {
            Link = link.DirectoryPathFormat();
            return load(Link, null);
        }

        /// <summary>
        /// Load settings from file with path by default.
        /// </summary>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public bool load()
        {
            return load(Settings._.CommonPath, null);
        }

        /// <summary>
        /// Save settings.
        /// </summary>
        public void save()
        {
            if(Link == null) {
                Log.Trace("Configuration: Ignore saving. Link is null.");
                return;
            }

            try
            {
                using(TextWriter stream = new StreamWriter(Link, false, Encoding.UTF8)) {
                    serialize(stream, Data);
                }
                InRAM = false;

                Log.Trace("Configuration: has been saved in '{0}'", Settings.WPath);
                Updated(this, new DataArgs<ISolutionEvents>() { Data = Data });
            }
            catch(Exception ex) {
                Log.Error("Cannot apply configuration '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Unload User data.
        /// </summary>
        public void unload()
        {
            Link = null;
            Data = null;
            Updated(this, new DataArgs<ISolutionEvents>() { Data = null });
        }

        /// <summary>
        /// Load settings by link to configuration file.
        /// </summary>
        /// <param name="link">Link to configuration file.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        protected virtual bool loadByLink(string link)
        {
            InRAM = false;
            var newCfg = new SolutionEvents();

            try
            {
                Data = loadJsonConfig(link);
                warnAboutJsonConfig(SysVersion.Parse(Data.Header.Compatibility));
            }
            catch(FileNotFoundException)
            {
                Data    = newCfg;
                InRAM   = true;
                Log.Info("Initialized with new settings.");
            }
            catch(Newtonsoft.Json.JsonException ex)
            {
                warnAboutXmlConfig();
                Log.Error($"Incorrect configuration data: {ex.Message}");
                Data = newCfg; //xml -> json 0.8-0.9
            }
            catch(Exception ex)
            {
                Log.Error($"Configuration file `{link}` is corrupted: {ex.Message}");
                Data    = newCfg; //TODO: actions in UI, e.g.: restore, new..
                InRAM   = true;
            }

            // Now we'll work with latest version
            Data.Header.Compatibility = Entity.Version.ToString();
            Updated(this, new DataArgs<ISolutionEvents>() { Data = Data });

            return !InRAM;
        }

        private SolutionEvents loadJsonConfig(string link)
        {
            using(StreamReader stream = new StreamReader(link, Encoding.UTF8, true))
            {
                var ret = deserialize(stream);
                if(ret == null) {
                    throw new UnspecSBEException("file is empty");
                }

                Log.Info($"Loaded settings (v{ ret.Header.Compatibility}): '{Settings.WPath}'");
                return ret;
            }
        }

        private void warnAboutJsonConfig(SysVersion cfgVer)
        {
            if(cfgVer > Entity.Version)
            {
                Log.Warn(
                    $"Configuration file v{cfgVer} is higher than supported v{Entity.Version}. Update app for best known behavior."
                );
                return;
            }
        }

        private void warnAboutXmlConfig()
        {
            const string _MSG = "Please use any version from `{0}` for auto-upgrading configuration `{1}`.";

            Log.Warn(_MSG, "0.4.x - 0.8.x", "<= v0.3");
            Log.Warn(_MSG, "0.9.x - 1.14.0", "<= v0.8");
        }
    }
}
