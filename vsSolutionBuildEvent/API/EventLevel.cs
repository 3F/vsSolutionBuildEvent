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
    public class EventLevel: IEventLevel, Bridge.IBuild, Bridge.IEvent
    {
        /// <summary>
        /// The solution has been opened
        /// </summary>
        public event EventHandler OpenedSolution = delegate(object sender, EventArgs e) { };

        /// <summary>
        /// The solution has been closed
        /// </summary>
        public event EventHandler ClosedSolution = delegate(object sender, EventArgs e) { };

        /// <summary>
        /// Main loader
        /// </summary>
        public IBootloader Bootloader
        {
            get;
            protected set;
        }

        /// <summary>
        /// Binder of action
        /// </summary>
        public Actions.Connection Action
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used Environment
        /// </summary>
        public IEnvironment Environment
        {
            get;
            protected set;
        }

        /// <summary>
        /// Container of user-variables
        /// </summary>
        protected IUserVariable uvariable = new UserVariable();

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        protected EnvDTE.CommandEvents cmdEvents;

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
                Config._.load(Environment.SolutionPath, Environment.SolutionFileName);
                Config._.updateActivation(Bootloader);

                UI.Plain.State.print(Config._.Data);
#if DEBUG
                Log.nlog.Warn("Used the [Debug version]");
#else
                if(vsSBE.Version.branchName.ToLower() != "releases") {
                    Log.nlog.Warn("Used the [Unofficial release]");
                }
#endif
                Action.loggingEventActivated = true;

                OpenedSolution(this, new EventArgs());
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
            ClosedSolution(this, new EventArgs());
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
                return Action.bindPre(ref pfCancelUpdate);
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
                return Action.bindCancel();
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
                int ret = Action.bindPost(fSucceeded, fModified, fCancelCommand);
                if(Action.reset()) {
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
                return Action.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Pre-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onProjectPre(string project)
        {
            try {
                return Action.bindProjectPre(project);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Pre-binding/simple: '{0}'", ex.Message);
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
                return Action.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Post-binding: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        public int onProjectPost(string project, int fSuccess)
        {
            try {
                return Action.bindProjectPost(project, fSuccess);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed Project.Post-binding/simple: '{0}'", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// During assembly.
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        public void onBuildRaw(string data)
        {
            try {
                Action.bindBuildRaw(data);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed build-raw: '{0}'", ex.Message);
            }
        }

        public EventLevel(DTE2 dte2, bool debug = false)
        {
            vsSBE.Settings.debugMode = debug;
            this.Environment = new Environment(dte2);
            init();
        }

        public EventLevel(string solutionFile, Dictionary<string, string> properties, bool debug = false)
        {
            vsSBE.Settings.debugMode = debug;
            this.Environment = new IsolatedEnv(solutionFile, properties);
            init();
        }

        protected void init()
        {
            attachCommandEvents();
            this.Bootloader = new Bootloader(Environment, uvariable);

            Action = new Actions.Connection(
                            new Actions.Command(Environment,
                                         new Script(Bootloader),
                                         new MSBuildParser(Environment, uvariable))
            );
        }

        protected void attachCommandEvents()
        {
            if(Environment.Events == null) {
                Log.nlog.Warn("Context of build action: Disabled for current Environment.");
                return; //this can be for emulated DTE2 context
            }

            cmdEvents = Environment.Events.CommandEvents; // protection from garbage collector
            lock(_lock) {
                cmdEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
                cmdEvents.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
            }
        }

        protected void detachCommandEvents()
        {
            if(cmdEvents == null) {
                return;
            }
            lock(_lock) {
                cmdEvents.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler(_cmdBeforeExecute);
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
                Action.updateContext((BuildType)id);
            }
        }
    }
}
