/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.API.Commands;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.UnifiedTypes;

namespace net.r_eg.vsSBE
{
    public class Environment: IEnvironment, IEnvironmentExt
    {
        /// <summary>
        /// Marking of unavailable property.
        /// </summary>
        public const string PROP_UNAV_STRING = "*Undefined*";

        /// <summary>
        /// List of EnvDTE projects.
        /// </summary>
        public IEnumerable<EnvDTE.Project> ProjectsDTE
        {
            get {
                return _DTEProjects;
            }
        }

        /// <summary>
        /// List of Microsoft.Build.Evaluation projects.
        /// </summary>
        public IEnumerable<Project> ProjectsMBE
        {
            get
            {
                foreach(var pname in ProjectsDTE.Select(p => getProjectNameFrom(p))) {
                    if(String.IsNullOrEmpty(pname)) {
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
            get
            {
                try {
                    return _DTEProjects.Select(p => getProjectNameFrom(p)).ToList<string>();
                }
                catch(Exception ex) {
                    Log.Error("Failed getting project from EnvDTE: {0}", ex.Message);
                }
                return new List<string>();
            }
        }

        /// <summary>
        /// DTE2 context.
        /// </summary>
        public DTE2 Dte2
        {
            get;
            protected set;
        }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        public EnvDTE.Events Events
        {
            get { return (Dte2 == null)? null : Dte2.Events; }
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
        /// Contains all of the commands in the environment
        /// </summary>
        public EnvDTE.Commands Commands
        {
            get { return (Dte2 == null)? null : Dte2.Commands; }
        }

        /// <summary>
        /// Active configuration for current solution
        /// </summary>
        public SolutionConfiguration2 SolutionActiveCfg
        {
            get
            {
                if(!IsOpenedSolution) {
                    return null;
                }
                return (SolutionConfiguration2)Dte2.Solution.SolutionBuild.ActiveConfiguration;
            }
        }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        public string SolutionActiveCfgString
        {
            get {
                return SolutionCfgFormat(SolutionActiveCfg);
            }
        }

        /// <summary>
        /// Specified type of current build action
        /// </summary>
        public BuildType BuildType
        {
            get { return buildType; }
            set { buildType = value; }
        }
        protected BuildType buildType = BuildType.Common;

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<SolutionConfiguration2> SolutionConfigurations
        {
            get
            {
                if(!IsOpenedSolution) {
                    yield break;
                }

                foreach(SolutionConfiguration2 cfg in Dte2.Solution.SolutionBuild.SolutionConfigurations) {
                    yield return cfg;
                }
            }
        }

        /// <summary>
        /// Getting name from "Set as StartUp Project"
        /// </summary>
        public virtual string StartupProjectString
        {
            get
            {
                if(!IsOpenedSolution || Dte2.Solution.SolutionBuild.StartupProjects == null) {
                    return null;
                }

                foreach(string project in (Array)Dte2.Solution.SolutionBuild.StartupProjects)
                {
                    if(!String.IsNullOrEmpty(project)) {
                        return project;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Get status of opened solution.
        /// </summary>
        public bool IsOpenedSolution
        {
            get
            {
                if(Dte2 == null) {
                    return false;
                }
                return Dte2.Solution.IsOpen;
            }
        }

        /// <summary>
        /// Full path to solution file.
        /// </summary>
        public string SolutionFile
        {
            get {
                // the state of the DTE2 object are always should be in modification
                return getFullPathToSln(Dte2);
            }
        }

        /// <summary>
        /// Full path to directory where placed solution file.
        /// </summary>
        public string SolutionPath
        {
            get {
                return Path.GetDirectoryName(SolutionFile);
            }
        }

        /// <summary>
        /// Name of used solution file without extension
        /// </summary>
        public string SolutionFileName
        {
            get {
                return Path.GetFileNameWithoutExtension(getFullPathToSln(Dte2));
            }
        }

        /// <summary>
        /// Access to OutputWindowPane through IOW
        /// </summary>
        public IOW OutputWindowPane
        {
            get
            {
                if(outputWindowPane == null) {
                    outputWindowPane = new OWP(Dte2);
                }
                return outputWindowPane;
            }
        }
        protected IOW outputWindowPane;

        /// <summary>
        /// Getting projects from DTE
        /// </summary>
        protected virtual IEnumerable<EnvDTE.Project> _DTEProjects
        {
            get
            {
                foreach(EnvDTE.Project project in DTEProjectsRaw)
                {
                    if(project.Kind == "{67294A52-A4F0-11D2-AA88-00C04F688DDE}" || project.ConfigurationManager == null) {
                        Log.Trace("Unloaded project '{0}' has ignored", project.Name);
                        continue; // skip for all unloaded projects
                    }
                    yield return project;
                }
            }
        }

        protected IEnumerable<EnvDTE.Project> DTEProjectsRaw
        {
            get
            {
                if(!IsOpenedSolution) {
                    yield break;
                }

                foreach(EnvDTE.Project project in Dte2.Solution.Projects)
                {
                    if(project.Kind != ProjectKinds.vsProjectKindSolutionFolder) {
                        yield return project;
                        continue;
                    }

                    foreach(EnvDTE.Project subproject in listSubProjectsDTE(project)) {
                        yield return subproject;
                    }
                }
            }
        }

        /// <summary>
        /// Gets instance of the Build.Evaluation.Project for accessing to properties etc.
        /// 
        /// Uses the 'StartUp Project' from list or first with the same Configuration|Platform if the name contains the null value.
        /// Note: we work with DTE because the ProjectCollection.GlobalProjectCollection can be is empty
        /// https://bitbucket.org/3F/vssolutionbuildevent/issue/8/
        /// </summary>
        /// <param name="name">Project name</param>
        /// <returns>Microsoft.Build.Evaluation.Project</returns>
        public virtual Project getProject(string name = null)
        {
            EnvDTE.Project selected = null;
            string startup          = StartupProjectString;

            if(name == null) {
                Log.Debug("default project is a '{0}'", startup);
            }

            foreach(EnvDTE.Project dteProject in _DTEProjects)
            {
                if(name == null && !String.IsNullOrEmpty(startup) && !dteProject.UniqueName.Equals(startup)) {
                    continue;
                }
                else if(name != null && !getProjectNameFrom(dteProject).Equals(name)) {
                    continue;
                }
                selected = dteProject;
                Log.Trace("selected = dteProject: '{0}'", dteProject.FullName);

                foreach(Project eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
                {
                    if(isEquals(dteProject, eProject)) {
                        return eProject;
                    }
                }
                break; // selected & LoadedProjects is empty
            }

            if(selected != null) {
                Log.Debug("getProject->selected '{0}'", selected.FullName);
                return tryLoadPCollection(selected);
            }
            throw new NotFoundException("The project '{0}' was not found. [startup: '{1}']", name, startup);
        }

        /// <summary>
        /// Getting global(general for all existing projects) property
        /// </summary>
        /// <param name="name">Property name</param>
        public string getSolutionProperty(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                return null;
            }

            if(name.Equals("Configuration")) {
                return (SolutionActiveCfg == null)? PROP_UNAV_STRING : SolutionActiveCfg.Name;
            }

            if(name.Equals("Platform")) {
                return (SolutionActiveCfg == null)? PROP_UNAV_STRING : SolutionActiveCfg.PlatformName;
            }

            return null;
        }

        /// <summary>
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        public string SolutionCfgFormat(SolutionConfiguration2 cfg)
        {
            if(cfg == null) {
                return String.Format("{0}|{0}", PROP_UNAV_STRING);
            }
            return String.Format("{0}|{1}", cfg.Name, cfg.PlatformName);
        }

        /// <param name="pHierProj"></param>
        /// <param name="force">Load in global collection with __VSHPROPID.VSHPROPID_ExtObject if true value</param>
        /// <returns>project name from Microsoft.Build.Evaluation rules or null value if project not found in GlobalProjectCollection and force value is false</returns>
        public string getProjectNameFrom(IVsHierarchy pHierProj, bool force = false)
        {
            Guid id;
            pHierProj.GetGuidProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out id);

            foreach(Project eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
            {
                string guidString = eProject.GetPropertyValue("ProjectGuid");
                if(!String.IsNullOrEmpty(guidString) && id == (new Guid(guidString))) {
                    return getProjectNameFrom(eProject);
                }
            }

            if(!force) {
                return null;
            }
            object dteProject;
            pHierProj.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out dteProject);
            return getProjectNameFrom(tryLoadPCollection((EnvDTE.Project)dteProject));
        }

        /// <summary>
        /// Execute command with DTE
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="args">Command arguments</param>
        public virtual void exec(string name, string args = "")
        {
            if(name == DTEC.BuildCancel) {
                CoreCmdSender.fire(new CoreCommandArgs() { Type = CoreCommandType.BuildCancel });
            }

            try {
                ((EnvDTE.DTE)Dte2).ExecuteCommand(name, args ?? String.Empty);
            }
            catch(OutOfMemoryException) {
                // this can be from Devenv
                Log.Debug("exec: We can't work with DTE commands at this moment in used environment. Command - '{0}'", name);
            }
        }

        /// <param name="dte2"></param>
        public Environment(DTE2 dte2)
        {
            Dte2 = dte2;
        }


        /// <summary>
        /// Checks equality of projects from different types.
        /// </summary>
        /// <param name="dteProject">The instance for EnvDTE</param>
        /// <param name="eProject">The instance for Build.Evaluation</param>
        /// <returns></returns>
        protected bool isEquals(EnvDTE.Project dteProject, Project eProject)
        {
            string ePrgName         = getProjectNameFrom(eProject);
            string ePrgCfg          = eProject.GetPropertyValue("Configuration");
            string ePrgPlatform     = eProject.GetPropertyValue("Platform");

            string dtePrgName       = getProjectNameFrom(dteProject);
            string dtePrgCfg        = dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
            string dtePrgPlatform   = dteProject.ConfigurationManager.ActiveConfiguration.PlatformName;

            Log.Trace("isEquals for '{0}' : '{1}' [{2} = {3} ; {4} = {5}]",
                            eProject.FullPath, dtePrgName, dtePrgCfg, ePrgCfg, dtePrgPlatform, ePrgPlatform);

            // see MS Connect Issue #503935 & https://bitbucket.org/3F/vssolutionbuildevent/issue/14/empty-property-outdir
            bool isEqualPlatforms = ePrgPlatform.Replace(" ", "") == dtePrgPlatform.Replace(" ", "");

            if(dtePrgName.Equals(ePrgName) && dtePrgCfg.Equals(ePrgCfg) && isEqualPlatforms)
            {
                Log.Trace("isEquals: matched");
                return true;
            }
            return false;
        }

        /// <summary>
        /// This solution for similar problems - MS Connect Issue #508628:
        /// http://connect.microsoft.com/VisualStudio/feedback/details/508628/
        /// </summary>
        protected Project tryLoadPCollection(EnvDTE.Project dteProject)
        {
            Dictionary<string, string> prop = getGlobalProperties(dteProject);

            Log.Debug("tryLoadPCollection :: '{0}' [{1} ; {2}]", dteProject.FullName, prop["Configuration"], prop["Platform"]);
            //ProjectCollection.GlobalProjectCollection.LoadProject(dteProject.FullName, prop, null);
            return new Project(dteProject.FullName, prop, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <summary>
        /// Gets project name from EnvDTE.Project
        /// </summary>
        /// <param name="dteProject"></param>
        /// <returns></returns>
        protected virtual string getProjectNameFrom(EnvDTE.Project dteProject)
        {
            //return dteProject.Name; // can be as 'AppName' and 'AppName_2013' for different .sln

            IVsSolution sln = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            IVsHierarchy hr;
            sln.GetProjectOfUniqueName(dteProject.FullName, out hr);

            string projectName = getProjectNameFrom(hr, false);
            if(!String.IsNullOrEmpty(projectName)) {
                return projectName;
            }
            return getProjectNameFrom(tryLoadPCollection(dteProject));
        }

        /// <summary>
        /// Gets project name from Microsoft.Build.Evaluation.Project
        /// </summary>
        /// <param name="eProject"></param>
        /// <returns></returns>
        protected virtual string getProjectNameFrom(Project eProject)
        {
            return eProject.GetPropertyValue("ProjectName");
            //note: we can to define as unified name for all .sln files in project file, e.g.: <PropertyGroup> <ProjectName>YourName</ProjectName> </PropertyGroup>
        }

        protected Dictionary<string, string> getGlobalProperties(EnvDTE.Project dteProject)
        {
            Dictionary<string, string> prop = new Dictionary<string, string>(ProjectCollection.GlobalProjectCollection.GlobalProperties); // copy from ProjectCollection
            
            if(!prop.ContainsKey("Configuration")) {
                prop["Configuration"] = dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName;
            }

            if(!prop.ContainsKey("Platform")) {
                prop["Platform"] = dteProject.ConfigurationManager.ActiveConfiguration.PlatformName;

                // Bug with $(OutDir) - see MS Connect Issue #503935 & https://bitbucket.org/3F/vssolutionbuildevent/issue/14/empty-property-outdir
                if(prop["Platform"] == "Any CPU") {
                    prop["Platform"] = "AnyCPU";
                }
            }
            
            if(!prop.ContainsKey("BuildingInsideVisualStudio")) {
                // by default(can be changed in other components) set as "true" in Microsoft.VisualStudio.Project.ProjectNode :: DoMSBuildSubmission & SetupProjectGlobalPropertiesThatAllProjectSystemsMustSet
                prop["BuildingInsideVisualStudio"] = "true";
            }
            if(!prop.ContainsKey("DevEnvDir"))
            {
                // http://technet.microsoft.com/en-us/microsoft.visualstudio.shell.interop.__vsspropid%28v=vs.71%29.aspx

                object dirObject = null;
                IVsShell shell = (IVsShell)Package.GetGlobalService(typeof(SVsShell));
                shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out dirObject);

                string dir = (string)dirObject;

                if(String.IsNullOrEmpty(dir)) {
                    prop["DevEnvDir"] = MSBuild.Parser.PROP_VALUE_DEFAULT;
                }
                else if(dir[dir.Length - 1] != Path.DirectorySeparatorChar) {
                    dir += Path.DirectorySeparatorChar;
                }
                prop["DevEnvDir"] = dir;
            }

            if(!prop.ContainsKey("SolutionDir")  
               || !prop.ContainsKey("SolutionName")
               || !prop.ContainsKey("SolutionFileName")
               || !prop.ContainsKey("SolutionExt")
               || !prop.ContainsKey("SolutionPath"))
            {
                string dir, file, opts;
                IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                solution.GetSolutionInfo(out dir, out file, out opts);

                string fname                = Path.GetFileName(file);
                string name                 = Path.GetFileNameWithoutExtension(file);
                string ext                  = Path.GetExtension(file);
                const string vDefault       = MSBuild.Parser.PROP_VALUE_DEFAULT;

                prop["SolutionDir"]         = (dir != null)? dir : vDefault;
                prop["SolutionName"]        = (name != null)? name : vDefault;
                prop["SolutionFileName"]    = (fname != null)? fname : vDefault;
                prop["SolutionExt"]         = (ext != null)? ext : vDefault;
                prop["SolutionPath"]        = (file != null)? file : vDefault;
            }

            if(!prop.ContainsKey("RunCodeAnalysisOnce")) {
                // by default set as "false" in Microsoft.VisualStudio.Package.GlobalPropertyHandler
                prop["RunCodeAnalysisOnce"] = "false";
            }

            return prop;
        }

        protected IEnumerable<EnvDTE.Project> listSubProjectsDTE(EnvDTE.Project project)
        {
            foreach(EnvDTE.ProjectItem item in project.ProjectItems)
            {
                if(item.SubProject == null) {
                    continue; //e.g. project is incompatible with used version of visual studio
                }

                if(item.SubProject.Kind != ProjectKinds.vsProjectKindSolutionFolder) {
                    yield return item.SubProject;
                    continue;
                }

                foreach(EnvDTE.Project subproject in listSubProjectsDTE(item.SubProject)) {
                    yield return subproject;
                }
            }
        }

        /// <summary>
        /// Gets full path to solution file.
        /// </summary>
        /// <param name="dte2">DTE2 context</param>
        /// <returns></returns>
        protected string getFullPathToSln(DTE2 dte2)
        {
            try {
                string path = dte2.Solution.FullName; // can be empty if it is the new solution
                if(String.IsNullOrWhiteSpace(path)) {
                    return dte2.Solution.Properties.Item("Path").Value.ToString();
                }
                return path;
            }
            catch(Exception ex) {
                Log.Debug("getFullPathToSln returns null: `{0}`", ex.Message);
                return null;
            }
        }
    }
}
