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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Configuration.User
{
    public sealed class Data: IData, IDataSvc
    {
        /// <summary>
        /// Global settings.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IGlobal Global
        {
            get { return global; }
            set { global = value; }
        }
        private IGlobal global = new Global();

        /// <summary>
        /// Common settings for specific route.
        /// </summary>
        [JsonIgnore]
        public Dictionary<IRoute, ICommon> Common
        {
            get { return common; }
            set { common = value; }
        }
        private Dictionary<IRoute, ICommon> common = new Dictionary<IRoute, ICommon>();

        /// <summary>
        /// Different headers of cache data.
        /// </summary>
        [JsonProperty(TypeNameHandling = TypeNameHandling.None, ItemTypeNameHandling = TypeNameHandling.All)]
        public Dictionary<string, ICacheHeader> Cache
        {
            get { return cache; }
            set { cache = value; }
        }
        private Dictionary<string, ICacheHeader> cache = new Dictionary<string, ICacheHeader>();

        /// <summary>
        /// List of data for removing from cache.
        /// </summary>
        private List<IUserValue> toRemovingFromCache = new List<IUserValue>();


        /// <summary>
        /// Prepares data to removing from cache.
        /// </summary>
        /// <param name="item">Data that should be soon removed.</param>
        public void toRemoveFromCache(IUserValue item)
        {
            if(!toRemovingFromCache.Contains(item)) {
                toRemovingFromCache.Add(item);
            }
        }

        /// <summary>
        /// Avoid of planned removing data from cache.
        /// </summary>
        public void avoidRemovingFromCache()
        {
            toRemovingFromCache.Clear();
        }

        /// <summary>
        /// Updating of the cache container from unused data etc.
        /// </summary>
        public void updateCache()
        {
            foreach(IUserValue rc in toRemovingFromCache) {
                rc.Manager.unset();
            }
            toRemovingFromCache.Clear();
        }

        /// <summary>
        /// Update Common property.
        /// </summary>
        /// <param name="isLoad">Update for loading or saving.</param>
        public void updateCommon(bool isLoad)
        {
            if(_Common == null) {
                _Common = new List<_KeyCommon>();
            }

            if(isLoad)
            {
                common.Clear();
                foreach(_KeyCommon w in _Common) {
                    common.Add(w.Route, w.Common);
                }
                return;
            }

            _Common.Clear();
            foreach(KeyValuePair<IRoute, ICommon> c in common) {
                _Common.Add(new _KeyCommon() { Route = c.Key, Common = c.Value });
            }
        }

        /// <summary>
        /// Wrapper of Common property above.
        /// Special for Newtonsoft.Json, because it can't work with complex keys for Dictonary by default.
        /// </summary>
        private struct _KeyCommon
        {
            [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
            public IRoute Route;

            [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
            public ICommon Common;
        }
        [JsonProperty(PropertyName = "Common", ItemTypeNameHandling = TypeNameHandling.Objects)]
        private List<_KeyCommon> _Common { get; set; }
    }
}
