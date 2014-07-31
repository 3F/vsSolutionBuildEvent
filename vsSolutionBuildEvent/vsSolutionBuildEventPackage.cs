/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;

namespace net.r_eg.vsSBE
{
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // To register the informations needed to in the Help/About dialog of Visual Studio
    [InstalledProductRegistration("#110", "#112", "0.6.1", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    //  To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]

    // Registers the tool window
    [ProvideToolWindow(typeof(UI.StatusToolWindow), Height=23, Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Top, Window=ToolWindowGuids80.Outputwindow)]

    // Package Guid
    [Guid(GuidList.PACKAGE_STRING)]

    public sealed class vsSolutionBuildEventPackage: Package, IVsSolutionEvents, IVsUpdateSolutionEvents, IListenerOWPL
    {
        /// <summary>
        /// top-level object in the Visual Studio
        /// </summary>
        public static DTE2 Dte2
        {
            get{ return (DTE2)Package.GetGlobalService(typeof(SDTE)); }
        }

        /// <summary>
        /// for register events -> _cookieSEvents
        /// </summary>
        private IVsSolution _solution                       = null;
        private uint _cookieSEvents;

        /// <summary>
        /// for register events -> _cookieUpdateSEvents
        /// </summary>
        private IVsSolutionBuildManager _solBuildManager    = null;
        private uint _cookieUpdateSEvents;

        /// <summary>
        /// VS IDE menu - Build / <Main App>
        /// </summary>
        private MenuCommand _menuItemMain                   = null;
        
        /// <summary>
        /// main form of settings
        /// </summary>
        private UI.EventsFrm _configFrm                     = null;

        /// <summary>
        /// General action
        /// </summary>
        private SBECommand _sbe                             = null;

        /// <summary>
        /// Working with the OutputWindowsPane -> "Build" pane
        /// </summary>
        private OutputWPListener _owpBuild;

        public vsSolutionBuildEventPackage()
        {
            Log.init(Dte2);
            Log.show();
            
            _sbe = new SBECommand(Dte2, new MSBuildParser(Dte2));

            _owpBuild = new OutputWPListener(Dte2, "Build");
            _owpBuild.attachEvents();
            _owpBuild.register(this);

            //TODO: don't like it
            UI.StatusToolWindow.control.setDTE(Dte2);
        }

        /// <summary>
        /// execute a command when clicked menu item (Build/<pack>)
        /// </summary>
        private void _menuMainCallback(object sender, EventArgs e)
        {
            if (_configFrm != null && !_configFrm.IsDisposed)
            {
                _configFrm.Focus();
                return;
            }
            _configFrm = new UI.EventsFrm(Dte2);
            _configFrm.Show();
        }

        private void _menuPanelCallback(object sender, EventArgs e)
        {
            ToolWindowPane window = FindToolWindow(typeof(UI.StatusToolWindow), 0, true);
            if(window == null || window.Frame == null) {
                throw new NotSupportedException("Cannot create UI.StatusToolWindow");
            }
            ErrorHandler.ThrowOnFailure(((IVsWindowFrame)window.Frame).Show());
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try
            {
                string path = Dte2.Solution.FullName; // may be empty e.g. if fNewSolution == 1 etc.
                if(string.IsNullOrEmpty(path)) {
                    path = Dte2.Solution.Properties.Item("Path").Value.ToString();
                }

                Config.load(Path.GetDirectoryName(path) + "\\");
                _state();
            }
            catch(Exception ex) {
                Log.nlog.Fatal("Cannot load configuration: " + ex.Message);
                return VSConstants.S_FALSE;
            }
            _menuItemMain.Visible = true;
            UI.StatusToolWindow.control.enabled(true);
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            _menuItemMain.Visible = false;
            UI.StatusToolWindow.control.enabled(false);

            if(_configFrm != null && !_configFrm.IsDisposed) {
                _configFrm.Close();
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try
            {
                if(_sbe.basic(Config.Data.preBuild, SBEQueueDTE.Type.PRE)) {
                    Log.nlog.Info("[Pre] finished SBE: " + Config.Data.preBuild.caption);
                }
            }
            catch (Exception e)
            {
                Log.nlog.Error("Pre-Build error: " + e.Message);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Cancel()
        {
            try
            {
                if(_sbe.basic(Config.Data.cancelBuild, SBEQueueDTE.Type.CANCEL)) {
                    Log.nlog.Info("[Cancel] finished SBE: " + Config.Data.cancelBuild.caption);
                }
            }
            catch (Exception e)
            {
                Log.nlog.Error("Cancel-Build error: " + e.Message);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            try
            {
                if(_sbe.basic(Config.Data.postBuild, SBEQueueDTE.Type.POST)) {
                    Log.nlog.Info("[Post] finished SBE: " + Config.Data.postBuild.caption);
                }
            }
            catch (Exception e)
            {
                Log.nlog.Error("Post-Build error: " + e.Message);
            }
            return VSConstants.S_OK;
        }

        void IListenerOWPL.raw(string data)
        {
            try {
                if(_sbe.supportOWP(Config.Data.transmitter, SBEQueueDTE.Type.TRANSMITTER, data)) {
                    //Log.nlog.Info("[Transmitter]: " + Config.Data.transmitter.caption);
                }
            }
            catch(Exception e) {
                Log.nlog.Error("Transmitter error: " + e.Message);
            }

            OutputWPBuildParser res = new OutputWPBuildParser(ref data);

            if(Config.Data.warningsBuild.enabled) {
                sbeEW(Config.Data.warningsBuild, OutputWPBuildParser.Type.Warnings, res);
            }

            if(Config.Data.errorsBuild.enabled) {
                sbeEW(Config.Data.errorsBuild, OutputWPBuildParser.Type.Errors, res);
            }

            if(Config.Data.outputCustomBuild.enabled) {
                sbeOutput(Config.Data.outputCustomBuild, ref data);
            }
        }

        void sbeEW(ISolutionEventEW evt, OutputWPBuildParser.Type type, OutputWPBuildParser info)
        {
            // TODO: capture code####, message..
            if(!info.checkRule(type, evt.isWhitelist, evt.codes)) {
                return;
            }

            try {
                if(_sbe.basic(evt, type == OutputWPBuildParser.Type.Warnings ? SBEQueueDTE.Type.WARNINGS : SBEQueueDTE.Type.ERRORS)) {
                    Log.nlog.Info(String.Format("['{0}'] finished SBE: {1}", type.ToString(), evt.caption));
                }
            }
            catch(Exception e) {
                Log.nlog.Error(String.Format("SBE '{0}' error: {1}", type.ToString(), e.Message));
            }
        }

        void sbeOutput(ISolutionEventOWP evt, ref string raw)
        {
            if(!(new OWPMatcher()).match(evt.eventsOWP, raw)) {
                return;
            }

            try {
                if(_sbe.basic(evt, SBEQueueDTE.Type.OWP)) {
                    Log.nlog.Info(String.Format("['{0}'] finished SBE: {1}", "Output", evt.caption));
                }
            }
            catch(Exception e) {
                Log.nlog.Error(String.Format("SBE '{0}' error: {1}", "Output", e.Message));
            }
        }

        private void _state()
        {
            Func<ISolutionEvent, string, string> aboutEvent = delegate(ISolutionEvent evt, string caption) {
                return String.Format("\n\t* [{0}][{1}]: {2}", evt.enabled ? "!" : "X", caption, evt.caption);
            };

            Log.print(String.Format("{0}{1}{2}{3}{4}{5}{6}\n---\n",
                                    aboutEvent(Config.Data.preBuild,            "Pre-Build"),
                                    aboutEvent(Config.Data.postBuild,           "Post-Build"),
                                    aboutEvent(Config.Data.cancelBuild,         "Cancel-Build"),
                                    aboutEvent(Config.Data.warningsBuild,       "Warnings-Build"),
                                    aboutEvent(Config.Data.errorsBuild,         "Errors-Build"),
                                    aboutEvent(Config.Data.outputCustomBuild,   "Output-Build"),
                                    aboutEvent(Config.Data.transmitter,         "Transmitter")));

            Log.nlog.Info("Use vsSBE panel: View -> Other Windows -> Solution Build-Events");
        }

        #region unused

        int IVsUpdateSolutionEvents.UpdateSolution_StartUpdate(ref int pfCancelUpdate)
        {
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy)
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

        #region cookies
        protected override void Initialize()
        {
            Log.nlog.Trace(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            try
            {
                OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

                // Build / <Main App>
                _menuItemMain = new MenuCommand(_menuMainCallback, new CommandID(GuidList.MAIN_CMD_SET, (int)PkgCmdIDList.CMD_MAIN));
                _menuItemMain.Visible = false;
                mcs.AddCommand(_menuItemMain);

                // View / Other Windows / <Status Panel>
                mcs.AddCommand(new MenuCommand(_menuPanelCallback, new CommandID(GuidList.PANEL_CMD_SET, (int)PkgCmdIDList.CMD_PANEL)));

                // register events - IVsSolutionEvents
                _solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution;
                _solution.AdviseSolutionEvents(this, out _cookieSEvents);

                // register events - IVsUpdateSolutionEvents
                _solBuildManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
                _solBuildManager.AdviseUpdateSolutionEvents(this, out _cookieUpdateSEvents);
            }
            catch(Exception e)
            {
                string msg = string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-", 
                                "Try to restart a VS IDE or reinstall current plugin in the Extension Manager...", 
                                e.StackTrace);

                Log.nlog.Fatal(msg);
                
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

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if(_solBuildManager != null && _cookieUpdateSEvents != 0) {
                _solBuildManager.UnadviseUpdateSolutionEvents(_cookieUpdateSEvents);
            }

            if(_solution != null && _cookieSEvents != 0) {
                _solution.UnadviseSolutionEvents(_cookieSEvents);
            }
        }
        #endregion
    }
}
