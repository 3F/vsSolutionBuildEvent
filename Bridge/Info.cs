/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using LibVersion = net.r_eg.vsSBE.Bridge.Version;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("EB5D3D29-C0D2-4CFC-B927-D6F43DFF37AD")]
    public class Info: IVersion
    {
        /// <summary>
        /// Main version number of library
        /// </summary>
        public System.Version Number
        {
            get { return LibVersion.number; }
        }

        /// <summary>
        /// Branch name from SCM
        /// </summary>
        public string BranchName
        {
            get { return LibVersion.B_NAME; }
        }

        /// <summary>
        /// SHA1 revision number from SCM
        /// </summary>
        public string BranchSha1
        {
            get { return LibVersion.B_SHA1; }
        }

        /// <summary>
        /// Count of revisions to used branch from SCM
        /// </summary>
        public string BranchRevCount
        {
            get { return LibVersion.B_REVC; }
        }

        /// <summary>
        /// About Bridge to library if used
        /// </summary>
        public IVersion Bridge
        {
            get { return null; }
        }
    }
}
