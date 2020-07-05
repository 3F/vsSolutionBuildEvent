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
