/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Event of internal logging
    /// </summary>
    public class LoggingEvent: SBEEvent, ISolutionEvent, ILoggingEvent
    {
        /// <summary>
        /// The unique label for any splitting from others.
        /// </summary>
        public const string IDENT_TH = "InternalLoggingAction";
    }
}
