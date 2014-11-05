/*
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;

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
    [ProvideToolWindow(typeof(UI.Xaml.StatusToolWindow), Height=23, Style=VsDockStyle.Linked, Orientation=ToolWindowOrientation.Top, Window=ToolWindowGuids80.Outputwindow)]

    // Package Guid
    [Guid(GuidList.PACKAGE_STRING)]

    public sealed class vsSolutionBuildEventPackage: Package, IVsSolutionEvents, IVsUpdateSolutionEvents2
    {
        /// <summary>
        /// top-level object in the Visual Studio
        /// TODO: replace on the Environment
        /// </summary>
        public static DTE2 Dte2
        {
            get{ return (DTE2)Package.GetGlobalService(typeof(SDTE)); }
        }

        /// <summary>
        /// top-level manipulation or maintenance of the solution
        /// </summary>
        public static IVsSolution Solution
        {
            get { return (IVsSolution)Package.GetGlobalService(typeof(SVsSolution)); }
        }

        /// <summary>
        /// access to the fundamental environment services
        /// </summary>
        public static IVsShell Shell
        {
            get { return (IVsShell)Package.GetGlobalService(typeof(SVsShell)); }
        }

        /// <summary>
        /// for register events -> _cookieSEvents
        /// </summary>
        private IVsSolution _solution;
        private uint _cookieSEvents;

        /// <summary>
        /// for register events -> _cookieUpdateSEvents
        /// </summary>
        private IVsSolutionBuildManager _solBuildManager;
        private uint _cookieUpdateSEvents;

        /// <summary>
        /// VS IDE menu - Build / <Main App>
        /// </summary>
        private MenuCommand _menuItemMain;
        
        /// <summary>
        /// main form of settings
        /// </summary>
        private UI.WForms.EventsFrm _configFrm;

        /// <summary>
        /// Main container of user-variables
        /// </summary>
        private IUserVariable uvariable = new UserVariable();

        /// <summary>
        /// SBE support
        /// </summary>
        private Connection _c;

        /// <summary>
        /// Used environment
        /// </summary>
        private Environment _env;

        /// <summary>
        /// Working with the OutputWindowsPane -> "Build" pane
        /// </summary>
        private OWP.Listener _owpBuild;

        public vsSolutionBuildEventPackage()
        {
            Log.show();
            _env = new Environment(Dte2);

            _c = new Connection(
                    new SBECommand(_env, 
                                    new Script(_env, uvariable), 
                                    new MSBuildParser(_env, uvariable))
            );

            _owpBuild = new OWP.Listener(_env, "Build");
            _owpBuild.attachEvents();
            _owpBuild.register(_c);

            UI.Xaml.StatusToolWindow.control.setDTE(Dte2);
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {            
            try
            {
                FindToolWindow(typeof(UI.Xaml.StatusToolWindow), 0, true);

                string path = Dte2.Solution.FullName; // may be empty e.g. if fNewSolution == 1 etc.
                if(string.IsNullOrEmpty(path)) {
                    path = Dte2.Solution.Properties.Item("Path").Value.ToString();
                }
                string dir = Path.GetDirectoryName(path);

                if(dir.ElementAt(dir.Length - 1) != Path.DirectorySeparatorChar) {
                    dir += Path.DirectorySeparatorChar;
                }

                Config._.load(dir);
                _c.updateContext(new SBECommand.ShellContext(Settings.WorkPath));
            }
            catch(Exception ex) {
                Log.nlog.Fatal("Cannot load configuration: " + ex.Message);
                return VSConstants.E_FAIL;
            }
            _state();
            _menuItemMain.Visible = true;
            UI.Xaml.StatusToolWindow.control.enabledPanel(true);
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            _menuItemMain.Visible = false;
            UI.Xaml.StatusToolWindow.control.enabledPanel(false);
            UI.Util.closeTool(_configFrm);

            return VSConstants.S_OK;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            return _c.bindPre(ref pfCancelUpdate);
        }

        public int UpdateSolution_Cancel()
        {
            return _c.bindCancel();
        }

        /// <summary>
        /// 
        /// When it works:
        ///    * Begin -> Done
        ///    * Begin -> Cancel -> Done
        /// </summary>
        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            int ret = _c.bindPost(fSucceeded, fModified, fCancelCommand);
            if(_c.reset()) {
                uvariable.unsetAll();
            }
            return ret;
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            return _c.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            return _c.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
        }

        /// <summary>
        /// to show the main window if clicked # Build/<pack> #
        /// </summary>
        private void _menuMainCallback(object sender, EventArgs e)
        {
            if(_configFrm != null && !_configFrm.IsDisposed) {
                _configFrm.Focus();
                return;
            }
            _configFrm = new UI.WForms.EventsFrm(_env);
            _configFrm.Show();
        }

        private void _menuPanelCallback(object sender, EventArgs e)
        {
            ToolWindowPane window = FindToolWindow(typeof(UI.Xaml.StatusToolWindow), 0, true); // find or create
            if(window == null || window.Frame == null) {
                throw new ComponentException("Cannot create UI.StatusToolWindow");
            }
            ErrorHandler.ThrowOnFailure(((IVsWindowFrame)window.Frame).Show());
        }

        private void _state()
        {
            Func<ISolutionEvent[], string, string> about = delegate(ISolutionEvent[] evt, string caption)
            {
                if(evt == null) {
                    return String.Format("\n\t-- /--] {0} :: Not Initialized", caption);
                }

                System.Text.StringBuilder info = new System.Text.StringBuilder();
                info.Append(String.Format("\n\t{0,2} /{1,2}] {2} :: ", evt.Where(i => i.Enabled).Count(), evt.Length, caption));
                foreach(ISolutionEvent item in evt) {
                    info.Append(String.Format("[{0}]", (item.Enabled) ? "!" : "X"));
                }
                return info.ToString();
            };

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(about(Config._.Data.PreBuild,      "Pre-Build     "));
            sb.Append(about(Config._.Data.PostBuild,     "Post-Build    "));
            sb.Append(about(Config._.Data.CancelBuild,   "Cancel-Build  "));
            sb.Append(about(Config._.Data.WarningsBuild, "Warnings-Build"));
            sb.Append(about(Config._.Data.ErrorsBuild,   "Errors-Build  "));
            sb.Append(about(Config._.Data.OWPBuild,      "Output-Build  "));
            sb.Append(about(Config._.Data.Transmitter,   "Transmitter   "));
            sb.Append("\n---\n");
            Log.print(sb.ToString());
            Log.nlog.Info("vsSBE tool pane: View -> Other Windows -> Solution Build-Events");
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
            catch(Exception ex)
            {
                string msg = string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-", 
                                "Try to restart a VS IDE or reinstall current plugin in the Extension Manager...", 
                                ex.StackTrace);

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
            UI.Util.closeTool(_configFrm);

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
