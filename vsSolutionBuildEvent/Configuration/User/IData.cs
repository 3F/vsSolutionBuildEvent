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
        /// The Common section in user configuration for a specifc <see cref="IRoute"/>.
        /// </summary>
        Dictionary<IRoute, ICommon> Common { get; set; }

        /// <summary>
        /// Various cache information entries.
        /// </summary>
        Dictionary<string, ICacheHeader> Cache { get; set; }

        /// <summary>
        /// Prepares data for removing from cache.
        /// </summary>
        /// <param name="item">What to remove.</param>
        void unsetFromCache(IUserValue item);

        /// <summary>
        /// Cancels a scheduled deletion from cache.
        /// </summary>
        void cancelCacheRemoving();

        /// <summary>
        /// Update actual cache data.
        /// </summary>
        /// <remarks>
        /// Apply scheduled deletion (<see cref="unsetFromCache"/>) etc.
        /// </remarks>
        void updateCache();
    }
}
