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
using System.IO;
using System.Linq;
using Microsoft.Build.Evaluation;
using net.r_eg.vsSBE.API.Commands;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.UnifiedTypes;

namespace net.r_eg.vsSBE
{
    using TProp = Dictionary<string, string>;

    /// <summary>
    /// Isolated environment for work without DTE
    /// </summary>
    public class IsolatedEnv: IEnvironment
    {
        /// <summary>
        /// Solution properties.
        /// </summary>
        protected TProp slnProperties = new TProp();

        /// <summary>
        /// Solution data
        /// </summary>
        private Sln.Parser.Result _sln;

        /// <summary>
        /// List of EnvDTE projects.
        /// </summary>
        public IEnumerable<EnvDTE.Project> ProjectsDTE
        {
            get {
                Log.Debug("Accessing to property 'Projects' has been disabled in Isolated environment.");
                yield break;
            }
        }

        /// <summary>
        /// List of Microsoft.Build.Evaluation projects.
        /// </summary>
        public IEnumerable<Project> ProjectsMBE
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
            get {
                return ProjectCollection.GlobalProjectCollection.LoadedProjects
                        //.Where(p => p.GetPropertyValue("Configuration") == properties["Configuration"] 
                        //            && p.GetPropertyValue("Platform") == properties["Platform"]
                        //)
                        .Select(p => getProjectNameFrom(p))
                        .Where(name => !String.IsNullOrEmpty(name))
                        .ToList<string>();
            }
        }

        /// <summary>
        /// Active configuration for current solution
        /// </summary>
        public EnvDTE80.SolutionConfiguration2 SolutionActiveCfg
        {
            get {
                //TODO:
                Log.Debug("Accessing to property 'SolutionActiveCfg' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        public string SolutionActiveCfgString
        {
            get {
                return formatCfg(slnProperties["Configuration"], slnProperties["Platform"]);
            }
        }

        /// <summary>
        /// Current context for actions.
        /// </summary>
        public BuildType BuildType
        {
            get;
            set;
        }

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations
        {
            get {
                //TODO: only list see in .sln -> SolutionConfigurationPlatforms
                Log.Debug("Accessing to property 'SolutionConfigurations' has been disabled in Isolated environment.");
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
        /// Get status of opened solution.
        /// </summary>
        public bool IsOpenedSolution
        {
            get;
            set;
        }

        /// <summary>
        /// DTE2 context.
        /// </summary>
        public EnvDTE80.DTE2 Dte2
        {
            get {
                Log.Debug("Accessing to property 'Dte2' has been disabled in Isolated environment.");
                return null;
            }
        }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        public EnvDTE.Events Events
        {
            get {
                Log.Debug("Accessing to property 'Events' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Sender of the core commands.
        /// </summary>
        public IFireCoreCommand CoreCmdSender
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
            get {
                Log.Debug("Accessing to property 'Commands' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Access to OutputWindowPane through IOW
        /// </summary>
        public IOW OutputWindowPane
        {
            get {
                Log.Debug("Accessing to property 'OutputWindowPane' has been disabled in Isolated environment.");
                return null;
            }
        }

        /// <summary>
        /// Gets instance of the Build.Evaluation.Project for accessing to properties etc.
        /// </summary>
        /// <param name="name">Specified project name. null value for project by default (~startup-project etc.)</param>
        /// <returns>Microsoft.Build.Evaluation.Project</returns>
        public virtual Project getProject(string name = null)
        {
            Log.Trace("getProject: started with '{0}'", name);

            if(String.IsNullOrEmpty(name)) {
                name = StartupProjectString;
                Log.Trace("getProject: use the StartupProject '{0}'", name);
            }
            Sln.Project project;

            foreach(Project eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
            {
                string eConfiguration   = eProject.GetPropertyValue("Configuration");
                string ePlatform        = eProject.GetPropertyValue("Platform");
                string eCfg             = formatCfg(eConfiguration, ePlatform);

                Log.Trace($"find in projects collection: `{eProject.FullPath}`");
                project = _sln.projects.Find(p => p.fullPath == eProject.FullPath);

                if(project.pGuid == null || project.fullPath == null) {
                    continue;
                }

                TProp prop = projectProperties(project, slnProperties);
                Log.Trace($" ? {prop["Configuration"]}|{prop["Platform"]} == {eCfg}");

                if(eCfg == formatCfg(prop["Configuration"], prop["Platform"])) {
                    return eProject;
                }
            }

            project = _sln.projects.Find(p => p.name == name);

            Log.Trace("trying to load project :: '{0}' ('{1}')", project.name, project.fullPath);
            if(String.IsNullOrEmpty(project.fullPath)) {
                throw new NotFoundException("Missed path to project '{0}' ['{1}', '{2}']", name, project.name, project.pGuid);
            }

            return new Project(
                            project.fullPath, 
                            projectProperties(project, slnProperties), 
                            null, 
                            ProjectCollection.GlobalProjectCollection
            );
        }

        /// <summary>
        /// Format configuration from the SolutionConfiguration2
        /// </summary>
        public string SolutionCfgFormat(EnvDTE80.SolutionConfiguration2 cfg)
        {
            if(cfg == null) {
                return String.Format("{0}|{0}", Environment.PROP_UNAV_STRING);
            }
            return formatCfg(cfg.Name, cfg.PlatformName);
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

            if(name.Equals("Configuration") && slnProperties.ContainsKey("Configuration")) {
                return slnProperties["Configuration"];
            }

            if(name.Equals("Platform") && slnProperties.ContainsKey("Platform")) {
                return slnProperties["Platform"];
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

            if(_sln?.projects != null && _sln.projects.Count > 0
                && name == null)
            {
                name = _sln.projects[0].name;
            }

            StartupProjectString = name;
            Log.Debug($"'StartUp Project' has been updated = '{name}'");
        }

        /// <param name="solutionFile">Full path to solution file (.sln)</param>
        /// <param name="properties">Solution properties / global properties for all project collection</param>
        public IsolatedEnv(string solutionFile, TProp properties)
        {
            SolutionFile        = solutionFile;
            SolutionPath        = Path.GetDirectoryName(SolutionFile);
            SolutionFileName    = Path.GetFileNameWithoutExtension(SolutionFile);

            _sln = (new Sln.Parser()).parse(SolutionFile);

            if(String.IsNullOrEmpty(StartupProjectString)) {
                updateStartupProject(null);
            }
            IsOpenedSolution = true;

            slnProperties = propertiesByDefault(properties);
            foreach(KeyValuePair<string, string> property in properties) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(property.Key, property.Value);
            }
        }

        /// <summary>
        /// Blank instance.
        /// </summary>
        /// <param name="properties">Solution properties.</param>
        public IsolatedEnv(TProp properties)
        {
            slnProperties = properties;
        }

        /// <summary>
        /// Gets project name from Microsoft.Build.Evaluation.Project
        /// </summary>
        /// <param name="eProject"></param>
        /// <returns></returns>
        protected virtual string getProjectNameFrom(Project eProject)
        {
            return eProject.GetPropertyValue("ProjectName");
        }

        protected TProp propertiesByDefault(TProp properties)
        {
            if(!properties.ContainsKey("Configuration"))
            {
                // ~
                if(_sln.configs.Count > 0) {
                    properties["Configuration"] = _sln.configs[0].configuration;
                }
                else {
                    properties["Configuration"] = "Release";
                }
            }

            if(!properties.ContainsKey("Platform"))
            {
                // ~
                if(_sln.configs.Count > 0) {
                    properties["Platform"] = _sln.configs[0].platform;
                }
                else {
                    properties["Platform"] = "x86";
                }
            }
            else {
                properties["Platform"] = platformName(properties["Platform"]);
            }

            return properties;
        }

        protected TProp projectProperties(Sln.Project project, TProp properties)
        {
            Log.Debug($"-> sln['{properties["Configuration"]}'; '{properties["Platform"]}']");

            if(!properties.ContainsKey("Configuration") || !properties.ContainsKey("Platform")) {
                Log.Warn("Solution Configuration & Platform are not defined.");
                return properties;
            }
            TProp ret = new TProp(properties);

            var cfg = _sln
                        .projectConfigs
                        .Where(c => 
                            c.pGuid == project.pGuid 
                            && c.sln.configuration == properties["Configuration"]
                            && platformName(c.sln.platform) == platformName(properties["Platform"])
                        )
                        .FirstOrDefault();

            if(cfg.configuration == null || cfg.platform == null) {
                Log.Warn($"Something went wrong with project configuration. `{cfg.configuration}|{cfg.platform}`");
                Log.Warn($"Are you sure that it's correct for your sln: `{properties["Configuration"]}|{properties["Platform"]}`");
                return properties;
                //throw new MismatchException();
            }

            string platform = platformName(cfg.platform);
            Log.Debug($"-> prj['{cfg.configuration}'; '{platform}']");

            ret["Configuration"]    = cfg.configuration;
            ret["Platform"]         = platform;

            return ret;
        }

        /// <summary>
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        protected string formatCfg(string name, string platform)
        {
            return String.Format("{0}|{1}", name, platform);
        }

        /// <summary>
        /// Rules of platform names, for example: 'Any CPU' to 'AnyCPU'
        /// see MS Connect Issue #503935 + https://bitbucket.org/3F/vssolutionbuildevent/issue/14/empty-property-outdir
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        private string platformName(string platform)
        {
            if(String.Compare(platform, "Any CPU", StringComparison.OrdinalIgnoreCase) == 0) {
                return "AnyCPU";
            }
            return platform;
        }
    }
}
