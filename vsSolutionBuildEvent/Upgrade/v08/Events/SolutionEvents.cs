using System;

namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    [Serializable]
    public class SolutionEvents
    {
        [NonSerialized]
        private SBESettings _settings = new SBESettings();
        /// <summary>
        /// global settings
        /// </summary>
        public SBESettings settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        [NonSerialized]
        private SBEEvent _preBuild = new SBEEvent();
        /// <summary>
        /// Before building solution
        /// </summary>
        public SBEEvent preBuild
        {
            get { return _preBuild; }
            set { _preBuild = value; }
        }

        [NonSerialized]
        private SBEEvent _postBuild = new SBEEvent();
        /// <summary>
        /// After building solution
        /// </summary>
        public SBEEvent postBuild
        {
            get { return _postBuild; }
            set { _postBuild = value; }
        }

        [NonSerialized]
        private SBEEvent _cancelBuild = new SBEEvent();
        /// <summary>
        /// When cancel building solution
        /// e.g. fatal error of compilation or cancel of user
        /// </summary>
        public SBEEvent cancelBuild
        {
            get { return _cancelBuild; }
            set { _cancelBuild = value; }
        }

        [NonSerialized]
        private SBEEventEW _warningsBuild = new SBEEventEW();
        /// <summary>
        /// Warnings during assembly
        /// </summary>
        public SBEEventEW warningsBuild
        {
            get { return _warningsBuild; }
            set { _warningsBuild = value; }
        }

        [NonSerialized]
        private SBEEventEW _errorsBuild = new SBEEventEW();
        /// <summary>
        /// Errors during assembly
        /// </summary>
        public SBEEventEW errorsBuild
        {
            get { return _errorsBuild; }
            set { _errorsBuild = value; }
        }

        [NonSerialized]
        private SBEEventOWP _outputCustomBuild = new SBEEventOWP();
        /// <summary>
        /// Output-Build customization
        /// </summary>
        public SBEEventOWP outputCustomBuild
        {
            get { return _outputCustomBuild; }
            set { _outputCustomBuild = value; }
        }

        [NonSerialized]
        private SBETransmitter _transmitter = new SBETransmitter();
        /// <summary>
        /// Transfer output data to outer handler
        /// </summary>
        public SBETransmitter transmitter
        {
            get { return _transmitter; }
            set { _transmitter = value; }
        }
    }

    public enum SolutionEventType
    {
        Pre, Post, Cancel, Warnings, Errors, OWP, Transmitter,
        /// <summary>
        /// Without identification - all ISolutionEvent
        /// </summary>
        General,
        /// <summary>
        /// Errors + Warnings
        /// </summary>
        EW,
        /// <summary>
        /// By individual projects
        /// </summary>
        ProjectPre,
        ProjectPost,
        /// <summary>
        /// The 'PRE' as deferred action of existing projects
        /// </summary>
        DeferredPre,
    }
}
