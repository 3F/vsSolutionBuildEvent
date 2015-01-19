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
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    /// <summary>
    /// *!* DRAFT *!* - it's not complete implementation *!* DRAFT *!*
    /// see latest version on: https://bitbucket.org/3F/vssolutionbuildevent/src
    /// TODO: 
    ///    * Direct pushing EW-codes with API
    ///    * lib: OWP -> EW & wrap BuildItem
    ///    * EW events
    ///    * Cancel evetns
    ///    * Abort signals
    ///    * not localized raw message
    /// </summary>
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
        /// Logger parameters
        /// </summary>
        protected Dictionary<string, string> args = new Dictionary<string, string>();

        /// <summary>
        /// Information of project by ident
        /// </summary>
        protected class Project
        {
            /// <summary>
            /// Project name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// All available properties with msbuild
            /// </summary>
            public Dictionary<object, string> Properties { get; set; }

            /// <summary>
            /// Full path to project file
            /// </summary>
            public string File { get; set; }

            /// <summary>
            /// Has error/s with some target/s
            /// </summary>
            public bool HasErrors { get; set; }
        }
        protected Dictionary<int, Project> projects = new Dictionary<int, Project>();

        /// <summary>
        /// Abort the all msbuild operations
        /// TODO: variant of normal termination !
        /// </summary>
        protected virtual void Abort()
        {
            throw new AbortException();
        }
        private class AbortException: Exception { }

        private object pUnkReserved = new object();

        public int solutionOpened()
        {
            return library.Event.solutionOpened(pUnkReserved, 0);
        }

        public int solutionClosed()
        {
            return library.Event.solutionClosed(pUnkReserved);
        }

        public int onPre()
        {
            int pfCancelUpdate = 0;
            return library.Event.onPre(ref pfCancelUpdate);
        }

        public int onCancel()
        {
            return library.Event.onCancel();
        }

        public int onPost(int fSucceeded)
        {
            return library.Event.onPost(fSucceeded, 0, 0);
        }

        public int onProjectPre(string project)
        {
            return library.Event.onProjectPre(project);
        }

        public int onProjectPost(string project, int fSuccess)
        {
            return library.Event.onProjectPost(project, fSuccess);
        }

        public void onBuildRaw(string data)
        {
            if(library != null) {
                library.Build.onBuildRaw(data);
            }
        }

        public override void Initialize(IEventSource evt)
        {
            args = extractArguments(Parameters);


            //TODO:
            evt.BuildFinished       += new BuildFinishedEventHandler(onBuildFinished);
            evt.ProjectStarted      += new ProjectStartedEventHandler(onProjectStarted);
            evt.ProjectFinished     += new ProjectFinishedEventHandler(onProjectFinished);            
            evt.AnyEventRaised      += new AnyEventHandler(onAnyEventRaised);
            evt.CustomEventRaised   += new CustomBuildEventHandler(onCustomEventRaised);
            evt.ErrorRaised         += new BuildErrorEventHandler(onErrorRaised);
            evt.MessageRaised       += new BuildMessageEventHandler(onMessageRaised);
            evt.StatusEventRaised   += new BuildStatusEventHandler(onStatusEventRaised);
            evt.TargetFinished      += new TargetFinishedEventHandler(onTargetFinished);
            evt.TargetStarted       += new TargetStartedEventHandler(onTargetStarted);
            evt.TaskFinished        += new TaskFinishedEventHandler(onTaskFinished);
            evt.TaskStarted         += new TaskStartedEventHandler(onTaskStarted);
            evt.WarningRaised       += new BuildWarningEventHandler(onWarningRaised);
        }

        public override void Shutdown()
        {
            solutionClosed();
        }

        protected void onWarningRaised(object sender, BuildWarningEventArgs e)
        {
            //TODO: e.Code
            //onBuildRaw(e.Message);
        }

        protected void onErrorRaised(object sender, BuildErrorEventArgs e)
        {
            if(projects.ContainsKey(e.BuildEventContext.ProjectInstanceId)) {
                projects[e.BuildEventContext.ProjectInstanceId].HasErrors = true;
            }

            //TODO: e.Code
            //onBuildRaw(e.Message);
        }

        protected void onProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            if(String.IsNullOrEmpty(SolutionFile))
            {
                if(!e.ProjectFile.ToLower().EndsWith(".sln")) {
                    return;
                }
                SolutionFile = e.ProjectFile;
                library = load(SolutionFile, getSolutionProperties(e.Properties), LibraryPath);
                solutionOpened();
                onPre();
                return;
            }

            if(e.ProjectFile.ToLower().EndsWith(".metaproj")) {
                return;
            }

            //string target = e.TargetNames.Trim().ToLower();
            //if(target != "build" && target != "rebuild" && target != "clean") {
            //    return; //work only with general targets
            //}

            if(projects.ContainsKey(e.ProjectId)) {
                // already pushed
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

        protected void onProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            //if(e.ProjectFile.ToLower().EndsWith(".metaproj")) {
            //    return;
            //}

            //if(projects.ContainsKey(e.BuildEventContext.ProjectInstanceId)) {
            //    onProjectPost(projects[e.BuildEventContext.ProjectInstanceId].Name, e.Succeeded? 1 : 0);
            //    return;
            //}
            //msg("Not found ProjectName property for '{0}'", e.ProjectFile);
        }

        protected void onTargetStarted(object sender, TargetStartedEventArgs e)
        {
            onBuildRaw(e.Message);
            if(!projects.ContainsKey(e.BuildEventContext.ProjectInstanceId)) {
                return;
            }

            switch(e.TargetName) { //.Trim().ToLower()
                case "PreBuildEvent": {
                    onProjectPre(projects[e.BuildEventContext.ProjectInstanceId].Name);
                    break;
                }
                case "PostBuildEvent": {
                    Project p = projects[e.BuildEventContext.ProjectInstanceId];
                    onProjectPost(p.Name, p.HasErrors? 0 : 1);
                    break;
                }
            }
        }

        protected void onBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            onPost((e.Succeeded)? 1 : 0);
        }

        protected void onTaskStarted(object sender, TaskStartedEventArgs e)
        {
            //TODO:
        }

        protected void onTaskFinished(object sender, TaskFinishedEventArgs e)
        {
            //TODO:
        }

        protected void onTargetFinished(object sender, TargetFinishedEventArgs e)
        {
            //TODO:
        }

        protected void onStatusEventRaised(object sender, BuildStatusEventArgs e)
        {
            //TODO:
        }

        protected void onMessageRaised(object sender, BuildMessageEventArgs e)
        {
            //TODO:
        }

        protected void onCustomEventRaised(object sender, CustomBuildEventArgs e)
        {
            //TODO:
        }

        protected void onAnyEventRaised(object sender, BuildEventArgs e)
        {
            //TODO:
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

            return ret;
        }

        protected Provider.ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath)
        {
            Provider.ILoader loader = new Provider.Loader();
            //loader.DebugMode = true;
            try {
                Provider.ILibrary library = loader.load(solutionFile, properties, libPath, false);
                msg("Library: loaded from '{0}' :: v{1} [{2}] /'{3}':{4}", 
                                    library.Dllpath, 
                                    library.Version.Number.ToString(), 
                                    library.Version.BranchSha1,
                                    library.Version.BranchName,
                                    library.Version.BranchRevCount);
                return library;
            }
            catch(DllNotFoundException ex)
            {
                msg(ex.Message);
                msg("* You can install vsSolutionBuildEvent as plugin for Visual Studio(if it's possible)");
                msg(
                    "* Or try manually place the {0} into directory with current logger({1})", 
                    "vsSolutionBuildEvent.dll with dependencies",
                    GetType().Assembly.Location);
                msg("* Or provide any path to library with current logger, e.g.: /l:<path>\\CI.MSBuild.dll;lib=<path to vsSolutionBuildEvent.dll>");

                msg("For more details, see our documentations.");
                msg("https://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
                msg("Minimum requirements: vsSolutionBuildEvent.dll v{0}", loader.MinVersion.ToString());
                msg(new String('=', 80));
            }
            catch(ReflectionTypeLoadException ex) {
                foreach(FileNotFoundException le in ex.LoaderExceptions) {
                    msg("{2} {0}{3} {0}{0}{4} {0}{1}",
                                        Environment.NewLine, new String('~', 80),
                                        le.FileName, le.Message, le.FusionLog);
                }
            }
            catch(Exception ex) {
                msg("Error with library: '{0}'", ex.ToString());
            }
            throw new AbortException();
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

        //TODO: new lightweight logger or main NLog for all projects. New reference with NLog very large for simple wrappers..
        protected void msg(string data, params object[] args)
        {
            Console.WriteLine(data, args);
        }
    }
}
