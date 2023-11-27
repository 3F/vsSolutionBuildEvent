/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Configuration.User
{
    public class Cache: ICacheHeader
    {
        /// <summary>
        /// When has been updated.
        /// UTC as a rule.
        /// </summary>
        public long Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Type of hashing.
        /// </summary>
        public HashType Algorithm
        {
            get;
            set;
        }

        /// <summary>
        /// Hash value of data.
        /// </summary>
        public string Hash
        {
            get;
            set;
        }
    }
}
