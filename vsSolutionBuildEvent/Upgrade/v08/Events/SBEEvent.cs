
namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    public class SBEEvent: ISolutionEvent
    {
        private string _command = "";
        /// <summary>
        /// execution of shell command
        /// </summary>
        public string command
        {
            get { return _command; }
            set { _command = value; }
        }

        private string _caption = "";
        /// <summary>
        /// output information to "Output" window or something else...
        /// </summary>
        public string caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        private bool _enabled = false;
        /// <summary>
        /// status of activate
        /// </summary>
        public bool enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        private bool _processHide = true;
        public bool processHide
        {
            get { return _processHide; }
            set { _processHide = value; }
        }

        private TModeCommands _mode = TModeCommands.Interpreter;
        /// <summary>
        /// processing mode
        /// </summary>
        public TModeCommands mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private bool _processKeep = false;
        /// <summary>
        /// not close after completion
        /// </summary>
        public bool processKeep
        {
            get { return _processKeep; }
            set { _processKeep = value; }
        }

        private string _interpreter = "";
        /// <summary>
        /// stream processor
        /// </summary>
        public string interpreter
        {
            get { return _interpreter; }
            set { _interpreter = value; }
        }

        private string _newline = "";
        /// <summary>
        /// treat newline as
        /// </summary>
        public string newline
        {
            get { return _newline; }
            set { _newline = value; }
        }

        private string _wrapper = "";
        /// <summary>
        /// symbol wrapper for commands or script
        /// </summary>
        public string wrapper
        {
            get { return _wrapper; }
            set { _wrapper = value; }
        }

        private bool _waitForExit = true;
        /// <summary>
        /// Wait until terminates script handling
        /// </summary>
        public bool waitForExit
        {
            get { return _waitForExit; }
            set { _waitForExit = value; }
        }

        private bool _parseVariablesMSBuild = true;
        /// <summary>
        /// support of MSBuild environment variables (properties)
        /// </summary>
        public bool parseVariablesMSBuild
        {
            get { return _parseVariablesMSBuild; }
            set { _parseVariablesMSBuild = value; }
        }

        private bool _buildFailedIgnore = false;
        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        public bool buildFailedIgnore
        {
            get { return _buildFailedIgnore; }
            set { _buildFailedIgnore = value; }
        }

        private string[] _toConfiguration = null;
        /// <summary>
        /// Run only for a specific configuration of solution
        /// strings format as:
        ///   'configname'|'platformname'
        ///   Compatible with: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        public string[] toConfiguration
        {
            get { return _toConfiguration; }
            set { _toConfiguration = value; }
        }

        private TExecutionOrder[] _executionOrder;
        /// <summary>
        /// Run for selected projects with execution order
        /// </summary>
        public TExecutionOrder[] executionOrder
        {
            get { return _executionOrder; }
            set { _executionOrder = value; }
        }

        private TOperation _dteExec = new TOperation();
        /// <summary>
        /// Common Environment Visual Studio. Executes the specified commands
        /// TODO: custom list
        /// </summary>
        public TOperation dteExec
        {
            get { return _dteExec; }
            set { _dteExec = value; }
        }
    }
}
