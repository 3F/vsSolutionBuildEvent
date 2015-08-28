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

using System;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Clients
{
    /// <summary>
    /// Safe wrapper for work with IBuild from client.
    /// TODO: use events in EventLevel over IBuild instead of this wrapper for more convenience...
    /// </summary>
    internal sealed class SBuild: IBuild
    {
        /// <summary>
        /// Link to IBuild instance of the client library
        /// </summary>
        private IBuild link;

        /// <summary>
        /// During assembly.
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        public void onBuildRaw(string data)
        {
            try {
                link.onBuildRaw(data);
            }
            catch(Exception ex) {
                Log.nlog.Error("[Client library] Failed build-raw: '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Sets current type of the build.
        /// </summary>
        /// <param name="type"></param>
        public void updateBuildType(BuildType type)
        {
            try {
                link.updateBuildType(type);
            }
            catch(Exception ex) {
                Log.nlog.Error("[Client library] Failed updateBuildType: '{0}'", ex.Message);
            }
        }

        /// <param name="o">IBuild instance</param>
        public SBuild(IBuild o)
        {
            link = o;
            if(link == null) {
                link = new SBuildEmpty();
            }
        }
    }
}
