/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("86A57222-31EF-4C98-B960-F1478DB1A565")]
    public interface IData
    {
        /// <summary>
        /// Global settings.
        /// </summary>
        IGlobal Global { get; set; }

        /// <summary>
        /// Common settings for specific route.
        /// </summary>
        Dictionary<IRoute, ICommon> Common { get; set; }

        /// <summary>
        /// Different headers of cache data.
        /// </summary>
        Dictionary<string, ICacheHeader> Cache { get; set; }

        /// <summary>
        /// Prepares data to removing from cache.
        /// </summary>
        /// <param name="item">Data that should be soon removed.</param>
        void toRemoveFromCache(IUserValue item);

        /// <summary>
        /// To avoid of planned removing data from cache.
        /// </summary>
        void avoidRemovingFromCache();

        /// <summary>
        /// Updating of the cache container from unused data etc.
        /// </summary>
        void updateCache();
    }
}
