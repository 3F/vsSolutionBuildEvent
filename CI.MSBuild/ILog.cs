/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.CI.MSBuild
{
    internal interface ILog
    {
        /// <summary>
        /// Flag of Diagnostic mode
        /// </summary>
        bool IsDiagnostic { get; }

        /// <summary>
        /// Message for information level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void info(string message, params object[] args);

        /// <summary>
        /// Message for debug level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void debug(string message, params object[] args);
    }
}
