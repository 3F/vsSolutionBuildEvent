using System.Collections.Generic;

namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    /// <summary>
    /// Errors + Warnings
    /// </summary>
    public interface ISolutionEventEW: ISolutionEvent
    {
        /// <summary>
        /// list of code####
        /// ..and "for all" if empty
        /// </summary>
        List<string> codes { get; set; }

        /// <summary>
        /// Whitelist or Blacklist codes
        /// </summary>
        bool isWhitelist { get; set; }
    }
}