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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.Xaml;
using Task = System.Threading.Tasks.Task;

namespace net.r_eg.vsSBE
{
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]

    // Information for Visual Studio Help/About dialog.
    [InstalledProductRegistration("#110", "#112", Version.numberWithRevString, IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]

    // Registers the tool window
    [ProvideToolWindow(typeof(StatusToolWindow), Height=25, Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Top, Window=ToolWindowGuids80.Outputwindow)]

    // Package Guid
    [Guid(GuidList.PACKAGE_STRING)]
    public sealed class vsSolutionBuildEventPackage: AsyncPackage, IDisposable, IVsSolutionEvents, IVsUpdateSolutionEvents2
    {
        /// <summary>
        /// For IVsSolutionEvents events
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolution.aspx
        /// </summary>
        private IVsSolution spSolution;

        /// <summary>
        /// Contains the cookie for advising IVsSolution
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolution.advisesolutionevents.aspx
        /// </summary>
        private uint _pdwCookieSolution;

        /// <summary>
        /// For IVsUpdateSolutionEvents2 events
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolutionbuildmanager2.aspx
        /// </summary>
        private IVsSolutionBuildManager2 spSolutionBM;

        /// <summary>
        /// Contains the cookie for advising IVsSolutionBuildManager2 / IVsSolutionBuildManager
        /// http://msdn.microsoft.com/en-us/library/bb141335.aspx
        /// </summary>
        private uint _pdwCookieSolutionBM;

        /// <summary>
        /// For work with ErrorList pane of Visual Studio.
        /// </summary>
        private VSTools.ErrorList.IPane errorList;

        /// <summary>
        /// The command for: Build / { main app tool }
        /// </summary>
        private MainToolCommand mainToolCmd;

        /// <summary>
        /// The command for: View / Other Windows / { Status Panel }
        /// </summary>
        private StatusToolCommand sToolCmd;

        /// <summary>
        /// Listener of the OutputWindowsPane
        /// </summary>
        private Receiver.Output.OWP owpListener;

        private object sync = new object();

        /// <summary>
        /// Reserved for future use with IVsSolutionEvents
        /// </summary>
        private readonly object pUnkReserved = new object();

        /// <summary>
        /// DTE2 Context
        /// </summary>
        public DTE2 Dte2
        {
            get => (DTE2)GetGlobalService(typeof(SDTE));
        }

        /// <summary>
        /// Support the all public events
        /// </summary>
        public API.IEventLevel Event
        {
            get;
            private set;
        }

        /// <summary>
        /// VSSDK003: Visual Studio 2017 Update 6 or later
        /// </summary>
        /// <param name="toolWindowType"></param>
        /// <returns></returns>
        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(Guid toolWindowType)
        {
            if(toolWindowType == typeof(StatusToolWindow).GUID) {
                return this;
            }

            ThreadHelper.ThrowIfNotOnUIThread();
            return base.GetAsyncToolWindowFactory(toolWindowType);
        }

        /// <summary>
        /// Priority call with SVsSolution.
        /// Part of IVsSolutionEvents - that the solution has been opened (Before initializing projects).
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolutionevents.onafteropensolution.aspx
        /// </summary>
        /// <param name="pUnkReserved"></param>
        /// <param name="fNewSolution"></param>
        /// <returns></returns>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            Monitor.Enter(sync);
            try
            {
                //Log.paneAttach(GetOutputPane(GuidList.OWP_SBE, Settings.OWP_ITEM_VSSBE)); // also may be problem with toolWindow as in other COM variant -_-
                Log._.paneAttach(Settings.OWP_ITEM_VSSBE, Dte2);
                Log._.clear(false);
                Log._.show();

                sToolCmd?.attachEvents();

                Event.OpenedSolution -= onLateOpenedSolution;
                Event.OpenedSolution += onLateOpenedSolution;

                return Event.solutionOpened(pUnkReserved, fNewSolution);
            }
            catch(Exception ex) {
                Log.Fatal("Problem when loading solution: " + ex.Message);
            }
            finally {
                Monitor.Exit(sync);
            }

            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Priority call with SVsSolution.
        /// Part of IVsSolutionEvents - that a solution has been closed.
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolutionevents.onafterclosesolution.aspx
        /// </summary>
        /// <param name="pUnkReserved"></param>
        /// <returns></returns>
        public int OnAfterCloseSolution(object pUnkReserved)
        {
            Monitor.Enter(sync);

            mainToolCmd?.setVisibility(false);
            try
            {
                Event.solutionClosed(pUnkReserved);
                mainToolCmd.closeConfigForm();

                sToolCmd?.detachEvents();
                //Log._.paneDetach((IVsOutputWindow)GetGlobalService(typeof(SVsOutputWindow)));
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.Fatal("Problem when closing solution: " + ex.Message);
            }
            finally {
                Monitor.Exit(sync);
            }

            return VSConstants.S_FALSE;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try {
                UI.Plain.State.BuildBegin();
                sToolCmd?.ToolContent.resetCounter();
                errorList.clear();
            }
            catch(Exception ex) {
                Log.Debug("Failed reset of warnings counter: '{0}'", ex.Message);
            }

            return Event.onPre(ref pfCancelUpdate);
        }

        public int UpdateSolution_Cancel()
        {
            return Event.onCancel();
        }

        /// <summary>
        /// 
        /// When it works:
        ///    * Begin -> Done
        ///    * Begin -> Cancel -> Done
        /// </summary>
        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            try {
                return Event.onPost(fSucceeded, fModified, fCancelCommand);
            }
            finally {
                //TODO: Summary of Errors / Warnings ~ ((IStatusTool)StatusTool.Content).Warnings
                UI.Plain.State.BuildEnd();
            }
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return Event.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return Event.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Trace.WriteLine($"Entering InitializeAsync() of: { ToString() }");

            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                errorList = new VSTools.ErrorList.Pane(this, cancellationToken);

                Log._.Received  -= onLogReceived;
                Log._.Received  += onLogReceived;

                initAppEvents();

                mainToolCmd = await MainToolCommand.initAsync(this, Event);
                mainToolCmd.setVisibility(false);

                // VS bug: https://github.com/microsoft/extendvs/issues/68
                _ = Task.Run(async () =>
                {
                    await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

                    sToolCmd = await StatusToolCommand.initAsync(this, Event);

                    // https://github.com/3F/vsSolutionBuildEvent/pull/45#discussion_r291835939
                    if(Dte2.Solution.IsOpen) OnAfterOpenSolution(pUnkReserved, 0);
                });

                spSolution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
                spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

                spSolutionBM = await GetServiceAsync(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
                spSolutionBM.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
            }
            catch(Exception ex)
            {
                string msg = string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-",
                                "Try to restart IDE or reinstall current plugin in Extension Manager.", 
                                ex.ToString());

                Debug.WriteLine(msg);

                Guid id = Guid.Empty;
                IVsUIShell uiShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;

                ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox
                (
                    0,
                    ref id,
                    $"Initialize { ToString() }",
                    msg,
                    String.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    0,
                    out int res
                ));
            }
        }

        protected override string GetToolWindowTitle(Type toolWindowType, int id)
        {
            if(toolWindowType == typeof(StatusToolWindow)) {
                return $"{nameof(StatusToolWindow)} loading";
            }

            return base.GetToolWindowTitle(toolWindowType, id);
        }

        protected override async Task<object> InitializeToolWindowAsync(Type toolWindowType, int id, CancellationToken ct)
        {
            return await Task.FromResult(String.Empty); // this is passed to the tool window constructor
        }

        private void initAppEvents()
        {
            var usrCfg = new UserConfig();
            usrCfg.load(usrCfg.getLink(Settings._.CommonPath, Config.Entity.NAME, null));

            Event = new API.EventLevel();
            ((IEntryPointCore)Event).load(Dte2, usrCfg.Data.Global.DebugMode);

            // Receiver

            owpListener = new Receiver.Output.OWP(Event.Environment);
            owpListener.attachEvents();
            owpListener.Receiving += (object sender, Receiver.Output.PaneArgs e) =>
            {
                if(e.Guid.CompareGuids(GuidList.OWP_BUILD_STRING)) {
                    Event.onBuildRaw(e.Raw);
                }
            };
        }

        /// <summary>
        /// When all projects are opened in IDE. 
        /// </summary>
        private void onLateOpenedSolution(object sender, EventArgs e)
        {
            mainToolCmd?.setVisibility(true);
        }

        private void onLogReceived(object sender, Logger.MessageArgs e)
        {
            if(Log._.isError(e.Level)) {
                errorList.error(e.Message);
            }
            else if(Log._.isWarn(e.Level)) {
                errorList.warn(e.Message);
            }
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if(disposed) {
                return;
            }
            disposed = true;

            mainToolCmd?.Dispose();
            sToolCmd?.Dispose();

            if(errorList != null) {
                ((IDisposable)errorList).Dispose();
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if(spSolutionBM != null && _pdwCookieSolutionBM != 0) {
                    spSolutionBM.UnadviseUpdateSolutionEvents(_pdwCookieSolutionBM);
                }

                if(spSolution != null && _pdwCookieSolution != 0) {
                    spSolution.UnadviseSolutionEvents(_pdwCookieSolution);
                }
            });

            base.Dispose(disposing);
        }

        #endregion

        #region unused API

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeCloseSolution(object pUnkReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
