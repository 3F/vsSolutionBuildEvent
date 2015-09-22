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
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
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
            
            BuildRequestData request = new BuildRequestData(
                                            new ProjectInstance(root, propertiesByDefault(evt), root.ToolsVersion, ProjectCollection.GlobalProjectCollection), 
                                            new string[] { ENTRY_POINT }, 
                                            new HostServices()
                                       );

            // holy hedgehogs...

#if !NET_40

            // Using of BuildManager from Microsoft.Build.dll, v4.0.0.0 - .NETFramework\v4.5\Microsoft.Build.dll
            // you should see IDisposable, and of course you can see CA1001 for block as in #else section below.
            using(BuildManager manager = new BuildManager(Settings.APP_NAME_SHORT)) {
                return build(manager, request, evt.Process.Hidden);
            }

#else

            // Using of BuildManager from Microsoft.Build.dll, v4.0.30319 - .NETFramework\v4.0\Microsoft.Build.dll
            // Does not implement IDisposable, and voila:
            // https://ci.appveyor.com/project/3Fs/vssolutionbuildevent/build/build-103
            return build(new BuildManager(Settings.APP_NAME_SHORT), request, evt.Process.Hidden);

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
            BuildResult result = manager.Build(new BuildParameters()
                                                    {
                                                        //MaxNodeCount = 12,
                                                        Loggers = new List<ILogger>() {
                                                                        new MSBuildLogger() {
                                                                            Silent = silent
                                                                        }
                                                                  }
                                                    }, 
                                                    request);

            return (result.OverallResult == BuildResultCode.Success);
        }

        protected Dictionary<string, string> propertiesByDefault(ISolutionEvent evt)
        {
            Dictionary<string, string> prop = new Dictionary<string, string>(cmd.Env.getProject(null).GlobalProperties);
            
            prop.Add("ProjectName", String.Format("_{0}", evt.Name));
            prop.Add("ActionName", evt.Name);
            prop.Add("BuildType", cmd.Env.BuildType.ToString());
            prop.Add("EventType", cmd.EventType.ToString());
            prop.Add("SupportMSBuild", evt.SupportMSBuild.ToString());
            prop.Add("SupportSBEScripts", evt.SupportSBEScripts.ToString());
            prop.Add("SolutionActiveCfg", cmd.Env.SolutionActiveCfgString);
            prop.Add("StartupProject", cmd.Env.StartupProjectString);

            return prop;
        }
    }
}
