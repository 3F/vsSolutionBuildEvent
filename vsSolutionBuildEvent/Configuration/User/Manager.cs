/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Configuration.User
{
    internal class Manager(IUserValue value): IManager
    {
        protected readonly IUserValue value = value ?? throw new ArgumentNullException(nameof(value));

        protected virtual IData Data
            => Settings._.Config.Usr.Data ?? throw new ArgumentNullException(nameof(Data));

        public object Value
        {
            get
            {
                if(!Data.Cache.ContainsKey(value.Guid))
                {
                    Data.Cache[value.Guid] = new Cache();
                }
                return Data.Cache[value.Guid];
            }
        }

        public ICacheHeader CacheHeader => (ICacheHeader)Value;

        public void unset()
        {
            Log.Debug($"Unset cache '{value.Guid}'");

            Data.Cache.Remove(value.Guid);
        }

        public void reset()
        {
            Log.Debug($"Reset cache '{value.Guid}'");

            CacheHeader.Hash = null;
            CacheHeader.Updated = 0;
        }
    }
}
