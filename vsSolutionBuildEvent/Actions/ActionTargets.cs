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

using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Action for Targets Mode
    /// </summary>
    public class ActionTargets: Action, IAction
    {
        protected class MSBuildLogger: ILogger
        {
            public string Parameters { get; set; }
            public LoggerVerbosity Verbosity { get; set; }
            public void Shutdown() {}

            public void Initialize(IEventSource eventSource)
            {
                Verbosity = LoggerVerbosity.Quiet;
                eventSource.WarningRaised   += warningRaised;
                eventSource.ErrorRaised     += errorRaised;
            }

            protected void warningRaised(object sender, BuildWarningEventArgs e)
            {
                Log.nlog.Warn("[.targets:{0}]: {1} - '{2}'", e.LineNumber, e.Code, e.Message);
            }

            protected void errorRaised(object sender, BuildErrorEventArgs e)
            {
                Log.nlog.Error("[.targets:{0}]: {1} - '{2}'", e.LineNumber, e.Code, e.Message);
            }
        }

        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            ProjectRootElement root = getXml(((IModeTargets)evt.Mode).Command);

            Project p = new Project(root, cmd.Env.getProject(null).GlobalProperties, root.ToolsVersion, ProjectCollection.GlobalProjectCollection);
            return p.Build(cmd.Env.BuildType.ToString(), new List<ILogger>() { new MSBuildLogger() });
        }

        /// <param name="cmd"></param>
        public ActionTargets(ICommand cmd)
            : base(cmd)
        {

        }

        protected ProjectRootElement getXml(string data)
        {
            using(StringReader reader = new StringReader(data)) {
                return ProjectRootElement.Create(System.Xml.XmlReader.Create(reader));
            }
        }
    }
}
