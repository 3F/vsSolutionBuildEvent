/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using LibVersion = net.r_eg.vsSBE.Version;

namespace net.r_eg.vsSBE.API
{
    public class Version: Bridge.IVersion
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
        public Bridge.IVersion Bridge
        {
            get { return new Bridge.Info(); }
        }
    }
}
