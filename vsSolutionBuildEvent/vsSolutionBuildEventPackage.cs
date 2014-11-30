/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using net.r_eg.vsSBE.SBEScripts.Dom;

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

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        private EnvDTE.CommandEvents _cmdEvents;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            try {
                FindToolWindow(typeof(UI.Xaml.StatusToolWindow), 0, true);

                Config._.load(extractPath(Dte2));

                _state();
                _menuItemMain.Visible = true;
                UI.Xaml.StatusToolWindow.control.enabledPanel(true);
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.nlog.Fatal("Cannot load configuration: " + ex.Message);
            }
            return VSConstants.S_FALSE;
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
            try {
                return _c.bindPre(ref pfCancelUpdate);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Pre-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        public int UpdateSolution_Cancel()
        {
            try {
                return _c.bindCancel();
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Cancel-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
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
                int ret = _c.bindPost(fSucceeded, fModified, fCancelCommand);
                if(_c.reset()) {
                    uvariable.unsetAll();
                }
                return ret;
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Post-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        public int UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            try {
                return _c.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Pre-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        public int UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            try {
                return _c.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Post-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        private void init()
        {
            Log.show();
            attachCommandEvents();
            _env = new Environment(Dte2);

            IBootloader bootloader = new Bootloader(_env, uvariable);
            Inspector._.extract(bootloader);

            _c = new Connection(
                    new Command(_env,
                                new Script(bootloader),
                                new MSBuildParser(_env, uvariable))
            );

            _owpBuild = new OWP.Listener(_env, "Build");
            _owpBuild.attachEvents();
            _owpBuild.register(_c);

            UI.Xaml.StatusToolWindow.control.setDTE(Dte2);
        }

        /// <summary>
        /// Getting work path from DTE-context of current solution
        /// </summary>
        /// <param name="dte2">DTE context</param>
        private string extractPath(DTE2 dte2)
        {
            string path = dte2.Solution.FullName; // empty if used the new solution 
            if(string.IsNullOrEmpty(path)) {
                path = dte2.Solution.Properties.Item("Path").Value.ToString();
            }
            string dir = Path.GetDirectoryName(path);

            if(dir.ElementAt(dir.Length - 1) != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }

        private void attachCommandEvents()
        {
            _cmdEvents = Dte2.Events.CommandEvents; // protection from garbage collector
            lock(_lock) {
                _cmdEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
                _cmdEvents.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
            }
        }

        private void detachCommandEvents()
        {
            lock(_lock) {
                Dte2.Events.CommandEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
            }
        }

        /// <summary>
        /// Provides the BuildAction
        /// Note: VSSOLNBUILDUPDATEFLAGS with IVsUpdateSolutionEvents4 exist only for VS2012 and higher
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsupdatesolutionevents4.updatesolution_beginupdateaction.aspx
        /// See for details: http://stackoverflow.com/q/27018762
        /// </summary>
        private void _cmdBeforeExecute(string guidString, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            Guid guid = new Guid(guidString);

            if(GuidList.VSStd97CmdID == guid || GuidList.VSStd2KCmdID == guid) {
                _c.updateContext((BuildType)id);
            }
        }

        /// <summary>
        /// to show the main window if clicked # Build/<pack> #
        /// </summary>
        private void _menuMainCallback(object sender, EventArgs e)
        {
            if(UI.Util.focusForm(_configFrm)) {
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

        #region maintenance
        protected override void Initialize()
        {
            Log.nlog.Trace(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            try {
                init();
                OleMenuCommandService mcs = (OleMenuCommandService)GetService(typeof(IMenuCommandService));

                // Build / <Main App>
                _menuItemMain = new MenuCommand(_menuMainCallback, new CommandID(GuidList.MAIN_CMD_SET, (int)PkgCmdIDList.CMD_MAIN));
                _menuItemMain.Visible = false;
                mcs.AddCommand(_menuItemMain);

                // View / Other Windows / <Status Panel>
                mcs.AddCommand(new MenuCommand(_menuPanelCallback, new CommandID(GuidList.PANEL_CMD_SET, (int)PkgCmdIDList.CMD_PANEL)));

                // To listen events fires from IVsSolutionEvents
                spSolution = (IVsSolution)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution));
                spSolution.AdviseSolutionEvents(this, out _pdwCookieSolution);

                // To listen events fires from IVsUpdateSolutionEvents2
                spSolutionBM = (IVsSolutionBuildManager2)ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager));
                spSolutionBM.AdviseUpdateSolutionEvents(this, out _pdwCookieSolutionBM);
            }
            catch(Exception ex)
            {
                string msg = string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-", 
                                "Try also to restart a VS IDE or reinstall current plugin in the Extension Manager...", 
                                ex.ToString());

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
            UI.Util.closeTool(_configFrm);

            if(spSolutionBM != null && _pdwCookieSolutionBM != 0) {
                spSolutionBM.UnadviseUpdateSolutionEvents(_pdwCookieSolutionBM);
            }

            if(spSolution != null && _pdwCookieSolution != 0) {
                spSolution.UnadviseSolutionEvents(_pdwCookieSolution);
            }

            detachCommandEvents();
            base.Dispose(disposing);
        }
        #endregion
    }
}
