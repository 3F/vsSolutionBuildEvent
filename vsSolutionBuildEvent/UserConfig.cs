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
        /// Loads data from user config file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public virtual bool load(string path, string prefix)
        {
            Link = getLink(path, Config.Entity.NAME + EXT, prefix);
            return loadByLink(Link);
        }

        /// <summary>
        /// Settings from other object.
        /// </summary>
        /// <param name="data">Object with configuration.</param>
        public void load(IUserData data)
        {
            Data = data;
            Updated(this, new DataArgs<IUserData>() { Data = Data });
        }

        /// <summary>
        /// Use link from other configuration for loading new settings.
        /// </summary>
        /// <param name="link">Link from other configuration.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public bool load(string link)
        {
            Link = link + EXT;
            return loadByLink(Link);
        }

        /// <summary>
        /// Load settings from file with path by default.
        /// </summary>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        public bool load()
        {
            return load(Settings.CfgManager.Config.Link);
        }

        /// <summary>
        /// Save settings.
        /// </summary>
        public void save()
        {
            if(Link == null) {
                Log.Trace("User Configuration: Ignore saving. Link is null.");
                return;
            }

            ((IUserDataSvc)Data).updateCommon(false);
            try
            {
                Data.updateCache();
                using(TextWriter stream = new StreamWriter(Link, false, Encoding.UTF8)) {
                    serialize(stream, Data);
                }
                InRAM = false;

                Log.Trace("User Configuration: has been updated '{0}'", Link);
                Updated(this, new DataArgs<IUserData>() { Data = Data });
            }
            catch(Exception ex) {
                Log.Debug("Cannot apply user configuration '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Unload User data.
        /// </summary>
        public void unload()
        {
            Link = null;
            Data = null;
            Updated(this, new DataArgs<IUserData>() { Data = null });
        }

        /// <summary>
        /// Load settings by link to configuration file.
        /// </summary>
        /// <param name="link">Link to configuration file.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        protected virtual bool loadByLink(string link)
        {
            InRAM = false;
            try
            {
                using(StreamReader stream = new StreamReader(link, Encoding.UTF8, true))
                {
                    Data = deserialize(stream);
                    if(Data == null) {
                        throw new UnspecSBEException("file is empty");
                    }
                }
                Log.Trace("User settings: has been loaded from '{0}'", link);
            }
            catch(FileNotFoundException)
            {
                Data    = new UserData();
                InRAM   = true;
                Log.Trace("User settings: Initialized new.");
            }
            catch(Exception ex)
            {
                Log.Debug("User settings is corrupt - '{0}'", ex.Message);
                Data    = new UserData();
                InRAM   = true;
            }

            ((IUserDataSvc)Data).updateCommon(true);
            Configuration.User.Manager.update(Data);

            Updated(this, new DataArgs<IUserData>() { Data = Data });
            return !InRAM;
        }
    }
}
