/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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

#if SDK15_OR_HIGH
using Microsoft.VisualStudio.Shell;
#endif

namespace net.r_eg.vsSBE.API
{
    using AppSettings = vsSBE.Settings;

    /// <remarks>
    /// TODO: add events for client library instead of direct call
    /// </remarks>
    public class EventLevel: IEventLevel, IEntryPointCore, IFireCoreCommand
    {
        internal readonly CancelBuildState buildState = new CancelBuildState();

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

        private readonly object sync = new object();

        public event CoreCommandHandler CoreCommand = delegate (object sender, CoreCommandArgs e) { };

        public event EventHandler OpenedSolution = delegate (object sender, EventArgs e) { };

        public event EventHandler ClosedSolution = delegate (object sender, EventArgs e) { };

        public Actions.Binder Action { get; protected set; }

        public IEnvironment Environment { get; protected set; }

        public ConfManager Config => AppSettings._.Config;

        public void load(object dte2, bool debug = false)
        {
            load(dte2, new API.Settings() { DebugMode = debug });
        }

        public void load(object dte2, ISettings cfg)
        {
            configure(cfg);
            
            Environment = new Environment((DTE2)dte2, this);
            init();

            clientLib.tryLoad(this, dte2);
        }

        /// <inheritdoc/>
        public void load(string sln, Dictionary<string, string> properties, bool debug = false)
        {
            load(sln, properties, new API.Settings() { DebugMode = debug });
        }

        /// <inheritdoc/>
        public void load(string sln, Dictionary<string, string> properties, ISettings cfg)
        {
            configure(cfg);

            Environment = new IsolatedEnv(sln, properties);
            init();

            clientLib.tryLoad(this, sln, properties);
        }

        /// <inheritdoc/>
        public int onPre(ref int pfCancelUpdate)
        {
            buildState.Reset();
            try
            {
                return mixup
                (
                    Action.bindPre(ref pfCancelUpdate),
                    clientLib.Event.onPre(ref pfCancelUpdate)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Solution.Pre-binding: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            finally
            {
                buildState.UpdateFlagIfCanceled(ref pfCancelUpdate);
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onCancel()
        {
            try
            {
                return mixup
                (
                    Action.bindCancel(),
                    clientLib.Event.onCancel()
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Solution.Cancel-binding: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            try
            {
                int ret = Action.bindPost(fSucceeded, fModified, fCancelCommand);
                if(Action.reset()) {
                    loader.UVars.UnsetAll();
                }

                return mixup(ret, clientLib.Event.onPost(fSucceeded, fModified, fCancelCommand));
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Solution.Post-binding: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel)
        {
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            try
            {
                return mixup
                (
                    Action.bindProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel),
                    clientLib.Event.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Project.Pre-binding: {ex.Message}");
                Log.Debug(ex.StackTrace); // to an unclear issue #43
            }
            finally
            {
                buildState.UpdateFlagIfCanceled(ref pfCancel);
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onProjectPre(string project)
        {
            try
            {
                return mixup
                (
                    Action.bindProjectPre(project),
                    clientLib.Event.onProjectPre(project)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Project.Pre-binding/simple: {ex.Message}");
                Log.Debug(ex.StackTrace); // to an unclear issue #43
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            try
            {
                return mixup
                (
                    Action.bindProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel),
                    clientLib.Event.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Project.Post-binding: {ex.Message}");
                Log.Debug(ex.StackTrace); // to an unclear issue #43
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onProjectPost(string project, int fSuccess)
        {
            try
            {
                return mixup
                (
                    Action.bindProjectPost(project, fSuccess),
                    clientLib.Event.onProjectPost(project, fSuccess)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Project.Post-binding/simple: {ex.Message}");
                Log.Debug(ex.StackTrace); // to an unclear issue #43
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            try
            {
                return mixup
                (
                    Action.bindCommandDtePre(guid, id, customIn, customOut, ref cancelDefault),
                    clientLib.Event.onCommandDtePre(guid, id, customIn, customOut, ref cancelDefault)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed EnvDTE.Command-binding/Before: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            return Codes.Failed;
        }

        /// <inheritdoc/>
        public int onCommandDtePost(string guid, int id, object customIn, object customOut)
        {
            try
            {
                return mixup
                (
                    Action.bindCommandDtePost(guid, id, customIn, customOut),
                    clientLib.Event.onCommandDtePost(guid, id, customIn, customOut)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed EnvDTE.Command-binding/After: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            return Codes.Failed;
        }

        //TODO: (string data, string guid, string item)
        public void onBuildRaw(string data)
        {
            if(data == null) return; // L-704

            try
            {
                Action.bindBuildRaw(data);

                clientLib.Build.onBuildRaw(data);
            }
            catch(Exception ex)
            {
                Log.Error($"Failed build-raw: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            var config      = AppSettings._.Config.Sln;
            var userConfig  = AppSettings._.Config.Usr;

            string path = Environment.SolutionPath;
            AppSettings._.setWorkPath(path);

            bool isNew = !config.loadPath(path, Environment.SolutionFileName);
            userConfig.load(config.Link);

            refreshComponents();

            UI.Plain.State.Print(config.Data);

            initPropByDefault(Action.Cmd.MSBuild); //LC: #815, #814
            buildState.Reset();
            OpenedSolution(this, EventArgs.Empty);

            if(slnEvents == null) {
                updateBuildType(BuildType.Common);
                return slnOpened(pUnkReserved, fNewSolution);
            }

            try
            {
                // Early Sln-Opened ~ Before initializing projects
                updateBuildType(BuildType.Before);
                return slnOpened(pUnkReserved, fNewSolution);
            }
            finally
            {
                // Late Sln-Opened (delay calling) ~ When all projects are opened in IDE
                lock(sync) {
                    slnEvents.Opened -= slnOpenedLowPriority;
                    slnEvents.Opened += slnOpenedLowPriority;
                }
            }
        }

        /// <inheritdoc/>
        public int solutionClosed(object pUnkReserved)
        {
            int ret;
            try
            {
                ret = mixup
                (
                    Action.bindSlnClosed(),
                    clientLib.Event.solutionClosed(pUnkReserved)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Solution.SlnClosed-binding: {ex.Message}");
                Log.Debug(ex.StackTrace);
                ret = Codes.Failed;
            }

            ClosedSolution(this, EventArgs.Empty);

            AppSettings._.Config.Sln.unload();
            AppSettings._.Config.Usr.unload();
            return ret;
        }

        /// <inheritdoc/>
        public void updateBuildType(BuildType type)
        {
            if(Environment != null) {
                Environment.BuildType = type;
            }
            
            if(clientLib != null && clientLib.Build != null) {
                clientLib.Build.updateBuildType(type);
            }
        }

        /// <inheritdoc/>
        public void fire(CoreCommandArgs c) => CoreCommand(this, c);

        public EventLevel()
        {
            CoreCommand += (object sender, CoreCommandArgs e) =>
            {
                if(e.Type == CoreCommandType.BuildCancel) buildState.Cancel();
            };
        }

        /// <summary>
        /// Initialize level
        /// </summary>
        protected void init()
        {
#if SDK17
            Log.Info($"SDK17 & {vsSBE.Version.S_INFO}");
#elif SDK15
            Log.Info($"SDK15 & {vsSBE.Version.S_INFO}");
#else
            Log.Info($"SDK10 & {vsSBE.Version.S_INFO}");
#endif

#if DEBUG
            Log.Warn($"Debug version");
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
                loader.Soba,
                buildState
            );
        }

        protected int mixup(int main, int client) 
            => (main == Codes.Success && client == Codes.Success) ? Codes.Success : Codes.Failed;

        /// <summary>
        /// Defines configuration with ISettings
        /// </summary>
        /// <param name="cfg"></param>
        protected void configure(ISettings cfg)
        {
            AppSettings._.DebugMode = cfg.DebugMode;

            (new Logger.Initializer()).configure();

            // TODO: event with common settings for IEntryPointCore
            AppSettings._.DebugModeUpdated -= onDebugModeUpdated;
            AppSettings._.DebugModeUpdated += onDebugModeUpdated;
        }

        protected void refreshComponents()
        {
            if(loader == null || !AppSettings._.Config.IsLoadedSln) {
                Log.Debug($"Components activation are ignored due to {AppSettings._.Config}");
                return;
            }

            var data = AppSettings._.Config.Sln.Data;

            foreach(IComponent c in loader.Soba.Registered)
            {
                var found = data.Components?.FirstOrDefault(p => p.ClassName == c.GetType().Name);
                if(found == null)
                {
                    // Each component provides its default state for IComponent.Enabled
                    // We'll just continue 'as is' if this component is not presented in config.
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
            try
            {
                return mixup
                (
                    Action.bindSlnOpened(),
                    clientLib.Event.solutionOpened(pUnkReserved, fNewSolution)
                );
            }
            catch(Exception ex)
            {
                Log.Error($"Failed Solution.SlnOpened-binding: {ex.Message}");
                Log.Debug(ex.StackTrace);
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
            if(Log._.isError(e.Level))
            {
                CoreCommand
                (
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
