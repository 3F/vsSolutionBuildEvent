/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using net.r_eg.vsSBE.Bridge;

namespace ClientDemo
{
    public class Build: IBuild
    {
        /// <summary>
        /// During assembly.
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        public void onBuildRaw(string data)
        {
            Log._.info($"Entering onBuildRaw(string data): '{data?.Substring(0, Math.Min(40, data.Length))}' ...");
        }

        /// <summary>
        /// Sets current type of the build.
        /// </summary>
        /// <param name="type"></param>
        public void updateBuildType(BuildType type)
        {
            Log._.info("Entering updateBuildType(BuildType type)");
        }
    }
}