/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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

        protected readonly IConfPlatform defaultCfg = new ConfigSln("Debug", "Any CPU");

        private ConfigItem currentSlnConf;

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

        /// <inheritdoc/>
        public List<string> ProjectsList
        {
            get => (Sln?.ProjectItems ?? Enumerable.Empty<MvsSln.Core.ProjectItem>() )
                .Where(p => !string.IsNullOrWhiteSpace(p.name))
                .Select(p => p.name)
                .ToList();
        }

        /// <summary>
        /// Active configuration for current solution
        /// </summary>
        public EnvDTE80.SolutionConfiguration2 SolutionActiveCfg => new DteSlnCfg(extractCfg(slnProperties));

        /// <summary>
        /// Formatted string with an active configuration for current solution.
        /// </summary>
        public string SolutionActiveCfgString => formatCfg(slnProperties);

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations
        {
            get => (Sln?.SolutionConfigs ?? new[] { extractCfg(slnProperties) })
                        .Select(c => new DteSlnCfg(c.Configuration, c.Platform));
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

        protected override ConfigItem ActiveSlnConf => extractCfg(slnProperties);

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

            __disabled($"{name}({args})");
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
            _properties     = ConfigureAsNew(properties ?? throw new ArgumentNullException(nameof(properties)));

            // better to use it before accessing to {Sln} property due to possible custom env updating 
            foreach(var p in _properties.Where(p => p.Value != null)) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(p.Key, p.Value);
            }

            SolutionPath        = Sln.SolutionDir;
            slnProperties       = Sln.Properties.ExtractDictionary.AddOrUpdate(_properties);
            SolutionFileName    = slnProperties.GetOrDefault(PropertyNames.SLN_NAME, PropertyNames.UNDEFINED);
            IsOpenedSolution    = true;
        }

        /// <summary>
        /// Blank instance.
        /// </summary>
        /// <param name="properties">Solution properties.</param>
        public IsolatedEnv(IDictionary<string, string> properties)
        {
            slnProperties = ConfigureAsNew(properties);
        }

        protected override void UpdateSlnEnv(ISlnResult sln) => AssignEnv(new XProjectEnv(sln, _properties));

        protected IDictionary<string, string> ConfigureAsNew(IDictionary<string, string> properties)
            => Configure(properties?.ToDictionary(k => k.Key, v => v.Value));

        protected IDictionary<string, string> Configure(IDictionary<string, string> properties)
        {
            if(properties == null)
            {
                return null;
            }

            void _SetIfNull(string key, string value)
            {
                if(!properties.ContainsKey(key) || properties[key] == null)
                {
                    properties[key] = value;
                }
            }

            _SetIfNull(nameof(defaultCfg.Configuration), defaultCfg.Configuration);
            _SetIfNull(nameof(defaultCfg.Platform), defaultCfg.Platform);
            return properties;
        }

        protected ConfigItem extractCfg(IDictionary<string, string> properties)
        {
            IConfPlatform def = Sln?.DefaultConfig;

            string configuration = properties.GetOrDefault(PropertyNames.CONFIG, def?.Configuration);
            string platform = properties.GetOrDefault(PropertyNames.PLATFORM, def?.Platform);

            if(currentSlnConf?.IsEqualByRule(configuration, platform) == true)
            {
                return currentSlnConf;
            }

            currentSlnConf = new(configuration, platform);
            return currentSlnConf;
        }

        protected string formatCfg(IDictionary<string, string> properties)
        {
            IConfPlatform def = extractCfg(properties);
            return formatCfg(def.Configuration, def.Platform);
        }

        private void __disabled(string name)
        {
            Log.Debug($"Accessing to '{name}' is disabled in Isolated environment.");
        }

        private T __disabled<T>(string name, T val = default)
        {
            __disabled(name);
            return val;
        }
    }
}
