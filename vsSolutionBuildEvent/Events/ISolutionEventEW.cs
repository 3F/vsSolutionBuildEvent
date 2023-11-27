/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Support of the Errors / Warnings from the build data
    /// </summary>
    [Guid("EC820CA4-75F1-4A99-B7BF-57448A7C01E6")]
    public interface ISolutionEventEW: ISolutionEvent
    {
        /// <summary>
        /// List of monitored codes
        /// Format: [any text] {error | warning} code####: localizable string
        /// http://msdn.microsoft.com/en-us/library/yxkt8b26%28v=vs.120%29.aspx
        /// </summary>
        List<string> Codes { get; set; }

        /// <summary>
        /// Whitelist or Blacklist for current codes
        /// </summary>
        bool IsWhitelist { get; set; }
    }
}