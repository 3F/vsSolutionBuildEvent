/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Logger
{
    [Serializable]
    public class MessageArgs: EventArgs
    {
        /// <summary>
        /// Received message.
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Received level.
        /// </summary>
        public string Level
        {
            get;
            set;
        }
    }
}
