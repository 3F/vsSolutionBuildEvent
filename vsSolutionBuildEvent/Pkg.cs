/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.Xaml;

#if SDK15_OR_HIGH
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;
#endif

namespace net.r_eg.vsSBE
{
#if SDK15_OR_HIGH
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]

    // To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
#else
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
#endif

    // Information for Visual Studio Help/About dialog.
    [InstalledProductRegistration("#110", "#112", Version.S_NUM, IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // Registers the tool window
    [ProvideToolWindow(typeof(StatusToolWindow), Height=38, Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Top, Window=ToolWindowGuids80.ErrorList)]

    // Package Guid
    [Guid(GuidList.PACKAGE_STRING)]
    public sealed class Pkg:

#if SDK15_OR_HIGH
         AsyncPackage,
#else
         Package,
#endif

        IVsSolutionEvents, IVsUpdateSolutionEvents2, IPkg, IDisposable
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

        private EnvDTE.DocumentEvents DocumentEvents;

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

        private readonly object sync = new object();

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

        public CancellationToken CancellationToken
        {
            get
            {
#if SDK15_OR_HIGH
                return DisposalToken;
#else
                return CancellationToken.None;
#endif
            }
        }

#if SDK15_OR_HIGH

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

#endif

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

                if(Settings._.Config.Sys.Data?.SuppressInitOwp != true)
                {
                    Log._.show();
                }

                sToolCmd?.attachEvents();

                Event.OpenedSolution -= onLateOpenedSolution;
                Event.OpenedSolution += onLateOpenedSolution;

                if(Event.Environment.SolutionFile == null)
                {
                    void _onDocOpened(EnvDTE.Document Document)
                    {
                        DocumentEvents.DocumentOpened -= _onDocOpened;

                        Dte2.Globals[Environment.DTE_DOC_SLN] = Document;
                        Event.solutionOpened(pUnkReserved, fNewSolution);
                    };
                    DocumentEvents.DocumentOpened += _onDocOpened;

                    return VSConstants.S_OK;
                }

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

                if(Dte2.Globals.VariableExists[Environment.DTE_DOC_SLN])
                {
                    Dte2.Globals[Environment.DTE_DOC_SLN] = null;
                }

                sToolCmd?.detachEvents();
                resetErrors();
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
            try
            {
                UI.Plain.State.BuildBegin();
                resetErrors();
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

#if SDK15_OR_HIGH

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="create">try to create tool when true</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        public async Task<ToolWindowPane> getToolWindowAsync(Type type, bool create = true, int id = 0)
        {
            return await FindToolWindowAsync
            (
                type, id, create, DisposalToken
            );
        }

        /// <summary>
        /// AsyncPackage.GetServiceAsync
        /// </summary>
        /// <param name="type">service type.</param>
        /// <returns></returns>
        public async Task<object> getSvcAsync(Type type)
        {
            return await GetServiceAsync(type);
        }

#else

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="create">try to create tool when true</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        public ToolWindowPane getToolWindow(Type type, bool create = true, int id = 0)
        {
            return FindToolWindow(type, id, create);
        }

        /// <summary>
        /// Package.GetService
        /// </summary>
        /// <param name="type">service type.</param>
        /// <returns></returns>
        public object getSvc(Type type) => GetService(type);

#endif

#if SDK15_OR_HIGH

        /// <summary>
        /// Modern 15+ Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            try
            {
                initAppEvents(cancellationToken);

                mainToolCmd = await MainToolCommand.InitAsync(this, Event);
                mainToolCmd.setVisibility(false);

                spSolution = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
                spSolution?.AdviseSolutionEvents(this, out _pdwCookieSolution);

                spSolutionBM = await GetServiceAsync(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
                spSolutionBM?.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
            }
            catch(Exception ex)
            {
                IVsUIShell uiShell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
                _showCriticalVsMsg(uiShell, ex);
            }

            // Because of VS bug: https://github.com/microsoft/extendvs/issues/68
            //  - MSVS Shell.15.0 15.7.27703
            //  - MSVS Threading 15.8.209
            _ = Task.Run(async () =>
            {
                // Helps to avoid related bug in MSVS Shell.15.0 15.9.28307
                //      for the case when tool is already attached when starting VS IDE.
                // Do not use true value in non-UI thread ........................v
                var tool = await getToolWindowAsync(StatusToolCommand.ToolType, false);

                await JoinableTaskFactory.SwitchToMainThreadAsync();

                sToolCmd = await StatusToolCommand.InitAsync(this, Event, tool);

                // https://github.com/3F/vsSolutionBuildEvent/pull/45#discussion_r291835939
                if(Dte2.Solution.IsOpen) OnAfterOpenSolution(pUnkReserved, 0);
            });
        }

#else

        /// <summary>
        /// Old VS10" - VS15" Synchronous Initialization of the package;
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine($"Entering Initialize() of: { ToString() }");

            base.Initialize();
            try
            {
                initAppEvents(CancellationToken.None);

                mainToolCmd = MainToolCommand.Init(this, Event);
                mainToolCmd.setVisibility(false);

                sToolCmd = StatusToolCommand.Init(this, Event);

                // https://github.com/3F/vsSolutionBuildEvent/pull/45#discussion_r291835939
                if(Dte2.Solution.IsOpen) OnAfterOpenSolution(pUnkReserved, 0);

                spSolution = GetService(typeof(SVsSolution)) as IVsSolution;
                spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

                spSolutionBM = GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;
                spSolutionBM.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
            }
            catch(Exception ex)
            {
                IVsUIShell uiShell = GetService(typeof(SVsUIShell)) as IVsUIShell;
                _showCriticalVsMsg(uiShell, ex);
            }
        }

#endif

#if SDK15_OR_HIGH

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

#endif

        private void initAppEvents(CancellationToken cancellationToken)
        {
            errorList = new VSTools.ErrorList.Pane(this, cancellationToken);

            Log._.Received -= onLogReceived;
            Log._.Received += onLogReceived;

            Event = new API.EventLevel();
            ((IEntryPointCore)Event).load
            (
                Dte2,
                Settings._.Config.Sys.Data?.DebugMode ?? 
#if DEBUG
                true
#else
                false
#endif
            );

            // Receiver

            owpListener = new Receiver.Output.OWP(Event.Environment);
            owpListener.attachEvents();
            owpListener.Receiving += (object sender, Receiver.Output.PaneArgs e) =>
            {
                if(e.Guid.CompareGuids(GuidList.OWP_BUILD_STRING)) {
                    Event.onBuildRaw(e.Raw);
                }
            };

            DocumentEvents = Event.Environment.Events.DocumentEvents;
        }

        private void resetErrors()
        {
            sToolCmd?.ToolContent.resetCounter();
            errorList.clear();
        }

        private void _showCriticalVsMsg(IVsUIShell uiShell, Exception ex)
        {
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread();
#endif
            string msg = String.Format
            (
                "{0}\n{1}\n\n-----\n{2}", 
                "Something went wrong -_-",
                "Try to restart IDE / reinstall plugin / or please contact with us!", 
                ex.ToString()
            );

            Debug.WriteLine(msg);

            Guid id = Guid.Empty;
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
                errorList.error(e.Raw, e.Src, e.SrcType);
            }
            else if(Log._.isWarn(e.Level)) {
                errorList.warn(e.Raw, e.Src, e.SrcType);
            }
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

#if SDK15_OR_HIGH
            _ = ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(DisposalToken);
#endif

                if(spSolutionBM != null && _pdwCookieSolutionBM != 0) {
                    spSolutionBM.UnadviseUpdateSolutionEvents(_pdwCookieSolutionBM);
                }

                if(spSolution != null && _pdwCookieSolution != 0) {
                    spSolution.UnadviseSolutionEvents(_pdwCookieSolution);
                }

#if SDK15_OR_HIGH
            });
#endif

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
