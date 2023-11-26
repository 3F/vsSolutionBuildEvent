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
        protected readonly ILog log;

        public void onBuildRaw(string data)
        {
            log.Info($"Entering onBuildRaw(string data): '{data?.Substring(0, Math.Min(40, data.Length))}' ...");
        }

        public void updateBuildType(BuildType type)
        {
            log.Info("Entering updateBuildType(BuildType type)");
        }

        public Build(ILog log)
        {
            this.log = log;
        }
    }
}