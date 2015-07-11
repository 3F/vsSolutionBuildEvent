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
using System.IO;
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace net.r_eg.vsSBE
{
    internal class Config
    {
        /// <summary>
        /// Notification about updating configuration
        /// </summary>
        public delegate void UpdateEvent();

        /// <summary>
        /// After update the SBE-data
        /// </summary>
        public event UpdateEvent Update = delegate { };

        /// <summary>
        /// Entity of configuration data.
        /// </summary>
        public struct Entity
        {
            /// <summary>
            /// Current config version
            /// Notice: version of app is controlled by Package
            /// </summary>
            public static readonly System.Version Version = new System.Version(0, 9);

            /// <summary>
            /// To file system
            /// </summary>
            public const string NAME = ".vssbe";
        }

        /// <summary>
        /// SBE data at runtime
        /// TODO: 
        /// </summary>
        public SolutionEvents Data
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Thread-safe getting the instance of Config class
        /// </summary>
        public static Config _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Config> _lazy = new Lazy<Config>(() => new Config());

        /// <summary>
        /// Link to configuration file
        /// </summary>
        protected string link;

        /// <summary>
        /// Loads our data from file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        public void load(string path, string prefix = null)
        {
            Settings.setWorkingPath(path);

            // check special version
            link = _formatLink(path, Entity.NAME, prefix);
            if(!File.Exists(link))
            {
                Log.nlog.Trace("Configuration: the special version is not found /'{0}':'{1}'", prefix, link);
                link = _formatLink(path, Entity.NAME);
            }

            Log.nlog.Debug("Configuration: trying to load - '{0}'", link);
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
                Log.nlog.Info("Loaded settings (v{0}): '{1}'\n\nReady:", Data.Header.Compatibility, Settings.WorkingPath);
            }
            catch(FileNotFoundException)
            {
                Data = new SolutionEvents();
                Log.nlog.Info("Initialized with new settings.");
            }
            catch(JsonException ex)
            {
                Data = _xmlTryUpgrade(link, ex);
            }
            catch(Exception ex)
            {
                Log.nlog.Error("Configuration file is corrupt - '{0}'", ex.Message);
                Data = new SolutionEvents(); //TODO: actions in UI, e.g.: restore, new..
            }

            // Now we work with latest version
            Data.Header.Compatibility = Entity.Version.ToString();
            Update();
        }

        /// <summary>
        /// Settings from other object.
        /// </summary>
        /// <param name="data"></param>
        public void load(SolutionEvents data)
        {
            Data = data;
        }

        /// <summary>
        /// Save settings with changing path.
        /// </summary>
        /// <param name="path">path to configuration file</param>
        public void save(string path)
        {
            Settings.setWorkingPath(path);
            save();
        }

        public void save()
        {
            try {
                using(TextWriter stream = new StreamWriter(link, false, Encoding.UTF8)) {
                    serialize(stream, Data);
                }
                Log.nlog.Trace("Configuration: saved into '{0}'", Settings.WorkingPath);
                Update();
            }
            catch(Exception ex) {
                Log.nlog.Error("Cannot apply configuration '{0}'", ex.Message);
            }
        }

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

        public string serialize(SolutionEvents data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter{ 
                AllowIntegerValues  = false,
                CamelCaseText       = true
            });
            settings.NullValueHandling = NullValueHandling.Include;
            return JsonConvert.SerializeObject(data, Formatting.Indented, settings);
        }

        public SolutionEvents deserialize(string data)
        {
            return JsonConvert.DeserializeObject<SolutionEvents>(data, new JsonSerializerSettings() {
                Binder = new JsonSerializationBinder()
            });
        }

        protected SolutionEvents deserialize(StreamReader stream)
        {
            using(JsonTextReader reader = new JsonTextReader(stream))
            {
                JsonSerializer js = new JsonSerializer() {
                    Binder = new JsonSerializationBinder() 
                };
                return js.Deserialize<SolutionEvents>(reader);
            }
        }

        protected void serialize(TextWriter stream, SolutionEvents data)
        {
            stream.Write(serialize(data));
        }

        /// <summary>
        /// Older versions support :: Check version and reorganize structure if needed..
        /// </summary>
        /// <param name="stream"></param>
        protected void compatibility(StreamReader stream)
        {
            System.Version cfg = System.Version.Parse(Data.Header.Compatibility);

            if(cfg.Major > Entity.Version.Major || (cfg.Major == Entity.Version.Major && cfg.Minor > Entity.Version.Minor)) {
                Log.nlog.Warn(
                    "Version {0} of configuration file is higher supported version {1}. Please update application. Several settings may be not correctly loaded.",
                    cfg.ToString(2), Entity.Version.ToString(2)
                );
            }

            if(cfg.Major == 0 && cfg.Minor < 4)
            {
                Log.show();
                Log.nlog.Info("Upgrading configuration for <= v0.3.x");
                //Upgrade.Migration03_04.migrate(stream);
                Log.nlog.Warn("[Obsolete] Not supported. Use of any v0.4.x - v0.8.x for upgrading <= v0.3.x");
            }
        }

        /// <param name="path">Full path to configuration file</param>
        /// <param name="name">File name</param>
        /// <param name="prefix">Special version of configuration file if used</param>
        /// <returns></returns>
        private string _formatLink(string path, string name, string prefix = null)
        {
            return Path.Combine(path, String.Format("{0}{1}", prefix, name));
        }

        /// <summary>
        /// Upgrades from xml.
        /// </summary>
        /// <param name="file">Configuration file</param>
        /// <param name="innerException"></param>
        /// <returns></returns>
        private SolutionEvents _xmlTryUpgrade(string file, JsonException innerException)
        {
            try {
                SolutionEvents ret = Upgrade.v08.Migration08_09.migrate(file);
                Log.nlog.Info("Successfully upgraded settings. *Save manually! :: -> {0}", Entity.NAME);
                return ret;
            }
            catch(Exception ex) {
                Log.nlog.Error("Incorrect configuration data: '{0}' -> '{1}'. Initialize new.", innerException.Message, ex.Message);
            }
            return new SolutionEvents();
        }

        private Config() { }
    }
}
