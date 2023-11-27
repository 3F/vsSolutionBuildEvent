/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("6F40D8EE-C79C-47EB-9EA0-5594C63779B0")]
    public interface IVersion
    {
        /// <summary>
        /// Main version number of library
        /// </summary>
        System.Version Number { get; }

        /// <summary>
        /// Branch name from SCM
        /// </summary>
        string BranchName { get; }

        /// <summary>
        /// SHA1 revision number from SCM
        /// </summary>
        string BranchSha1 { get; }

        /// <summary>
        /// Count of revisions to used branch from SCM
        /// </summary>
        string BranchRevCount { get; }

        /// <summary>
        /// About Bridge to library if used
        /// </summary>
        IVersion Bridge { get; }
    }
}
