using System.Collections.Generic;

namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    /// <summary>
    /// Errors & Warnings
    /// </summary>
    public interface ISolutionEventOWP: ISolutionEvent
    {
        /// <summary>
        /// List of term
        /// </summary>
        List<TEventOWP> eventsOWP { get; set; }
    }

    public enum TEventOWPTerm
    {
        Default,
        Regexp,
        Wildcards
    }

    /// <summary>
    /// Customization of OutputWindowPane
    /// </summary>
    public struct TEventOWP
    {
        /// <summary>
        /// various condition
        /// </summary>
        public string term { get; set; }

        /// <summary>
        /// type of recognition
        /// </summary>
        public TEventOWPTerm type { get; set; }
    }
}