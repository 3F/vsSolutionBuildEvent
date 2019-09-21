/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
