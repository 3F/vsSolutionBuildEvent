/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using net.r_eg.MvsSln;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Provider;
using BuildType = net.r_eg.vsSBE.Bridge.BuildType;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    public class EventManager: Logger, ILogger
    {
        /// <summary>
        /// Full path to solution file
        /// </summary>
        public string SolutionFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Our the vsSolutionBuildEvent library
        /// </summary>
        public ILibrary library;

        /// <summary>
        /// Internal logging
        /// </summary>
        internal ILog log;

        /// <summary>
        /// Initializer of library
        /// </summary>
        internal Initializer initializer;

        /// <summary>
        /// Specified type of build action
        /// </summary>
        protected volatile BuildType buildType = BuildType.Common;

        /// <summary>
        /// About projects by ident
        /// </summary>
        protected IDictionary<int, Project> projects = new Dictionary<int, Project>();

        /// <summary>
        /// All received CoreCommand from library.
        /// </summary>
        protected Stack<ICoreCommand> receivedCommands = new Stack<ICoreCommand>();

        /// <summary>
        /// List of started targets and their Pre/Post states.
        /// </summary>
        private ConcurrentDictionary<int, bool> ptargets = new ConcurrentDictionary<int, bool>();

        /// <summary>
        /// Reserved for future use with IVsSolutionEvents
        /// </summary>
        private readonly object pUnkReserved = new object();

        /// <summary>
        /// To abort all processes as soon as possible
        /// </summary>
        private volatile bool abort = false;

        private readonly object _sync = new object();

        /// <summary>
        /// Initializer of the Build.Framework.ILogger objects.
        /// Subscribes to specific events etc.
        /// </summary>
        /// <param name="evt"></param>
        public override void Initialize(IEventSource evt)
        {
            log         = new Log(Verbosity);
            initializer = new Initializer(Parameters, log);

            if(!log.IsDiagnostic) {
                log.info($"set {Log.DIAG_KEY}= true to enable diagnostic mode.");
            }

            // load with properties by default
            library = initializer.load();
            setPropertiesByDefault();

            attachCoreCommandListener(library);
            library.Event.solutionOpened(pUnkReserved, 0);

            // bring song
            evt.TargetStarted   += onTargetStarted;
            evt.ProjectStarted  += onProjectStarted;
            evt.AnyEventRaised  += onAnyEventRaised;
            evt.ErrorRaised     += onErrorRaised;
            evt.WarningRaised   += onWarningRaised;
            evt.BuildStarted    += onBuildStarted;
            evt.BuildFinished   += onBuildFinished;
        }

        public override void Shutdown()
        {
            if(library != null) {
                library.Event.solutionClosed(pUnkReserved);
                detachCoreCommandListener(library);
            }
        }

        protected void onProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            termination(); // This is first place where we can use it

            if(e.ProjectFile.EndsWith(".metaproj", StringComparison.InvariantCultureIgnoreCase)
                || e.ProjectFile.EndsWith(".user", StringComparison.InvariantCultureIgnoreCase))
            {
                debug($"ignored '{e.ProjectFile}'");
                return;
            }

            if(projects.ContainsKey(e.ProjectId)) {
                // already pushed
                return;
            }
            //updateBuildType(e.TargetNames);

            if(e.Properties == null) {
                debug("onProjectStarted: e.Properties is null :: '{0}' ({1})", e.ProjectFile, e.Message);
                return;
            }

            var properties = e.Properties.OfType<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value.ToString());

            if(properties.ContainsKey(PropertyNames.PRJ_NAME))
            {
                projects[e.ProjectId] = new Project() {
                    Name            = properties[PropertyNames.PRJ_NAME],
                    File            = e.ProjectFile,
                    Properties      = properties
                };
            }
        }

        protected void onTargetStarted(object sender, TargetStartedEventArgs e)
        {
            int pid = e.BuildEventContext.ProjectInstanceId;
            if(!projects.ContainsKey(pid)) {
                return;
            }

            // the PreBuildEvent & PostBuildEvent should be only for condition '$(PreBuildEvent)'!='' ...
            switch(e.TargetName)
            {
                case "BeforeBuild":
                case "BeforeRebuild":
                case "BeforeClean":
                {
                    if(!ptargets.ContainsKey(pid)) {
                        ptargets[pid] = false; //pre
                        library.Event.onProjectPre(projects[pid].Name);
                    }
                    break;
                }
                case "AfterBuild":
                case "AfterRebuild":
                case "AfterClean":
                {
                    if(!ptargets.ContainsKey(pid) || !ptargets[pid]) {
                        ptargets[pid] = true; //post
                        library.Event.onProjectPost(projects[pid].Name, projects[pid].HasErrors ? 0 : 1);
                    }                    
                    break;
                }
            }
        }

        protected void onWarningRaised(object sender, BuildWarningEventArgs e)
        {
            if(library != null) {
                library.Build.onBuildRaw(formatEW("warning", e.Code, e.Message, e.File, e.LineNumber));
            }
        }

        protected void onErrorRaised(object sender, BuildErrorEventArgs e)
        {
            if(projects.ContainsKey(e.BuildEventContext.ProjectInstanceId)) {
                projects[e.BuildEventContext.ProjectInstanceId].HasErrors = true;
            }

            if(library != null) {
                library.Build.onBuildRaw(formatEW("error", e.Code, e.Message, e.File, e.LineNumber));
            }
        }

        protected void onPre(string targets)
        {
            updateBuildType(targets);

            int pfCancelUpdate = 0;
            library.Event.onPre(ref pfCancelUpdate);
        }

        /// <summary>
        /// Note with msbuild: 
        /// Be careful - the onBuildStarted(i.e. all before onTargetStarted) is not safe for any unhandled exceptions!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onBuildStarted(object sender, BuildStartedEventArgs e)
        {
            try {
                ptargets.Clear();

                // yes, we're ready
                onPre(initializer.Properties.Targets);
            }
            catch(Exception ex) {
                debug("Error onBuildStarted: '{0}'", ex.Message);
                abort = true;
            }
        }

        protected void onBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            if(library == null) {
                return;
            }

            if(!e.Succeeded) {
                library.Event.onCancel();
            }
            library.Event.onPost((e.Succeeded ? 1 : 0), 0, 0);
        }

        /// <summary>
        /// see also: MessageRaised or StatusEventRaised
        /// </summary>
        protected void onAnyEventRaised(object sender, BuildEventArgs e)
        {
            if(library != null) {
                library.Build.onBuildRaw(e.Message);
            }
        }

        /// <summary>
        /// Handler of core commands.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="c"></param>
        protected void command(object sender, CoreCommandArgs c)
        {
            switch(c.Type)
            {
                case CoreCommandType.AbortCommand: {
                    abortCommand(c);
                    break;
                }
                case CoreCommandType.BuildCancel: {
                    abort = true;
                    break;
                }
                case CoreCommandType.Nop: {
                    break;
                }
                case CoreCommandType.RawCommand: {
                    rawCommand(c);
                    break;
                }
            }
            receivedCommands.Push(c);
        }

        /// <summary>
        /// Aborts latest command if it's possible
        /// </summary>
        /// <param name="c"></param>
        protected void abortCommand(ICoreCommand c)
        {
            if(receivedCommands.Count < 1) {
                return;
            }
            ICoreCommand last = receivedCommands.Peek();

            if(last.Type == CoreCommandType.BuildCancel) {
                abort = false;
            }
        }

        protected void rawCommand(ICoreCommand c)
        {
            if(c.Args.Length < 1) {
                msg("RawCommand is incorrect or broken.");
                return;
            }

            switch(c.Args[0].ToString()) {
                case "property.set": {
                    setProperty(c.Args[1].ToString(), c.Args[2].ToString());
                    return;
                }
                case "property.del": {
                    setProperty(c.Args[1].ToString(), null);
                    return;
                }
            }
        }

        protected bool attachCoreCommandListener(ILibrary library)
        {
            if(library == null) {
                return false;
            }

            lock(_sync) {
                detachCoreCommandListener(library);
                library.EntryPoint.CoreCommand += command;
            }
            return true;
        }

        protected bool detachCoreCommandListener(ILibrary library)
        {
            if(library == null) {
                return false;
            }

            lock(_sync) {
                library.EntryPoint.CoreCommand -= command;
            }
            return true;
        }

        /// <summary>
        /// Updates the type by target name if it exists in BuildType list
        /// https://msdn.microsoft.com/en-us/library/vstudio/ms164311.aspx
        /// </summary>
        /// <param name="targets">each target separately or with a semicolon if this is a multiple targets</param>
        protected void updateBuildType(string targets)
        {
            foreach(string target in targets.Split(';'))
            {
                if(Enum.IsDefined(typeof(BuildType), target.Trim()))
                {
                    buildType = (BuildType)Enum.Parse(typeof(BuildType), target);
                    library.Build.updateBuildType(buildType);
                    debug("updateBuildType: '{0}' from - '{1}'", buildType, targets);
                    return; // use as 'or' for a multiple targets
                }
            }
        }

        /// <summary>
        /// Logic of termination the msbuild job
        /// </summary>
        protected void termination()
        {
            if(!abort) {
                return;
            }

            //TODO: Another way ? fix me
            throw new LoggerException(
                            String.Format(
                                "The build has been canceled{0}.", 
                                (receivedCommands.Count < 1)? "" : " by user script"
                            ));
        }

        protected virtual void setProperty(string name, string value)
        {
            debug("setProperty '{0}' = `{1}`", name, value);
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
        }

        /// <summary>
        /// Note: 
        /// The main core is already provides properties by default,
        /// but all this are not available for msbuild targets from CIM.
        /// Thus, we need also provide this via current environment etc.
        /// 
        /// FIXME:
        /// </summary>
        protected void setPropertiesByDefault()
        {
            setProperty("vsSolutionBuildEvent", library.Version.Number.ToString()); //optional
            setProperty("vssbeCIM", Version.S_NUM_REV);
        }

        /// <summary>
        /// Specification of this format: http://msdn.microsoft.com/en-us/library/yxkt8b26.aspx
        /// </summary>
        /// <param name="type">Type of message</param>
        /// <param name="code">code####</param>
        /// <param name="msg">Localizable string</param>
        /// <param name="file">Filename</param>
        /// <param name="line">Line</param>
        /// <returns></returns>
        protected virtual string formatEW(string type, string code, string msg, string file, int line)
        {
            return $"{file}({line}): {type} {code}: {msg}";
        }

        private void msg(string data, params object[] args)
        {
            log.info(data, args);
        }

        private void debug(string data, params object[] args)
        {
            log.debug(data, args);
        }
    }
}
