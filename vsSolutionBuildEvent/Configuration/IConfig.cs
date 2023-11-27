/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
        /// Load settings from path.
        /// </summary>
        /// <param name="path">Full path to directory where configuration file is located.</param>
        /// <param name="prefix">Special version of configuration file if not null.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        bool loadPath(string path, string prefix = null);

        /// <summary>
        /// Load settings from other object.
        /// </summary>
        /// <param name="data">Object with configuration.</param>
        void load(T data);

        /// <summary>
        /// Load settings using direct link to the file.
        /// </summary>
        /// <param name="link">Full path to.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        bool load(string link);

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
