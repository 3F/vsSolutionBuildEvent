
namespace net.r_eg.vsSBE.Upgrade.v08.Events
{
    public interface ISolutionEvent
    {
        /// <summary>
        /// execution of shell command
        /// </summary>
        string command { get; set; }

        /// <summary>
        /// output information to "Output" window or something else...
        /// </summary>
        string caption { get; set; }

        /// <summary>
        /// status of activate
        /// </summary>
        bool enabled { get; set; }

        /// <summary>
        /// Hide Process
        /// </summary>
        bool processHide { get; set; }

        /// <summary>
        /// not close after completion
        /// </summary>
        bool processKeep { get; set; }

        /// <summary>
        /// processing mode
        /// </summary>
        TModeCommands mode { get; set; }

        /// <summary>
        /// stream processor
        /// </summary>
        string interpreter { get; set; }

        /// <summary>
        /// treat newline as
        /// </summary>
        string newline { get; set; }

        /// <summary>
        /// symbol wrapper for commands or script
        /// </summary>
        string wrapper { get; set; }

        /// <summary>
        /// Wait until terminates script handling
        /// </summary>
        bool waitForExit { get; set; }

        /// <summary>
        /// support of MSBuild environment variables (properties)
        /// </summary>
        bool parseVariablesMSBuild { get; set; }

        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        bool buildFailedIgnore { get; set; }

        /// <summary>
        /// Run only for a specific configuration of solution
        /// strings format as:
        ///   'configname'|'platformname'
        ///   Compatible with: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        string[] toConfiguration { get; set; }

        /// <summary>
        /// Run for selected projects with execution order
        /// </summary>
        TExecutionOrder[] executionOrder { get; set; }

        /// <summary>
        /// Common Environment Visual Studio. Executes the specified commands
        /// TODO: custom list
        /// </summary>
        TOperation dteExec { get; set; }
    }

    /// <summary>
    /// Processing mode
    /// </summary>
    public enum TModeCommands
    {
        /// <summary>
        /// external commands
        /// </summary>
        File,
        /// <summary>
        /// command script
        /// </summary>
        Interpreter,
        /// <summary>
        /// DTE commands
        /// </summary>
        Operation,
    }

    /// <summary>
    /// Atomic DTE operation
    /// </summary>
    public class TOperation
    {
        /// <summary>
        /// exec-command
        /// </summary>
        public string[] cmd = new string[] { "" };
        /// <summary>
        /// optional ident
        /// </summary>
        public string caption = "";
        /// <summary>
        /// Abort operations on first error
        /// </summary>
        public bool abortOnFirstError = false;
    }

    public struct TExecutionOrder
    {
        public string project;
        public Type order;

        public enum Type
        {
            Before,
            After
        }
    }
}