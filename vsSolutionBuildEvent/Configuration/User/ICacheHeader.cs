/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("B305C9E9-F900-4F86-A4B8-BF57A97A9AEA")]
    public interface ICacheHeader
    {
        /// <summary>
        /// When has been updated.
        /// UTC as a rule.
        /// </summary>
        long Updated { get; set; }

        /// <summary>
        /// Type of hashing.
        /// </summary>
        HashType Algorithm { get; set; } 

        /// <summary>
        /// Hash value of data.
        /// </summary>
        string Hash { get; set; }
    }
}
