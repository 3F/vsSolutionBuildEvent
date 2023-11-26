/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.VSTools.ErrorList
{
    [Guid("EA256A50-31B6-45A3-A0BA-773E5CBB6165")]
    public interface IPane
    {
        /// <summary>
        /// To add new error in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void error(string message);

        /// <summary>
        /// To add new warning in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void warn(string message);

        /// <summary>
        /// To add new information in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void info(string message);

        /// <summary>
        /// To clear all messages.
        /// </summary>
        void clear();
    }
}
