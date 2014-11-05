using System.Collections.Generic;

namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    public class SBEEventEW: SBEEvent, ISolutionEventEW
    {
        private List<string> _codes = new List<string>();
        /// <summary>
        /// list of code####
        /// ..and "for all" if empty
        /// </summary>
        public List<string> codes
        {
            get { return _codes; }
            set { _codes = value; }
        }

        private bool _isWhitelist = true;
        /// <summary>
        /// Whitelist or Blacklist codes
        /// </summary>
        public bool isWhitelist
        {
            get { return _isWhitelist; }
            set { _isWhitelist = value; }
        }
    }
}
