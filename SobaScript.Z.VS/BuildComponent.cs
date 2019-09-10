/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.VS contributors: https://github.com/3F/Varhead/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.VS.Build;

namespace net.r_eg.SobaScript.Z.VS
{
    [Component("Build", "Managing of build process at runtime.")]
    public class BuildComponent: ComponentAbstract, IComponent
    {
        protected IBuildEnv env;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "Build ";

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data.Trim());
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            IPM pm = new PM(request, emsbuild);
            switch(subtype)
            {
                case "cancel": {
                    return StCancel(pm);
                }
                case "projects": {
                    return StProjects(pm);
                }
                case "type": {
                    return StType(pm);
                }
                case "solution": {
                    return StSolution(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        /// <summary>
        /// Gets instance of ProjectsMap by solution file.
        /// </summary>
        /// <param name="sln">Full path to solution file.</param>
        /// <returns></returns>
        internal virtual ProjectsMap GetProjectsMap(string sln) => new ProjectsMap(sln);

        public BuildComponent(ISobaScript soba, IBuildEnv env)
            : base(soba)
        {
            this.env = env;
        }

        /// <summary>
        /// To cancel the build task
        ///     `cancel = true`
        /// </summary>
        /// <param name="pm"></param>
        /// <returns>found command</returns>
        [Property("cancel", "To immediately cancel the build task if it's possible.", CValType.Void, CValType.Boolean)]
        protected string StCancel(IPM pm)
        {
            if(!pm.It(LevelType.Property, "cancel") || !pm.IsRight(LevelType.RightOperandStd)) {
                throw new IncorrectNodeException(pm);
            }

            if(Value.ToBoolean(pm.FirstLevel.Data)) {
                LSender.Send(this, "attempt to cancel the build");
                env.CancelBuild();
            }

            return Value.Empty;
        }

        /// <summary>
        /// Work with configuration manager through SolutionContexts.
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
        protected string StProjects(IPM pm)
        {
            if(!pm.It(LevelType.Property, "projects")) {
                throw new IncorrectNodeException(pm);
            }

            if(pm.Is(LevelType.Method, "find")) {
                return StProjectsFind(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// .find(...) level
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("find",
                "To find project by name. It compares part of name, therefore you can use simply like a \"ZenLib\" or full name \"Zenlib\\ZenLib.vcxproj\" etc.",
                "projects",
                nameof(StProjects),
                new[] { "name" },
                new[] { "Project name" },
                CValType.Void,
                CValType.String)]
        protected string StProjectsFind(IPM pm)
        {
            ILevel level = pm.FirstLevel; // level of the `find` property

            if(level.Is(ArgumentType.StringDouble)) {
                string name = (string)level.Args[0].data;
                return ProjectCfg(name, pm.PinTo(1));
            }

            throw new PMLevelException(level, "find(string name)");
        }

        /// <summary>
        /// Configuration level of project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property(
            "IsBuildable",
            "Gets or Sets. Whether the project or item configuration of project can be built.\nAssociated with current SolutionContext.", 
            "find",
            nameof(StProjectsFind), 
            CValType.Boolean, 
            CValType.Boolean)]
        [Property(
            "IsDeployable",
            "Gets or Sets. Whether the current project or item configuration of project can be deployed.\nAssociated with current SolutionContext.", 
            "find",
            nameof(StProjectsFind), 
            CValType.Boolean, 
            CValType.Boolean)]
        protected string ProjectCfg(string projectName, IPM pm)
        {
            ISolutionContext context = env.GetSolutionContext(projectName);

            if(pm.It(LevelType.Property, "IsBuildable"))
            {
                if(pm.IsRight(LevelType.RightOperandStd)) {
                    context.IsBuildable = Value.ToBoolean(pm.FirstLevel.Data);
                    return Value.Empty;
                }

                if(pm.IsRight(LevelType.RightOperandEmpty)) {
                    return Value.From(context.IsBuildable);
                }
            }

            if(pm.It(LevelType.Property, "IsDeployable"))
            {
                if(pm.IsRight(LevelType.RightOperandStd)) {
                    context.IsDeployable = Value.ToBoolean(pm.FirstLevel.Data);
                    return Value.Empty;
                }

                if(pm.IsRight(LevelType.RightOperandEmpty)) {
                    return Value.From(context.IsDeployable);
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
        [Property("type", "Get type of current build action, or last used type if it already finished.", CValType.Enum, CValType.Void)]
        protected string StType(IPM pm)
        {
            if(!pm.It(LevelType.Property, "type") || !pm.IsRight(LevelType.RightOperandEmpty)) {
                throw new IncorrectNodeException(pm);
            }

            return Value.From(env.BuildType);
        }

        /// <summary>
        /// Work with solution node.
        /// Sample: #[Build solution]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("solution", "Work with solution data.")]
        [Property("current", "Link to current used solution.", "solution", nameof(StSolution)), Property("", "current", nameof(StSolution))]
        [Method("path",
                "Use specific solution from selected path.",
                "solution",
                nameof(StSolution),
                new[] { "sln" },
                new[] { "Full path to solution file." },
                CValType.Void,
                CValType.String), Property("", "path", nameof(StSolution))]
        protected string StSolution(IPM pm)
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
                return StSlnPMap(env.SolutionFile, pm.PinTo(2));
            }

            // solution.path("file").
            if(pm.Is(1, LevelType.Method, "path"))
            {
                ILevel lvlPath = pm.Levels[1];
                lvlPath.Is("solution.path(string sln)", ArgumentType.StringDouble);
                return StSlnPMap((string)lvlPath.Args[0].data, pm.PinTo(2));
            }

            throw new IncorrectNodeException(pm, 1);
        }

        /// <summary>
        /// Work with ProjectsMap of solution node.
        /// </summary>
        /// <param name="sln"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("First", "First project in Project Build Order.", "", nameof(StSolution)), Property("", "First", nameof(StSlnPMap))]
        [Property("Last", "Last project in Project Build Order.", "", nameof(StSolution)), Property("", "Last", nameof(StSlnPMap))]
        [Property("FirstRaw", "First project from defined list.\nIgnores used Build type.", "", nameof(StSolution)), Property("", "FirstRaw", nameof(StSlnPMap))]
        [Property("LastRaw", "Last project from defined list.\nIgnores used Build type.", "", nameof(StSolution)), Property("", "LastRaw", nameof(StSlnPMap))]
        [Property("GuidList", "Get list of project Guids.\nIn direct order of definition.", "", nameof(StSolution), CValType.List)]
        [Method("projectBy",
                "Get project by Guid string.",
                "",
                nameof(StSolution),
                new[] { "guid" },
                new[] { "Identifier of project." },
                CValType.Void,
                CValType.String), Property("", "projectBy", nameof(StSlnPMap))]
        protected string StSlnPMap(string sln, IPM pm)
        {
            if(string.IsNullOrWhiteSpace(sln)) {
                throw new ArgumentException($"Failed {nameof(StSlnPMap)}: sln is empty");
            }
            ProjectsMap map = GetProjectsMap(sln);

            if(pm.Is(LevelType.Property, "First")) {
                return UseProjectsMap(map.FirstBy(env.IsCleanOperation), pm.PinTo(1));
            }

            if(pm.Is(LevelType.Property, "Last")) {
                return UseProjectsMap(map.LastBy(env.IsCleanOperation), pm.PinTo(1));
            }

            if(pm.Is(LevelType.Property, "FirstRaw")) {
                return UseProjectsMap(map.First, pm.PinTo(1));
            }

            if(pm.Is(LevelType.Property, "LastRaw")) {
                return UseProjectsMap(map.Last, pm.PinTo(1));
            }

            if(pm.FinalEmptyIs(LevelType.Property, "GuidList")) {
                return Value.From(map.GuidList);
            }

            if(pm.Is(LevelType.Method, "projectBy"))
            {
                ILevel lvlPrjBy = pm.FirstLevel;
                lvlPrjBy.Is("projectBy(string guid)", ArgumentType.StringDouble);
                return UseProjectsMap(map.GetProjectBy((string)lvlPrjBy.Args[0].data), pm.PinTo(1));
            }

            throw new IncorrectNodeException(pm);
        }
        
        /// <summary>
        /// Unpacks ProjectsMap.Project struct for user.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("name", "The name of project.", "", nameof(StSlnPMap), CValType.String)]
        [Property("path", "Path to project.", "", nameof(StSlnPMap), CValType.String)]
        [Property("type", "Type of project.", "", nameof(StSlnPMap), CValType.String)]
        [Property("guid", "Guid of project.", "", nameof(StSlnPMap), CValType.String)]
        private protected string UseProjectsMap(ProjectItem project, IPM pm)
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
    }
}
