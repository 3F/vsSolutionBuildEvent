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
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge.Exceptions;
using net.r_eg.vsSBE.UI.Xaml;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// View / Other Windows / { Status Panel }
    /// </summary>
    internal sealed class StatusToolCommand
    {
        private readonly AsyncPackage package;

        private readonly MenuCommand mcmd;

        private ToolWindowPane window;

        public static StatusToolCommand Instance
        {
            get;
            private set;
        }

        /// <param name="package">Owner package.</param>
        public static async Task<StatusToolCommand> initAsync(AsyncPackage package)
        {
            if(Instance != null) {
                Log.Debug($"Dual initialization of the command: { nameof(StatusToolCommand) }");
                return Instance;
            }

            // Switch to the main thread - the call to AddCommand in StatusToolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            Instance = new StatusToolCommand
            (
                package, 
                await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService
            );

            return Instance;
        }

        public async Task<ToolWindowPane> findToolPaneAsync()
        {
            window = await package.FindToolWindowAsync(
                typeof(StatusToolWindow), 
                1,
                true, // find or create 
                package.DisposalToken
            );

            if(window == null || window.Frame == null) {
                throw new NotFoundException($"Cannot create { GetType().FullName }");
            }
            return window;
        }

        public IStatusTool getTool() => (IStatusTool)window.Content;

        public IStatusToolEvents getToolEvents() => (IStatusToolEvents)window;

        public void setVisibility(bool val)
        {
            mcmd.Visible = val;
        }

        /// <param name="package">Owner package, not null.</param>
        /// <param name="svc">Command service to add command to, not null.</param>
        private StatusToolCommand(AsyncPackage package, OleMenuCommandService svc)
        {
            this.package    = package ?? throw new ArgumentNullException(nameof(package));
            svc             = svc ?? throw new ArgumentNullException(nameof(svc));

            mcmd = new MenuCommand
            (
                action,
                new CommandID(GuidList.PANEL_CMD_SET, (int)PkgCmdIDList.CMD_PANEL)
            );

            svc.AddCommand(mcmd);
        }

        private async void action(object sender, EventArgs e)
        {
            if(window == null) {
                throw new ArgumentNullException(nameof(window));
            }

            await package.JoinableTaskFactory.SwitchToMainThreadAsync();

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
