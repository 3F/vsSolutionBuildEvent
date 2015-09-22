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
using System.Text;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Exceptions;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;
using IUserDataSvc = net.r_eg.vsSBE.Configuration.User.IDataSvc;
using UserData = net.r_eg.vsSBE.Configuration.User.Data;

namespace net.r_eg.vsSBE
{
    internal class UserConfig: PackerAbstract<IUserData, UserData>, IConfig<IUserData>
    {
        /// <summary>
        /// Extension of UserConfig.
        /// </summary>
        internal const string EXT = ".user";

        /// <summary>
        /// When data is updated.
        /// </summary>
        public event EventHandler<DataArgs<IUserData>> Updated = delegate(object sender, DataArgs<IUserData> e) { };

        /// <summary>
        /// User data at runtime.
        /// </summary>
        public IUserData Data
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Thread-safe getting instance from UserConfig.
        /// </summary>
        public static UserConfig _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<UserConfig> _lazy = new Lazy<UserConfig>(() => new UserConfig());

        /// <summary>
        /// Link to configuration file.
        /// </summary>
        protected string link;


        /// <summary>
        /// Loads data from user config file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        public virtual void load(string path, string prefix = null)
        {
            link = getLink(path, Config.Entity.NAME + EXT, prefix);
            try
            {
                using(StreamReader stream = new StreamReader(link, Encoding.UTF8, true))
                {
                    Data = deserialize(stream);
                    if(Data == null) {
                        throw new SBEException("file is empty");
                    }
                }
                Log.Trace("User settings: has been loaded from '{0}'", link);
            }
            catch(FileNotFoundException)
            {
                Data = new UserData();
                Log.Trace("User settings: Initialized new.");
            }
            catch(Exception ex)
            {
                Log.Debug("User settings is corrupt - '{0}'", ex.Message);
                Data = new UserData();
            }

            ((IUserDataSvc)Data).updateCommon(true);
            Configuration.User.Manager.update(Data);

            Updated(this, new DataArgs<IUserData>() { Data = Data });
            configure(Data.Global);
        }

        /// <summary>
        /// Settings from other object.
        /// </summary>
        /// <param name="data">Object with configuration.</param>
        public void load(IUserData data)
        {
            Data = data;
            Updated(this, new DataArgs<IUserData>() { Data = Data });
            configure(Data.Global);
        }

        /// <summary>
        /// Save settings.
        /// </summary>
        public void save()
        {
            ((IUserDataSvc)Data).updateCommon(false);
            try
            {
                Data.updateCache();
                using(TextWriter stream = new StreamWriter(link, false, Encoding.UTF8)) {
                    serialize(stream, Data);
                }

                Log.Trace("User Configuration: has been updated '{0}'", link);
                Updated(this, new DataArgs<IUserData>() { Data = Data });
            }
            catch(Exception ex) {
                Log.Debug("Cannot apply user configuration '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Apply global configuration.
        /// </summary>
        /// <param name="cfg"></param>
        protected void configure(Configuration.User.IGlobal cfg)
        {
            Settings._.DebugMode = cfg.DebugMode;
        }

        protected UserConfig() { }
    }
}
