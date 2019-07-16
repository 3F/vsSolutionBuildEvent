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
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UnifiedTypes;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;
using ProjectItem = net.r_eg.MvsSln.Core.ProjectItem;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Isolated environment for work without DTE
    /// </summary>
    public class IsolatedEnv: EnvAbstract, IEnvironment
    {
        protected IDictionary<string, string> slnProperties = new Dictionary<string, string>();

        /// <summary>
        /// Parsed solution data.
        /// </summary>
        private ISlnResult sln;

        /// <summary>
        /// Activated environment for projects processing.
        /// </summary>
        private IXProjectEnv slnEnv;

        /// <summary>
        /// List of EnvDTE projects.
        /// </summary>
        public IEnumerable<DProject> ProjectsDTE
        {
            get
            {
                __disabled(nameof(ProjectsDTE));
                yield break;
            }
        }

        /// <summary>
        /// List of Microsoft.Build.Evaluation projects.
        /// </summary>
        public IEnumerable<EProject> ProjectsMBE
        {
            get
            {
                foreach(var pname in ProjectsList)
                {
                    if(!String.IsNullOrWhiteSpace(pname)) {
                        yield return getProject(pname);
                    }
                }
            }
        }

        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        public List<string> ProjectsList
        {
            get => slnEnv?.PrjCollection.LoadedProjects
                //.Where(p => p.GetPropertyValue(PropertyNames.CONFIG) == properties[PropertyNames.CONFIG] 
                //            && p.GetPropertyValue(PropertyNames.PLATFORM) == properties[PropertyNames.PLATFORM]
                //)
                .Select(p => getProjectNameFrom(p))
                .Where(name => !String.IsNullOrWhiteSpace(name))
                .ToList();
        }

        /// <summary>
        /// Active configuration for current solution
        /// </summary>
        public EnvDTE80.SolutionConfiguration2 SolutionActiveCfg
        {
            //TODO:
            get => __disabled<EnvDTE80.SolutionConfiguration2>(nameof(SolutionActiveCfg));
        }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        public string SolutionActiveCfgString
        {
            get => formatCfg(slnProperties);
        }

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations
        {
            get
            {
                //TODO: only list see in .sln -> SolutionConfigurationPlatforms
                __disabled(nameof(SolutionConfigurations));
                yield break;
            }
        }

        /// <summary>
        /// Project by default or "StartUp Project".
        /// </summary>
        public string StartupProjectString
        {
            get;
            protected set;
        }

        /// <summary>
        /// DTE2 context.
        /// </summary>
        public EnvDTE80.DTE2 Dte2
        {
            get => __disabled<EnvDTE80.DTE2>(nameof(Dte2));
        }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        public EnvDTE.Events Events
        {
            get => __disabled<EnvDTE.Events>(nameof(Events));
        }

        /// <summary>
        /// Get status of opened solution.
        /// </summary>
        public bool IsOpenedSolution
        {
            get;
            set;
        }

        /// <summary>
        /// Full path to directory where placed solution file.
        /// </summary>
        public string SolutionPath
        {
            get;
            protected set;
        }

        /// <summary>
        /// Full path to solution file.
        /// </summary>
        public string SolutionFile
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of used solution file without extension
        /// </summary>
        public string SolutionFileName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Contains all of the commands in the environment
        /// </summary>
        public EnvDTE.Commands Commands
        {
            get => __disabled<EnvDTE.Commands>(nameof(Commands));
        }

        /// <summary>
        /// Access to OutputWindowPane through IOW
        /// </summary>
        public IOW OutputWindowPane
        {
            get => __disabled<IOW>(nameof(OutputWindowPane));
        }

        /// <summary>
        /// Gets instance of the Build.Evaluation.Project for accessing to properties etc.
        /// </summary>
        /// <param name="name">Specified project name. null value for project by default (~startup-project etc.)</param>
        /// <returns>Microsoft.Build.Evaluation.Project</returns>
        public virtual EProject getProject(string name)
        {
            Log.Trace($"getProject: started with '{name}' /{StartupProjectString}");

            if(String.IsNullOrEmpty(name)) {
                name = StartupProjectString;
            }

            ProjectItem project = sln.ProjectItems.FirstOrDefault(p => p.name == name);
            if(project.fullPath == null) {
                throw new NotFoundException($"Project '{name}' was not found. ['{project.name}', '{project.pGuid}']");
            }

            return slnEnv.GetOrLoadProject(project);
        }

        /// <summary>
        /// Getting solution(for all projects) property
        /// </summary>
        /// <param name="name">Property name</param>
        public string getSolutionProperty(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                return null;
            }

            if(name.Equals(PropertyNames.CONFIG) && slnProperties.ContainsKey(PropertyNames.CONFIG)) {
                return slnProperties[PropertyNames.CONFIG];
            }

            if(name.Equals(PropertyNames.PLATFORM) && slnProperties.ContainsKey(PropertyNames.PLATFORM)) {
                return slnProperties[PropertyNames.PLATFORM];
            }

            return null;
        }

        /// <summary>
        /// Execute command with DTE
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="args">Command arguments</param>
        public void exec(string name, string args = "")
        {
            if(name == DTEC.BuildCancel) {
                CoreCmdSender.fire(new CoreCommandArgs() { Type = CoreCommandType.BuildCancel });
                return;
            }

            Log.Warn("Disabled for this Environment. Command: '{0}', args: '{1}'", name, args);
        }

        /// <summary>
        /// To update the project by default or "StartUp Project".
        /// </summary>
        /// <param name="name">Uses default behavior if empty or null.</param>
        public void updateStartupProject(string name)
        {
            if(name == String.Empty) {
                name = null;
            }

            if(name == null) {
                name = sln?.ProjectItems?.FirstOrDefault().name;
            }

            StartupProjectString = name;
            Log.Debug($"'StartUp Project' has been updated = '{name}'");
        }

        /// <param name="solutionFile">Full path to solution file (.sln)</param>
        /// <param name="properties">Solution properties / global properties for all project collection</param>
        public IsolatedEnv(string solutionFile, IDictionary<string, string> properties)
        {
            SolutionFile = solutionFile ?? throw new ArgumentNullException(nameof(solutionFile));

            if(properties == null) {
                throw new ArgumentNullException(nameof(properties));
            }

            sln = new SlnParser().Parse
            (
                SolutionFile, 
                SlnItems.Projects | SlnItems.SolutionConfPlatforms | SlnItems.ProjectConfPlatforms
            );

            foreach(var p in properties) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(p.Key, p.Value);
            }

            slnEnv = new XProjectEnv(sln, properties);

            slnProperties       = sln.Properties;
            SolutionPath        = sln.SolutionDir;
            SolutionFileName    = slnProperties.GetOrDefault(PropertyNames.SLN_NAME, PropertyNames.UNDEFINED);

            if(String.IsNullOrEmpty(StartupProjectString)) {
                updateStartupProject(null);
            }

            IsOpenedSolution = true;
        }

        /// <summary>
        /// Blank instance.
        /// </summary>
        /// <param name="properties">Solution properties.</param>
        public IsolatedEnv(IDictionary<string, string> properties)
        {
            slnProperties = properties;
        }

        protected string formatCfg(IDictionary<string, string> properties)
        {
            return formatCfg(
                properties.GetOrDefault(PropertyNames.CONFIG, sln?.DefaultConfig.Configuration),
                properties.GetOrDefault(PropertyNames.PLATFORM, sln?.DefaultConfig.Platform)
            );
        }

        private void __disabled(string name)
        {
            Log.Debug($"Accessing to '{name}' is disabled in Isolated environment.");
        }

        private T __disabled<T>(string name, T val = default(T))
        {
            __disabled(name);
            return val;
        }
    }
}
