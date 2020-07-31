/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.vsSBE.API;

#if VSSDK_15_AND_NEW
using System.Threading.Tasks;
#endif

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Build / { Main App }
    /// </summary>
    internal sealed class MainToolCommand: IDisposable
    {
        private readonly MenuCommand mcmd;

        private readonly IEventLevel apievt;

        private readonly IPkg pkg;

        private UI.WForms.EventsFrm configFrm;

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

#if VSSDK_15_AND_NEW

        public static async Task<MainToolCommand> InitAsync(IPkg pkg, IEventLevel evt)
        {
            if(Instance != null) {
                return Instance;
            }

            // Switch to the main thread - the call to AddCommand in MainToolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.CancellationToken);

            Instance = new MainToolCommand
            (
                pkg,
                await pkg.getSvcAsync(typeof(IMenuCommandService)) as OleMenuCommandService,
                evt
            );

            return Instance;
        }

#else

        public static MainToolCommand Init(IPkg pkg, IEventLevel evt)
        {
            if(Instance != null) {
                return Instance;
            }

            Instance = new MainToolCommand
            (
                pkg,
                pkg.getSvc(typeof(IMenuCommandService)) as OleMenuCommandService, 
                evt
            );

            return Instance;
        }

#endif

        /// <param name="pkg"></param>
        /// <param name="svc">Command service to add command to, not null.</param>
        /// <param name="evt">Supported public events, not null.</param>
        private MainToolCommand(IPkg pkg, OleMenuCommandService svc, IEventLevel evt)
        {
            this.pkg    = pkg ?? throw new ArgumentNullException(nameof(pkg));
            svc         = svc ?? throw new ArgumentNullException(nameof(svc));
            apievt      = evt ?? throw new ArgumentNullException(nameof(evt));

            mcmd = new MenuCommand
            (
                onAction,
                new CommandID(GuidList.MAIN_CMD_SET, (int)PkgCmdIDList.CMD_MAIN)
            );

            svc.AddCommand(mcmd);
        }

        private void onAction(object sender, EventArgs e)
        {
            try
            {
                if(UI.Util.focusForm(configFrm)) {
                    return;
                }
                configFrm = new UI.WForms.EventsFrm(Bootloader._);
                configFrm.Show();
            }
            catch(Exception ex) {
                Log.Error($"Failed UI: {ex.Message}");
                Log.Debug(ex.StackTrace);
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
