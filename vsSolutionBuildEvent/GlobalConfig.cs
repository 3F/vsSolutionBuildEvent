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
using net.r_eg.vsSBE.Configuration;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE
{
    internal sealed class GlobalConfig: UserConfig, IConfig<IUserData>
    {
        /// <summary>
        /// Path to global configuration file.
        /// </summary>
        public string ConfigPathGlobal
        {
            get
            {
                if(configPathGlobal != null) {
                    return configPathGlobal;
                }

                string root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                string path = Path.Combine(root, Settings.APP_NAME);
                if(!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                configPathGlobal = path;
                return configPathGlobal;
            }
        }
        private string configPathGlobal;

        /// <summary>
        /// Thread-safe getting instance from GlobalConfig.
        /// </summary>
        public static new GlobalConfig _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<GlobalConfig> _lazy = new Lazy<GlobalConfig>(() => new GlobalConfig());

        /// <summary>
        /// Loads data from user config file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        public override void load(string path, string prefix)
        {
            base.load((path)?? ConfigPathGlobal, prefix);
        }

        private GlobalConfig() { }
    }
}
