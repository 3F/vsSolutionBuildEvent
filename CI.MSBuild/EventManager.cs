/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    public class EventManager: Logger
    {
        /// <summary>
        /// Our the vsSolutionBuildEvent library
        /// </summary>
        protected Provider.ILibrary library;

        /// <summary>
        /// Find & get path to library
        /// </summary>
        protected string LibraryPath
        {
            get
            {
                if(args.ContainsKey("lib")) {
                    return args["lib"];
                }
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Full path to solution file
        /// </summary>
        protected string SolutionFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Specified type of build action
        /// </summary>
        protected volatile BuildType buildType = BuildType.Common;

        /// <summary>
        /// Logger parameters
        /// </summary>
        protected Dictionary<string, string> args = new Dictionary<string, string>();

        /// <summary>
        /// About projects by ident
        /// </summary>
        protected Dictionary<int, Project> projects = new Dictionary<int, Project>();

        /// <summary>
        /// Internal logging
        /// </summary>
        internal ILog log;

        /// <summary>
        /// Reserved for future use with IVsSolutionEvents
        /// </summary>
        private object pUnkReserved = new object();

        /// <summary>
        /// Initializer of the Build.Framework.ILogger objects.
        /// Subscribes to specific events etc.
        /// </summary>
        /// <param name="evt"></param>
        public override void Initialize(IEventSource evt)
        {
            log = new Log(Verbosity);

            setCulture("en-US"); //TODO: key for user
            args = extractArguments(Parameters);

            evt.TargetStarted   += new TargetStartedEventHandler(onTargetStarted);
            evt.ProjectStarted  += new ProjectStartedEventHandler(onProjectStarted);
            evt.AnyEventRaised  += new AnyEventHandler(onAnyEventRaised);
            evt.ErrorRaised     += new BuildErrorEventHandler(onErrorRaised);
            evt.WarningRaised   += new BuildWarningEventHandler(onWarningRaised);
            evt.BuildFinished   += new BuildFinishedEventHandler(onBuildFinished);
        }

        public override void Shutdown()
        {
            if(library != null) {
                library.Event.solutionClosed(pUnkReserved);
            }
        }

        public void setCulture(string name)
        {
            try {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(name);
                // also -> CurrentCulture - for time etc.
            }
            catch(Exception ex) {
                msg(ex.Message);
            }
        }

        /// <summary>
        /// Initialize library with ProjectStarted event.
        /// This is the first event from all where we can define the SolutionFile
        /// </summary>
        /// <param name="e"></param>
        /// <returns>true if loaded</returns>
        protected bool initLibraryOnProjectStarted(ProjectStartedEventArgs e)
        {
            if(!e.ProjectFile.TrimEnd().ToLower().EndsWith(".sln")) {
                debug("solutionOpened() has been ignored for '{0}'", e.ProjectFile);
                return false;
            }

            SolutionFile    = e.ProjectFile; // should be .sln
            library         = load(SolutionFile, getSolutionProperties(e.Properties), LibraryPath);
            return true;
        }

        protected void onProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            if(library == null || String.IsNullOrEmpty(SolutionFile)) {
                if(initLibraryOnProjectStarted(e)) {
                    onPre(e); // or use some target for delays as variant
                }
                return;
            }

            if(e.ProjectFile.ToLower().EndsWith(".metaproj")) {
                debug(".metaproj has been ignored for '{0}'", e.ProjectFile);
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

            Dictionary<object, string> properties = e.Properties.OfType<DictionaryEntry>().ToDictionary(k => k.Key, v => v.Value.ToString());
            if(properties.ContainsKey("ProjectName"))
            {
                projects[e.ProjectId] = new Project() {
                    Name            = properties["ProjectName"],
                    File            = e.ProjectFile,
                    Properties      = properties
                };
            }
        }

        protected void onTargetStarted(object sender, TargetStartedEventArgs e)
        {
            if(!projects.ContainsKey(e.BuildEventContext.ProjectInstanceId)) {
                return;
            }

            switch(e.TargetName) { //.Trim().ToLower()
                case "PreBuildEvent": {
                    library.Event.onProjectPre(projects[e.BuildEventContext.ProjectInstanceId].Name);
                    break;
                }
                case "PostBuildEvent": {
                    Project p = projects[e.BuildEventContext.ProjectInstanceId];
                    library.Event.onProjectPost(p.Name, p.HasErrors ? 0 : 1);
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

        protected void onPre(ProjectStartedEventArgs e)
        {
            updateBuildType(e.TargetNames);

            int pfCancelUpdate = 0;
            library.Event.onPre(ref pfCancelUpdate);
        }

        protected void onBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            if(library == null) {
                return;
            }

            if(!e.Succeeded) {
                library.Event.onCancel();
            }
            library.Event.onPost((e.Succeeded)? 1 : 0, 0, 0);
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
        /// Gets solution properties (global properties for all projects)
        /// 
        /// Note: ProjectStartedEventArgs.GlobalProperties available only with .NET 4.5
        /// http://msdn.microsoft.com/en-us/library/microsoft.build.framework.projectstartedeventargs.globalproperties%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="properties">DictionaryEntry properties</param>
        /// <returns></returns>
        protected Dictionary<string, string> getSolutionProperties(IEnumerable properties)
        {
            Dictionary<string, string> _properties = properties.OfType<DictionaryEntry>().ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

            Dictionary<string, string> ret = new Dictionary<string, string>();
            if(_properties.ContainsKey("Configuration")) {
                ret["Configuration"] = _properties["Configuration"];
            }
            if(_properties.ContainsKey("Platform")) {
                ret["Platform"] = _properties["Platform"];
            }
            if(_properties.ContainsKey("SolutionDir")) {
                ret["SolutionDir"] = _properties["SolutionDir"];
            }
            if(_properties.ContainsKey("SolutionName")) {
                ret["SolutionName"] = _properties["SolutionName"];
            }
            if(_properties.ContainsKey("SolutionFileName")) {
                ret["SolutionFileName"] = _properties["SolutionFileName"];
            }
            if(_properties.ContainsKey("SolutionExt")) {
                ret["SolutionExt"] = _properties["SolutionExt"];
            }
            if(_properties.ContainsKey("SolutionPath")) {
                ret["SolutionPath"] = _properties["SolutionPath"];
            }

            if(Verbosity == LoggerVerbosity.Diagnostic)
            {
                foreach(KeyValuePair<string, string> p in ret) {
                    msg(String.Format("SolutionProperty found: ['{0}' => '{1}']", p.Key, p.Value));
                }
            }
            return ret;
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

        protected Provider.ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath)
        {
            debug("Loading: '{0}' /'{1}'", solutionFile, libPath);

            Provider.ILoader loader     = new Provider.Loader();
            loader.Settings.DebugMode   = (Verbosity == LoggerVerbosity.Diagnostic);
            try
            {
                Provider.ILibrary library = loader.load(solutionFile, properties, libPath, false);
                msg("Library: loaded from '{0}' :: v{1} [{2}] API: v{3} /'{4}':{5}", 
                                    library.Dllpath, 
                                    library.Version.Number.ToString(), 
                                    library.Version.BranchSha1,
                                    library.Version.Bridge.Number.ToString(2),
                                    library.Version.BranchName,
                                    library.Version.BranchRevCount);

                library.Event.solutionOpened(pUnkReserved, 0);
                return library;
            }
            catch(DllNotFoundException ex)
            {
                msg(ex.Message);
                msg("* You can install vsSolutionBuildEvent as plugin for Visual Studio(if it's possible)");
                msg(
                    "* Or try manually place the {0} into directory with current logger({1})", 
                    "vsSolutionBuildEvent.dll with dependencies",
                    Assembly.GetExecutingAssembly().Location);
                msg("* Or set any path to library for this logger, e.g.: /l:CI.MSBuild.dll;lib=<path_to_vsSolutionBuildEvent.dll>");

                msg("See our documentation for more details.");
                msg("https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
                msg("Minimum requirements: vsSolutionBuildEvent.dll v{0}", loader.MinVersion.ToString());
                msg(new String('=', 80));
            }
            catch(ReflectionTypeLoadException ex)
            {
                foreach(FileNotFoundException le in ex.LoaderExceptions) {
                    msg("{2} {0}{3} {0}{0}{4} {0}{1}",
                                        Environment.NewLine, new String('~', 80),
                                        le.FileName, le.Message, le.FusionLog);
                }
            }
            catch(Exception ex) {
                msg("Error with loading: '{0}'", ex.ToString());
            }

            // Abort all msbuild operations
            throw new AbortException();
        }

        /// <summary>
        /// Format specification: http://msdn.microsoft.com/en-us/library/yxkt8b26.aspx
        /// </summary>
        /// <param name="type">Type of message</param>
        /// <param name="code">code####</param>
        /// <param name="msg">Localizable string</param>
        /// <param name="file">Filename</param>
        /// <param name="line">Line</param>
        /// <returns></returns>
        protected virtual string formatEW(string type, string code, string msg, string file, int line)
        {
            return String.Format("{0}({1}): {2} {3}: {4}", file, line, type, code, msg);
        }

        /// <summary>
        /// Extract arguments from format: 
        /// name1=value1;name2=value2;name3=value3
        /// http://msdn.microsoft.com/en-us/library/ms164311.aspx
        /// </summary>
        /// <param name="raw">raw line from params to logger</param>
        /// <returns></returns>
        protected Dictionary<string, string> extractArguments(string raw)
        {
            if(String.IsNullOrEmpty(raw)) {
                return new Dictionary<string, string>();
            }

            return raw.Split(';')
                        .Select(p => p.Split('='))
                        .Select(p => new KeyValuePair<string, string>(p[0], (p.Length < 2)? null : p[1]))
                        .ToDictionary(k => k.Key, v => v.Value);
        }

        private void header()
        {
            msg(new String('=', 60));
            msg("[[ vsSolutionBuildEvent CI.MSBuild ]] Welcomes You!");
            msg(new String('=', 60));
            msg("Version: v{0}", System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
            msg("Feedback: entry.reg@gmail.com");
            msg(new String('_', 60));
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
