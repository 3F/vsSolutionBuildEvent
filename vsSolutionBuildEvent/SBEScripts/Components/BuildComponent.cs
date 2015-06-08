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
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Component of operations with the build
    /// </summary>
    [Component("Build", "Operations with the build")]
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
                    Log.nlog.Trace("BuildComponent: use stCancel");
                    return stCancel(ident);
                }
                case "projects": {
                    // is meant the SolutionContexts !
                    Log.nlog.Trace("BuildComponent: use stProjects");
                    return stProjects(ident);
                }
                case "type": {
                    Log.nlog.Trace("BuildComponent: use stType");
                    return stType(ident);
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
        [Property("cancel", "Immediately cancellation of the build projects.", CValueType.Boolean, CValueType.Boolean)]
        protected string stCancel(string data)
        {
            Match m = Regex.Match(data, @"cancel\s*=\s*(false|true|1|0)\s*$");
            if(!m.Success) {
                throw new OperationNotFoundException("Failed stCancel - '{0}'", data);
            }
            string val = m.Groups[1].Value;
            Log.nlog.Debug("stCancel: value is {0}", val);

            if(Value.toBoolean(val))
            {
                Log.nlog.Trace("stCancel: pushed true");
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
        [Property("projects", "Work with configuration manager of projects through the SolutionContexts.")]
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
                "The find() method compares as part of the name, and you can use simply like a find(\"ZenLib\") or for unique identification full \"Zenlib\\ZenLib.vcxproj\" etc.",
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
            "Gets or Sets whether the project or project item configuration can be built.", 
            "find", 
            "stProjectConf", 
            CValueType.Boolean, 
            CValueType.Boolean
        )]
        [Property(
            "IsDeployable", 
            "Gets or Sets whether the current project is built when the solution configuration associated with this SolutionContext is selected.", 
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

            Log.nlog.Debug("stProjectConf: property - '{0}', value - '{1}'", property, value);
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
    }
}
