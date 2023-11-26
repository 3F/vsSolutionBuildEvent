/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Clients
{
    /// <summary>
    /// The empty variant of safe wrapper for work with IBuild from client.
    /// TODO: use events in EventLevel over IBuild instead of this wrapper for more convenience...
    /// </summary>
    internal sealed class SBuildEmpty: IBuild
    {
        /// <summary>
        /// During assembly.
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        public void onBuildRaw(string data)
        {

        }

        /// <summary>
        /// Sets current type of the build.
        /// </summary>
        /// <param name="type"></param>
        public void updateBuildType(BuildType type)
        {

        }
    }
}