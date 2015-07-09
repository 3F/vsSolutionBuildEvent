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
using System.Linq;
using Microsoft.Build.Evaluation;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Isolated environment for work without DTE
    /// </summary>
    public class IsolatedEnv: IEnvironment
    {
        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        public List<string> ProjectsList
        {
            get {
                return ProjectCollection.GlobalProjectCollection.LoadedProjects
                        .Where(p => p.GetPropertyValue("Configuration") == properties["Configuration"] 
                                    && p.GetPropertyValue("Platform") == properties["Platform"]
                        )
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
                Log.nlog.Debug("Accessing to property 'SolutionActiveCfg' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        public string SolutionActiveCfgString
        {
            get {
                return formatCfg(properties["Configuration"], properties["Platform"]);
            }
        }

        /// <summary>
        /// Specified type of current build action
        /// </summary>
        public BuildType BuildType
        {
            get; set;
        }

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations
        {
            get {
                //TODO: only list see in .sln -> SolutionConfigurationPlatforms
                Log.nlog.Debug("Accessing to property 'SolutionConfigurations' has been disabled in Isolated environment.");
                yield break;
            }
        }

        /// <summary>
        /// Name from the "Set as StartUp Project"
        /// </summary>
        public string StartupProjectString
        {
            get;
            protected set;
        }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        public EnvDTE.Events Events
        {
            get {
                Log.nlog.Debug("Accessing to property 'Events' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Path to solution file
        /// </summary>
        public string SolutionPath
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
                Log.nlog.Debug("Accessing to property 'Commands' has been disabled in Isolated environment.");
                return null; 
            }
        }

        /// <summary>
        /// Access to OutputWindowPane through IOW
        /// </summary>
        public IOW OutputWindowPane
        {
            get {
                Log.nlog.Debug("Accessing to property 'OutputWindowPane' has been disabled in Isolated environment.");
                return null;
            }
        }

        /// <summary>
        /// Solution properties (global properties for all project collection)
        /// </summary>
        protected Dictionary<string, string> properties = new Dictionary<string, string>();

        /// <summary>
        /// Solution data
        /// </summary>
        private SolutionParser.Result _sln;


        /// <summary>
        /// Gets instance of the Build.Evaluation.Project for accessing to properties etc.
        /// </summary>
        /// <param name="name">Specified project name. null value for project by default (~startup-project etc.)</param>
        /// <returns>Microsoft.Build.Evaluation.Project</returns>
        public virtual Project getProject(string name = null)
        {
            Log.nlog.Trace("getProject: started with '{0}'", name);

            if(String.IsNullOrEmpty(name)) {
                name = StartupProjectString;
                Log.nlog.Trace("getProject: use the StartupProject '{0}'", name);
            }
            SolutionParser.Project project = _sln.projects.Find(p => p.Name == name);

            foreach(Project eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
            {
                string pName = getProjectNameFrom(eProject);
                string pCfg  = formatCfg(eProject.GetPropertyValue("Configuration"), eProject.GetPropertyValue("Platform"));

                Log.nlog.Trace("find in projects collection: project '{0}' == '{1}', '{2}' == '{3}' [{4} = {5}]",
                                eProject.FullPath, project.FullPath, pName, name, SolutionActiveCfgString, pCfg);

                if(SolutionActiveCfgString != pCfg) {
                    continue;
                }

                if(project.FullPath == eProject.FullPath || pName == name) {
                    return eProject;
                }
            }

            Log.nlog.Trace("trying to load project :: '{0}' ('{1}')", project.Name, project.FullPath);
            if(String.IsNullOrEmpty(project.FullPath)) {
                throw new NotFoundException("Missed path to project '{0}' ['{1}', '{2}']", name, project.Name, project.Guid);
            }

            Log.nlog.Debug("-> ['{0}' ; '{1}']", properties["Configuration"], properties["Platform"]);
            return new Project(project.FullPath, properties, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <summary>
        /// Format configuration from the SolutionConfiguration2
        /// </summary>
        public string SolutionCfgFormat(EnvDTE80.SolutionConfiguration2 cfg)
        {
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

            if(name.Equals("Configuration") && properties.ContainsKey("Configuration")) {
                return properties["Configuration"];
            }

            if(name.Equals("Platform") && properties.ContainsKey("Platform")) {
                return properties["Platform"];
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
            Log.nlog.Warn("Disabled for this Environment. Command: '{0}', args: '{1}'", name, args);
        }

        /// <param name="solutionFile">Full path to solution file (.sln)</param>
        /// <param name="properties">Solution properties / global properties for all project collection</param>
        public IsolatedEnv(string solutionFile, Dictionary<string, string> properties)
        {
            SolutionPath        = Path.GetDirectoryName(solutionFile);
            SolutionFileName    = Path.GetFileNameWithoutExtension(solutionFile);

            this.properties = propertiesByDefault(properties);
            foreach(KeyValuePair<string, string> property in properties) {
                ProjectCollection.GlobalProjectCollection.SetGlobalProperty(property.Key, property.Value);
            }

            _sln = (new SolutionParser()).parse(solutionFile);
            if(String.IsNullOrEmpty(StartupProjectString) && _sln.projects.Count > 0) {
                StartupProjectString = _sln.projects[0].Name;
            }
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

        protected Dictionary<string, string> propertiesByDefault(Dictionary<string, string> properties)
        {
            if(!properties.ContainsKey("Configuration"))
            {
                // ~
                if(_sln.configs.Count > 0) {
                    properties["Configuration"] = _sln.configs[0].Configuration;
                }
                else {
                    properties["Configuration"] = "Release";
                }
            }

            if(!properties.ContainsKey("Platform"))
            {
                // ~
                if(_sln.configs.Count > 0) {
                    properties["Platform"] = _sln.configs[0].Platform;
                }
                else {
                    properties["Platform"] = "x86";
                }
            }
            else {
                // Bug with $(OutDir) - see MS Connect Issue #503935 & https://bitbucket.org/3F/vssolutionbuildevent/issue/14/empty-property-outdir
                if(properties["Platform"] == "Any CPU") {
                    properties["Platform"] = "AnyCPU";
                }
            }

            return properties;
        }

        /// <summary>
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        protected string formatCfg(string name, string platform)
        {
            return String.Format("{0}|{1}", name, platform);
        }
    }
}
