/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

using AppVersion = net.r_eg.vsSBE.Version;

namespace net.r_eg.vsSBE.API
{
    public class Version: Bridge.IVersion
    {
        public System.Version Number
        {
            get { return AppVersion.number; }
        }

        public string BranchName
        {
            get { return AppVersion.branchName; }
        }

        public string BranchSha1
        {
            get { return AppVersion.branchSha1; }
        }

        public string BranchRevCount
        {
            get { return AppVersion.branchRevCount; }
        }
    }
}
