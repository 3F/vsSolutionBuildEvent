﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using net.r_eg.MvsSln;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Action for Targets Mode
    /// </summary>
    public class ActionTargets: Action, IAction
    {
        /// <summary>
        /// Entry point for user code.
        /// </summary>
        public const string ENTRY_POINT = "Init";

        /// <summary>
        /// Logger of the build process.
        /// </summary>
        protected class MSBuildLogger: ILogger
        {
            public bool Silent { get; set; }
            public string Parameters { get; set; }
            public LoggerVerbosity Verbosity { get; set; }
            public void Shutdown() { }

            public void Initialize(IEventSource eventSource)
            {
                Verbosity = (Silent)? LoggerVerbosity.Quiet : LoggerVerbosity.Normal;
                eventSource.WarningRaised   += warningRaised;
                eventSource.ErrorRaised     += errorRaised;

                if(!Silent) {
                    eventSource.AnyEventRaised += anyEventRaised;
                }
            }

            protected void anyEventRaised(object sender, BuildEventArgs e)
            {
                Log.Info(e.Message);
            }

            protected void warningRaised(object sender, BuildWarningEventArgs e)
            {
                Log.Warn("[.targets:{0}]: {1} - '{2}'", e.LineNumber, e.Code, e.Message);
            }

            protected void errorRaised(object sender, BuildErrorEventArgs e)
            {
                Log.Error("[.targets:{0}]: {1} - '{2}'", e.LineNumber, e.Code, e.Message);
            }
        }

        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            string command = ((IModeTargets)evt.Mode).Command;
            ProjectRootElement root = getXml(parse(evt, command));
            
            var request = new BuildRequestData
            (
                new ProjectInstance(root, propertiesByDefault(evt), root.ToolsVersion, ProjectCollection.GlobalProjectCollection), 
                new string[] { ENTRY_POINT }, 
                new HostServices()
            );

            // holy hedgehogs...
            // https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/build/build-103
            // Only modern BuildManager implements IDisposable

#if NET_40
            // Not a .NETFramework\v4.0\Microsoft.Build.dll v4.0.30319
            return build(new BuildManager(Settings.APP_NAME_SHORT), request, evt.Process.Hidden);
#else
            using BuildManager manager = new(Settings.APP_NAME_SHORT);
            return build(manager, request, evt.Process.Hidden);
#endif

        }

        /// <param name="cmd"></param>
        public ActionTargets(ICommand cmd)
            : base(cmd)
        {

        }

        /// <summary>
        /// Gets xml data as ProjectRootElement from string.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ProjectRootElement getXml(string data)
        {
            using(StringReader reader = new StringReader(data)) {
                return ProjectRootElement.Create(System.Xml.XmlReader.Create(reader));
            }
        }

        /// <summary>
        /// Build user-action with additional BuildManager.
        /// Most important for our post-build operations with msbuild tool.
        /// Only for Visual Studio we may safe begin simple from Evaluation.Project.Build(...)
        /// </summary>
        /// <param name="manager">Build Manager.</param>
        /// <param name="request">Configuration.</param>
        /// <param name="silent"></param>
        /// <returns></returns>
        protected bool build(BuildManager manager, BuildRequestData request, bool silent = true)
        {
            BuildResult result = manager.Build
            (
                new BuildParameters()
                {
                    //MaxNodeCount = 12,
                    Loggers = new List<ILogger>() {
                                    new MSBuildLogger() {
                                        Silent = silent
                                    }
                              }
                }, 
                request
            );

            return result.OverallResult == BuildResultCode.Success;
        }

        protected Dictionary<string, string> propertiesByDefault(ISolutionEvent evt)
            => new Dictionary<string, string>(cmd.Env.getProject(null).GlobalProperties)
            {
                { PropertyNames.PRJ_NAME, $"_{evt.Name}" },
                { "ActionName", evt.Name },
                { "BuildType", cmd.Env.BuildType.ToString() },
                { "EventType", cmd.EventType.ToString() },
                { "SupportMSBuild", evt.SupportMSBuild.ToString() },
                { "SupportSBEScripts", evt.SupportSBEScripts.ToString() },
                { "SolutionActiveCfg", cmd.Env.SolutionActiveCfgString },
                { "StartupProject", cmd.Env.StartupProjectString }
            };
    }
}
