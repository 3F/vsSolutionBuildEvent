/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Configuration.User
{
    public struct Route: IRoute
    {
        /// <summary>
        /// Identifier of Event.
        /// </summary>
        public SolutionEventType Event
        {
            get;
            set;
        }

        /// <summary>
        /// Identifier of Mode.
        /// </summary>
        public ModeType Mode
        {
            get;
            set;
        }
    }
}
