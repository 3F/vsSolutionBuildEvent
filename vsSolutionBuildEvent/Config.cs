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
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE
{
    internal sealed class Config: PackerAbstract<ISolutionEvents, SolutionEvents>, IConfig<ISolutionEvents>
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
            /// Version of app is controlled by Package!
            /// </summary>
            public static readonly System.Version Version = new System.Version(0, 9);

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
            private set;
        }
        
        /// <summary>
        /// Thread-safe getting instance from Config.
        /// </summary>
        public static Config _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Config> _lazy = new Lazy<Config>(() => new Config());

        /// <summary>
        /// Link to configuration file.
        /// </summary>
        private string link;


        /// <summary>
        /// Loads our data from file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        public void load(string path, string prefix)
        {
            Settings._.setWorkPath(path);
            link = getLink(path, Entity.NAME, prefix);

            Log.Debug("Configuration: trying to load - '{0}'", link);
            try
            {
                using(StreamReader stream = new StreamReader(link, Encoding.UTF8, true))
                {
                    Data = deserialize(stream);
                    if(Data == null) {
                        throw new SBEException("file is empty");
                    }
                    compatibility(stream);
                }
                Log.Info("Loaded settings (v{0}): '{1}'", Data.Header.Compatibility, Settings.WPath);
            }
            catch(FileNotFoundException)
            {
                Data = new SolutionEvents();
                Log.Info("Initialized with new settings.");
            }
            catch(Newtonsoft.Json.JsonException ex)
            {
                Data = _xmlTryUpgrade(link, ex);
            }
            catch(Exception ex)
            {
                Log.Error("Configuration file is corrupt - '{0}'", ex.Message);
                Data = new SolutionEvents(); //TODO: actions in UI, e.g.: restore, new..
            }

            // Now we work with latest version
            Data.Header.Compatibility = Entity.Version.ToString();
            Updated(this, new DataArgs<ISolutionEvents>() { Data = Data });
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
        /// Save settings.
        /// </summary>
        public void save()
        {
            try
            {
                using(TextWriter stream = new StreamWriter(link, false, Encoding.UTF8)) {
                    serialize(stream, Data);
                }

                Log.Trace("Configuration: has been saved in '{0}'", Settings.WPath);
                Updated(this, new DataArgs<ISolutionEvents>() { Data = Data });
            }
            catch(Exception ex) {
                Log.Error("Cannot apply configuration '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Activation of data with bootloader.
        /// TODO:
        /// </summary>
        /// <param name="bootloader"></param>
        public void updateActivation(IBootloader bootloader)
        {
            foreach(IComponent c in bootloader.Registered)
            {
                if(Data.Components == null || Data.Components.Length < 1) {
                    //c.Enabled = true;
                    continue;
                }

                Configuration.Component found = Data.Components.Where(p => p.ClassName == c.GetType().Name).FirstOrDefault();
                if(found != null) {
                    c.Enabled = found.Enabled;
                }
            }
        }

        /// <summary>
        /// Checks version and reorganizes structure if needed..
        /// </summary>
        /// <param name="stream"></param>
        private void compatibility(StreamReader stream)
        {
            System.Version cfg = System.Version.Parse(Data.Header.Compatibility);

            if(cfg.Major > Entity.Version.Major || (cfg.Major == Entity.Version.Major && cfg.Minor > Entity.Version.Minor)) {
                Log.Warn(
                    "Version {0} of configuration file is higher supported version {1}. Please update application. Several settings may be not correctly loaded.",
                    cfg.ToString(2), Entity.Version.ToString(2)
                );
            }

            if(cfg.Major == 0 && cfg.Minor < 4)
            {
                Log._.show();
                Log.Info("Upgrading configuration for <= v0.3.x");
                //Upgrade.Migration03_04.migrate(stream);
                Log.Warn("[Obsolete] Not supported. Use of any v0.4.x - v0.8.x for upgrading <= v0.3.x");
            }
        }

        /// <summary>
        /// Upgrades from xml.
        /// </summary>
        /// <param name="file">Configuration file</param>
        /// <param name="inner"></param>
        /// <returns></returns>
        private ISolutionEvents _xmlTryUpgrade(string file, Newtonsoft.Json.JsonException inner)
        {
            try {
                ISolutionEvents ret = Upgrade.v08.Migration08_09.migrate(file);
                Log.Info("Successfully upgraded settings. *Save manually! :: -> {0}", Entity.NAME);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Incorrect configuration data: '{0}' -> '{1}'. Initialize new.", inner.Message, ex.Message);
            }
            return new SolutionEvents();
        }

        private Config() { }
    }
}
