/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Support of the Errors + Warnings Event type
    /// </summary>
    public class SBEEventEW: SBEEvent, ISolutionEvent, ISolutionEventEW
    {
        /// <inheritdoc cref="ISolutionEventEW.Codes"/>
        public List<string> Codes { get; set; } = new List<string>();

        /// <inheritdoc cref="ISolutionEventEW.IsWhitelist"/>
        public bool IsWhitelist { get; set; } = true;
    }
}
