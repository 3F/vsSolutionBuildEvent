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
using net.r_eg.vsSBE.API;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Build / { Main App }
    /// </summary>
    internal sealed class MainToolCommand: IDisposable
    {
        private readonly AsyncPackage package;

        private readonly IEventLevel apievt;

        private UI.WForms.EventsFrm configFrm;

        private readonly MenuCommand mcmd;

        public static MainToolCommand Instance
        {
            get;
            private set;
        }

        public void setVisibility(bool val)
        {
            mcmd.Visible = val;
        }

        public void closeConfigForm()
        {
            UI.Util.closeTool(configFrm);
        }

        /// <param name="package">Owner package.</param>
        public static async Task<MainToolCommand> initAsync(AsyncPackage package, IEventLevel evt)
        {
            if(Instance != null) {
                Log.Debug($"Dual initialization of the command: { nameof(MainToolCommand) }");
                return Instance;
            }

            // Switch to the main thread - the call to AddCommand in MainToolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            Instance = new MainToolCommand
            (
                package, 
                await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService,
                evt
            );

            return Instance;
        }

        /// <param name="package">Owner package, not null.</param>
        /// <param name="svc">Command service to add command to, not null.</param>
        /// <param name="evt">Supported public events, not null.</param>
        private MainToolCommand(AsyncPackage package, OleMenuCommandService svc, IEventLevel evt)
        {
            this.package    = package ?? throw new ArgumentNullException(nameof(package));
            svc             = svc ?? throw new ArgumentNullException(nameof(svc));
            apievt          = evt ?? throw new ArgumentNullException(nameof(evt));

            mcmd = new MenuCommand
            (
                action,
                new CommandID(GuidList.MAIN_CMD_SET, (int)PkgCmdIDList.CMD_MAIN)
            );

            svc.AddCommand(mcmd);
        }

        private void action(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15

            try
            {
                if(UI.Util.focusForm(configFrm)) {
                    return;
                }
                configFrm = new UI.WForms.EventsFrm(apievt.Bootloader);
                configFrm.Show();
            }
            catch(Exception ex) {
                Log.Error("Failed UI: `{0}`", ex.Message);
            }
        }

        private void free()
        {
            if(configFrm != null && !configFrm.IsDisposed) {
                configFrm.Close();
            }
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }
            disposed = true;

            free();
        }

        #endregion
    }
}
