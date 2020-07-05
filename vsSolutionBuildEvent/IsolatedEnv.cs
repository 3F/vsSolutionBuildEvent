/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.EvMSBuild;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UnifiedTypes;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Isolated environment for work without DTE
    /// </summary>
    public class IsolatedEnv: EnvAbstract, IEnvironment, IEvEnv
    {
        protected IDictionary<string, string> slnProperties = new Dictionary<string, string>();

        private string _startupProjectString;

        private readonly IDictionary<string, string> _properties;

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
            get => SlnEnv?.ValidProjects
                    .Where(p => !string.IsNullOrWhiteSpace(p.GetProjectName()));
        }

        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        public List<string> ProjectsList
        {
            get => SlnEnv?.ValidProjects
                // TODO: possible duplicates because only ProjectsDTE provides an uniqiue list
                .Select(p => p.GetProjectName())
                .Where(name => !string.IsNullOrWhiteSpace(name))
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
        /// Formatted string with an active configuration for current solution.
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
        /// Project Name by default or "StartUp Project".
        /// </summary>
        public override string StartupProjectString
        {
            get
            {
                if(_startupProjectString == null) {
                    updateStartupProject(null);
                }
                return _startupProjectString;
            }

            protected set => _startupProjectString = value;
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
        public override string SolutionFile
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
        /// An unified unscoped and out of Project instance the property value by its name.
        /// Remarks: Any property values cannot be null.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>Found non-null property value or null if not.</returns>
        public string GetMutualPropValue(string name)
        {
            return getSolutionProperty(name);
        }

        /// <summary>
        /// Getting an unified property for all existing projects. 
        /// Aka "Solution property".
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
        /// To update the Project Name by default aka "StartUp Project".
        /// </summary>
        /// <param name="name">Uses default behavior if empty or null.</param>
        public void updateStartupProject(string name)
        {
            if(string.IsNullOrEmpty(name)) {
                name = Sln?.ProjectItems?.FirstOrDefault().name;
            }

            StartupProjectString = name;
            Log.Debug($"'StartUp Project' has been updated = '{name}'");
        }

        /// <param name="solutionFile">Full path to solution file (.sln)</param>
        /// <param name="properties">Solution properties / global properties for all project collection</param>
        public IsolatedEnv(string solutionFile, IDictionary<string, string> properties)
        {
            SolutionFile    = solutionFile ?? throw new ArgumentNullException(nameof(solutionFile));
            _properties     = properties ?? throw new ArgumentNullException(nameof(properties));

            // better to use it before accessing to {Sln} property due to possible custom env updating 
            foreach(var p in properties) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(p.Key, p.Value);
            }

            SolutionPath        = Sln.SolutionDir;
            slnProperties       = Sln.Properties.ExtractDictionary.AddOrUpdate(properties);
            SolutionFileName    = slnProperties.GetOrDefault(PropertyNames.SLN_NAME, PropertyNames.UNDEFINED);
            IsOpenedSolution    = true;
        }

        /// <summary>
        /// Blank instance.
        /// </summary>
        /// <param name="properties">Solution properties.</param>
        public IsolatedEnv(IDictionary<string, string> properties)
        {
            slnProperties = properties;
        }

        protected override void UpdateSlnEnv(ISlnResult sln)
        {
            SlnEnv = new XProjectEnv(sln, _properties);
            SlnEnv.Assign();
        }

        protected string formatCfg(IDictionary<string, string> properties)
        {
            IConfPlatform def = Sln?.DefaultConfig;

            return formatCfg(
                properties.GetOrDefault(PropertyNames.CONFIG, def?.Configuration),
                properties.GetOrDefault(PropertyNames.PLATFORM, def?.Platform)
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
