/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace ClientDemo
{
    public class BasicImplementation: IEntryPointClient, ILog
    {
        private readonly IStatus status = new StatusFrm();

        private readonly object _lock = new();

        public ClientType Type { get; } = ClientType.Isolated;

        public IEntryPointCore Core { get; set; }

        public IVersion Version { get; set; }

        public IEvent2 Event { get; protected set; }

        public IBuild Build { get; protected set; }

        public void Info(string message, params object[] args)
        {
            Write(args?.Length > 0 ? string.Format(message, args) : message);
        }

        public void Show() => status.Show();

        public void load(object dte2)
        {
            Info("Entering load(object dte2)");
            Init();
        }
        
        public void load(string sln, Dictionary<string, string> properties)
        {
            Info($"Entering load(sln: {sln}, properties)");
            Init();
        }
        
        public void load()
        {
            Info("Entering load()");
            Init();
        }

        protected void Init()
        {
            Info
            (
                "Version of core library: v{0} [{1}] API: v{2} /'{3}':{4}",
                Version.Number.ToString(),
                Version.BranchSha1,
                Version.Bridge.Number.ToString(2),
                Version.BranchName,
                Version.BranchRevCount
            );

            Event = new Event(this);
            Build = new Build(this);

            AttachCoreCommandListener();
            Show();
        }

        protected void AttachCoreCommandListener()
        {
            lock(_lock)
            {
                DetachCoreCommandListener();
                Core.CoreCommand += OnCoreCommand;
            }
        }

        protected void DetachCoreCommandListener()
        {
            lock(_lock)
            {
                Core.CoreCommand -= OnCoreCommand;
            }
        }

        private void OnCoreCommand(object sender, CoreCommandArgs e) => Info($"CoreCommand: {e.Type}");

        private void Write(string msg)
        {
            string tFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + " .fff";
            status.Report($"[{DateTime.Now.ToString(tFormat)}] {msg}{Environment.NewLine}");
        }
    }
}
