﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Configuration.User
{
    public interface IUserValue
    {
        /// <summary>
        /// External node ID.
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// External node access manager.
        /// </summary>
        IManager Manager { get; }
    }
}
