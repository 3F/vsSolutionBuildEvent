/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.Xaml;

namespace net.r_eg.vsSBE
{
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // To register the informations needed to in the Help/About dialog of Visual Studio
    [InstalledProductRegistration("#110", "#112", Version.numberWithRevString, IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    //  To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]

    // Registers the tool window
    [ProvideToolWindow(typeof(UI.Xaml.StatusToolWindow), Height=25, Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Top, Window=ToolWindowGuids80.Outputwindow)]

    // Package Guid
    [Guid(GuidList.PACKAGE_STRING)]
    public sealed class vsSolutionBuildEventPackage: Package, IDisposable, IVsSolutionEvents, IVsUpdateSolutionEvents2
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
        /// Listener of the OutputWindowsPane
        /// </summary>
        private Receiver.Output.OWP _owpListener;

        /// <summary>
        /// The command for menu - Build / Events Solution
        /// </summary>
        private MenuCommand _menuItemMain;
        
        /// <summary>
        /// Main form of settings
        /// </summary>
        private UI.WForms.EventsFrm configFrm;

        /// <summary>
        /// DTE2 Context
        /// </summary>
        public DTE2 Dte2
        {
            get {
                return (DTE2)Package.GetGlobalService(typeof(SDTE)); 
            }
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
        /// Gets instance of StatusToolWindow.
        /// Ensures that the Frame of value is also != null
        /// </summary>
        private ToolWindowPane StatusTool
        {
            get
            {
                if(statusTool != null) {
                    return statusTool;
                }

                ToolWindowPane window = FindToolWindow(typeof(UI.Xaml.StatusToolWindow), 0, true); // find or create
                if(window == null || window.Frame == null) {
                    return null;
                }

                statusTool = window;
                return statusTool;
            }
        }
        private ToolWindowPane statusTool;

        /// <summary>
        /// Priority call with SVsSolution.
        /// Part of IVsSolutionEvents - that the solution has been opened.
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivssolutionevents.onafteropensolution.aspx
        /// </summary>
        /// <param name="pUnkReserved"></param>
        /// <param name="fNewSolution"></param>
        /// <returns></returns>
        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try
            {
                //Log.paneAttach(GetOutputPane(GuidList.OWP_SBE, Settings.OWP_ITEM_VSSBE)); // also may be problem with toolWindow as in other COM variant -_-
                Log._.paneAttach(Settings.OWP_ITEM_VSSBE, Dte2);
                Log._.clear(false);
                Log._.show();
                eventsOfStatusTool(true);

                int ret = Event.solutionOpened(pUnkReserved, fNewSolution);
                _menuItemMain.Visible = (ret == VSConstants.S_OK);
                return ret;
            }
            catch(Exception ex) {
                Log.Fatal("Problem with loading solution: " + ex.Message);
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
            _menuItemMain.Visible = false;
            try
            {
                Event.solutionClosed(pUnkReserved);
                UI.Util.closeTool(configFrm);

                eventsOfStatusTool(false);
                //Log._.paneDetach((IVsOutputWindow)GetGlobalService(typeof(SVsOutputWindow)));
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.Fatal("Problem with closing solution: " + ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try {
                UI.Plain.State.lineBegin();
                ((IStatusTool)StatusTool.Content).resetCounter();
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
                UI.Plain.State.summaryWarn(StatusTool);
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
        /// CA1001: well, the VisualStudio.Shell.Package is already uses `void Dispose(bool disposing)`
        ///         And this will never be used at all... but in addition and for CA we also implemented IDisposable
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void initAppEvents()
        {
            var usrCfg = new UserConfig();
            usrCfg.load(usrCfg.getLink(Settings._.CommonPath, Config.Entity.NAME, null));

            Event = new API.EventLevel();
            ((IEntryPointCore)Event).load(Dte2, usrCfg.Data.Global.DebugMode);

            // Receiver

            _owpListener = new Receiver.Output.OWP(Event.Environment);
            _owpListener.attachEvents();
            _owpListener.Receiving += (object sender, Receiver.Output.PaneArgs e) =>
            {
                if(e.Guid.CompareGuids(GuidList.OWP_BUILD_STRING)) {
                    Event.onBuildRaw(e.Raw);
                }
            };
        }

        /// <summary>
        /// Control of events for status tool - Solution Build-Events
        /// </summary>
        /// <param name="attach"></param>
        private void eventsOfStatusTool(bool attach)
        {
            if(StatusTool == null) {
                Log.Debug("Cannot find or create UI.StatusToolWindow");
                return;
            }
            IStatusToolEvents ste = (IStatusToolEvents)StatusTool;

            if(attach) {
                ste.attachEvents(Event).attachEvents(Settings.CfgManager.Config).attachEvents();
                return;
            }
            ste.detachEvents().detachEvents(Settings.CfgManager.Config).detachEvents(Event);
        }

        /// <summary>
        /// Handler of showing the main window if clicked # Build / Events Solution
        /// </summary>
        private void _menuMainCallback(object sender, EventArgs e)
        {
            try
            {
                if(UI.Util.focusForm(configFrm)) {
                    return;
                }
                configFrm = new UI.WForms.EventsFrm(Event.Bootloader);
                configFrm.Show();
            }
            catch(Exception ex) {
                Log.Error("Failed UI: `{0}`", ex.Message);
            }
        }

        /// <summary>
        /// Handler of showing the status-tool window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _menuPanelCallback(object sender, EventArgs e)
        {
            if(StatusTool == null) {
                throw new ComponentException("Cannot create UI.StatusToolWindow");
            }
            ErrorHandler.ThrowOnFailure(((IVsWindowFrame)StatusTool.Frame).Show());
        }

        #region unused

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

        #region maintenance
        protected override void Initialize()
        {
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            try
            {
                initAppEvents();

                OleMenuCommandService mcs = (OleMenuCommandService)GetService(typeof(IMenuCommandService));

                // Build / <Main App>
                _menuItemMain = new MenuCommand(_menuMainCallback, new CommandID(GuidList.MAIN_CMD_SET, (int)PkgCmdIDList.CMD_MAIN));
                _menuItemMain.Visible = false;
                mcs.AddCommand(_menuItemMain);

                // View / Other Windows / <Status Panel>
                mcs.AddCommand(new MenuCommand(_menuPanelCallback, new CommandID(GuidList.PANEL_CMD_SET, (int)PkgCmdIDList.CMD_PANEL)));

                // To listen events that fired as a IVsSolutionEvents
                spSolution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
                spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

                // To listen events that fired as a IVsUpdateSolutionEvents2
                spSolutionBM = (IVsSolutionBuildManager2)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager));
                spSolutionBM.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
            }
            catch(Exception ex)
            {
                string msg = string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-",
                                "Try to restart IDE or reinstall current plugin in Extension Manager.", 
                                ex.ToString());

                Debug.WriteLine(msg);
                
                int res;
                Guid id = Guid.Empty;
                IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));

                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                    uiShell.ShowMessageBox(
                           0,
                           ref id,
                           "Initialize vsSolutionBuildEvent",
                           msg,
                           string.Empty,
                           0,
                           OLEMSGBUTTON.OLEMSGBUTTON_OK,
                           OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                           OLEMSGICON.OLEMSGICON_WARNING,
                           0,
                           out res));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "configFrm")]
        protected override void Dispose(bool disposing)
        {
            UI.Util.closeTool(configFrm); //<- CA2213: the Util will manage this

            if(spSolutionBM != null && _pdwCookieSolutionBM != 0) {
                spSolutionBM.UnadviseUpdateSolutionEvents(_pdwCookieSolutionBM);
            }

            if(spSolution != null && _pdwCookieSolution != 0) {
                spSolution.UnadviseSolutionEvents(_pdwCookieSolution);
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
