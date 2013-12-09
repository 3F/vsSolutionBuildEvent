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
    [InstalledProductRegistration("#110", "#112", "0.2.2", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]

    //  To be automatically loaded when a specified UI context is active
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]

    // Package Guid
    [Guid(GuidList.guidvsSolutionBuildEventPkgString)]

    public sealed class vsSolutionBuildEventPackage : Package, IVsSolutionEvents, IVsUpdateSolutionEvents
    {
        /// <summary>
        /// this object for a top-level functionality
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


        //TODO
        public const string PANE_ITEM                       = "Solution Build-Events";

        public vsSolutionBuildEventPackage()
        {
            _dte = (DTE2)Package.GetGlobalService(typeof(SDTE));
        }

        /// <summary>
        /// execute a command when the a menu item (Build/<pack>) is clicked
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (_configFrm != null && !_configFrm.IsDisposed)
            {
                return;
            }
            _configFrm = new EventsFrm();
            _configFrm.Show();
        }

        //TODO:
        public OutputWindowPane Pane
        {
            get
            {
                try
                {
                    return _dte.ToolWindows.OutputWindow.OutputWindowPanes.Item(PANE_ITEM);
                }
                catch { }
                return _dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(PANE_ITEM);
            }
        }

        int IVsSolutionEvents.OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            Config.load(Path.GetDirectoryName(_dte.Solution.FullName) + "\\");
            _info();

            _menuItem.Visible = true;
            return VSConstants.S_OK;
        }

        //TODO:
        private void _info()
        {
            Pane.Clear();
            string stat = string.Format(
                CultureInfo.CurrentCulture,
                "loaded settings: {6}\n\nReady:\n\t* [Pre-Build][{0}]: {1}\n\t* [Post-Build][{2}]: {3}\n\t* [Cancel-Build][{4}]: {5}\n---\n",
                Config.data.preBuild.enabled.ToString(), Config.data.preBuild.caption,
                Config.data.postBuild.enabled.ToString(), Config.data.postBuild.caption,
                Config.data.cancelBuild.enabled.ToString(), Config.data.cancelBuild.caption, Config.getWorkPath());

            Pane.OutputString(stat);
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
                    Pane.OutputString("[Pre] finished SBE: " + Config.data.preBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Pane.OutputString("Pre-Build error: " + e.Message + Environment.NewLine);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Cancel()
        {
            try
            {
                if((new SBECommand()).basic(Config.data.cancelBuild)){
                    Pane.OutputString("[Cancel] finished SBE: " + Config.data.cancelBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Pane.OutputString("Cancel-Build error: " + e.Message + Environment.NewLine);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents.UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand)
        {
            try
            {
                if((new SBECommand()).basic(Config.data.postBuild)){
                    Pane.OutputString("[Post] finished SBE: " + Config.data.postBuild.caption + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Pane.OutputString("Post-Build error: " + e.Message + Environment.NewLine);
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

        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                CommandID menuCommandID = new CommandID(GuidList.guidvsSolutionBuildEventCmdSet, (int)PkgCmdIDList.cmdSolutionBuildEvent);
                _menuItem               = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(_menuItem);
            }

            // register events - IVsSolutionEvents
            _solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            if (_solution != null)
            {
                _solution.AdviseSolutionEvents(this, out _cookieSEvents);
            }

            // register events - IVsUpdateSolutionEvents
            _solBuildManager = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager;
            if (_solBuildManager != null)
            {
                _solBuildManager.AdviseUpdateSolutionEvents(this, out _cookieUpdateSEvents);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_solBuildManager != null && _cookieUpdateSEvents != 0)
            {
                _solBuildManager.UnadviseUpdateSolutionEvents(_cookieUpdateSEvents);
            }

            if (_solution != null && _cookieSEvents != 0)
            {
                _solution.UnadviseSolutionEvents(_cookieSEvents);
            }
        }
    }
}
