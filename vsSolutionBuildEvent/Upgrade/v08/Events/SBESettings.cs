
namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    public class SBESettings
    {
        /// <summary>
        /// this value used by default if current attr not found after deserialize
        /// :: v0.2.x/v0.1.x
        /// </summary>
        private string _compatibility = "0.1";
        /// <summary>
        /// for identification of compatibility between versions
        /// </summary>
        public string compatibility
        {
            get { return _compatibility; }
            set { _compatibility = value; }
        }

        //TODO: direct..
        public string application = "http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/";
    }
}
