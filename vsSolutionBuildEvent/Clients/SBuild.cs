/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
                Log.Error($"[Client library] Failed build-raw: {ex.Message}");
                Log.Debug(ex.StackTrace);
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
                Log.Error($"[Client library] Failed updateBuildType: {ex.Message}");
                Log.Debug(ex.StackTrace);
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
