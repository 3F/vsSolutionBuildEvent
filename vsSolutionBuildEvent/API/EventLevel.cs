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
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.API.Commands;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Clients;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using System.Linq;

#if VSSDK_15_AND_NEW
using Microsoft.VisualStudio.Shell;
#endif

namespace net.r_eg.vsSBE.API
{
    using AppSettings = vsSBE.Settings;

    /// <summary>
    /// TODO: add events for client library instead of direct call
    /// </summary>
    public class EventLevel: IEventLevel, IEntryPointCore, IFireCoreCommand
    {
        /// <summary>
        /// Event of core commands.
        /// </summary>
        public event CoreCommandHandler CoreCommand = delegate(object sender, CoreCommandArgs e) { };

        /// <summary>
        /// When the solution has been opened
        /// </summary>
        public event EventHandler OpenedSolution = delegate(object sender, EventArgs e) { };

        /// <summary>
        /// When the solution has been closed
        /// </summary>
        public event EventHandler ClosedSolution = delegate(object sender, EventArgs e) { };

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        protected EnvDTE.CommandEvents cmdEvents;

        /// <summary>
        /// The low priority level of the solution events.
        /// </summary>
        protected EnvDTE.SolutionEvents slnEvents;

        /// <summary>
        /// Access to client library
        /// </summary>
        protected IClientLibrary clientLib = new ClientLibrary();

        private Bootloader loader;

        /// <summary>
        /// object synch.
        /// </summary>
        private readonly object sync = new object();

        /// <summary>
        /// Binder of action
        /// </summary>
        public Actions.Binder Action
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
        /// Manager of configurations.
        /// </summary>
        public IManager ConfigManager
        {
            get {
                return AppSettings.CfgManager;
            }
        }

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="debug">Optional flag of debug mode</param>
        public void load(object dte2, bool debug = false)
        {
            load(dte2, new API.Settings() { DebugMode = debug });
        }

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="cfg">Specific settings</param>
        public void load(object dte2, ISettings cfg)
        {
            configure(cfg);
            
            this.Environment = new Environment((DTE2)dte2, this);
            init();

            clientLib.tryLoad(this, dte2);
        }

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="debug">Optional flag of debug mode</param>
        public void load(string sln, Dictionary<string, string> properties, bool debug = false)
        {
            load(sln, properties, new API.Settings() { DebugMode = debug });
        }

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="cfg">Specific settings</param>
        public void load(string sln, Dictionary<string, string> properties, ISettings cfg)
        {
            configure(cfg);

            this.Environment = new IsolatedEnv(sln, properties);
            init();

            clientLib.tryLoad(this, sln, properties);
        }

        /// <summary>
        /// 'PRE' of the solution.
        /// Called before any build actions have begun.
        /// </summary>
        /// <param name="pfCancelUpdate">Pointer to a flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onPre(ref int pfCancelUpdate)
        {
            try {
                int ret = Action.bindPre(ref pfCancelUpdate);

                clientLib.Event.onPre(ref pfCancelUpdate);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Solution.Pre-binding: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// Called when a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCancel()
        {
            try {
                int ret = Action.bindCancel();

                clientLib.Event.onCancel();
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Solution.Cancel-binding: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'POST' of the solution.
        /// Called when a build is completed.
        /// </summary>
        /// <param name="fSucceeded">true if no update actions failed.</param>
        /// <param name="fModified">true if any update action succeeded.</param>
        /// <param name="fCancelCommand">true if update actions were canceled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            try
            {
                int ret = Action.bindPost(fSucceeded, fModified, fCancelCommand);
                if(Action.reset()) {
                    loader.UVars.UnsetAll();
                }

                clientLib.Event.onPost(fSucceeded, fModified, fCancelCommand);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Solution.Post-binding: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            try {
                int ret = Action.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);

                clientLib.Event.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Project.Pre-binding: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPre(string project)
        {
            try {
                int ret = Action.bindProjectPre(project);

                clientLib.Event.onProjectPre(project);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Project.Pre-binding/simple: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            try {
                int ret = Action.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);

                clientLib.Event.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Project.Post-binding: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPost(string project, int fSuccess)
        {
            try {
                int ret = Action.bindProjectPost(project, fSuccess);

                clientLib.Event.onProjectPost(project, fSuccess);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Project.Post-binding/simple: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// Before executing Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <param name="cancelDefault">Whether the command has been cancelled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            try {
                int ret = Action.bindCommandDtePre(guid, id, customIn, customOut, ref cancelDefault);

                clientLib.Event.onCommandDtePre(guid, id, customIn, customOut, ref cancelDefault);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed EnvDTE.Command-binding/Before: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// After executed Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCommandDtePost(string guid, int id, object customIn, object customOut)
        {
            try {
                int ret = Action.bindCommandDtePost(guid, id, customIn, customOut);

                clientLib.Event.onCommandDtePost(guid, id, customIn, customOut);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed EnvDTE.Command-binding/After: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// During assembly.
        /// TODO: (string data, string guid, string item)
        /// </summary>
        /// <param name="data">Raw data of building process</param>
        public void onBuildRaw(string data)
        {
            try {
                Action.bindBuildRaw(data);

                clientLib.Build.onBuildRaw(data);
            }
            catch(Exception ex) {
                Log.Error("Failed build-raw: '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            var config      = ConfigManager.Config;
            var userConfig  = ConfigManager.UserConfig;

            if(config == null || userConfig == null) {
                throw new ArgumentException($"Config is not ready for loading. User: {userConfig != null} / Main: {config != null}");
            }

            bool isNew = !config.load(Environment.SolutionPath, Environment.SolutionFileName);
            userConfig.load(config.Link);

            //ConfigManager.addAndUse(config, userConfig, ContextType.Solution);
            refreshComponents();

            UI.Plain.State.Print(config.Data);

            initPropByDefault(Action.Cmd.MSBuild); //LC: #815, #814
            OpenedSolution(this, EventArgs.Empty);

            if(slnEvents == null) {
                updateBuildType(BuildType.Common);
                return slnOpened(pUnkReserved, fNewSolution);
            }

            try {
                // Early Sln-Opened ~ Before initializing projects
                updateBuildType(BuildType.Before);
                return slnOpened(pUnkReserved, fNewSolution);
            }
            finally {
                // Late Sln-Opened (delay calling) ~ When all projects are opened in IDE
                lock(sync) {
                    slnEvents.Opened -= slnOpenedLowPriority;
                    slnEvents.Opened += slnOpenedLowPriority;
                }
            }
        }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionClosed(object pUnkReserved)
        {
            int ret;
            try {
                ret = Action.bindSlnClosed();
                clientLib.Event.solutionClosed(pUnkReserved);
            }
            catch(Exception ex) {
                Log.Error("Failed Solution.SlnClosed-binding: `{0}`", ex.Message);
                ret = Codes.Failed;
            }

            ClosedSolution(this, EventArgs.Empty);

            ConfigManager.Config.unload();
            ConfigManager.UserConfig.unload();
            return ret;
        }

        /// <summary>
        /// Sets current type of the build
        /// </summary>
        /// <param name="type"></param>
        public void updateBuildType(Bridge.BuildType type)
        {
            if(Environment != null) {
                Environment.BuildType = type;
            }
            
            if(clientLib != null && clientLib.Build != null) {
                clientLib.Build.updateBuildType(type);
            }
        }

        /// <summary>
        /// Send the core command for all clients.
        /// </summary>
        /// <param name="c"></param>
        public void fire(CoreCommandArgs c)
        {
            CoreCommand(this, c);
        }

        /// <summary>
        /// Initialize level
        /// </summary>
        protected void init()
        {
#if DEBUG
            Log.Warn("Used [Debug version]");
#else
            //if(vsSBE.Version.branchName.ToLower() != "releases") {
            //    Log.Warn("Used [Unofficial release]");
            //}
#endif

            loader = Bootloader.Init(Environment);

            if(Environment.Events != null) {
                slnEvents = Environment.Events.SolutionEvents;
            }

            Environment.CoreCmdSender = this;
            attachCommandEvents();

            Log._.Received -= onLogReceived;
            Log._.Received += onLogReceived;

            //TODO: extract all below into new methods. It's valuable for CoreCommand etc.
            //+ do not forget about ClientLibrary, Provider, etc.

            Action = new Actions.Binder
            (
                new Actions.Command
                (
                    Environment,
                    loader.Soba,
                    loader.Soba.EvMSBuild
                ),
                loader.Soba
            );
        }

        /// <summary>
        /// Defines configuration with ISettings
        /// </summary>
        /// <param name="cfg"></param>
        protected void configure(ISettings cfg)
        {
            ConfigManager.addAndUse(new Config(), new UserConfig(), ContextType.Static); //TODO: Solution & Common context
            (new Logger.Initializer()).configure();

            // TODO: event with common settings for IEntryPointCore
            AppSettings._.DebugModeUpdated -= onDebugModeUpdated;
            AppSettings._.DebugModeUpdated += onDebugModeUpdated;

            AppSettings._.DebugMode = cfg.DebugMode;
            // ...
        }

        protected void refreshComponents()
        {
            if(loader == null || AppSettings.CfgManager.Config == null) {
                Log.Debug("Changing of activation has been ignored.");
                return;
            }

            var data = AppSettings.CfgManager.Config.Data;

            foreach(IComponent c in loader.Soba.Registered) {
                if(data.Components == null || data.Components.Length < 1) {
                    //c.Enabled = true;
                    continue;
                }

                var found = data.Components.Where(p => p.ClassName == c.GetType().Name).FirstOrDefault();
                if(found == null) {
                    continue;
                }

#if DEBUG
                if(c.Enabled != found.Enabled) {
                    Log.Trace($"Bootloader - Component '{found.ClassName}': Changing of activation status '{c.Enabled}' -> '{found.Enabled}'");
                }
#endif
                c.Enabled = found.Enabled;
            }
        }

        /// <summary>
        /// To initialize properties by default for project.
        /// </summary>
        protected virtual void initPropByDefault(IEvMSBuild msbuild)
        {
            IAppSettings app = AppSettings._;
            const string _PFX = AppSettings.APP_NAME_SHORT;

            msbuild.SetGlobalProperty(AppSettings.APP_NAME, vsSBE.Version.S_NUM_REV);
            msbuild.SetGlobalProperty($"{_PFX}_CommonPath", app.CommonPath);
            msbuild.SetGlobalProperty($"{_PFX}_LibPath", app.LibPath);
            msbuild.SetGlobalProperty($"{_PFX}_WorkPath", app.WorkPath);
        }

        protected void attachCommandEvents()
        {
            if(Environment.Events == null) {
                Log.Info("Context of build action: uses a limited types.");
                return; //this can be for emulated DTE2 context
            }

            cmdEvents = Environment.Events.CommandEvents; // protection from garbage collector
            lock(sync) {
                detachCommandEvents();
                cmdEvents.BeforeExecute += _cmdBeforeExecute;
                cmdEvents.AfterExecute  += _cmdAfterExecute;
            }
        }

        protected void detachCommandEvents()
        {
            if(cmdEvents == null) {
                return;
            }

            lock(sync) {
                cmdEvents.BeforeExecute -= _cmdBeforeExecute;
                cmdEvents.AfterExecute  -= _cmdAfterExecute;
            }
        }

        /// <summary>
        /// The low priority handler for work only when all projects are opened.
        /// Mainly we work with priority ( http://stackoverflow.com/q/27018762 ) so this should be handled after the all 'OnAfterOpenProject' etc. as non-priority 'IVsSolutionEvents.OnAfterOpenSolution'
        /// </summary>
        private void slnOpenedLowPriority()
        {
            lock(sync)
            {
                if(slnEvents != null) {
                    slnEvents.Opened -= slnOpenedLowPriority;
                }
                updateBuildType(BuildType.After);
                slnOpened(new object(), 0); //TODO: use from solutionOpened(object pUnkReserved, int fNewSolution)
            }
        }

        private int slnOpened(object pUnkReserved, int fNewSolution)
        {
            try {
                int ret = Action.bindSlnOpened();
                clientLib.Event.solutionOpened(pUnkReserved, fNewSolution);
                return ret;
            }
            catch(Exception ex) {
                Log.Error("Failed Solution.SlnOpened-binding: `{0}`", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// TODO: event with common settings for IEntryPointCore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDebugModeUpdated(object sender, DataArgs<bool> e)
        {
            fire(new CoreCommandArgs()
            { 
                Type = CoreCommandType.RawCommand,
                Args = new object[] { new KeyValuePair<string, bool>("DebugMode", e.Data) }
            });
        }

        private void onLogReceived(object sender, Logger.MessageArgs e)
        {
            if(Log._.isError(e.Level)) {
                CoreCommand(
                    sender, 
                    new CoreCommandArgs() {
                        Type = CoreCommandType.BuildCancel
                    }
                );
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
            onCommandDtePre(guidString, id, customIn, customOut, ref cancelDefault);

            Guid guid = new Guid(guidString);
            if(GuidList.VSStd97CmdID != guid && GuidList.VSStd2KCmdID != guid) {
                return;
            }

            if(UnifiedTypes.Build.VSCommand.existsById(id)) {
                updateBuildType(UnifiedTypes.Build.VSCommand.getByCommandId(id));
            }
        }

        private void _cmdAfterExecute(string guid, int id, object customIn, object customOut)
        {
            onCommandDtePost(guid, id, customIn, customOut);
        }
    }
}
