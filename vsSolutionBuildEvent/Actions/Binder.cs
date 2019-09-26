/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.VS;
using net.r_eg.SobaScript.Z.VS.Owp;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.Logger;
using Task = System.Threading.Tasks.Task;

namespace net.r_eg.vsSBE.Actions
{
    using OWPIdent = Receiver.Output.Ident;
    using OWPItems = Receiver.Output.Items;

    /// <summary>
    /// Binder / Coordinator of main routes.
    /// </summary>
    public class Binder
    {
        /// <summary>
        /// To support the 'execution order' features.
        /// Contains the current states of all projects.
        /// </summary>
        protected Dictionary<string, EOProject> projects;

        /// <summary>
        /// Contains current incoming project.
        /// </summary>
        protected IExecutionOrder current = new ExecutionOrder();

        protected ISobaCLoader cLoader;

        private readonly object sync = new object();
        private readonly object _plock = new object();

        /// <summary>
        /// The main handler of commands.
        /// </summary>
        public ICommand Cmd
        {
            get;
            protected set;
        }

        /// <summary>
        /// Flag of permission for any actions.
        /// </summary>
        protected bool IsAllowActions
        {
            get { return !Settings._.IgnoreActions; }
        }

        /// <summary>
        /// Access to available events.
        /// </summary>
        protected ISolutionEvents SlnEvents
        {
            get { return Settings.Cfg; }
        }

        protected sealed class EOProject: ExecutionOrder
        {
            public string aProject;
            public string aType;
        }

        /// <summary>
        /// Binding 'PRE' of Solution
        /// </summary>
        public int bindPre(ref int pfCancelUpdate)
        {
            if(isDisabledAll(SlnEvents.PreBuild)) {
                return Codes.Success;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Pre);
            }

            foreach(SBEEvent item in SlnEvents.PreBuild)
            {
                if(hasExecutionOrder(item)) {
                    Log.Info("[Pre] SBE has deferred action: '{0}' :: waiting... ", item.Caption);
                    Status._.add(SolutionEventType.Pre, StatusType.Deferred);
                }
                else {
                    Status._.add(SolutionEventType.Pre, (execPre(item) == Codes.Success)? StatusType.Success : StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Pre, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        /// <summary>
        /// Binding 'POST' of Solution
        /// </summary>
        public int bindPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            SBEEvent[] evt = SlnEvents.PostBuild;

            if(isDisabledAll(evt)) {
                return Codes.Success;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Post);
            }

            foreach(SBEEvent item in evt)
            {
                if(fSucceeded != 1 && item.IgnoreIfBuildFailed) {
                    Log.Info("[Post] ignored action '{0}' :: Build FAILED. See option 'Ignore if the build failed'", item.Caption);
                    continue;
                }

                if(!isReached(item)) {
                    Log.Info("[Post] ignored action '{0}' ::  not reached selected projects in execution order", item.Caption);
                    continue;
                }

                try {
                    if(Cmd.exec(item, SolutionEventType.Post)) {
                        Log.Info($"[Post] finished '{item.Name}': {item.Caption}");
                    }
                    Status._.add(SolutionEventType.Post, StatusType.Success);
                }
                catch(Exception ex) {
                    Log.Error("Post-Build error: {0}", ex.Message);
                    Status._.add(SolutionEventType.Post, StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Post, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        /// <summary>
        /// Binding 'PRE' of Projects
        /// </summary>
        public int bindProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            onProject(pHierProj, ExecutionOrderType.Before);
            return Codes.Success;
        }

        /// <summary>
        /// Binding 'PRE' of Projects
        /// </summary>
        public int bindProjectPre(string project)
        {
            onProject(project, ExecutionOrderType.Before);
            return Codes.Success;
        }

        /// <summary>
        /// Binding 'POST' of Projects
        /// </summary>
        public int bindProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            onProject(pHierProj, ExecutionOrderType.After, fSuccess == 1 ? true : false);
            return Codes.Success;
        }

        /// <summary>
        /// Binding 'POST' of Projects
        /// </summary>
        public int bindProjectPost(string project, int fSuccess)
        {
            onProject(project, ExecutionOrderType.After, fSuccess == 1 ? true : false);
            return Codes.Success;
        }

        /// <summary>
        /// Binding 'Cancel/Abort' of Solution
        /// </summary>
        public int bindCancel()
        {
            SBEEvent[] evt = SlnEvents.CancelBuild;

            if(isDisabledAll(evt)) {
                return Codes.Success;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.Cancel);
            }

            foreach(SBEEvent item in evt)
            {
                if(!isReached(item)) {
                    Log.Info("[Cancel] ignored action '{0}' :: not reached selected projects in execution order", item.Caption);
                    continue;
                }

                try {
                    if(Cmd.exec(item, SolutionEventType.Cancel)) {
                        Log.Info($"[Cancel] finished '{item.Name}': {item.Caption}");
                    }
                    Status._.add(SolutionEventType.Cancel, StatusType.Success);
                }
                catch(Exception ex) {
                    Log.Error("Cancel-Build error: {0}", ex.Message);
                    Status._.add(SolutionEventType.Cancel, StatusType.Fail);
                }
            }
            return Status._.contains(SolutionEventType.Cancel, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        /// <summary>
        /// Binding 'Sln-Opened'
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int bindSlnOpened()
        {
            return bindSln(SlnEvents.SlnOpened, SolutionEventType.SlnOpened);
        }

        /// <summary>
        /// Binding 'Sln-Closed'
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int bindSlnClosed()
        {
            return bindSln(SlnEvents.SlnClosed, SolutionEventType.SlnClosed);
        }

        /// <summary>
        /// Full process of building.
        /// </summary>
        /// <param name="data">Raw data</param>
        public void bindBuildRaw(string data)
        {
            bindBuildRaw(data, GuidList.OWP_BUILD_STRING);
        }

        /// <summary>
        /// Full process of building.
        /// </summary>
        /// <param name="data">Raw data</param>
        /// <param name="guid">Guid string of pane</param>
        /// <param name="item">Name of item pane</param>
        public void bindBuildRaw(string data, string guid, string item = null)
        {
            OWPItems._.getEW(new OWPIdent() { guid = guid, item = item ?? Settings._.DefaultOWPItem }).updateRaw(data); //TODO:
            if(!IsAllowActions)
            {
                if(!isDisabledAll(SlnEvents.Transmitter)) {
                    _ignoredAction(SolutionEventType.Transmitter);
                }
                else if(!isDisabledAll(SlnEvents.WarningsBuild)) {
                    _ignoredAction(SolutionEventType.Warnings);
                }
                else if(!isDisabledAll(SlnEvents.ErrorsBuild)) {
                    _ignoredAction(SolutionEventType.Errors);
                }
                else if(!isDisabledAll(SlnEvents.OWPBuild)) {
                    _ignoredAction(SolutionEventType.OWP);
                }
                return;
            }

            //TODO: IStatus

            foreach(ITransmitter evt in SlnEvents.Transmitter)
            {
                if(!isExecute(evt, current)) {
                    Log.Info("[Transmitter] ignored action '{0}' :: by execution order", evt.Caption);
                }
                else {
                    try {
                        if(Cmd.exec(evt, SolutionEventType.Transmitter)) {
                            //Log.Trace("[Transmitter]: " + SlnEvents.transmitter.caption);
                        }
                    }
                    catch(Exception ex) {
                        Log.Error("Transmitter error: {0}", ex.Message);
                    }
                }
            }

            //TODO: ExecStateType

            foreach(ISolutionEventEW evt in SlnEvents.WarningsBuild) {
                if(evt.Enabled) {
                    sbeEW(evt, Receiver.Output.EWType.Warnings);
                }
            }

            foreach(ISolutionEventEW evt in SlnEvents.ErrorsBuild) {
                if(evt.Enabled) {
                    sbeEW(evt, Receiver.Output.EWType.Errors);
                }
            }

            foreach(ISolutionEventOWP evt in SlnEvents.OWPBuild) {
                if(evt.Enabled) {
                    sbeOutput(evt, ref data, guid, item);
                }
            }
        }

        /// <summary>
        /// Binding of the execution Command ID for EnvDTE /Before.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <param name="cancelDefault">Whether the command has been cancelled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int bindCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            return commandEvent(true, guid, id, customIn, customOut, ref cancelDefault);
        }

        /// <summary>
        /// Binding of the execution Command ID for EnvDTE /After.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int bindCommandDtePost(string guid, int id, object customIn, object customOut)
        {
            bool cancelDefault = false;
            return commandEvent(false, guid, id, customIn, customOut, ref cancelDefault);
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

        public Binder(ICommand cmd, ISobaCLoader loader)
        {
            Cmd     = cmd ?? throw new ArgumentNullException(nameof(cmd));
            cLoader = loader ?? throw new ArgumentNullException(nameof(loader));

            projects = new Dictionary<string, EOProject>();
            attachLoggingEvent();
        }

        /// <summary>
        /// Entry point to Errors/Warnings
        /// </summary>
        protected int sbeEW(ISolutionEventEW evt, Receiver.Output.EWType type)
        {
            Receiver.Output.IItemEW info = OWPItems._.getEW(new OWPIdent() { guid = GuidList.OWP_BUILD_STRING });

            // TODO: capture code####, message..
            if(!info.checkRule(type, evt.IsWhitelist, evt.Codes)) {
                return Codes.Success;
            }

            if(!isExecute(evt, current)) {
                Log.Info("[{0}] ignored action '{1}' :: by execution order", type, evt.Caption);
                return Codes.Success;
            }

            try {
                if(Cmd.exec(evt, (type == Receiver.Output.EWType.Warnings)? SolutionEventType.Warnings : SolutionEventType.Errors)) {
                    Log.Info($"[{type}] finished '{evt.Name}': {evt.Caption}");
                }
                return Codes.Success;
            }
            catch(Exception ex) {
                Log.Error("SBE '{0}' error: {1}", type, ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// Entry point to the OWP
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="raw"></param>
        /// <param name="guid">Guid string of pane</param>
        /// <param name="item">Name of item pane</param>
        protected int sbeOutput(ISolutionEventOWP evt, ref string raw, string guid, string item)
        {
            if(!(new Receiver.Output.Matcher()).match(evt.Match, raw, guid, item)) {
                return Codes.Success;
            }

            if(!isExecute(evt, current)) {
                Log.Info("[Output] ignored action '{0}' :: by execution order", evt.Caption);
                return Codes.Success;
            }

            try {
                if(Cmd.exec(evt, SolutionEventType.OWP)) {
                    Log.Info($"[Output] finished '{evt.Name}': {evt.Caption}");
                }
                return Codes.Success;
            }
            catch(Exception ex) {
                Log.Error("SBE 'Output' error: {0}", ex.Message);
            }
            return Codes.Failed;
        }

        /// <param name="pre">Flag of Before/After execution.</param>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <param name="cancelDefault">Whether the command has been cancelled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        protected int commandEvent(bool pre, string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            if(SlnEvents == null) { // activation of this event type can be before opening solution
                return Codes.Failed;
            }
            ICommandEvent[] evt = SlnEvents.CommandEvent;

            if(isDisabledAll(evt)) {
                return Codes.Success;
            }

            if(!IsAllowActions) {
                return _ignoredAction(SolutionEventType.CommandEvent);
            }

            foreach(ICommandEvent item in evt)
            {
                if(item.Filters == null) {
                    // well, should be some protection for user if we will listen all events... otherwise we can lose control
                    continue;
                }

                var Is = item.Filters.Where(f => 
                            (
                              ((pre && f.Pre) || (!pre && f.Post))
                                && 
                                (
                                  (f.Id == id && f.Guid != null && f.Guid.CompareGuids(guid))
                                   && 
                                   (
                                     (
                                       (f.CustomIn != null && f.CustomIn.EqualsMixedObjects(customIn))
                                       || (f.CustomIn == null && customIn.IsNullOrEmptyString())
                                     )
                                     &&
                                     (
                                       (f.CustomOut != null && f.CustomOut.EqualsMixedObjects(customOut))
                                       || (f.CustomOut == null && customOut.IsNullOrEmptyString())
                                     )
                                   )
                                )
                            )).Select(f => f.Cancel);

                if(Is.Count() < 1) {
                    continue;
                }

                Log.Trace("[CommandEvent] catched: '{0}', '{1}', '{2}', '{3}', '{4}' /'{5}'",
                                                        guid, id, customIn, customOut, cancelDefault, pre);

                commandEvent(item);

                if(pre && Is.Any(f => f)) {
                    cancelDefault = true;
                    Log.Info("[CommandEvent] original command has been canceled for action: '{0}'", item.Caption);
                }
            }
            return Status._.contains(SolutionEventType.CommandEvent, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        protected void commandEvent(ICommandEvent item)
        {
            try
            {
                if(Cmd.exec(item, SolutionEventType.CommandEvent)) {
                    Log.Info("[CommandEvent] finished: '{0}'", item.Caption);
                }
                Status._.add(SolutionEventType.CommandEvent, StatusType.Success);
            }
            catch(Exception ex) {
                Log.Error("CommandEvent error: '{0}'", ex.Message);
            }
            Status._.add(SolutionEventType.CommandEvent, StatusType.Fail);
        }

        protected int bindSln(SBEEvent[] evt, SolutionEventType type)
        {
            if(isDisabledAll(evt)) {
                return Codes.Success;
            }

            if(!IsAllowActions) {
                return _ignoredAction(type);
            }

            string typeString = SolutionEventType.SlnOpened.ToString();
            foreach(SBEEvent item in evt)
            {
                try {
                    if(Cmd.exec(item, type)) {
                        Log.Info($"[{typeString}] finished '{item.Name}': {item.Caption}");
                    }
                    Status._.add(type, StatusType.Success);
                }
                catch(Exception ex) {
                    Log.Error("[{0}] error: `{1}`", typeString, ex.Message);
                    Status._.add(type, StatusType.Fail);
                }
            }

            return Status._.contains(type, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        /// <summary>
        /// Immediately execute 'PRE' of Solution
        /// </summary>
        protected int execPre(SBEEvent evt)
        {
            try {
                if(Cmd.exec(evt, SolutionEventType.Pre)) {
                    Log.Info($"[Pre] finished '{evt.Name}': {evt.Caption}");
                }
                return Codes.Success;
            }
            catch(Exception ex) {
                Log.Error("Pre-Build error: {0}", ex.Message);
            }
            return Codes.Failed;
        }

        protected int execPre()
        {
            int idx = 0;
            foreach(SBEEvent item in SlnEvents.PreBuild) {
                if(hasExecutionOrder(item)) {
                    Status._.update(SolutionEventType.Pre, idx, (execPre(item) == Codes.Success)? StatusType.Success : StatusType.Fail);
                }
                ++idx;
            }
            return Status._.contains(SolutionEventType.Pre, StatusType.Fail)? Codes.Failed : Codes.Success;
        }

        protected string getProjectName(IVsHierarchy pHierProj)
        {
            string projectName = ((IEnvironmentExt)Cmd.Env).getProjectNameFrom(pHierProj, true);
            if(!String.IsNullOrEmpty(projectName)) {
                return projectName;
            }

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivshierarchy.getproperty.aspx
            // http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.__vshpropid.aspx
            pHierProj.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_Name, out object name);

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
            Log.Debug("hasExecutionOrder(->isReached) for '{0}' is true", evt.Caption);

            return evt.ExecutionOrder.Any(e =>
                                            projects.ContainsKey(e.Project)
                                               && projects[e.Project].Order == e.Order
                                          );
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
            Log.Debug("hasExecutionOrder(->isExecute) for '{0}' is true", evt.Caption);

            foreach(IExecutionOrder eo in evt.ExecutionOrder)
            {
                if(!projects.ContainsKey(eo.Project)) {
                    continue;
                }

                // The 'Before' base:
                if(eo.Order == ExecutionOrderType.Before)
                {
                    // Before1 -> After1
                    return (projects[eo.Project].Order == ExecutionOrderType.Before);
                }

                // The 'After' base:
                if(projects[eo.Project].Order != ExecutionOrderType.After) {
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
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            onProject(getProjectName(pHierProj), type, fSuccess);
        }

        protected void onProject(string project, ExecutionOrderType type, bool fSuccess = true)
        {
            lock(_plock)
            {
                var eop = new EOProject() { Project = project, Order = type };

                int max         = Cmd.Env.ProjectsDTE.Count();
                var _projects   = projects.Where(p => !ExecutionOrder.IsSpecial(p.Key));
                int count       = _projects.Count();

                // 'First Project' & 'First Type' Before
                if(count < 1) {
                    eop.aProject    = ExecutionOrder.FIRST_PROJECT;
                    eop.aType       = ExecutionOrder.FIRST_TYPE;
                }
                // 'Last Project' & 'Last Type' Before
                else if(!projects.ContainsKey(project) && count + 1 == max) {
                    eop.aProject    = ExecutionOrder.LAST_PROJECT;
                    eop.aType       = ExecutionOrder.LAST_TYPE;
                }
                else
                {
                    // 'First Project' After
                    if(projects.ContainsKey(project) && projects[project].aProject == ExecutionOrder.FIRST_PROJECT) {
                        eop.aProject = ExecutionOrder.FIRST_PROJECT;
                    }
                    // 'Last Project' After
                    else if(projects.ContainsKey(project) && projects[project].aProject == ExecutionOrder.LAST_PROJECT) {
                        eop.aProject = ExecutionOrder.LAST_PROJECT;
                    }

                    // 'First Type' After
                    if(type == ExecutionOrderType.After && !projects.Any(p => p.Value.Order == ExecutionOrderType.After)) {
                        eop.aType = ExecutionOrder.FIRST_TYPE;
                    }
                    // 'Last Type' After
                    else if(type == ExecutionOrderType.After && count == max) {
                        var list = _projects.Where(p => p.Value.Order == ExecutionOrderType.Before);
                        if(list.Count() == 1 && list.FirstOrDefault().Key == project) {
                            eop.aType = ExecutionOrder.LAST_TYPE;
                        }
                    }
                }

                projects[project]   = eop;
                current.Project     = project;
                current.Order       = type;

                if(eop.aProject != null) {
                    projects[eop.aProject] = eop; // alias to 'First/Last Project'
                }

                if(eop.aType != null) {
                    projects[eop.aType] = eop; // alias to 'First/Last Type'
                }
            }

            Log.Trace($"onProject: '{project}'({projects[project].aProject}/{projects[project].aType}):{type} == {fSuccess}");

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
            SBEEvent[] evt = SlnEvents.PreBuild;
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
                    Log.Info("[PRE] ignored action '{0}' :: Build FAILED. See option 'Ignore if the build failed'", evt[i].Caption);
                    continue;
                }

                if(!hasExecutionOrder(evt[i])) {
                    Log.Trace("[PRE] deferred: executionOrder is null or not contains elements :: {0}", evt[i].Caption);
                    return;
                }

                if(evt[i].ExecutionOrder.Any(o => projects.ContainsKey(o.Project)
                                                    && projects[o.Project].Project == project
                                                    && o.Order == type))
                {
                    Log.Info("Incoming '{0}'({1}) :: Execute deferred action: '{2}'", project, type, evt[i].Caption);
                    Status._.update(SolutionEventType.Pre, i, (execPre(evt[i]) == Codes.Success)? StatusType.Success : StatusType.Fail);
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

        protected void attachLoggingEvent()
        {
            lock(sync) {
                detachLoggingEvent();
                Log._.Received += onLogging;
            }
        }

        protected void detachLoggingEvent()
        {
            Log._.Received -= onLogging;
        }
        
        /// <summary>
        /// Works with all processes of internal logging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLogging(object sender, MessageArgs e)
        {
            if(SlnEvents == null) {
                return; // can be early initialization
            }

            if(Thread.CurrentThread.Name == LoggingEvent.IDENT_TH) {
                return; // self protection
            }

            if(isDisabledAll(SlnEvents.Logging)) {
                return;
            }

            if(!IsAllowActions) {
                _ignoredAction(SolutionEventType.Logging);
                return;
            }

            (new Task(() =>
            {
                if(Thread.CurrentThread.Name == null) {
                    Thread.CurrentThread.Name = LoggingEvent.IDENT_TH;
                }

                lock(sync)
                {
                    var ld = cLoader.GetComponent(typeof(OwpComponent)) as ILogInfo;
                    ld?.UpdateLogInfo(e.Message, e.Level);

                    foreach(LoggingEvent evt in SlnEvents.Logging)
                    {
                        if(!isExecute(evt, current)) {
                            Log.Info("[Logging] ignored action '{0}' :: by execution order", evt.Caption);
                            continue;
                        }

                        try
                        {
                            if(Cmd.exec(evt, SolutionEventType.Logging)) {
                                Log.Trace("[Logging]: " + evt.Caption);
                            }
                        }
                        catch(Exception ex) {
                            Log.Error("LoggingEvent error: {0}", ex.Message);
                        }
                    }
                }

            })).Start();
        }

        private int _ignoredAction(SolutionEventType type)
        {
            Log.Trace("[{0}] Ignored action. It's already started in other processes of VS.", type);
            return Codes.Success;
        }
    }
}
