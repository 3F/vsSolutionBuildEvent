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
using System.Collections;
using System.Diagnostics;
using EnvDTE;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components.Build;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Managing of build process at runtime. And similar operations for projects and solution.
    /// </summary>
    [Component("Build", "Managing of build process at runtime.")]
    public class BuildComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "Build "; }
        }

        /// <summary>
        /// Work with DTE-Commands
        /// </summary>
        protected DTEOperation DTEO
        {
            get {
                if(dteo == null) {
                    Debug.Assert(env != null);
                    dteo = new DTEOperation(env, Events.SolutionEventType.General);
                }
                return dteo;
            }
        }
        protected DTEOperation dteo;

        /// <param name="env">Used environment</param>
        public BuildComponent(IEnvironment env)
            : base(env)
        {

        }

        /// <param name="loader">Initialize with loader</param>
        public BuildComponent(IBootloader loader)
            : base(loader)
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data.Trim());
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            IPM pm = new PM(request, msbuild);
            switch(subtype) {
                case "cancel": {
                    return stCancel(pm);
                }
                case "projects": {
                    return stProjects(pm);
                }
                case "type": {
                    return stType(pm);
                }
                case "solution": {
                    return stSolution(pm);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        /// <summary>
        /// To cancel the build task
        ///     `cancel = true`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns>found command</returns>
        [Property("cancel", "To immediately cancel the build task if it's possible.", CValueType.Void, CValueType.Boolean)]
        protected string stCancel(IPM pm)
        {
            if(!pm.It(LevelType.Property, "cancel") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(Value.toBoolean(pm.Levels[0].Data)) {
                Log.Debug("attempt to cancel the build");
                DTEO.exec("Build.Cancel");
            }

            return Value.Empty;
        }

        /// <summary>
        /// Work with configuration manager of projects through SolutionContexts.
        /// 
        /// http://msdn.microsoft.com/en-us/library/EnvDTE.Configuration_properties.aspx
        /// http://msdn.microsoft.com/en-us/library/envdte.solutioncontext.aspx
        /// 
        /// Sample: 
        ///  #[Build projects]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("projects", "Work with configuration manager of projects through SolutionContexts.")]
        protected string stProjects(IPM pm)
        {
            if(!pm.It(LevelType.Property, "projects")) {
                throw new IncorrectNodeException(pm);
            }

            if(pm.Is(LevelType.Method, "find")) {
                return stProjectsFind(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// .find(...) level
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method(
                "find",
                "To find project by name. It compares part of name, therefore you can use simply like a \"ZenLib\" or full name \"Zenlib\\ZenLib.vcxproj\" etc.",
                "projects",
                "stProjects",
                new string[] { "name" },
                new string[] { "Project name" },
                CValueType.Void,
                CValueType.String
        )]
        protected string stProjectsFind(IPM pm)
        {
            ILevel level = pm.Levels[0]; // level of the `find` property

            if(level.Is(ArgumentType.StringDouble)) {
                string name = (string)level.Args[0].data;
                return projectCfg(getContextByProject(name), pm.pinTo(1));
            }

            throw new ArgumentPMException(level, "find(string name)");
        }

        /// <summary>
        /// Configuration level of project.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property(
            "IsBuildable",
            "Gets or Sets. Whether the project or item configuration of project can be built.\nAssociated with current SolutionContext.", 
            "find",
            "stProjectsFind", 
            CValueType.Boolean, 
            CValueType.Boolean
        )]
        [Property(
            "IsDeployable",
            "Gets or Sets. Whether the current project or item configuration of project can be deployed.\nAssociated with current SolutionContext.", 
            "find",
            "stProjectsFind", 
            CValueType.Boolean, 
            CValueType.Boolean
        )]
        protected string projectCfg(SolutionContext context, IPM pm)
        {
            Debug.Assert(context != null);

            if(pm.It(LevelType.Property, "IsBuildable"))
            {
                if(pm.IsRight(LevelType.RightOperandStd)) {
                    context.ShouldBuild = Value.toBoolean(pm.Levels[0].Data);
                    return Value.Empty;
                }

                if(pm.IsRight(LevelType.RightOperandEmpty)) {
                    return Value.from(context.ShouldBuild);
                }
            }

            if(pm.It(LevelType.Property, "IsDeployable"))
            {
                if(pm.IsRight(LevelType.RightOperandStd)) {
                    context.ShouldDeploy = Value.toBoolean(pm.Levels[0].Data);
                    return Value.Empty;
                }

                if(pm.IsRight(LevelType.RightOperandEmpty)) {
                    return Value.from(context.ShouldDeploy);
                }
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Work with type of build action
        /// Sample: #[Build type]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns>current type</returns>
        [Property("type", "Get type of current build action, or last used type if it already finished.", CValueType.Enum, CValueType.Void)]
        protected string stType(IPM pm)
        {
            if(!pm.It(LevelType.Property, "type") || !pm.IsRight(LevelType.RightOperandEmpty)) {
                throw new IncorrectNodeException(pm);
            }

            return Value.from(env.BuildType);
        }

        /// <summary>
        /// Work with solution node.
        /// Sample: #[Build solution]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("solution", "Work with solution data.")]
        [Property("current", "Link to current used solution.", "solution", "stSolution"), Property("", "current", "stSolution")]
        [Method("path",
                "Use specific solution from selected path.",
                "solution",
                "stSolution",
                new string[] { "sln" },
                new string[] { "Full path to solution file." },
                CValueType.Void,
                CValueType.String), Property("", "path", "stSolution")]
        protected string stSolution(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "solution")) {
                throw new IncorrectNodeException(pm);
            }

            // solution.current.
            if(pm.Is(1, LevelType.Property, "current"))
            {
                if(!env.IsOpenedSolution) {
                    throw new NotSupportedOperationException("Property 'current' is not available. Open the Solution or use 'path()' method instead.");
                }
                return stSlnPMap(env.SolutionFile, pm.pinTo(2));
            }

            // solution.path("file").
            if(pm.Is(1, LevelType.Method, "path"))
            {
                ILevel lvlPath = pm.Levels[1];
                lvlPath.Is("solution.path(string sln)", ArgumentType.StringDouble);
                return stSlnPMap((string)lvlPath.Args[0].data, pm.pinTo(2));
            }

            throw new IncorrectNodeException(pm, 1);
        }

        /// <summary>
        /// Work with ProjectsMap of solution node.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("First", "First project in Project Build Order.", "", "stSolution"), Property("", "First", "stSlnPMap")]
        [Property("Last", "Last project in Project Build Order.", "", "stSolution"), Property("", "Last", "stSlnPMap")]
        [Property("FirstRaw", "First project from defined list.\nIgnores used Build type.", "", "stSolution"), Property("", "FirstRaw", "stSlnPMap")]
        [Property("LastRaw", "Last project from defined list.\nIgnores used Build type.", "", "stSolution"), Property("", "LastRaw", "stSlnPMap")]
        [Property("GuidList", "Get list of project Guids.\nIn direct order of definition.", "", "stSolution", CValueType.List)]
        [Method("projectBy",
                "Get project by Guid string.",
                "",
                "stSolution",
                new string[] { "guid" },
                new string[] { "Identifier of project." },
                CValueType.Void,
                CValueType.String), Property("", "projectBy", "stSlnPMap")]
        protected string stSlnPMap(string sln, IPM pm)
        {
            if(String.IsNullOrWhiteSpace(sln)) {
                throw new InvalidArgumentException("Failed stSlnPMap: sln is empty");
            }
            ProjectsMap map = getProjectsMap(sln);

            if(pm.Is(LevelType.Property, "First")) {
                return projectsMap(map.FirstBy(env.BuildType), pm.pinTo(1));
            }

            if(pm.Is(LevelType.Property, "Last")) {
                return projectsMap(map.LastBy(env.BuildType), pm.pinTo(1));
            }

            if(pm.Is(LevelType.Property, "FirstRaw")) {
                return projectsMap(map.First, pm.pinTo(1));
            }

            if(pm.Is(LevelType.Property, "LastRaw")) {
                return projectsMap(map.Last, pm.pinTo(1));
            }

            if(pm.FinalEmptyIs(LevelType.Property, "GuidList")) {
                return Value.from(map.GuidList);
            }

            if(pm.Is(LevelType.Method, "projectBy"))
            {
                ILevel lvlPrjBy = pm.Levels[0];
                lvlPrjBy.Is("projectBy(string guid)", ArgumentType.StringDouble);
                return projectsMap(map.getProjectBy((string)lvlPrjBy.Args[0].data), pm.pinTo(1));
            }

            throw new IncorrectNodeException(pm);
        }
        
        /// <summary>
        /// Unpacks ProjectsMap.Project struct for user.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("name", "The name of project.", "", "stSlnPMap", CValueType.String)]
        [Property("path", "Path to project.", "", "stSlnPMap", CValueType.String)]
        [Property("type", "Type of project.", "", "stSlnPMap", CValueType.String)]
        [Property("guid", "Guid of project.", "", "stSlnPMap", CValueType.String)]
        protected string projectsMap(ProjectsMap.Project project, IPM pm)
        {
            if(pm.FinalEmptyIs(LevelType.Property, "name")) {
                return project.name;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "path")) {
                return project.path;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "type")) {
                return project.type;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "guid")) {
                return project.guid;
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Gets instance of ProjectsMap by solution file.
        /// </summary>
        /// <param name="sln">Full path to solution file.</param>
        /// <returns></returns>
        protected virtual ProjectsMap getProjectsMap(string sln)
        {
            return new ProjectsMap(sln);
        }

        /// <param name="name">Project name</param>
        /// <returns></returns>
        protected SolutionContext getContextByProject(string name)
        {
            if(env.SolutionActiveCfg == null) {
                //TODO:
                throw new ComponentException("The SolutionActiveCfg has been disabled for current environment.");
            }

            IEnumerator slnc = env.SolutionActiveCfg.SolutionContexts.GetEnumerator();

            while(slnc.MoveNext())
            {
                var con = (SolutionContext)slnc.Current;
                if(con.ProjectName.Contains(name)) {
                    return con;
                }
            }

            throw new NotFoundException("The project '{0}' was not found.", name);
        }
    }
}
