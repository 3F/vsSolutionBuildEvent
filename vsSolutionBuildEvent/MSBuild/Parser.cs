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
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.MSBuild
{
    public class Parser: IMSBuild, IEvaluator
    {
        /// <summary>
        /// Default value for all undefined properties.
        /// </summary>
        public const string PROP_VALUE_DEFAULT = "*Undefined*";

        /// <summary>
        /// Max of supported containers for processing.
        /// </summary>
        protected const uint CONTAINERS_LIMIT = 1 << 16;

        /// <summary>
        /// Provides operation with environment
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Used instance of user-variables
        /// </summary>
        protected IUserVariable uvariable;

        /// <summary>
        /// References of evaluated properties.
        /// </summary>
        private Dictionary<string, HashSet<string>> references = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Used environment.
        /// </summary>
        public IEnvironment Env
        {
            get { return env; }
        }

        /// <summary>
        /// Container of user-variables.
        /// </summary>
        public IUserVariable UVariable
        {
            get { return uvariable; }
        }

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
                Log.Debug("Evaluate: use '{0}:{1}' from user-variable", name, projectName);
                return getUVariableValue(name, projectName);
            }

            if(projectName == null)
            {
                string slnProp = env.getSolutionProperty(name);
                if(slnProp != null) {
                    Log.Debug("Solution-context for getProperty - '{0}' = '{1}'", name, slnProp);
                    return slnProp;
                }
            }

            Project project         = getProject(projectName);
            ProjectProperty prop    = project.GetProperty(name);

            if(prop != null) {
                return prop.EvaluatedValue;
            }
            Log.Debug("getProperty: return default value");
            return PROP_VALUE_DEFAULT;
            //throw new MSBPropertyNotFoundException("variable - '{0}' : project - '{1}'", name, (projectName == null)? "<default>" : projectName);
        }

        /// <summary>
        /// Gets all properties from specific project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public List<PropertyItem> listProperties(string projectName = null)
        {
            List<PropertyItem> properties = new List<PropertyItem>();

            Project project = getProject(projectName);
            foreach(ProjectProperty property in project.Properties)
            {
                string eValue = property.EvaluatedValue;
                if(projectName == null)
                {
                    string slnProp = env.getSolutionProperty(property.Name);
                    if(slnProp != null) {
                        Log.Debug("Solution-context for listProperties - '{0}' = '{1}'", property.Name, slnProp);
                        eValue = slnProp;
                    }
                }

                properties.Add(new PropertyItem(property.Name, eValue));
            }
            return properties;
        }

        /// <summary>
        /// Evaluate data with MSBuild engine.
        /// alternative to Microsoft.Build.BuildEngine - http://msdn.microsoft.com/en-us/library/Microsoft.Build.BuildEngine
        /// </summary>
        /// <param name="unevaluated">raw string as $(..data..)</param>
        /// <param name="projectName">specific project or null value for default</param>
        /// <returns>Evaluated value</returns>
        public virtual string evaluate(string unevaluated, string projectName = null)
        {
            const string container  = "vsSBE_latestEvaluated";
            Project project         = getProject(projectName);

            Log.Trace("evaluate: '{0}' -> [{1}]", unevaluated, projectName);
            lock(_lock)
            {
                try {
                    defProperties(project);
                    project.SetProperty(container, Tokens.characters(_wrapProperty(ref unevaluated)));
                    return project.GetProperty(container).EvaluatedValue;
                }
                finally {
                    project.RemoveProperty(project.GetProperty(container));
                }
            }
        }

        /// <summary>
        /// Entry point to evaluating MSBuild data.
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>All evaluated values for data</returns>
        public virtual string parse(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty; // convert to not null value
            }

            if(String.IsNullOrWhiteSpace(data)) {
                return data; // save all white-space characters
            }

            StringHandler sh = new StringHandler();
            lock(_lock)
            {
                return hquotes(
                            sh.recovery(
                                containerIn(
                                    sh.protectEscContainer(
                                        sh.protectMixedQuotes(data)
                                    ),
                                    sh, 
                                    CONTAINERS_LIMIT
                                )
                            )
                       );
            }
        }

        /// <summary>
        /// Evaluate data with MSBuild.
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>Evaluated value</returns>
        public string evaluate(string data)
        {
            return parse(data);
        }

        /// <summary>
        /// Getting project instance by name.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <returns>Returns new instance if fail.</returns>
        public Project getProject(string name)
        {
            try {
                Log.Trace("MSBuild - getProject: Trying of getting project instance - '{0}'", name);
                return env.getProject(name);
            }
            catch(MSBProjectNotFoundException) {
                Log.Trace("MSBuild - getProject: use empty project by default.");
                return new Project();
            }
        }

        /// <summary>
        /// To initialize properties by default for project.
        /// </summary>
        /// <param name="project">Uses GlobalProjectCollection if null.</param>
        public virtual void initPropByDefault(Project project = null)
        {
            IAppSettings app    = Settings._;
            const string _PFX   = Settings.APP_NAME_SHORT;

            setGlobalProperty(project, Settings.APP_NAME, Version.numberWithRevString);
            setGlobalProperty(project, $"{_PFX}_CommonPath", app.CommonPath);
            setGlobalProperty(project, $"{_PFX}_LibPath", app.LibPath);
            setGlobalProperty(project, $"{_PFX}_WorkPath", app.WorkPath);
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used user-variables</param>
        public Parser(IEnvironment env, IUserVariable uvariable)
        {
            this.env        = env;
            this.uvariable  = uvariable;
        }

        /// <summary>
        /// The instance with default value for UserVariable
        /// </summary>
        /// <param name="env">Used environment</param>
        public Parser(IEnvironment env)
            : this(env, new UserVariable())
        {

        }

        /// <summary>
        /// Handler of general containers.
        /// Moving upward from deepest container.
        /// 
        /// $(name) or $(name:project) or $([MSBuild]::MakeRelative($(path1), ...):project) ..
        /// https://msdn.microsoft.com/en-us/library/vstudio/dd633440%28v=vs.120%29.aspx
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sh"></param>
        /// <param name="limit">Limitation to containers. Aborts if reached</param>
        /// <exception cref="LimitException"></exception>
        /// <returns></returns>
        protected string containerIn(string data, StringHandler sh, uint limit)
        {
            Regex con   = RPattern.ContainerInCompiled;
            int maxRep  = 1; // rule of depth, e.g.: $(p1 = $(Platform))$(p2 = $(p1))$(p2)
                             //TODO: it's slowest but fully compatible with classic rules with minimal programming.. so, improve performance

            references.Clear();
            uint step = 0;
            do
            {
                if(step++ > limit) {
                    sh.flush();
                    throw new LimitException("Restriction of supported containers '{0}' reached. Aborted.", limit);
                }

                data = con.Replace(data,
                                    delegate(Match m)
                                    {
                                        string raw = m.Groups[1].Value;
                                        Log.Trace("containerIn: raw - `{0}`", raw);
                                        return evaluate(prepare(sh.recovery(raw)));
                                    },
                                    maxRep);

                // protect before new checking
                data = sh.protectEscContainer(sh.protectMixedQuotes(data));

            } while(con.IsMatch(data));

            return data;
        }

        /// <summary>
        /// Prepare data for next evaluation step.
        /// </summary>
        /// <param name="raw">a single container e.g.: '(..data..)'</param>
        /// <exception cref="IncorrectSyntaxException"></exception>
        protected PreparedData prepare(string raw)
        {
            Match m = RPattern.PItem.Match(raw.Trim());
            if(!m.Success) {
                throw new IncorrectSyntaxException("prepare: failed - '{0}'", raw);
            }

            Group rTSign            = m.Groups["tsign"];
            Group rVariable         = m.Groups[1];
            Group rStringDataD      = m.Groups[2];
            Group rStringDataS      = m.Groups[3];
            Group rDataWithProject  = m.Groups[4];
            Group rProject          = m.Groups[5];
            Group rData             = m.Groups[6];

            PreparedData ret = new PreparedData()
            {
                property = new PreparedData.Property() {
                    raw = raw
                }
            };
            //Log.Trace("prepare: raw '{0}'", raw);


            /* Variable */

            if(rVariable.Success)
            {
                ret.variable.name = rVariable.Value;
                switch(rTSign.Value) {
                    case "+": {
                        ret.variable.operation = PreparedData.VariableType.DefProperty;
                        break;
                    }
                    case "-": {
                        ret.variable.operation = PreparedData.VariableType.UndefProperty;
                        break;
                    }
                    default: {
                        ret.variable.operation = PreparedData.VariableType.Default;
                        break;
                    }
                }
                // all $() in right operand cannot be evaluated because it's escaped and already unwrapped property.
                // i.e. this already should be evaluated with prev. steps - because we are moving upward from deepest container !
                Log.Trace("prepare: variable name = '{0}'", ret.variable.name);
            }


            /* Data */

            if(rStringDataD.Success) {
                ret.property.unevaluated    = parse(Tokens.unescapeQuotes('"', rStringDataD.Value));
                ret.variable.type           = PreparedData.ValueType.StringFromDouble;
            }
            else if(rStringDataS.Success) {
                ret.property.unevaluated    = Tokens.unescapeQuotes('\'', rStringDataS.Value);
                ret.variable.type           = PreparedData.ValueType.StringFromSingle;
            }
            else {
                ret.property.unevaluated    = hquotes((rDataWithProject.Success ? rDataWithProject : rData).Value.Trim());
                ret.variable.type           = (rVariable.Success)? PreparedData.ValueType.Unknown : PreparedData.ValueType.Property;

                int lp = ret.property.unevaluated.IndexOf('.');
                ret.property.name = ((lp == -1)? ret.property.unevaluated : ret.property.unevaluated.Substring(0, lp)).Trim();

                //TODO:
                if(ret.property.name.IndexOf('[') != -1) {
                    ret.property.name = null; // avoid static methods
                }
            }

            ret.property.complex = !isPropertySimple(ref ret.property.unevaluated);

            Log.Trace("prepare: value = '{0}'({1})", ret.property.unevaluated, ret.variable.type);
            Log.Trace("prepare: complex {0}", ret.property.complex);


            /* Project */

            if(rDataWithProject.Success)
            {
                string project = rProject.Value.Trim();

                if(rVariable.Success) {
                    //TODO: non standard! but it also can be as a $(varname = $($(..data..):project))
                    ret.variable.project = project;
                }
                else {
                    ret.property.project = project;
                }
            }

            Log.Trace("prepare: project [v:'{0}'; p:'{1}']", ret.variable.project, ret.property.project);
            return ret;
        }

        /// <summary>
        /// Pre-filter for data inside quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected string hquotes(string data)
        {
            /*
                Firstly, the all original expressions can be evaluated by original engine inside double quotes.
                For all other (inc. incorrect data) we should evaluate it manually if used protection of course
                ~ $([System.DateTime]::Parse("$(p:project)").ToBinary()); $([System.DateTime]::Parse("$([System.DateTime]::UtcNow.Ticks)").ToBinary()) etc.

                Generally it's important only for ~ $(p:project) expressions with protection... so TODO
            */

            Func<string, char, string> h = delegate (string _data, char qtype)
            {
                return Regex.Replace(_data,
                                        (qtype == '"')? RPattern.DoubleQuotesContent : RPattern.SingleQuotesContent,
                                        delegate(Match m)
                                        {
                                            string content = m.Groups[1].Value;
                                            return String.Format("{0}{1}{0}", qtype, parse(content));
                                        },
                                        RegexOptions.IgnorePatternWhitespace);
            };

            /*
            TODO:
                From single quotes is still protected for compatibility with original logic.
                Because it can be different for ~

                    $([MSBuild]::Multiply('$([System.Math]::Log(2))', 16)) -> 1,10903548889591E+16
                    \                     \_(1) 0,693147180559945_/
                    \_______________(2)__________________________________/

                    
                    $([MSBuild]::Multiply('$([System.Math]::Log(2))', 16)) -> 11,0903548889591
                    \______________________(1)___________________________/

                $([System.Math]::Exp(1.10903548889591E+16)) = ∞ 
                $([System.Math]::Exp(11.0903548889591)) = 65535,9999999983
            */

            //return h(h(data, '\''), '"');
            return h(data, '"');
        }

        /// <summary>
        /// Exceptions to the general rules
        /// for example: $(registry:Hive\MyKey\MySubKey@Value)
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>formatted raw to evaluation</returns>
        protected virtual string customRule(string left, string right)
        {
            if(left == "registry" && !String.IsNullOrEmpty(right)) { // Registry Properties :: https://msdn.microsoft.com/en-us/library/vstudio/ms171458.aspx
                Log.Trace("Rule: Registry Property");
                return String.Format("{0}:{1}", left, right);
            }
            return null;
        }

        /// <summary>
        /// The final value from prepared information
        /// </summary>
        /// <param name="prepared"></param>
        protected string evaluate(PreparedData prepared)
        {
            string evaluated = String.Empty;

            string custom = customRule(prepared.property.unevaluated, prepared.property.project);
            if(custom != null)
            {
                Log.Trace("Evaluate: custom '{0}'", custom);
                evaluated = evaluate(custom, null);
            }
            else if(prepared.variable.type == PreparedData.ValueType.StringFromDouble 
                    || prepared.variable.type == PreparedData.ValueType.StringFromSingle 
                    || !String.IsNullOrEmpty(prepared.variable.name))
            {
                //Note: * content from double quotes should be already evaluated in prev. steps.
                //      * content from single quotes shouldn't be evaluated by rules with protector above.
                Log.Trace("Evaluate: use content from string or use escaped property");
                evaluated = prepared.property.unevaluated;
            }
            else if(!prepared.property.complex)
            {
                Log.Trace("Evaluate: use getProperty");
                evaluated = getProperty(prepared.property.unevaluated, prepared.property.project);
                try {
                    evaluated = unlooping(evaluated, prepared, false);
                }
                catch(PossibleLoopException) {
                    // last chance with direct evaluation
                    evaluated = evaluate(prepared.property.unevaluated, prepared.property.project);
                    evaluated = unlooping(evaluated, prepared, true);
                }
            }
            else
            {
                Log.Trace("Evaluate: use evaluation with msbuild engine");
                evaluated = evaluate(prepared.property.unevaluated, prepared.property.project);
                evaluated = unlooping(evaluated, prepared, true);
            }
            Log.Debug("Evaluated: '{0}'", evaluated);


            // alternative to SBE-Scripts
            if(!String.IsNullOrEmpty(prepared.variable.name))
            {
                Log.Debug("Evaluate: ready to define variable - '{0}':'{1}'", 
                                                                        prepared.variable.name, 
                                                                        prepared.variable.project);

                uvariable.set(prepared.variable.name, prepared.variable.project, evaluated);
                uvariable.evaluate(prepared.variable.name, prepared.variable.project, new EvaluatorBlank(), true);

                if(prepared.variable.operation == PreparedData.VariableType.DefProperty) {
                    defProperty(prepared.variable, evaluated);
                }
                else if(prepared.variable.operation == PreparedData.VariableType.UndefProperty) {
                    undefProperty(prepared.variable);
                }
                evaluated = String.Empty;
            }

            return evaluated;
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
        /// Define properties of project context from user-variables.
        /// </summary>
        /// <param name="project"></param>
        protected void defProperties(Project project)
        {
            foreach(TUserVariable uvar in uvariable.Variables) {
                defProperty(uvar, project);
            }
        }

        protected void defProperty(TUserVariable uvar, Project project)
        {
            if(uvar.status != TUserVariable.StatusType.Started) {
                setGlobalProperty(project, uvar.ident, getUVariableValue(uvar.ident));
                return;
            }

            if(uvar.prev != null && ((TUserVariable)uvar.prev).unevaluated != null)
            {
                TUserVariable prev = (TUserVariable)uvar.prev;
                setGlobalProperty(project, uvar.ident, (prev.evaluated == null)? prev.unevaluated : prev.evaluated);
            }
        }

        protected void defProperty(PreparedData.Variable variable, string evaluated)
        {
            Log.Debug("Set MSBuild property: `{0}`:`{1}`", variable.name, variable.project);
            //defProperty(uvariable.getVariable(variable.name, variable.project), getProject(variable.project));
            setGlobalProperty(getProject(variable.project), variable.name, evaluated);
        }

        protected bool undefProperty(PreparedData.Variable variable)
        {
            Log.Debug("Unset MSBuild property: `{0}`:`{1}`", variable.name, variable.project);
            Project project = getProject(variable.project);

            //uvariable.unset(variable.name, variable.project); //leave for sbe-scripts
            return removeGlobalProperty(project, variable.name);
        }

        /// <param name="project">Uses GlobalProjectCollection if null.</param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns>Returns true if the value changes, otherwise returns false.</returns>
        protected virtual bool setGlobalProperty(Project project, string name, string val)
        {
            try
            {
                if(project == null) {
                    ProjectCollection.GlobalProjectCollection.SetGlobalProperty(name, val);
                    return true;
                }

                return project.SetGlobalProperty(name, val);
            }
            finally {
                sendRawCoreCommand(new[] { "property.set", name, val }); //TODO: to CoreCommandType
            }
        }

        /// <param name="project">Uses GlobalProjectCollection if null.</param>
        /// <param name="name"></param>
        /// <returns>Returns true if the value of the global property was set.</returns>
        protected virtual bool removeGlobalProperty(Project project, string name)
        {
            try
            {
                if(project == null) {
                    return ProjectCollection.GlobalProjectCollection.RemoveGlobalProperty(name);
                }

                return project.RemoveGlobalProperty(name);
            }
            finally {
                sendRawCoreCommand(new[] { "property.del", name }); //TODO: to CoreCommandType
            }
        }

        protected void sendRawCoreCommand(object[] cmd)
        {
            if(env == null || env.CoreCmdSender == null) {
                return;
            }

            env.CoreCmdSender.fire(new CoreCommandArgs() {
                Type = CoreCommandType.RawCommand,
                Args = cmd
            });
        }

        /// <summary>
        /// Operations for recursive properties and to avoiding some looping.
        /// 
        /// The recursive property means that value contains same definition, even after evaluation of this, for example: 
        /// `$(var)` -> `value from $(var)` -> `value from value from $(var)` ... 
        /// 
        /// TODO: I need a COW... fix mooo ~@@``
        /// </summary>
        /// <param name="data">Data with possible recursive property or some looping.</param>
        /// <param name="prepared">Describes property.</param>
        /// <param name="eqEscape">To escape equality if true, otherwise throw PossibleLoopException.</param>
        /// <returns>Evaluated data.</returns>
        protected string unlooping(string data, PreparedData prepared, bool eqEscape)
        {
            string pname = prepared.property.name;

            if(pname == null || data.IndexOf("$(") == -1) {
                return data; // all clear
            }

            HashSet<string> pchecked    = new HashSet<string>();
            Action<string> unlink       = null;

            unlink = delegate(string _name)
            {
                if(!pchecked.Add(_name)) {
#if DEBUG
                    Log.Debug("unlooping-unlink: has been protected with `{0}`", _name);
#endif
                    return;
                    //throw new MismatchException("");
                }

                if(!references.ContainsKey(_name)) {
                    return;
                }

                if(references[_name].Contains(pname)) {
                    throw new LimitException("Found looping for used properties: `{0}` <-> `{1}`", _name, pname);
                }

                foreach(string refvar in references[_name]) {
                    unlink(refvar);
                }
            };

            data = RPattern.ContainerInNamedCompiled.Replace(data, delegate(Match m)
            {
                string found = m.Groups["name"].Value;

                if(pname == found)
                {
                    if(eqEscape) {
                        return "$" + m.Groups[0].Value;
                    }
                    throw new PossibleLoopException();
                }

                if(!references.ContainsKey(pname)) {
                    references[pname] = new HashSet<string>();
                }
                references[pname].Add(found);

                // checking
                unlink(found);

                return m.Groups[0].Value;
            });

            return data;
        }

        protected virtual bool isPropertySimple(ref string data)
        {
            if(data.IndexOfAny(new char[] { '.', ':', '(', ')', '\'', '"', '[', ']' }) != -1) {
                return false;
            }
            return true;
        }

        private string _wrapProperty(ref string data)
        {
            if(!data.StartsWith("$("))
            {
                Log.Trace("wrap: '{0}'", data);
                return String.Format("$({0})", data);
            }
            return data;
        }
    }
}
