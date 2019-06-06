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

namespace net.r_eg.vsSBE.Configuration
{
    [Guid("5ED372D0-2585-4B87-95DB-C356B5B266D6")]
    public interface IConfig<T>
    {
        /// <summary>
        /// When data is updated.
        /// </summary>
        event EventHandler<DataArgs<T>> Updated;

        /// <summary>
        /// User data at runtime.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Link to configuration file.
        /// </summary>
        string Link { get; }

        /// <summary>
        /// Get status of configuration data.
        /// true value if data exists only in RAM, otherwise used existing file.
        /// </summary>
        bool InRAM { get; }

        /// <summary>
        /// Load settings from file.
        /// </summary>
        /// <param name="path">Path to configuration file.</param>
        /// <param name="prefix">Special version of configuration file.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        bool load(string path, string prefix);

        /// <summary>
        /// Load settings from other object.
        /// </summary>
        /// <param name="data">Object with configuration.</param>
        void load(T data);

        /// <summary>
        /// Use link from other configuration for loading new settings.
        /// </summary>
        /// <param name="link">Link from other configuration.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        bool load(string link);

        /// <summary>
        /// Load settings from file with path by default.
        /// </summary>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        bool load();

        /// <summary>
        /// Save settings.
        /// </summary>
        void save();

        /// <summary>
        /// Unload User data.
        /// </summary>
        void unload();
    }
}
