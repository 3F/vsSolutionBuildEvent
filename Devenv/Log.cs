/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Devenv
{
    /// <summary>
    /// TODO: new lightweight logger or NLog from main plugin
    /// </summary>
    internal class Log: ILog
    {
        /// <summary>
        /// Flag of Diagnostic mode
        /// </summary>
        public bool IsDiagnostic
        {
            get;
            set;
        }

        /// <summary>
        /// Writes message for information level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void info(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        /// <summary>
        /// Writes message for debug level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void debug(string message, params object[] args)
        {
            if(IsDiagnostic) {
                info(message, args);
            }
        }

        /// <param name="diagnostic">Flag of Diagnostic mode</param>
        public Log(bool diagnostic)
        {
            IsDiagnostic = diagnostic;
        }
    }
}
