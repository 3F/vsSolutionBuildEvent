/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.API;
using net.r_eg.vsSBE.Bridge.Exceptions;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.UI.Xaml;

#if VSSDK_15_AND_NEW
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

        public static StatusToolCommand Instance
        {
            get;
            private set;
        }

        public IStatusTool ToolContent
        {
            get => (IStatusTool)toolPane.Content;
        }

        public IStatusToolEvents ToolEvents
        {
            get => (IStatusToolEvents)toolPane;
        }

        private IConfig<ISolutionEvents> Config
        {
            get => Settings.CfgManager.Config;
        }

#if VSSDK_15_AND_NEW

        /// <param name="pkg">Owner package.</param>
        /// <param name="evt">Supported public events, not null.</param>
        public static async Task<StatusToolCommand> InitAsync(IPkg pkg, IEventLevel evt)
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

            Instance.toolPane = await Instance.initToolPaneAsync();

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

#if VSSDK_15_AND_NEW

        /// <summary>
        /// NOTE: Be careful with FindToolWindowAsync and ShowToolWindowAsync.
        ///       It may produce deadlocks when it is called from InitializeAsync thread:
        ///       https://github.com/3F/vsSolutionBuildEvent/pull/45#pullrequestreview-246288512
        /// </summary>
        /// <returns></returns>
        private async Task<ToolWindowPane> initToolPaneAsync()
        {
            return initToolPane(
                await pkg.getToolWindowAsync(typeof(StatusToolWindow), 0)
            );
        }

#else

        private ToolWindowPane initToolPane()
        {
            return initToolPane(
                pkg.getToolWindow(typeof(StatusToolWindow), 0)
            );
        }

#endif

        private ToolWindowPane initToolPane(ToolWindowPane tool)
        {
            Log.Trace("FindToolWindow/Async completed");

            if(tool == null || tool.Frame == null) {
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
#if VSSDK_15_AND_NEW
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.CancellationToken);
#endif

                IVsWindowFrame windowFrame = (IVsWindowFrame)toolPane.Frame;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

#if VSSDK_15_AND_NEW
            });
#endif
        }

#region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }
            disposed = true;

            toolPane?.Dispose();
        }

#endregion
    }
}
