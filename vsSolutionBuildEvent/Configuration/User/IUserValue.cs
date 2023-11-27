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
    [Guid("85814141-144E-4C4A-8C1D-B5884D01BBCE")]
    public interface IUserValue
    {
        /// <summary>
        /// Type of link to external value.
        /// </summary>
        LinkType Type { get; set; }

        /// <summary>
        /// Guid of external node.
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// Manager of accessing to remote value.
        /// </summary>
        IManager Manager { get; }
    }
}
