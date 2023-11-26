/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Configuration.User
{
    /// <summary>
    /// Manager of accessing to remote value.
    /// </summary>
    internal class Manager: IManager
    {
        /// <summary>
        /// Unspecified raw value.
        /// </summary>
        public object Value
        {
            get
            {
                checkOnNull();

                //if(value.Type == LinkType.CacheHeader)
                //{
                    if(!data.Cache.ContainsKey(value.Guid)) {
                        data.Cache[value.Guid] = new Cache();
                    }
                    return data.Cache[value.Guid];
                //}
            }
        }

        /// <summary>
        /// Trying to get value as ICacheHeader.
        /// </summary>
        public ICacheHeader CacheHeader
        {
            get {
                return (ICacheHeader)Value;
            }
        }

        /// <summary>
        /// Boxed value.
        /// </summary>
        protected IUserValue value;

        /// <summary>
        /// Common container of data.
        /// </summary>
        private static IData data;


        /// <summary>
        /// To erase current value from common data.
        /// </summary>
        public void unset()
        {
            checkOnNull();
            Log.Trace("Configuration Manager: unset '{0}' /'{1}'", value.Guid, value.Type);

            if(value.Type == LinkType.CacheHeader)
            {
                data.Cache.Remove(value.Guid);
            }
        }

        /// <summary>
        /// Reset data from value.
        /// </summary>
        public void reset()
        {
            checkOnNull();
            Log.Trace("Configuration Manager: reset '{0}' /'{1}'", value.Guid, value.Type);

            if(value.Type == LinkType.CacheHeader)
            {
                CacheHeader.Hash    = null;
                CacheHeader.Updated = 0;
            }
        }

        /// <summary>
        /// Update container.
        /// </summary>
        /// <param name="container">Container of data</param>
        public static void update(IData container)
        {
            data = container;
        }
        
        /// <param name="value">Boxed value</param>
        public Manager(IUserValue value)
        {
            this.value = value;
        }

        protected void checkOnNull()
        {
            if(data == null) {
                throw new ArgumentException("Configuration Manager: container is not initialized.");
            }

            if(value == null) {
                throw new ArgumentException("Configuration Manager: IUserValue value is not initialized.");
            }
        }
    }
}