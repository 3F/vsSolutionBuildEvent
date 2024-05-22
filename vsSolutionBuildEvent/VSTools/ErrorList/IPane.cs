/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.VSTools.ErrorList
{
    public interface IPane
    {
        /// <summary>
        /// To add new error in ErrorList.
        /// </summary>
        void error(string message, string src, string type);

        /// <summary>
        /// To add new warning in ErrorList.
        /// </summary>
        void warn(string message, string src, string type);

        /// <summary>
        /// To add new information in ErrorList.
        /// </summary>
        void info(string message, string src, string type);

        /// <summary>
        /// To clear all messages.
        /// </summary>
        void clear();
    }
}
