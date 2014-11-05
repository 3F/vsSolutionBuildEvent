using System.Collections.Generic;

namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    public class SBEEventOWP: SBEEvent, ISolutionEventOWP
    {
        private List<TEventOWP> _eventsOWP = new List<TEventOWP>();
        /// <summary>
        /// List of term
        /// </summary>
        public List<TEventOWP> eventsOWP
        {
            get { return _eventsOWP; }
            set { _eventsOWP = value; }
        }
    }
}
