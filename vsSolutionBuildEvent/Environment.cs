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
using System.IO;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.EvMSBuild;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.API;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UnifiedTypes;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE
{
    // TODO: more unified integration with IsolatedEnv /EnvDTE to MvsSln as possible 
    //       ~such as full processing by MvsSln through information about .sln file from EnvDTE.
    public class Environment: EnvAbstract, IEnvironment, IEnvironmentExt, IEvEnv
    {
        [Obsolete("Use " + nameof(PropertyNames), false)]
        public const string PROP_UNAV_STRING = PropertyNames.UNDEFINED;

        /// <summary>
        /// VS treats a single file (.dmp etc) when its starting as an open solution.
        /// </summary>
        internal const string DTE_DOC_SLN = "EnvDteDocumentSln";

        protected IEventLevel elvl;

        private string startupProject;

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
            get => ProjectsDTE.Select(p => p.GetXProject(SlnEnv, true)?.Project);
        }

        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        public List<string> ProjectsList
        {
            get
            {
                try
                {
                    return ProjectsDTE
                            .Select(p => getProjectNameFrom(p, true))
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList();
                }
                catch(Exception ex) {
                    Log.Error($"Failed getting project from EnvDTE: {ex.Message}");
                    Log.Debug(ex.StackTrace);
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
                return (SolutionConfiguration2)Dte2?.Solution?.SolutionBuild?.ActiveConfiguration;
            }
        }

        /// <summary>
        /// Formatted string with an active configuration for current solution.
        /// </summary>
        public string SolutionActiveCfgString
        {
            get => SolutionCfgFormat(SolutionActiveCfg);
        }

        /// <summary>
        /// All configurations for current solution.
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
        /// Project Name by default or "StartUp Project".
        /// </summary>
        public override string StartupProjectString
        {
            get
            {
                if(!IsOpenedSolution) {
                    return null;
                }

                var _st = Dte2?.Solution?.SolutionBuild?.StartupProjects;

                if(_st == null || startupProject != null) {
                    return startupProject;
                }

                // `Dte2.Solution.SolutionBuild.StartupProjects` represents relative path to project file:
                //  [0] "miniupnpc\\miniupnpc.vcxproj"   object {string}
                //  [0] "StrongDC.vcxproj"   object {string}
                // https://docs.microsoft.com/en-us/dotnet/api/envdte.solutionbuild.startupprojects

                foreach(string project in (Array)_st)
                {
                    if(!string.IsNullOrEmpty(project))
                    {
                        return Sln?.ProjectItems?
                                .FirstOrDefault(p => p.path == project)
                                .name;
                    }
                }

                return null;
            }

            protected set => updateStartupProject(value);
        }

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
        public override string SolutionFile
        {
            // the state of the DTE2 object are always should be in modification
            get => getFullPathToSln(Dte2);
            protected set => throw new NotSupportedException("Not available for EnvDTE mode");
        }

        /// <summary>
        /// Full path to directory where placed solution file.
        /// </summary>
        public string SolutionPath
        {
            get => Path.GetDirectoryName(SolutionFile);
        }

        /// <summary>
        /// Name of used solution file without extension.
        /// </summary>
        public string SolutionFileName
        {
            get => Path.GetFileNameWithoutExtension(SolutionFile);
        }

        /// <summary>
        /// Access to OutputWindowPane through IOW.
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
        } protected IOW outputWindowPane;

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
                        continue;
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
        /// To update the Project Name by default aka "StartUp Project".
        /// </summary>
        /// <param name="name">Uses default behavior if empty or null.</param>
        public void updateStartupProject(string name)
        {
            // null value will unlock accessing to SolutionBuild.StartupProjects above
            startupProject = (name == string.Empty) ? null : name;
            Log.Debug($"'StartUp Project' has been updated = '{startupProject}'");
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

            if(name.Equals(PropertyNames.CONFIG)) {
                return (SolutionActiveCfg == null)? PropertyNames.UNDEFINED : SolutionActiveCfg.Name;
            }

            if(name.Equals(PropertyNames.PLATFORM)) {
                return (SolutionActiveCfg == null)? PropertyNames.UNDEFINED : SolutionActiveCfg.PlatformName;
            }

            return null;
        }

        /// <summary>
        /// Gets project name from IVsHierarchy.
        /// </summary>
        /// <param name="pHierProj"></param>
        /// <param name="force">Load in global collection with __VSHPROPID.VSHPROPID_ExtObject if true.</param>
        /// <returns></returns>
        public string getProjectNameFrom(IVsHierarchy pHierProj, bool force = false)
        {
            return getProjectNameFrom(pHierProj.GetEnvDteProject(), force);
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
        /// <param name="elvl"></param>
        public Environment(DTE2 dte2, IEventLevel elvl = null)
        {
            Dte2        = dte2;
            this.elvl   = elvl;

            //TODO: ?disposing whole environment

            if(elvl != null) {
                elvl.ClosedSolution += onClosedSolution; 
            }
        }

        protected override void UpdateSlnEnv(ISlnResult sln) => AssignEnv(new MvsSln.Core.IsolatedEnv(sln));

        protected virtual string getProjectNameFrom(DProject dteProject, bool force = false)
        {
            return dteProject?.GetProjectName(SlnEnv, force);
        }

        protected IEnumerable<DProject> listSubProjectsDTE(DProject project)
        {
            foreach(EnvDTE.ProjectItem item in project.ProjectItems)
            {
                if(item.SubProject == null) {
                    continue; //eg. project is incompatible with used version of visual studio
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
            if(dte2 == null)
            {
                return null;
            }

            string path = dte2.Solution?.FullName; // can be empty when new solution is creating
            if(!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            try
            {
                if(dte2.Globals.VariableExists[DTE_DOC_SLN] && dte2.Globals[DTE_DOC_SLN] != null)
                {
                    // see Pkg.OnAfterOpenSolution
                    return ((EnvDTE.Document)dte2.Globals[DTE_DOC_SLN]).FullName;
                }

                // VS may throw an exception when accessing to "Path" for such .dmp file (no .sln),
                // try/catch is official way https://docs.microsoft.com/en-us/dotnet/api/envdte80.solution2.properties

                return dte2.Solution.Properties.Item("Path").Value.ToString();
            }
            catch(Exception ex)
            {
                Log.Trace($"{nameof(getFullPathToSln)} returns null because of {ex.Message}");
            }

            return null;
        }

        private void onClosedSolution(object sender, EventArgs e)
        {
            ((IDisposable)_slnEnv)?.Dispose();
        }
    }
}
