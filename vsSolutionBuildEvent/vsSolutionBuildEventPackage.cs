/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;

namespace reg.ext.vsSolutionBuildEvent
{
    // Managed Package Registration
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // To register the informations needed to in the Help/About dialog of Visual Studio
    [InstalledProductRegistration("#110", "#112", "0.3.2", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    //  To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]

    // Package Guid
    [Guid(GuidList.guidvsSolutionBuildEventPkgString)]

    public sealed class vsSolutionBuildEventPackage: Package, IVsSolutionEvents, IVsUpdateSolutionEvents, IListenerOWPL
    {
        /// <summary>
        /// for a top-level functionality
        /// </summary>
        private DTE2 _dte                                   = null;

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
        /// commands with menu of VS - Build/
        /// </summary>
        private MenuCommand _menuItem                       = null;
        
        /// <summary>
        /// main form of settings
        /// </summary>
        private EventsFrm _configFrm                        = null;

        /// <summary>
        /// Working with the OutputWindowsPane -> "Build" pane
        /// </summary>
        private OutputWPListener _owpBuild;

        public vsSolutionBuildEventPackage()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(SDTE));
            PaneVS.instance.setDTE(_dte);

            _owpBuild = new OutputWPListener(_dte, "Build");
            _owpBuild.attachEvents();
            _owpBuild.register(this);
        }

        /// <summary>
        /// execute a command when clicked menu item (Build/<pack>)
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (_configFrm != null && !_configFrm.IsDisposed)
            {
                _configFrm.Focus();
                return;
            }
            _configFrm = new EventsFrm();
            _configFrm.Show();
        }

        //TODO:
        private void _info()
        {
            Func<ISolutionEvent, string, string> aboutEvent = delegate(ISolutionEvent evt, string caption) {
                return String.Format("\n\t* [{0}][{1}]: {2}", caption, evt.enabled.ToString(), evt.caption);
            };


            // TODO: Pane wrapper
            PaneVS.instance.outputString(String.Format("{0} {1}",
                String.Format("loaded settings: {0}\n\nReady:", Config.getWorkPath()),
                String.Format("{0}{1}{2}\n---\n",
                    aboutEvent(Config.data.preBuild, "Pre-Build"),
                    aboutEvent(Config.data.postBuild, "Post-Build"),
                    aboutEvent(Config.data.cancelBuild, "Cancel-Build"))
            ));
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            Config.load(Path.GetDirectoryName(_dte.Solution.FullName) + "\\");
            _info();

            _menuItem.Visible = true;
            return VSConstants.S_OK;
        }

        int IVsSolutionEvents.OnAfterCloseSolution(object pUnkReserved)
        {
            _configFrm.Close();
            _menuItem.Visible = false;
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Begin(ref int pfCancelUpdate)
        {
            try
            {
                if((new SBECommand()).basic(Config.data.preBuild)){
                    PaneVS.instance.outputString("[Pre] finished SBE: " + Config.data.preBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                PaneVS.instance.outputString("Pre-Build error: " + e.Message + Environment.NewLine);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Cancel()
        {
            try
            {
                if((new SBECommand()).basic(Config.data.cancelBuild)){
                    PaneVS.instance.outputString("[Cancel] finished SBE: " + Config.data.cancelBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                PaneVS.instance.outputString("Cancel-Build error: " + e.Message + Environment.NewLine);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            try
            {
                if((new SBECommand()).basic(Config.data.postBuild)){
                    PaneVS.instance.outputString("[Post] finished SBE: " + Config.data.postBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                PaneVS.instance.outputString("Post-Build error: " + e.Message + Environment.NewLine);
            }
            return VSConstants.S_OK;
        }

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

        void IListenerOWPL.raw(string data)
        {
            OutputWPBuildParser res = new OutputWPBuildParser(ref data);

            string test = data;
        }

        #region cookies
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            try {
                // menu
                OleMenuCommandService mcs   = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
                CommandID menuCommandID     = new CommandID(GuidList.guidvsSolutionBuildEventCmdSet, (int)PkgCmdIDList.cmdSolutionBuildEvent);
                _menuItem                   = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(_menuItem);

                // register events - IVsSolutionEvents
                _solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution;
                _solution.AdviseSolutionEvents(this, out _cookieSEvents);

                // register events - IVsUpdateSolutionEvents
                _solBuildManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
                _solBuildManager.AdviseUpdateSolutionEvents(this, out _cookieUpdateSEvents);
            }
            catch(Exception e) {
                IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
                
                int res;
                Guid id = Guid.Empty;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(
                    uiShell.ShowMessageBox(
                           0,
                           ref id,
                           "Initialize vsSolutionBuildEvent",
                           string.Format("{0}\n{1}\n\n-----\n{2}", 
                                "Something went wrong -_-", 
                                "Try to restart a VS IDE or reinstall current plugin in the Extension Manager...", 
                                e.StackTrace),
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
