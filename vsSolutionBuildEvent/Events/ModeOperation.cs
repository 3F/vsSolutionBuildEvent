/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with Environment of Visual Studio.
    /// </summary>
    public class ModeOperation: IMode, IModeOperation
    {
        /// <summary>
        /// Type of implementation.
        /// </summary>
        public ModeType Type
        {
            get { return ModeType.Operation; }
        }

        /// <summary>
        /// Atomic commands for handling.
        /// </summary>
        public string[] Command
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for atomic commands.
        /// </summary>
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }
        private string caption = String.Empty;

        /// <summary>
        /// Abort operations on first error.
        /// </summary>
        public bool AbortOnFirstError
        {
            get { return abortOnFirstError; }
            set { abortOnFirstError = value; }
        }
        private bool abortOnFirstError = false;

        /// <param name="command"></param>
        /// <param name="caption"></param>
        public ModeOperation(string[] command, string caption)
        {
            Command         = command;
            this.caption    = caption;
        }

        public ModeOperation()
        {

        }
    }
}
