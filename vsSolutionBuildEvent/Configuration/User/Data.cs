/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Configuration.User
{
    public sealed class Data: IData, IDataSvc
    {
        private readonly List<IUserValue> removeFromCache = new();

        [JsonProperty(TypeNameHandling = TypeNameHandling.None, ItemTypeNameHandling = TypeNameHandling.All)]
        public Dictionary<string, ICacheHeader> Cache { get; set; } = new();

        [JsonIgnore]
        public Dictionary<IRoute, ICommon> Common { get; set; } = new();

        [JsonProperty(PropertyName = "Common", ItemTypeNameHandling = TypeNameHandling.Objects)]
        private List<_KeyCommon> _Common { get; set; }

        /// <summary>
        /// Wrapper for Common property because it can't handle complex keys when Dictonary by default.
        /// </summary>
        private struct _KeyCommon
        {
            [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
            public IRoute Route;

            [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
            public ICommon Common;
        }

        public void unsetFromCache(IUserValue item)
        {
            if(!removeFromCache.Contains(item)) {
                removeFromCache.Add(item);
            }
        }

        public void cancelCacheRemoving()
        {
            removeFromCache.Clear();
        }

        public void updateCache()
        {
            foreach(IUserValue rc in removeFromCache) {
                rc.Manager.unset();
            }
            removeFromCache.Clear();
        }

        public void updateCommon(bool isLoad)
        {
            _Common ??= new List<_KeyCommon>();

            if(isLoad)
            {
                Common.Clear();
                foreach(_KeyCommon w in _Common) {
                    Common.Add(w.Route, w.Common);
                }
                return;
            }

            _Common.Clear();
            foreach(KeyValuePair<IRoute, ICommon> c in Common) {
                _Common.Add(new _KeyCommon() { Route = c.Key, Common = c.Value });
            }
        }
    }
}
