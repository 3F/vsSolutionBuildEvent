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
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.UnifiedTypes;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE
{
    public class Environment: EnvAbstract, IEnvironment, IEnvironmentExt
    {
        [Obsolete("Use " + nameof(PropertyNames), false)]
        public const string PROP_UNAV_STRING = PropertyNames.UNDEFINED;

        /// <summary>
        /// List of EnvDTE projects.
        /// </summary>
        public IEnumerable<DProject> ProjectsDTE
        {
            get => _DTEProjects;
        }

        /// <summary>
        /// List of Microsoft.Build.Evaluation projects.
        /// </summary>
        public IEnumerable<EProject> ProjectsMBE
        {
            get
            {
#if VSSDK_15_AND_NEW
                ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

                foreach(var pname in ProjectsDTE.Select(p => getProjectNameFrom(p))) {
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
            get
            {
#if VSSDK_15_AND_NEW
                ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

                try {
                    return _DTEProjects.Select(p => getProjectNameFrom(p)).ToList();
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
            get => Dte2?.Events;
        }

        /// <summary>
        /// Contains all of the commands in the environment
        /// </summary>
        public EnvDTE.Commands Commands
        {
            get => Dte2?.Commands;
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
                return (SolutionConfiguration2)Dte2?.Solution.SolutionBuild.ActiveConfiguration;
            }
        }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        public string SolutionActiveCfgString
        {
            get => SolutionCfgFormat(SolutionActiveCfg);
        }

        /// <summary>
        /// All configurations for current solution
        /// </summary>
        public IEnumerable<SolutionConfiguration2> SolutionConfigurations
        {
            get
            {
                var slnConfigs = Dte2?.Solution?.SolutionBuild?.SolutionConfigurations;

                if(slnConfigs == null || !IsOpenedSolution) {
                    yield break;
                }

                foreach(SolutionConfiguration2 cfg in slnConfigs) {
                    yield return cfg;
                }
            }
        }

        /// <summary>
        /// Project by default or "StartUp Project".
        /// </summary>
        public virtual string StartupProjectString
        {
            get
            {
                if(!IsOpenedSolution || Dte2?.Solution?.SolutionBuild?.StartupProjects == null) {
                    return null;
                }

                if(_startupProject != null) {
                    return _startupProject;
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
        private string _startupProject;

        /// <summary>
        /// Get status of opened solution.
        /// </summary>
        public bool IsOpenedSolution
        {
            get => Dte2?.Solution?.IsOpen == true;
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
        protected virtual IEnumerable<DProject> _DTEProjects
        {
            get
            {
                foreach(DProject project in DTEProjectsRaw)
                {
                    if(project.ConfigurationManager == null || project.Kind == MvsSln.Types.Kinds.PRJ_UNLOADED) {
                        Log.Trace($"Unloaded project '{project.Name}' is ignored");
                        continue; // skip for all unloaded projects
                    }
                    yield return project;
                }
            }
        }

        protected IEnumerable<DProject> DTEProjectsRaw
        {
            get
            {
                if(!IsOpenedSolution || Dte2?.Solution?.Projects == null) {
                    yield break;
                }

                foreach(DProject project in Dte2.Solution.Projects)
                {
                    if(project.Kind != MvsSln.Types.Kinds.PRJ_SLN_DIR) {
                        yield return project;
                        continue;
                    }

                    foreach(DProject subproject in listSubProjectsDTE(project)) {
                        yield return subproject;
                    }
                }
            }
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

            _startupProject = name;
            Log.Debug($"'StartUp Project' has been updated = '{name}'");
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
        public virtual EProject getProject(string name)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            DProject selected   = null;
            string startup      = StartupProjectString;

            if(name == null) {
                Log.Debug("default project is a '{0}'", startup);
            }

            foreach(DProject dteProject in _DTEProjects)
            {
                if(name == null && !String.IsNullOrEmpty(startup) && !dteProject.UniqueName.Equals(startup)) {
                    continue;
                }
                else if(name != null && !getProjectNameFrom(dteProject).Equals(name)) {
                    continue;
                }

                selected = dteProject;
                Log.Trace("selected = dteProject: '{0}'", dteProject.FullName);

                foreach(EProject eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
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

            if(name.Equals(PropertyNames.CONFIG)) {
                return (SolutionActiveCfg == null)? PropertyNames.UNDEFINED : SolutionActiveCfg.Name;
            }

            if(name.Equals(PropertyNames.PLATFORM)) {
                return (SolutionActiveCfg == null)? PropertyNames.UNDEFINED : SolutionActiveCfg.PlatformName;
            }

            return null;
        }

        /// <param name="pHierProj"></param>
        /// <param name="force">Load in global collection with __VSHPROPID.VSHPROPID_ExtObject if true value</param>
        /// <returns>project name from Microsoft.Build.Evaluation rules or null value if project not found in GlobalProjectCollection and force value is false</returns>
        public string getProjectNameFrom(IVsHierarchy pHierProj, bool force = false)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            pHierProj.GetGuidProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid id);

            foreach(EProject eProject in ProjectCollection.GlobalProjectCollection.LoadedProjects)
            {
                string guidString = eProject.GetPropertyValue(PropertyNames.PRJ_GUID);
                if(!String.IsNullOrEmpty(guidString) && id == (new Guid(guidString))) {
                    return getProjectNameFrom(eProject);
                }
            }

            if(!force) {
                return null;
            }
            pHierProj.GetProperty((uint)VSConstants.VSITEMID.Root, (int)__VSHPROPID.VSHPROPID_ExtObject, out object dteProject);
            return getProjectNameFrom(tryLoadPCollection((DProject)dteProject));
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
        protected bool isEquals(DProject dteProject, EProject eProject)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            string ePrgName         = getProjectNameFrom(eProject);
            string ePrgCfg          = eProject.GetPropertyValue(PropertyNames.CONFIG);
            string ePrgPlatform     = eProject.GetPropertyValue(PropertyNames.PLATFORM);

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
        protected EProject tryLoadPCollection(DProject dteProject)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            Dictionary<string, string> prop = getGlobalProperties(dteProject);

            Log.Debug("tryLoadPCollection :: '{0}' [{1} ; {2}]", dteProject.FullName, prop[PropertyNames.CONFIG], prop[PropertyNames.PLATFORM]);
            //ProjectCollection.GlobalProjectCollection.LoadProject(dteProject.FullName, prop, null);
            return new EProject(dteProject.FullName, prop, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <summary>
        /// Gets project name from EnvDTE.Project
        /// </summary>
        /// <param name="dteProject"></param>
        /// <returns></returns>
        protected virtual string getProjectNameFrom(DProject dteProject)
        {
            //return dteProject.Name; // can be as 'AppName' and 'AppName_2013' for different .sln

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            IVsSolution sln = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            sln.GetProjectOfUniqueName(dteProject.FullName, out IVsHierarchy hr);

            string projectName = getProjectNameFrom(hr, false);
            if(!String.IsNullOrEmpty(projectName)) {
                return projectName;
            }
            return getProjectNameFrom(tryLoadPCollection(dteProject));
        }

        protected Dictionary<string, string> getGlobalProperties(DProject dteProject)
        {
            var prop = new Dictionary<string, string>(ProjectCollection.GlobalProjectCollection.GlobalProperties);

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            if(!prop.ContainsKey(PropertyNames.CONFIG))
            {
                prop[PropertyNames.CONFIG] = cfgRule.Configuration(
                    dteProject.ConfigurationManager.ActiveConfiguration.ConfigurationName
                );
            }

            if(!prop.ContainsKey(PropertyNames.PLATFORM))
            {
                prop[PropertyNames.PLATFORM] = cfgRule.Platform(
                    dteProject.ConfigurationManager.ActiveConfiguration.PlatformName
                );
            }
            
            if(!prop.ContainsKey(PropertyNames.VS_BUILD)) {
                prop[PropertyNames.VS_BUILD] = "true";
            }

            if(!prop.ContainsKey(PropertyNames.DEVENV_DIR))
            {
                // http://technet.microsoft.com/en-us/microsoft.visualstudio.shell.interop.__vsspropid%28v=vs.71%29.aspx

                IVsShell shell = (IVsShell)Package.GetGlobalService(typeof(SVsShell));
                shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out object dirObject);

                string dir = (string)dirObject;

                if(String.IsNullOrEmpty(dir)) {
                    prop[PropertyNames.DEVENV_DIR] = PropertyNames.UNDEFINED;
                }
                else {
                    prop[PropertyNames.DEVENV_DIR] = dir.DirectoryPathFormat();
                }
            }

            if(!prop.ContainsKey(PropertyNames.SLN_DIR)  
               || !prop.ContainsKey(PropertyNames.SLN_NAME)
               || !prop.ContainsKey(PropertyNames.SLN_FNAME)
               || !prop.ContainsKey(PropertyNames.SLN_EXT)
               || !prop.ContainsKey(PropertyNames.SLN_PATH))
            {
                IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                solution.GetSolutionInfo(out string dir, out string file, out string opts);

                var _s = file.GetFileProperties();

                prop[PropertyNames.SLN_DIR]     = _s[PropertyNames.SLN_DIR];
                prop[PropertyNames.SLN_NAME]    = _s[PropertyNames.SLN_NAME];
                prop[PropertyNames.SLN_FNAME]   = _s[PropertyNames.SLN_FNAME];
                prop[PropertyNames.SLN_EXT]     = _s[PropertyNames.SLN_EXT];
                prop[PropertyNames.SLN_PATH]    = _s[PropertyNames.SLN_PATH];
            }

            if(!prop.ContainsKey(PropertyNames.CODE_ANAL_ORUN)) {
                prop[PropertyNames.CODE_ANAL_ORUN] = "false";
            }

            return prop;
        }

        protected IEnumerable<DProject> listSubProjectsDTE(DProject project)
        {
            foreach(EnvDTE.ProjectItem item in project.ProjectItems)
            {
                if(item.SubProject == null) {
                    continue; //e.g. project is incompatible with used version of visual studio
                }

                if(item.SubProject.Kind != MvsSln.Types.Kinds.PRJ_SLN_DIR) {
                    yield return item.SubProject;
                    continue;
                }

                foreach(DProject subproject in listSubProjectsDTE(item.SubProject)) {
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
