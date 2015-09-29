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
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
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
    /// Component of operations with the build
    /// </summary>
    [Component("Build", "Operations with build processes.")]
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

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data.Trim(), @"^\[Build
                                                    \s+
                                                    (                  #1 - full ident
                                                      ([A-Za-z_0-9]+)  #2 - subtype
                                                      .*
                                                    )
                                                 \]$", 
                                                 RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed BuildComponent - '{0}'", data);
            }
            string ident    = m.Groups[1].Value;
            string subtype  = m.Groups[2].Value;

            switch(subtype) {
                case "cancel": {
                    Log.Trace("BuildComponent: use stCancel");
                    return stCancel(ident);
                }
                case "projects": {
                    // is meant the SolutionContexts !
                    Log.Trace("BuildComponent: use stProjects");
                    return stProjects(ident);
                }
                case "type": {
                    Log.Trace("BuildComponent: use stType");
                    return stType(ident);
                }
                case "solution": {
                    Log.Trace("BuildComponent: use stSolution");
                    return stSolution(ident);
                }
            }
            throw new SubtypeNotFoundException("BuildComponent: not found subtype - '{0}'", subtype);
        }

        /// <summary>
        /// The Cancel operation
        /// Sample: #[Build cancel = true]
        /// </summary>
        /// <param name="data"></param>
        /// <returns>found command</returns>
        [Property("cancel", "Cancel build immediately if true.", CValueType.Void, CValueType.Boolean)]
        protected string stCancel(string data)
        {
            Match m = Regex.Match(data, @"cancel\s*=\s*(false|true|1|0)\s*$");
            if(!m.Success) {
                throw new OperationNotFoundException("Failed stCancel - '{0}'", data);
            }
            string val = m.Groups[1].Value;
            Log.Debug("stCancel: value is {0}", val);

            if(Value.toBoolean(val))
            {
                Log.Trace("stCancel: pushed true");
                DTEO.exec("Build.Cancel");
            }
            return String.Empty;
        }

        /// <summary>
        /// Work with configuration manager of projects through the SolutionContexts
        /// 
        /// http://msdn.microsoft.com/en-us/library/EnvDTE.Configuration_properties.aspx
        /// http://msdn.microsoft.com/en-us/library/envdte.solutioncontext.aspx
        /// 
        /// Therefore operand 'find' used for permanent getting as - contains or not ?
        /// Values for example:
        /// * "bzip2.vcxproj"
        /// * "Zenlib\ZenLib.vcxproj"
        /// 
        /// Sample: 
        ///  #[Build projects.find("name")]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Property("projects", "Work with configuration manager of projects through SolutionContexts.")]
        protected string stProjects(string data)
        {
            Match m = Regex.Match(data,
                                    String.Format(@"projects
                                                    \s*\.\s*
                                                    find
                                                    \s*
                                                    \({0}\)    #1 - project name
                                                    \s*(.+)    #2 - operation
                                                    ",
                                                    RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stProjects - '{0}'", data);
            }

            string project      = m.Groups[1].Value;
            string operation    = m.Groups[2].Value;

            if(env.SolutionActiveCfg == null) {
                //TODO:
                throw new ComponentException("Disabled for this environment");
            }

            SolutionContext context = null;
            IEnumerator slnc        = env.SolutionActiveCfg.SolutionContexts.GetEnumerator();

            while(slnc.MoveNext()) {
                SolutionContext item = (SolutionContext)slnc.Current;
                if(((SolutionContext)slnc.Current).ProjectName.Contains(project)) {
                    context = item;
                    break;
                }
            }
            if(context == null) {
                throw new NotFoundException("Not found project - '{0}'", project);
            }
            return stProjectConf(context, operation);
        }

        /// <summary>
        /// Work with item of SolutionContexts
        /// 
        /// Samples:
        ///  #[Build projects.find("name").IsBuildable = true]
        ///  #[Build projects.find("name").IsBuildable]
        ///  #[Build projects.find("name").IsDeployable = true]
        ///  #[Build projects.find("name").IsDeployable]
        /// </summary>
        /// <param name="context">Working SolutionContext</param>
        /// <param name="data">String data with operations</param>
        /// <returns></returns>
        [
            Method
            (
                "find", 
                "The find() compares as part of name, and you can use simply like a find(\"ZenLib\") or for unique identification full \"Zenlib\\ZenLib.vcxproj\" etc.",
                "projects",
                "stProjects",
                new string[] { "name" }, 
                new string[] { "Project name" },
                CValueType.Void,
                CValueType.String
            ),
        ]
        [Property(
            "IsBuildable",
            "Gets or Sets. Whether the project or item configuration of project can be built.\nAssociated with current SolutionContext.", 
            "find", 
            "stProjectConf", 
            CValueType.Boolean, 
            CValueType.Boolean
        )]
        [Property(
            "IsDeployable",
            "Gets or Sets. Whether the current project or item configuration of project can be deployed.\nAssociated with current SolutionContext.", 
            "find", 
            "stProjectConf", 
            CValueType.Boolean, 
            CValueType.Boolean
        )]
        protected string stProjectConf(SolutionContext context, string data)
        {
            Debug.Assert(context != null);

            Match m = Regex.Match(data, @"\.\s*
                                          ([A-Za-z_0-9]+)            #1 - property
                                          (?:
                                            \s*=\s*(false|true|1|0)  #2 - value (optional)
                                           |
                                            \s*$
                                          )", RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed stProjectConf - '{0}'", data);
            }
            string property = m.Groups[1].Value;
            string value    = (m.Groups[2].Success)? m.Groups[2].Value : null;

            Log.Debug("stProjectConf: property - '{0}', value - '{1}'", property, value);
            switch(property) {
                case "IsBuildable":
                {
                    if(value != null) {
                        context.ShouldBuild = Value.toBoolean(value);
                        return String.Empty;
                    }
                    return Value.from(context.ShouldBuild);
                }
                case "IsDeployable":
                {
                    if(value != null) {
                        context.ShouldDeploy = Value.toBoolean(value);
                        return String.Empty;
                    }
                    return Value.from(context.ShouldDeploy);
                }
            }
            throw new OperationNotFoundException("stProjectConf: not found property - '{0}'", property);
        }

        /// <summary>
        /// Work with type of build action
        /// Sample: #[Build type]
        /// </summary>
        /// <param name="data"></param>
        /// <returns>current type</returns>
        [Property("type", "Get type of current build action, or last used type if it already finished.", CValueType.Enum, CValueType.Void)]
        protected string stType(string data)
        {
            if(!Regex.Match(data, @"type\s*$").Success) {
                throw new OperationNotFoundException("Failed stType - '{0}'", data);
            }
            return env.BuildType.ToString();
        }

        /// <summary>
        /// Work with solution node.
        /// Sample: #[Build solution]
        /// </summary>
        /// <param name="data"></param>
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
        protected string stSolution(string data)
        {
            Log.Trace("stSolution: started with '{0}'", data);
            IPM pm = new PM(data);
            
            if(!pm.Is(0, LevelType.Property, "solution")) {
                throw new SyntaxIncorrectException("Failed stSolution - '{0}'", data);
            }

            // solution.current.
            if(pm.Is(1, LevelType.Property, "current")) {
                return stSlnPMap(env.SolutionFile, pm.pinTo(2));
            }

            // solution.path("file").
            if(pm.Is(1, LevelType.Method, "path"))
            {
                Argument[] args = pm.Levels[1].Args;
                if(args.Length != 1 || args[0].type != ArgumentType.StringDouble) {
                    throw new InvalidArgumentException("stSolution: incorrect arguments to `solution.path(string sln)`");
                }
                return stSlnPMap((string)args[0].data, pm.pinTo(2));
            }
            
            throw new OperationNotFoundException("stSolution: not found - '{0}' /'{1}'", pm.Levels[1].Data, pm.Levels[1].Type);
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

            if(pm.Is(0, LevelType.Property, "First")) {
                return projectsMap(map.FirstBy(env.BuildType), pm.pinTo(1));
            }

            if(pm.Is(0, LevelType.Property, "Last")) {
                return projectsMap(map.LastBy(env.BuildType), pm.pinTo(1));
            }

            if(pm.Is(0, LevelType.Property, "FirstRaw")) {
                return projectsMap(map.First, pm.pinTo(1));
            }

            if(pm.Is(0, LevelType.Property, "LastRaw")) {
                return projectsMap(map.Last, pm.pinTo(1));
            }

            if(pm.FinalEmptyIs(0, LevelType.Property, "GuidList")) {
                return Value.from(map.GuidList);
            }

            if(pm.Is(0, LevelType.Method, "projectBy"))
            {
                Argument[] args = pm.Levels[0].Args;
                if(args.Length != 1 || args[0].type != ArgumentType.StringDouble) {
                    throw new InvalidArgumentException("stSlnPMap: incorrect arguments to `projectBy(string guid)`");
                }
                return projectsMap(map.getProjectBy((string)args[0].data), pm.pinTo(1));
            }

            throw new OperationNotFoundException("stSlnPMap: not found - '{0}' /'{1}'", pm.Levels[0].Data, pm.Levels[0].Type);
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
            if(pm.FinalEmptyIs(0, LevelType.Property, "name")) {
                return project.name;
            }

            if(pm.FinalEmptyIs(0, LevelType.Property, "path")) {
                return project.path;
            }

            if(pm.FinalEmptyIs(0, LevelType.Property, "type")) {
                return project.type;
            }

            if(pm.FinalEmptyIs(0, LevelType.Property, "guid")) {
                return project.guid;
            }

            throw new OperationNotFoundException("Failed projectsMap - '{0}' /'{1}'", pm.Levels[0].Data, pm.Levels[0].Type);
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
    }
}
