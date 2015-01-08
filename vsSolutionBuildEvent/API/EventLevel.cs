/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.API
{
    public class EventLevel: Bridge.IEvent, IDisposable
    {
        /// <summary>
        /// Notification about of the solution after opening/creating
        /// </summary>
        public delegate void OpenedSolutionEvent();

        /// <summary>
        /// Notification about of the solution after closing
        /// </summary>
        public delegate void ClosedSolutionEvent();

        /// <summary>
        /// The solution has been opened
        /// </summary>
        public event OpenedSolutionEvent OpenedSolution = delegate { };

        /// <summary>
        /// The solution has been closed
        /// </summary>
        public event ClosedSolutionEvent ClosedSolution = delegate { };

        /// <summary>
        /// Main loader
        /// </summary>
        public IBootloader Bootloader
        {
            get;
            protected set;
        }

        /// <summary>
        /// DTE2 Context
        /// </summary>
        protected DTE2 dte2;

        /// <summary>
        /// Container of user-variables
        /// </summary>
        protected IUserVariable uvariable = new UserVariable();

        /// <summary>
        /// SBE support
        /// </summary>
        protected Actions.Connection action;

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        protected EnvDTE.CommandEvents cmdEvents;

        /// <summary>
        /// Working with the OutputWindowsPane -> "Build" pane
        /// </summary>
        private OWP.Listener _owpListener;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            try {
                Config._.load(extractPath(dte2));
                Config._.updateActivation(Bootloader);

                OpenedSolution();
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.nlog.Fatal("Cannot load configuration: " + ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int solutionClosed(object pUnkReserved)
        {
            ClosedSolution();
            return VSConstants.S_OK;
        }

        /// <summary>
        /// 'PRE' of the solution.
        /// Called before any build actions have begun.
        /// </summary>
        /// <param name="pfCancelUpdate">Pointer to a flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onPre(ref int pfCancelUpdate)
        {
            try {
                return action.bindPre(ref pfCancelUpdate);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Pre-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// Called when a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onCancel()
        {
            try {
                return action.bindCancel();
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Cancel-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'POST' of the solution.
        /// Called when a build is completed.
        /// </summary>
        /// <param name="fSucceeded">true if no update actions failed.</param>
        /// <param name="fModified">true if any update action succeeded.</param>
        /// <param name="fCancelCommand">true if update actions were canceled.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            try {
                int ret = action.bindPost(fSucceeded, fModified, fCancelCommand);
                if(action.reset()) {
                    uvariable.unsetAll();
                }
                return ret;
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Solution.Post-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'PRE' of Projects.
        /// Called right before a project configuration begins to build.
        /// </summary>
        /// <param name="pHierProj">Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="pfCancel">Pointer to a flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            try {
                return action.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Pre-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'POST' of Projects.
        /// Called right after a project configuration is finished building.
        /// </summary>
        /// <param name="pHierProj">Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <param name="fCancel">Flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            try {
                return action.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Post-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        public void Dispose()
        {
            detachCommandEvents(); //optional, i.e. should with cmdEvents ...
        }

        public EventLevel(DTE2 dte2)
        {
            this.dte2 = dte2;
            init();
        }

        protected void init()
        {
            Log.show();
            attachCommandEvents();

            IEnvironment env    = new Environment(dte2);
            this.Bootloader     = new Bootloader(env, uvariable);

            action = new Actions.Connection(
                            new Actions.Command(env,
                                         new Script(Bootloader),
                                         new MSBuildParser(env, uvariable))
            );

            _owpListener = new OWP.Listener(env, "Build");
            _owpListener.attachEvents();
            _owpListener.register(action);
        }

        /// <summary>
        /// Getting work path from DTE-context of current solution
        /// </summary>
        /// <param name="dte2">DTE context</param>
        protected string extractPath(DTE2 dte2)
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

        protected void attachCommandEvents()
        {
            cmdEvents = dte2.Events.CommandEvents; // protection from garbage collector
            lock(_lock) {
                cmdEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
                cmdEvents.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
            }
        }

        protected void detachCommandEvents()
        {
            lock(_lock) {
                dte2.Events.CommandEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
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

            if(GuidList.VSStd97CmdID != guid && GuidList.VSStd2KCmdID != guid) {
                return;
            }

            if(Enum.IsDefined(typeof(BuildType), id)) {
                action.updateContext((BuildType)id);
            }
        }
    }
}
