/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.API;
using net.r_eg.vsSBE.Bridge.Exceptions;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.UI.Xaml;

#if SDK15_OR_HIGH
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
#endif

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// View / Other Windows / { Status Panel }
    /// </summary>
    internal sealed class StatusToolCommand: IDisposable
    {
        private readonly MenuCommand mcmd;

        private readonly IEventLevel apievt;

        private readonly IPkg pkg;

        private ToolWindowPane toolPane;

        public static Type ToolType => typeof(StatusToolWindow);

        public static StatusToolCommand Instance
        {
            get;
            private set;
        }

        public IStatusTool ToolContent => (IStatusTool)toolPane.Content;

        public IStatusToolEvents ToolEvents => (IStatusToolEvents)toolPane;

        private IConfig<ISolutionEvents> Config => Settings._.Config.Sln;

#if SDK15_OR_HIGH

        /// <param name="pkg">Owner package.</param>
        /// <param name="evt">Supported public events, not null.</param>
        /// <param name="tool">Tool pane instance to use this instead of new when null is passed.</param>
        public static async Task<StatusToolCommand> InitAsync(IPkg pkg, IEventLevel evt, ToolWindowPane tool = null)
        {
            if(Instance != null) {
                return Instance;
            }

            // Switch to the main thread - the call to AddCommand in StatusToolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.CancellationToken);

            Instance = new StatusToolCommand
            (
                pkg, 
                await pkg.getSvcAsync(typeof(IMenuCommandService)) as OleMenuCommandService,
                evt
            );

            Instance.toolPane = await Instance.initToolPaneAsync(tool as StatusToolWindow);

            return Instance;
        }

#else

        /// <param name="pkg">Owner package.</param>
        /// <param name="evt">Supported public events, not null.</param>
        public static StatusToolCommand Init(IPkg pkg, IEventLevel evt)
        {
            if(Instance != null) {
                return Instance;
            }

            Instance = new StatusToolCommand
            (
                pkg, 
                pkg.getSvc(typeof(IMenuCommandService)) as OleMenuCommandService,
                evt
            );

            Instance.toolPane = Instance.initToolPane();

            return Instance;
        }

#endif

        public void attachEvents() 
            => ToolEvents.attachEvents(apievt)
                            .attachEvents(Config)
                            .attachEvents();

        public void detachEvents()
            => ToolEvents.detachEvents()
                            .detachEvents(Config)
                            .detachEvents(apievt);

#if SDK15_OR_HIGH

        /// <summary>
        /// NOTE: Be careful with FindToolWindowAsync and ShowToolWindowAsync.
        ///       It may produce deadlocks when it is called from InitializeAsync thread:
        ///       https://github.com/3F/vsSolutionBuildEvent/pull/45#pullrequestreview-246288512
        /// </summary>
        private async Task<ToolWindowPane> initToolPaneAsync(StatusToolWindow tool = null)
        {
            return initToolPane(
                tool ?? await pkg.getToolWindowAsync(ToolType)
            );
        }

#else

        private ToolWindowPane initToolPane(StatusToolWindow tool = null)
        {
            return initToolPane(
                tool ?? pkg.getToolWindow(ToolType)
            );
        }

#endif

        private ToolWindowPane initToolPane(ToolWindowPane tool)
        {
            Log.Trace("FindToolWindow/Async completed");

            if(tool?.Frame == null) {
                throw new NotFoundException($"Cannot find or create { nameof(StatusToolWindow) }");
            }
            return tool;
        }

        /// <param name="pkg">Owner package, not null.</param>
        /// <param name="svc">Command service to add command to, not null.</param>
        /// <param name="evt">Supported public events, not null.</param>
        private StatusToolCommand(IPkg pkg, OleMenuCommandService svc, IEventLevel evt)
        {
            this.pkg    = pkg ?? throw new ArgumentNullException(nameof(pkg));
            svc         = svc ?? throw new ArgumentNullException(nameof(svc));
            apievt      = evt ?? throw new ArgumentNullException(nameof(evt));

            mcmd = new MenuCommand
            (
                onAction,
                new CommandID(GuidList.PANEL_CMD_SET, (int)PkgCmdIDList.CMD_PANEL)
            );

            svc.AddCommand(mcmd);
        }

        private void onAction(object sender, EventArgs e)
        {
#if SDK15_OR_HIGH
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.CancellationToken);
#endif

                IVsWindowFrame windowFrame = (IVsWindowFrame)toolPane.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

#if SDK15_OR_HIGH
            });
#endif
        }

#region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool _)
        {
            if(!disposed)
            {
                toolPane?.Dispose();
                disposed = true;
            }
        }

        #endregion
    }
}
