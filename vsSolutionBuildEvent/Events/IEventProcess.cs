/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Declaring of handling process
    /// </summary>
    public interface IEventProcess
    {
        /// <summary>
        /// Waiting completion
        /// </summary>
        bool Waiting { get; set; }

        /// <summary>
        /// Hiding of processing or not
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        /// How long to wait the execution, in seconds. 0 value - infinitely
        /// </summary>
        int TimeLimit { get; set; }
    }
}
