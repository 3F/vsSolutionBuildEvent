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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    public class Connection: OWP.IListenerOWPL
    {
        /// <summary>
        /// Ignored all action if value as true
        /// Support of cycle control, e.g.: PRE -> POST [recursive DTE: PRE -> POST] -> etc.
        /// </summary>
        public static volatile bool silent = false;

        /// <summary>
        /// Execution order support.
        /// Contains the current states of all projects
        /// </summary>
        protected Dictionary<string, ExecutionOrderType> projects;

        /// <summary>
        /// Contains the current incoming project
        /// </summary>
        protected IExecutionOrder current = new ExecutionOrder();

        /// <summary>
        /// Connection handler
        /// </summary>
        protected ICommand cmd;

        /// <summary>
        /// Type of build action
        /// </summary>
        protected BuildType buildType;

        /// <summary>
        /// Checks the allow state for action
        /// </summary>
        protected bool IsAllowActions
        {
            get { return !silent; }
        }

        public Connection(ICommand cmd)
        {
            this.cmd = cmd;
            projects = new Dictionary<string, ExecutionOrderType>();
        }

        /// <summary>
        /// Updating context with the BuildType
        /// </summary>
        /// <param name="buildType">Type of build action</param>
        public void updateContext(BuildType buildType)
        {
            this.buildType = buildType;
            cmd.updateContext(buildType);
        }

        /// <summary>
        /// Binding 'PRE' of Solution
        /// </summary>
        public int bindPre(ref int pfCancelUpdate)
        {
            if(isDisabledAll(Config._.Data.PreBuild)) {
                return VSConstants.S_OK;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Pre);
            }

            foreach(SBEEvent item in Config._.Data.PreBuild)
            {
                if(hasExecutionOrder(item)) {
                    Log.nlog.Info("[Pre] SBE has deferred action: '{0}' :: waiting... ", item.Caption);
                    Status._.add(SolutionEventType.Pre, StatusType.Deferred);
                }
                else {
                    Status._.add(SolutionEventType.Pre, (execPre(item) == VSConstants.S_OK)? StatusType.Success : StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Pre, StatusType.Fail)? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        /// <summary>
        /// Binding 'POST' of Solution
        /// </summary>
        public int bindPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            SBEEvent[] evt = Config._.Data.PostBuild;

            if(isDisabledAll(evt)) {
                return VSConstants.S_OK;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Post);
            }

            foreach(SBEEvent item in evt)
            {
                if(fSucceeded != 1 && item.IgnoreIfBuildFailed) {
                    Log.nlog.Info("[Post] ignored action '{0}' :: Build FAILED. See option 'Ignore if the build failed'", item.Caption);
                    continue;
                }

                if(!isReached(item)) {
                    Log.nlog.Info("[Post] ignored action '{0}' ::  not reached selected projects in execution order", item.Caption);
                    continue;
                }

                try {
                    if(cmd.exec(item, SolutionEventType.Post)) {
                        Log.nlog.Info("[Post] finished SBE: {0}", item.Caption);
                    }
                    Status._.add(SolutionEventType.Post, StatusType.Success);
                }
                catch(Exception ex) {
                    Log.nlog.Error("Post-Build error: {0}", ex.Message);
                    Status._.add(SolutionEventType.Post, StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Post, StatusType.Fail)? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        /// <summary>
        /// Binding 'PRE' of Projects
        /// </summary>
        public int bindProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
            onProject(pHierProj, ExecutionOrderType.Before);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Binding 'POST' of Projects
        /// </summary>
        public int bindProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            onProject(pHierProj, ExecutionOrderType.After, fSuccess == 1 ? true : false);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Binding 'Cancel/Abort' of Solution
        /// </summary>
        public int bindCancel()
        {
            SBEEvent[] evt = Config._.Data.CancelBuild;

            if(isDisabledAll(evt)) {
                return VSConstants.S_OK;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Cancel);
            }

            foreach(SBEEvent item in evt)
            {
                if(!isReached(item)) {
                    Log.nlog.Info("[Cancel] ignored action '{0}' :: not reached selected projects in execution order", item.Caption);
                    continue;
                }

                try {
                    if(cmd.exec(item, SolutionEventType.Cancel)) {
                        Log.nlog.Info("[Cancel] finished SBE: {0}", item.Caption);
                    }
                    Status._.add(SolutionEventType.Cancel, StatusType.Success);
                }
                catch(Exception ex) {
                    Log.nlog.Error("Cancel-Build error: {0}", ex.Message);
                    Status._.add(SolutionEventType.Cancel, StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Cancel, StatusType.Fail)? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        /// <summary>
        /// Resetting all progress of handling events
        /// </summary>
        /// <returns>true value if successful resetted</returns>
        public bool reset()
        {
            if(!IsAllowActions) {
                return false;
            }
            projects.Clear();
            current = new ExecutionOrder();
            Status._.flush();
            return true;
        }

        /// <summary>
        /// Listener of raw OWP
        /// </summary>
        void OWP.IListenerOWPL.raw(string data)
        {
            if(!IsAllowActions)
            {
                if(!isDisabledAll(Config._.Data.Transmitter)) {
                    _ignoredAction(SolutionEventType.Transmitter);
                }
                else if(!isDisabledAll(Config._.Data.WarningsBuild)) {
                    _ignoredAction(SolutionEventType.Warnings);
                }
                else if(!isDisabledAll(Config._.Data.ErrorsBuild)) {
                    _ignoredAction(SolutionEventType.Errors);
                }
                else if(!isDisabledAll(Config._.Data.OWPBuild)) {
                    _ignoredAction(SolutionEventType.OWP);
                }
                return;
            }

            //TODO: IStatus
            
            foreach(SBETransmitter evt in Config._.Data.Transmitter)
            {
                if(!isExecute(evt, current)) {
                    Log.nlog.Info("[Transmitter] ignored action '{0}' :: by execution order", evt.Caption);
                }
                else {
                    try {
                        if(cmd.exec(evt, SolutionEventType.Transmitter)) {
                            //Log.nlog.Trace("[Transmitter]: " + Config._.Data.transmitter.caption);
                        }
                    }
                    catch(Exception ex) {
                        Log.nlog.Error("Transmitter error: {0}", ex.Message);
                    }
                }
            }

            //TODO: ExecStateType

            foreach(SBEEventEW evt in Config._.Data.WarningsBuild) {
                if(evt.Enabled) {
                    sbeEW(evt, OWP.BuildItem.Type.Warnings);
                }
            }

            foreach(SBEEventEW evt in Config._.Data.ErrorsBuild) {
                if(evt.Enabled) {
                    sbeEW(evt, OWP.BuildItem.Type.Errors);
                }
            }

            foreach(SBEEventOWP evt in Config._.Data.OWPBuild) {
                if(evt.Enabled) {
                    sbeOutput(evt, ref data);
                }
            }
        }

        /// <summary>
        /// Entry point to Errors/Warnings
        /// </summary>
        protected int sbeEW(ISolutionEventEW evt, OWP.BuildItem.Type type)
        {
            OWP.BuildItem info = OWP.Items._.Build;

            // TODO: capture code####, message..
            if(!info.checkRule(type, evt.IsWhitelist, evt.Codes)) {
                return VSConstants.S_OK;
            }

            if(!isExecute(evt, current)) {
                Log.nlog.Info("[{0}] ignored action '{1}' :: by execution order", type, evt.Caption);
                return VSConstants.S_OK;
            }

            try {
                if(cmd.exec(evt, (type == OWP.BuildItem.Type.Warnings)? SolutionEventType.Warnings : SolutionEventType.Errors)) {
                    Log.nlog.Info("[{0}] finished SBE: {1}", type, evt.Caption);
                }
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.nlog.Error("SBE '{0}' error: {1}", type, ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Entry point to the OWP
        /// </summary>
        protected int sbeOutput(ISolutionEventOWP evt, ref string raw)
        {
            if(!(new OWP.Matcher()).match(evt.Match, raw)) {
                return VSConstants.S_OK;
            }

            if(!isExecute(evt, current)) {
                Log.nlog.Info("[Output] ignored action '{0}' :: by execution order", evt.Caption);
                return VSConstants.S_OK;
            }

            try {
                if(cmd.exec(evt, SolutionEventType.OWP)) {
                    Log.nlog.Info("[Output] finished SBE: {0}", evt.Caption);
                }
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.nlog.Error("SBE 'Output' error: {0}", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Immediately execute 'PRE' of Solution
        /// </summary>
        protected int execPre(SBEEvent evt)
        {
            try {
                if(cmd.exec(evt, SolutionEventType.Pre)) {
                    Log.nlog.Info("[Pre] finished SBE: {0}", evt.Caption);
                }
                return VSConstants.S_OK;
            }
            catch(Exception ex) {
                Log.nlog.Error("Pre-Build error: {0}", ex.Message);
            }
            return VSConstants.S_FALSE;
        }

        protected int execPre()
        {
            int idx = 0;
            foreach(SBEEvent item in Config._.Data.PreBuild) {
                if(hasExecutionOrder(item)) {
                    Status._.update(SolutionEventType.Pre, idx, (execPre(item) == VSConstants.S_OK)? StatusType.Success : StatusType.Fail);
                }
                ++idx;
            }
            return Status._.contains(SolutionEventType.Pre, StatusType.Fail)? VSConstants.S_FALSE : VSConstants.S_OK;
        }

        protected string getProjectName(IVsHierarchy pHierProj)
        {
            object name;
            // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivshierarchy.getproperty.aspx
            // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.__vshpropid.aspx
            pHierProj.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_Name, out name);

            return name.ToString();
        }

        /// <summary>
        /// Checking state from incoming projects
        /// In general, this checking for single the event-action like a POST/Cacnel
        /// note: 
        ///   * The 'POST' - exists as successfully completed event, therefore we should getting only the 'After' state.
        ///   * The 'Cancel' - works differently and contains realy the reached state: 'Before' or 'After'.
        /// </summary>
        protected bool isReached(ISolutionEvent evt)
        {
            if(!hasExecutionOrder(evt)) {
                return true;
            }
            Log.nlog.Debug("hasExecutionOrder(->isReached) for '{0}' is true", evt.Caption);

            return evt.ExecutionOrder.Where(e =>
                                              projects.ContainsKey(e.Project)
                                               && projects[e.Project] == e.Order
                                            ).Count() > 0;
        }

        /// <summary>
        /// Checking state with current range of order.
        /// 
        /// In general, this checking for multiple the event-action like a EW/OWP:
        ///   * Before1 -> After1|Cancel
        ///   * After1  -> POST/Cancel
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="incoming">Current incoming project</param>
        /// <returns>true value if it's allowed to execute for current state</returns>
        protected bool isExecute(ISolutionEvent evt, IExecutionOrder incoming)
        {
            if(!hasExecutionOrder(evt)) {
                return true;
            }
            Log.nlog.Debug("hasExecutionOrder(->isExecute) for '{0}' is true", evt.Caption);

            foreach(IExecutionOrder eo in evt.ExecutionOrder)
            {
                if(!projects.ContainsKey(eo.Project)) {
                    continue;
                }

                // The 'Before' base:
                if(eo.Order == ExecutionOrderType.Before)
                {
                    // Before1 -> After1
                    return (projects[eo.Project] == ExecutionOrderType.Before);
                }

                // The 'After' base:
                if(projects[eo.Project] != ExecutionOrderType.After) {
                    return false; // waiting for 'After' state..
                }

                if(incoming.Project != eo.Project) {
                    return true; // After1  -> POST/Cancel
                }
                return (incoming.Order == ExecutionOrderType.After); // 'After' is reached ?
            }
            return false;
        }

        protected void onProject(IVsHierarchy pHierProj, ExecutionOrderType type, bool fSuccess = true)
        {
            string project      = getProjectName(pHierProj);
            projects[project]   = type;

            current.Project = project;
            current.Order   = type;

            Log.nlog.Trace("onProject: '{0}':{1} == {2}", project, type, fSuccess);

            if(Status._.contains(SolutionEventType.Pre, StatusType.Deferred)) {
                monitoringPre(project, type, fSuccess);
            }
        }

        /// <summary>
        /// Monitoring for deferred PRE-actions - "it's time or not"
        /// </summary>
        /// <param name="project">incoming project name</param>
        /// <param name="type">type of execution order</param>
        /// <param name="fSuccess">Flag indicating success</param>
        protected void monitoringPre(string project, ExecutionOrderType type, bool fSuccess)
        {
            SBEEvent[] evt = Config._.Data.PreBuild;
            for(int i = 0; i < evt.Length; ++i)
            {
                if(!evt[i].Enabled || Status._.get(SolutionEventType.Pre, i) != StatusType.Deferred) {
                    continue;
                }

                if(!IsAllowActions) {
                    _ignoredAction(SolutionEventType.DeferredPre);
                    return;
                }

                if(!fSuccess && evt[i].IgnoreIfBuildFailed) {
                    Log.nlog.Info("[PRE] ignored action '{0}' :: Build FAILED. See option 'Ignore if the build failed'", evt[i].Caption);
                    continue;
                }

                if(!hasExecutionOrder(evt[i])) {
                    Log.nlog.Trace("[PRE] deferred: executionOrder is null or not contains elements :: {0}", evt[i].Caption);
                    return;
                }

                if(evt[i].ExecutionOrder.Where(o => o.Project == project && o.Order == type).Count() > 0) {
                    Log.nlog.Info("Incoming '{0}'({1}) :: Execute the deferred action: '{2}'", project, type, evt[i].Caption);
                    Status._.update(SolutionEventType.Pre, i, (execPre(evt[i]) == VSConstants.S_OK)? StatusType.Success : StatusType.Fail);
                }
            }
        }

        /// <param name="evt">Array of handling events</param>
        /// <returns>true value if all event are disabled for present array</returns>
        protected bool isDisabledAll(ISolutionEvent[] evt)
        {
            foreach(ISolutionEvent item in evt) {
                if(item.Enabled) {
                    return false;
                }
            }
            return true;
        }

        protected bool hasExecutionOrder(ISolutionEvent evt)
        {
            if(evt.ExecutionOrder == null || evt.ExecutionOrder.Length < 1) {
                return false;
            }
            return true;
        }

        private int _ignoredAction(SolutionEventType type)
        {
            Log.nlog.Trace("[{0}] ignored action. Is already started from another VSprocess.", type);
            return VSConstants.S_OK;
        }
    }
}
