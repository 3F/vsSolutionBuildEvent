﻿/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
