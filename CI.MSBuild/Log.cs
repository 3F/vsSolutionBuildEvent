/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using Microsoft.Build.Framework;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    //TODO: Either use from MvsSln or NLog from main plugin
    internal class Log: ILog
    {
        internal const string DIAG_KEY = "__vssbe_diag";

        /// <summary>
        /// Flag of Diagnostic mode
        /// </summary>
        public bool IsDiagnostic
        {
            //level == LoggerVerbosity.Diagnostic;
            get => string.Equals
            (
                //TODO: 
                Environment.GetEnvironmentVariable(DIAG_KEY, EnvironmentVariableTarget.Process)?.Trim(), 
                "true", 
                StringComparison.InvariantCultureIgnoreCase
            );
        }

        /// <summary>
        /// Level for this instance.
        /// </summary>
        protected LoggerVerbosity level;

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

        public Log(LoggerVerbosity level)
        {
            this.level = level;
        }
    }
}
