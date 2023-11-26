/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using net.r_eg.vsSBE.API;

#if SDK15_OR_HIGH
using System.Threading.Tasks;
#endif

namespace net.r_eg.vsSBE
{
    internal sealed class MainToolCommand: IDisposable
    {
        private readonly MenuCommand mcmd;

        private UI.WForms.EventsFrm configFrm;

        private static volatile MainToolCommand _instance;

        private static readonly object sync = new();

        public static MainToolCommand Instance => _instance;

        public void setVisibility(bool value) => mcmd.Visible = value;

        public void closeConfigForm() => UI.Util.closeTool(configFrm);

#if SDK15_OR_HIGH

        public static async Task<MainToolCommand> InitAsync(IPkg pkg, IEventLevel evt)
        {
            if(Instance != null) return Instance;

            // Switch to the main thread - the call to AddCommand in MainToolCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(pkg.CancellationToken);

            OleMenuCommandService svc = await pkg.getSvcAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            lock(sync)
            {
                if(Instance == null) _instance = new MainToolCommand(pkg, svc, evt);
                return Instance;
            }
        }

#else

        public static MainToolCommand Init(IPkg pkg, IEventLevel evt)
        {
            if(Instance != null) return Instance;

            OleMenuCommandService svc = pkg.getSvc(typeof(IMenuCommandService)) as OleMenuCommandService;
            lock(sync)
            {
                if(Instance == null) _instance = new MainToolCommand(pkg, svc, evt);
                return Instance;
            }
        }

#endif

        /// <param name="pkg"></param>
        /// <param name="svc">Command service to add command to, not null.</param>
        /// <param name="evt">Supported public events, not null.</param>
        private MainToolCommand(IPkg pkg, OleMenuCommandService svc, IEventLevel evt)
        {
            if(pkg == null) throw new ArgumentNullException(nameof(pkg));
            if(svc == null) throw new ArgumentNullException(nameof(svc));
            if(evt == null) throw new ArgumentNullException(nameof(evt));

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
                if(UI.Util.focusForm(configFrm)) return;

                configFrm = new UI.WForms.EventsFrm(Bootloader._);
                configFrm.Show();
            }
            catch(Exception ex)
            {
                Log.Error($"Failed UI: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        #region IDisposable

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool _)
        {
            if(!disposed)
            {
                if(configFrm != null && !configFrm.IsDisposed)
                {
                    configFrm.Close();
                }

                disposed = true;
            }
        }

        #endregion
    }
}
