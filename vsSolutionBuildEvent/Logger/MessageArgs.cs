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
        /// Formatted message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Source name.
        /// </summary>
        public string Src { get; set; }

        /// <summary>
        /// Source type.
        /// </summary>
        public string SrcType { get; set; }

        /// <summary>
        /// Raw message without level, stamp etc.
        /// </summary>
        public string Raw { get; set; }

        public MessageArgs
        (
            string message,
            string level,
            string src = null,
            string type = null,
            string raw = null
        )
        {
            Message = message;
            Level = level;
            Src = src;
            SrcType = type;
            Raw = raw;
        }

        public MessageArgs()
        {

        }
    }
}
