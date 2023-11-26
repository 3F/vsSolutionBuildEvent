/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Receiver.Output
{
    [Serializable]
    public class PaneArgs: EventArgs
    {
        /// <summary>
        /// Raw message
        /// </summary>
        public string Raw
        {
            get;
            set;
        }

        /// <summary>
        /// Guid string of pane
        /// </summary>
        public string Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Name of item pane
        /// </summary>
        public string Item
        {
            get;
            set;
        }
    }
}
