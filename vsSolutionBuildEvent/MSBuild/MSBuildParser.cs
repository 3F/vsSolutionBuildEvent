/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE80;
using Microsoft.Build.Collections;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild.Exceptions;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.MSBuild
{
    public class MSBuildParser: IMSBuild, IEvaluator
    {
        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Used instance of user-variables
        /// </summary>
        protected IUserVariable uvariable;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        /// <summary>
        /// MSBuild Property from default Project
        /// </summary>
        /// <param name="name">key property</param>
        /// <returns>evaluated value</returns>
        public string getProperty(string name)
        {
            return getProperty(name, null);
        }

        /// <summary>
        /// MSBuild Property from specific project
        /// </summary>
        /// <param name="name">key of property</param>
        /// <param name="projectName">Specific project</param>
        /// <exception cref="MSBPropertyNotFoundException"></exception>
        /// <returns>Evaluated value of property</returns>
        public virtual string getProperty(string name, string projectName)
        {
            if(uvariable.isExist(name, projectName)) {
                Log.nlog.Debug("Evaluate: use '{0}:{1}' from user-variable", name, projectName);
                return getUVariableValue(name, projectName);
            }

            if(projectName == null)
            {
                string slnProp = env.getSolutionGlobalProperty(name);
                if(slnProp != null) {
                    Log.nlog.Debug("Solution-context for getProperty - '{0}' = '{1}'", name, slnProp);
                    return slnProp;
                }
            }

            Project project = env.getProject(projectName);
            ProjectProperty prop = project.GetProperty(name);

            if(prop != null) {
                return prop.EvaluatedValue;
            }
            throw new MSBPropertyNotFoundException("variable - '{0}' : project - '{1}'", name, (projectName == null) ? "<default>" : projectName);
        }

        public List<TMSBuildPropertyItem> listProperties(string projectName = null)
        {
            List<TMSBuildPropertyItem> properties = new List<TMSBuildPropertyItem>();

            Project project = env.getProject(projectName);
            foreach(ProjectProperty property in project.Properties)
            {
                string eValue = property.EvaluatedValue;
                if(projectName == null)
                {
                    string slnProp = env.getSolutionGlobalProperty(property.Name);
                    if(slnProp != null) {
                        Log.nlog.Debug("Solution-context for listProperties - '{0}' = '{1}'", property.Name, slnProp);
                        eValue = slnProp;
                    }
                }

                properties.Add(new TMSBuildPropertyItem(property.Name, eValue));
            }
            return properties;
        }

        /// <summary>
        /// Evaluate data with the MSBuild engine.
        /// alternative to Microsoft.Build.BuildEngine - http://msdn.microsoft.com/en-us/library/Microsoft.Build.BuildEngine
        /// </summary>
        /// <param name="unevaluated">raw string as $(..data..)</param>
        /// <param name="projectName">push null if default</param>
        /// <returns>evaluated value</returns>
        public virtual string evaluate(string unevaluated, string projectName)
        {
            const string container  = "vsSBE_latestEvaluated";
            Project project         = env.getProject(projectName);

            lock(_eLock)
            {
                try {
                    defVariables(project);
                    project.SetProperty(container, StringHandler.hSymbols(_wrapVariable(ref unevaluated)));
                    return project.GetProperty(container).EvaluatedValue;
                }
                finally {
                    project.RemoveProperty(project.GetProperty(container));
                }
            }
        }

        /// <summary>
        /// Simple handler for MSBuild environment
        /// </summary>
        /// <param name="data">text with $(ident) data</param>
        /// <returns>text with evaluated properties</returns>
        [Obsolete("Use parse()", false)]
        public string parseVariable(string data)
        {
            return Regex.Replace(data, @"
                                         (?<!\$)\$
                                         \(
                                           (?:
                                             (
                                               [^\:\r\n)]+?
                                             )
                                             \:
                                             (
                                               [^)\r\n]+?
                                             )
                                             |
                                             (
                                               [^)]*?
                                             )
                                           )
                                         \)", delegate(Match m)
            {
                // 3   -> $(name)
                // 1,2 -> $(name:project)

                if(m.Groups[3].Success) {
                    return getProperty(m.Groups[3].Value);
                }
                return getProperty(m.Groups[1].Value, m.Groups[2].Value);

            }, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace).Replace("$$(", "$(");
        }

        /// <summary>
        /// Handler of variables/properties MSBuild
        /// </summary>
        /// <param name="data">string with $(ident) data</param>
        /// <returns>All evaluated values for data</returns>
        public virtual string parse(string data)
        {
            /*
                    (
                      \${1,2}
                    )
                    (?=
                      (
                        \(
                          (?>
                            [^()]
                            |
                            (?2)
                          )*
                        \)
                      )
                    )            -> for .NET: v             
             */
            return Regex.Replace(data,  @"(
                                            \${1,2}
                                          )
                                          (
                                            \(
                                              (?>
                                                [^()]
                                                |
                                                \((?<R>)
                                                |
                                                \)(?<-R>)
                                              )*
                                              (?(R)(?!))
                                            \)
                                          )", delegate(Match m)
            {
                // 1 - $ or $$
                // 2 - (name) or (name:project) or ([MSBuild]::MakeRelative($(path1), ...):project) .. 
                //      http://msdn.microsoft.com/en-us/library/vstudio/dd633440%28v=vs.120%29.aspx

                if(m.Groups[1].Value.Length > 1) { //escape
                    return m.Value.Substring(1);
                }

                return evaluateVariable(prepareVariables(m.Groups[2].Value));
            }, 
            RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Internal processing.
        /// 
        /// All variables which are not included in MSBuild environment.
        /// Customization for our data.
        /// </summary>
        /// <param name="raw">Where to look</param>
        /// <param name="ident">Expected variable. What we're looking for..</param>
        /// <param name="vTrue">Value if found</param>
        /// <returns>String with evaluated data as vTrue value or unevaluated as is</returns>
        [Obsolete("Use the SBE-Scripts", false)]
        public virtual string parseVariablesSBE(string raw, string ident, string vTrue)
        {
            return Regex.Replace(raw, @"(
                                          \${1,2}     #1 -> $ or $$
                                        )
                                        \(
                                           (
                                             [^)]+?   #2 -> unevaluated data
                                           )
                                        \)", delegate(Match m)
            {
                if(m.Groups[2].Value != ident || m.Groups[1].Value.Length > 1) {
                    return m.Value;
                }
                return (vTrue == null)? "" : vTrue;
            }, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Evaluating data with current object
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>Evaluated end value</returns>
        public string evaluate(string data)
        {
            return parse(data);
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used user-variables</param>
        public MSBuildParser(IEnvironment env, IUserVariable uvariable)
        {
            this.env        = env;
            this.uvariable  = uvariable;
        }

        /// <summary>
        /// Instance with default UserVariable
        /// </summary>
        /// <param name="env">Used environment</param>
        public MSBuildParser(IEnvironment env)
        {
            this.env        = env;
            this.uvariable  = new UserVariable();
        }

        /// <param name="raw">raw data at format - '(..data..)'</param>
        /// <exception cref="IncorrectSyntaxException"></exception>
        protected TPreparedData prepareVariables(string raw)
        {
            TPreparedData ret = new TPreparedData();

            Match m = Regex.Match(raw.Trim(), @"^\(  
                                                   (?:
                                                     (\#)?           # 1 -> special char  (optional)
                                                     ([A-Za-z_0-9]+) # 2 -> variable name (optional)
                                                     \s*=\s*
                                                   )?
                                                   (?:
                                                      (.+[^:])       # 3 -> unevaluated data
                                                      :([^:][^)]+)   # 4 -> specific project for variable if 2 is present or for unevaluated data
                                                    |                # or:
                                                      (.+)           # 5 -> unevaluated data
                                                   )
                                               \)$", RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                Log.nlog.Debug("impossible to prepare data '{0}'", raw);
                throw new IncorrectSyntaxException("prepare failed - '{0}'", raw);
            }
            bool hasVar     = m.Groups[2].Success;
            bool hasProject = m.Groups[3].Success ? true : false;

            if(hasVar) {
                ret.variable.name           = m.Groups[2].Value.Trim();
                ret.variable.isPersistence  = (m.Groups[1].Success && m.Groups[1].Value == "#") ? true : false;
            }

            ret.property.raw = (hasProject ? m.Groups[3] : m.Groups[5]).Value.Trim();

            bool composite          = (ret.property.raw[0] == '$') ? true : false;
            ret.property.escaped    = (composite && ret.property.raw[1] == '$') ? true : false;

            if(hasProject)
            {
                string project = m.Groups[4].Value.Trim();

                if(!composite) {
                    ret.property.project = project;
                }
                else if(hasVar) {
                    ret.variable.project = project;
                }
                else {
                    // should be variable of variable e.g.: ($(var:project):project)
                    ret.property.project = project;
                }
            }

            ret.property.complex = !_isPropertySimple(ref ret.property.raw) || composite;

            if(!ret.property.complex) {
                ret.property.unevaluated = ret.property.raw;
                ret.property.completed   = true;
                Log.nlog.Debug("Prepared: simple - '{0}'", ret.property.unevaluated);
                return ret;
            }
            if(ret.property.escaped) {
                ret.property.unevaluated = ret.property.raw.Substring(1);
                ret.property.completed   = true;
                Log.nlog.Debug("Prepared: complex escaped - '{0}'", ret.property.unevaluated);
                return ret;
            }
            
            // try to simplify
            m = Regex.Match(ret.property.raw, @"^
                                                  \$
                                                  \(
                                                     ([A-Za-z_0-9]+)  # 1 - name
                                                     (?::([^)]+))?    # 2 - project
                                                  \)
                                                $", RegexOptions.IgnorePatternWhitespace);

            if(m.Success)
            {
                ret.property.complex        = false;
                ret.property.unevaluated    = m.Groups[1].Value;
                ret.property.completed      = true;
                ret.property.project        = (m.Groups[2].Success) ?
                                                           m.Groups[2].Value.Trim() : 
                                                           (!String.IsNullOrEmpty(ret.property.project)) ? ret.property.project : null;
                
                Log.nlog.Debug("Prepared: found simple property '{0}' for '{1}'", ret.property.unevaluated, ret.property.project);
                return ret;
            }

            ret.property.unevaluated = ret.property.raw;

            // nested data
            ret = prepareNested(ret);

            ret.property.unevaluated = _wrapVariable(ref ret.property.unevaluated);
            Log.nlog.Debug("Prepared: step out");
            return ret;
        }

        /// <summary>
        /// Getting value from User-Variable by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns>Evaluated value of variable</returns>
        protected string getUVariableValue(string name, string project)
        {
            if(uvariable.isUnevaluated(name, project)) {
                uvariable.evaluate(name, project, this, true);
            }
            return uvariable.get(name, project);
        }

        /// <summary>
        /// Getting value from User-Variable by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Evaluated value of variable</returns>
        protected string getUVariableValue(string ident)
        {
            if(uvariable.isUnevaluated(ident)) {
                uvariable.evaluate(ident, this, true);
            }
            return uvariable.get(ident);
        }

        /// <summary>
        /// Defining variables for project context from all defined user-variables
        /// </summary>
        /// <param name="project"></param>
        protected void defVariables(Project project)
        {
            foreach(TUserVariable uvar in uvariable.Variables)
            {
                if(uvar.status != TUserVariable.StatusType.Started) {
                    project.SetGlobalProperty(uvar.ident, getUVariableValue(uvar.ident));
                    continue;
                }

                if(uvar.prev != null && ((TUserVariable)uvar.prev).unevaluated != null)
                {
                    TUserVariable prev = (TUserVariable)uvar.prev;
                    project.SetGlobalProperty(uvar.ident, (prev.evaluated == null)? prev.unevaluated : prev.evaluated);
                }
            }
        }

        /// <summary>
        /// End value from prepared information with TPreparedData
        /// </summary>
        /// <param name="prepared"></param>
        protected string evaluateVariable(TPreparedData prepared)
        {
            string evaluated = String.Empty;
            
            if(prepared.property.completed && !prepared.property.complex)
            {
                Log.nlog.Debug("Evaluate: use the getProperty");
                evaluated = getProperty(prepared.property.unevaluated, prepared.property.project);
            }
            else if(prepared.property.escaped){
                Log.nlog.Debug("Evaluate: escaped value");
                evaluated = prepared.property.unevaluated;
            }
            else
            {
                if(!prepared.property.completed) {
                    Log.nlog.Debug("Evaluate: use the evaluateVariable for: '{0}' -> [{1}]", 
                                                 prepared.property.nested.data, prepared.property.project);
                    evaluated = evaluate(prepared.property.nested.data, prepared.property.project);
                }
                else {
                    Log.nlog.Debug("Evaluate: ready to '{0}'", prepared.property.nested.data);
                    evaluated = prepared.property.nested.data;
                }
            }

            // *!* Deprecated and used as alternative for SBE-Scripts
            if(!String.IsNullOrEmpty(prepared.variable.name))
            {
                //INFO: prepared.variable.isPersistence - [reserved]
                Log.nlog.Debug("Evaluate: found definition of user-variable");
                uvariable.set(prepared.variable.name, prepared.variable.project, evaluated);
                uvariable.evaluate(prepared.variable.name, prepared.variable.project, this, true);
                evaluated = "";
            }

            Log.nlog.Debug("Evaluated: '{0}'", evaluated);
            return evaluated;
        }

        /// <summary>
        /// Work with nested complex data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="limit">Maximum of nesting level. Aborts if reached</param>
        /// <exception cref="MSBPropertyParseException"></exception>
        /// <exception cref="LimitException"></exception>
        /// <returns></returns>
        protected TPreparedData prepareNested(TPreparedData data, int limit = 50)
        {
            if(String.IsNullOrEmpty(data.property.unevaluated)) {
                throw new MSBPropertyParseException("the 'unevaluated' of TPreparedData is empty. raw == '{0}'", data.property.raw);
            }

            data.property.nested    = new TPreparedData.Nested();
            string unevaluated      = data.property.unevaluated;   
                     
            Dictionary<uint, string> strings    = new Dictionary<uint, string>();
            bool isWrapped                      = unevaluated.StartsWith("$(");

            string patternS = @"(
                                  (?:
                                      ""          # note: "" - is a single double quote, i.e. escaped in @ mode
                                       (?:
                                          [^""\\]
                                        |
                                          \\""?
                                       )*""
                                   |
                                     '(?:
                                         [^'\\]
                                       |
                                         \\'?
                                      )*'
                                  )
                                )            #1 - strings - "", ''
                                ([\s,)])     #2 - limiter symbol";


            string patternV = @"(?:
                                  (\${1,2})                  #1 - $ or $$
                                  \(
                                     (?:
                                        (
                                          [^$:()]+
                                          [^()]
                                        )                    #2 - name [Simple]
                                        (?:
                                          :
                                          (
                                            [^:]
                                            [^$()]+ [^()]
                                          )                  #3 - project [Simple] (optional)
                                        )?
                                      |
                                        (
                                          [^$:]+? \)?        #4 - name [Complex]
                                        )
                                        (?:
                                          :
                                          (
                                            [^:]
                                            [^$()]+ [^()]
                                           |
                                            [^:$]+?
                                            [^$)]+ \)?
                                          )                  #5 - project [Complex] (optional)
                                        )?
                                     )
                                  \)
                                )";

            Log.nlog.Debug("nested: started with '{0}'", unevaluated);

            int level   = 0;
            uint ident  = 0;

            Func<bool> h = null;
            h = delegate()
            {
                Log.nlog.Debug("nested: level {0}", level);

                if(level > limit) {
                    throw new LimitException("Nesting level of '{0}' reached. Aborted.", limit);
                }

                // string arguments
                unevaluated = Regex.Replace(unevaluated, patternS, delegate(Match m)
                {
                    strings[ident] = m.Groups[1].Value;
                    Log.nlog.Debug("nested: protect string '{0}'", strings[ident]);

                    // no conflict, because all variants with '!' as argument is not possible without quotes.
                    return String.Format("!p{0}!{1}", ident++, m.Groups[2].Value);
                }, RegexOptions.IgnorePatternWhitespace);

                Log.nlog.Debug("nested: strings '{0}'", unevaluated);

                // complex-type arguments
                unevaluated = _nestedComplex(ref patternV, ref unevaluated, strings);

                if(Regex.Match(unevaluated, patternV, RegexOptions.IgnorePatternWhitespace).Success
                    || Regex.Match(unevaluated, patternS, RegexOptions.IgnorePatternWhitespace).Success)
                {
                    Log.nlog.Debug("nested: step in");
                    ++level;
                    h();
                    --level;
                }
                return true;
            };
            h();

            data.property.nested.data = Regex.Replace(unevaluated, @"!p(\d+)!", delegate(Match _m)
            {
                return strings[uint.Parse(_m.Groups[1].Value)];
            });

            data.property.completed = isWrapped;
            data.property.complex   = true;

            Log.nlog.Debug("nested: completed as '{0}'", data.property.completed);
            return data;
        }

        /// <summary>
        /// Handler for complex-type arguments
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="data">Unevaluated data</param>
        /// <param name="strings">Protected strings</param>
        /// <returns>prepared value for this step</returns>
        private string _nestedComplex(ref string pattern, ref string data, Dictionary<uint, string> strings)
        {
            return Regex.Replace(data, pattern, delegate(Match m)
            {
                Log.nlog.Debug("nested: complex in");
                string evaluated = String.Empty;

                if(m.Groups[1].Value.Length > 1) {
                    evaluated = m.Value.Substring(1);
                    Log.nlog.Debug("nested: escaped '{0}'", evaluated);
                    return evaluated;
                }

                string eData    = m.Groups[m.Groups[2].Success ? 2 : 4].Value.Trim();
                string eProject = null;

                if(m.Groups[3].Success) {
                    eProject = m.Groups[3].Value.Trim();
                }
                else if(m.Groups[5].Success) {
                    eProject = m.Groups[5].Value.Trim();
                }

                // Evaluating for nested 'data:project' - to support variable of variable:

                eData = Regex.Replace(eData, @"!p(\d+)!", delegate(Match _m)
                {
                    uint index      = uint.Parse(_m.Groups[1].Value);
                    string str      = strings[index];
                    strings.Remove(index); // deallocate protected string
                    return str;
                });

                //if(!String.IsNullOrEmpty(eProject) && !_isPropertySimple(ref eProject)) {
                //    //eProject = evaluateVariable(eProject, null); // should be set as part of eData, e.g.: $(project) if needed
                //}
                evaluated  = _isPropertySimple(ref eData) ? getProperty(eData, eProject) : evaluate(eData, eProject);
                Log.nlog.Debug("nested: evaluated '{0}':'{1}' == '{2}'", eData, eProject, evaluated);

                return evaluated;
            }, RegexOptions.IgnorePatternWhitespace);
        }

        private string _wrapVariable(ref string var)
        {
            if(!var.StartsWith("$(")) {
                Log.nlog.Debug("wrap: '{0}'", var);
                return String.Format("$({0})", var);
            }
            return var;
        }

        private bool _isPropertySimple(ref string unevaluated)
        {
            if(unevaluated.IndexOfAny(new char[]{'.', ':', '(', ')', '\'', '"', '[', ']'}) != -1) {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// item of property: name = value
    /// </summary>
    public struct TMSBuildPropertyItem
    {
        public string name;
        public string value;

        public TMSBuildPropertyItem(string name, string value)
        {
            this.name  = name;
            this.value = value;
        }
    }
}
