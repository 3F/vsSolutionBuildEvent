/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) E-MSBuild contributors: https://github.com/3F/E-MSBuild/graphs/contributors
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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Build.Evaluation;
using net.r_eg.Components;
using net.r_eg.EvMSBuild.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.EvMSBuild
{
    /// <summary>
    /// [E-MSBuild]
    /// 
    /// Advanced Evaluator of MSBuild scripts aka Advanced MSBuild 
    /// with user-variables support through Varhead and more.
    /// https://github.com/3F/E-MSBuild
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class EvMSBuilder: IEvMSBuild, IEvaluator, IEvMSBuildEx
    {
        public const string UNDEF_VAL = "*Undefined*";

        /// <summary>
        /// Max of supported containers for processing.
        /// </summary>
        protected const uint CONTAINERS_LIMIT = 1 << 16;

        public event EventHandler<PropertyArgs> GlobalPropertyChanged = delegate (object sender, PropertyArgs msg) { };

        protected IEvEnv env;

        /// <summary>
        /// References of evaluated properties.
        /// </summary>
        private Dictionary<string, HashSet<string>> references = new Dictionary<string, HashSet<string>>();

        private readonly char[] simprop = new char[] { '.', ':', '(', ')', '\'', '"', '[', ']' };

        private readonly object sync = new object();

        /// <summary>
        /// Container of user-variables through Varhead.
        /// </summary>
        public IUVars UVars
        {
            get;
            protected set;
        }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through E-MSBuild supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public string Eval(string data)
        {
            if(string.IsNullOrEmpty(data)) {
                return string.Empty; // convert to not null value
            }

            if(string.IsNullOrWhiteSpace(data)) {
                return data; // save all white-space characters
            }

            var sh = new StringHandler();
            lock(sync)
            {
                return HQuotes
                (
                    sh.Recovery
                    (
                        ContainerIn
                        (
                            sh.ProtectEscContainer
                            (
                                sh.ProtectMixedQuotes(data)
                            ),
                            sh, 
                            CONTAINERS_LIMIT
                        )
                    )
                );
            }
        }

        /// <summary>
        /// Evaluates mixed data through some engine like E-MSBuild, SobaScript, etc.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public string Evaluate(string data) => Eval(data);

        /// <summary>
        /// Get evaluated variable or property value for specified scope.
        /// </summary>
        /// <param name="name">Access to property or variable by its name.</param>
        /// <param name="scope">Where is placed. null value for global or unspecified scope.</param>
        /// <returns>Evaluated value.</returns>
        public virtual string GetPropValue(string name, string scope = null)
        {
            if(UVars.IsExist(name, scope)) {
                LSender.Send(this, $"Evaluate: use '{name}:{scope}' from user-variable");
                return GetUVarValue(name, scope);
            }

            if(scope == null)
            {
                string slnProp = env.GetMutualPropValue(name);
                if(slnProp != null) {
                    LSender.Send(this, $"Solution-context for getProperty - '{name}' = '{slnProp}'");
                    return slnProp;
                }
            }

            ProjectProperty prop = GetProject(scope).GetProperty(name);

            if(prop != null) {
                return prop.EvaluatedValue;
            }
            LSender.Send(this, "getProperty: return default value");
            return UNDEF_VAL;
        }

        /// <summary>
        /// List all properties for specified scope.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IEnumerable<PropertyItem> ListProperties(string scope = null)
        {
            var properties = new List<PropertyItem>();

            foreach(ProjectProperty property in GetProject(scope).Properties)
            {
                string eValue = property.EvaluatedValue;
                if(scope == null)
                {
                    string slnProp = env.GetMutualPropValue(property.Name);
                    if(slnProp != null) {
                        LSender.Send(this, $"Solution-context for listProperties - '{property.Name}' = '{slnProp}'");
                        eValue = slnProp;
                    }
                }

                properties.Add(new PropertyItem(property.Name, eValue, scope));
            }
            return properties;
        }

        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="scope">Where to place. null value for global or unspecified scope.</param>
        /// <returns>Returns true if the value has changes, otherwise false.</returns>
        public bool SetGlobalProperty(string name, string value, string scope = null)
            => SetGlobalProperty(scope == null ? null : GetProject(scope), name, value);

        /// <param name="name"></param>
        /// <param name="scope">Where is placed. null value for global or unspecified scope.</param>
        /// <returns>Returns true if the property was removed.</returns>
        public bool RemoveGlobalProperty(string name, string scope = null)
            => RemoveGlobalProperty(scope == null ? null : GetProject(scope), name);

        /// <summary>
        /// Define properties using user-variables and specific project instance.
        /// </summary>
        /// <param name="project"></param>
        void IEvMSBuildEx.DefProperties(Project project)
        {
            foreach(TVariable uvar in UVars.Variables) {
                DefProperty(uvar, project);
            }
        }

        public EvMSBuilder()
            : this(new EvEnvBlank())
        {

        }

        public EvMSBuilder(IUVars uvars)
            : this(new EvEnvBlank(), uvars)
        {

        }

        public EvMSBuilder(IEvMin ev)
            : this(new EvEnvBlank(ev))
        {

        }

        public EvMSBuilder(IEvMin ev, IUVars uvars)
            : this(new EvEnvBlank(ev), uvars)
        {

        }

        public EvMSBuilder(IEvEnv env)
            : this(env, new UVars())
        {

        }

        public EvMSBuilder(IEvEnv env, IUVars uvars)
        {
            this.env    = env ?? throw new ArgumentNullException(nameof(env));
            UVars       = uvars ?? throw new ArgumentNullException(nameof(uvars));
        }

        protected virtual bool IsSimpleProperty(ref string data) 
            => data?.IndexOfAny(simprop) == -1;

        /// <summary>
        /// The final value from prepared information
        /// </summary>
        /// <param name="prepared"></param>
        protected string Evaluate(Analysis prepared)
        {
            string evaluated = string.Empty;

            string custom = CustomRule
            (
                prepared.property.unevaluated, 
                prepared.property.scope
            );

            if(custom != null)
            {
                LSender.Send(this, $"Evaluate: custom '{custom}'", MsgLevel.Trace);

                evaluated = Obtain(custom, null);
            }
            else if
                (
                    prepared.variable.type == ValueType.StringFromDouble 
                    || prepared.variable.type == ValueType.StringFromSingle 
                    || !string.IsNullOrEmpty(prepared.variable.name)
                )
            {
                //Note: * content from double quotes should be already evaluated in prev. steps.
                //      * content from single quotes shouldn't be evaluated by rules with protector above.
                LSender.Send(this, "Evaluate: use content from string or use escaped property", MsgLevel.Trace);

                evaluated = prepared.property.unevaluated;
            }
            else if(!prepared.property.complex)
            {
                LSender.Send(this, "Evaluate: use getProperty", MsgLevel.Trace);

                evaluated = GetPropValue(prepared.property.unevaluated, prepared.property.scope);

                try {
                    evaluated = Unlooping(evaluated, prepared, false);
                }
                catch(PossibleLoopException) {
                    // last chance with direct evaluation
                    evaluated = Obtain(prepared.property.unevaluated, prepared.property.scope);
                    evaluated = Unlooping(evaluated, prepared, true);
                }
            }
            else
            {
                LSender.Send(this, "Evaluate: use std evaluation engine", MsgLevel.Trace);

                evaluated = Obtain(prepared.property.unevaluated, prepared.property.scope);
                evaluated = Unlooping(evaluated, prepared, true);
            }

            LSender.Send(this, $"Evaluated: '{evaluated}'");

            // updating of variables

            if(!string.IsNullOrEmpty(prepared.variable.name)) {
                evaluated = DefineVariable(prepared, evaluated);
            }

            return evaluated;
        }

        /// <summary>
        /// Pre-filter for data inside quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual string HQuotes(string data)
        {
            /*
                The all original expressions can be evaluated by original engine inside double quotes.
                For all other (including incorrect data) we should evaluate it manually if used protection of course
                ~ $([System.DateTime]::Parse("$(p:scope)").ToBinary()); $([System.DateTime]::Parse("$([System.DateTime]::UtcNow.Ticks)").ToBinary()) etc.

                Mainly, it's important only for ~ $(p:scope) expressions with protection... so TODO
            */

            string h(string _data, char qtype)
            {
                return Regex.Replace
                (
                    _data,
                    (qtype == '"') ? RPattern.DoubleQuotesContent : RPattern.SingleQuotesContent,
                    (Match m) => 
                    {
                        string content = m.Groups[1].Value;
                        return string.Format("{0}{1}{0}", qtype, Eval(content));
                    },
                    RegexOptions.IgnorePatternWhitespace
                 );
            }

            /*
            TODO:
                It still will be protected from single quotes due to compatibility with original logic.
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
        /// An exceptions to the general rules such as:
        /// $(registry:Hive\MyKey\MySubKey@Value) etc.
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns>formatted raw to evaluation</returns>
        protected virtual string CustomRule(string left, string right)
        {
            // Registry Properties :: https://msdn.microsoft.com/en-us/library/vstudio/ms171458.aspx
            if(left == "registry" && !string.IsNullOrEmpty(right))
            {
                LSender.Send(this, "Rule: Registry Property", MsgLevel.Trace);
                return string.Format("{0}:{1}", left, right);
            }
            return null;
        }

        protected virtual string Obtain(string unevaluated, string scope = null)
        {
            LSender.Send(this, $"{nameof(VaLier)} '{unevaluated}' -> [{scope}]", MsgLevel.Trace);

            using(var v = new VaLier(GetProject(scope), this)) {
                return v.Compute(Tokens.EscapeCharacters(WrapProperty(ref unevaluated)));
            }
        }

        /// <summary>
        /// Getting value from User-Variables by using scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope"></param>
        /// <returns>Evaluated value.</returns>
        protected string GetUVarValue(string name, string scope)
        {
            if(UVars.IsUnevaluated(name, scope)) {
                UVars.Evaluate(name, scope, this, true);
            }
            return UVars.GetValue(name, scope);
        }

        /// <summary>
        /// Getting value from User-Variables by using an unique identifier.
        /// </summary>
        /// <param name="ident">Unique identificator.</param>
        /// <returns>Evaluated value.</returns>
        protected string GetUVarValue(string ident)
        {
            if(UVars.IsUnevaluated(ident)) {
                UVars.Evaluate(ident, this, true);
            }
            return UVars.GetValue(ident);
        }

        /// <summary>
        /// Prepare data for the next evaluation step.
        /// </summary>
        /// <param name="raw">a single container eg.: '(…data…)'</param>
        /// <exception cref="IncorrectSyntaxException"></exception>
        protected Analysis Prepare(string raw)
        {
            Match m = RPattern.PItem.Match(raw.Trim());
            if(!m.Success) {
                throw new IncorrectSyntaxException($"prepare: failed: {raw}");
            }

            Group rTSign            = m.Groups["tsign"];
            Group rVSign            = m.Groups["vsign"];
            Group rVariable         = m.Groups[1];
            Group rStringDataD      = m.Groups[2];
            Group rStringDataS      = m.Groups[3];
            Group rDataWithProject  = m.Groups[4];
            Group rScope            = m.Groups[5];
            Group rData             = m.Groups[6];

            Analysis ret = new Analysis()
            {
                property = new AProperty() {
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
                        ret.variable.tSign = TSignType.DefProperty;
                        break;
                    }
                    case "-": {
                        ret.variable.tSign = TSignType.UndefProperty;
                        break;
                    }
                    default: {
                        ret.variable.tSign = TSignType.Default;
                        break;
                    }
                }

                switch(rVSign.Value) {
                    case "+": {
                        ret.variable.vSign = VSignType.Increment;
                        break;
                    }
                    case "-": {
                        ret.variable.vSign = VSignType.Decrement;
                        break;
                    }
                    default: {
                        ret.variable.vSign = VSignType.Default;
                        break;
                    }
                }

                // all $() in right operand cannot be evaluated because it's escaped and already unwrapped property.
                // i.e. this already should be evaluated with prev. steps - because we are moving upward from deepest container !
                LSender.Send(this, $"prepare: variable name = '{ret.variable.name}'; tSign({ret.variable.tSign}), vSign({ret.variable.vSign})", MsgLevel.Trace);
            }


            /* Data */

            if((rStringDataD.Success || rStringDataS.Success) 
                && (rData.Success && !string.IsNullOrWhiteSpace(rData.Value)))
            {
                throw new IncorrectSyntaxException("Incorrect composition of data: string with raw data.");
            }

            if(rStringDataD.Success) {
                ret.property.unevaluated    = Eval(Tokens.UnescapeQuotes('"', rStringDataD.Value));
                ret.variable.type           = ValueType.StringFromDouble;
            }
            else if(rStringDataS.Success) {
                ret.property.unevaluated    = Tokens.UnescapeQuotes('\'', rStringDataS.Value);
                ret.variable.type           = ValueType.StringFromSingle;
            }
            else {
                ret.property.unevaluated    = HQuotes((rDataWithProject.Success ? rDataWithProject : rData).Value.Trim());
                ret.variable.type           = (rVariable.Success)? ValueType.Unknown : ValueType.Property;

                int lp = ret.property.unevaluated.IndexOf('.');
                ret.property.name = ((lp == -1)? ret.property.unevaluated : ret.property.unevaluated.Substring(0, lp)).Trim();

                //TODO:
                if(ret.property.name.IndexOf('[') != -1) {
                    ret.property.name = null; // avoid static methods
                }
            }

            ret.property.complex = !IsSimpleProperty(ref ret.property.unevaluated);

            LSender.Send(this, $"prepare: value = '{ret.property.unevaluated}'({ret.variable.type}); complex: {ret.property.complex}", MsgLevel.Trace);

            /* Project */

            if(rDataWithProject.Success)
            {
                string scope = rScope.Value.Trim();

                if(rVariable.Success) {
                    //TODO: non standard! but it also can be as a $(varname = $($(..data..):scope))
                    ret.variable.scope = scope;
                }
                else {
                    ret.property.scope = scope;
                }
            }

            LSender.Send(this, $"prepare: scope [v:'{ret.variable.scope}'; p:'{ret.property.scope}']", MsgLevel.Trace);
            return ret;
        }

        /// <summary>
        /// Handler of the general containers.
        /// Moving upward from deepest container.
        /// 
        /// $(name) or $(name:scope) or $([MSBuild]::MakeRelative($(path1), ...):scope) ..
        /// https://msdn.microsoft.com/en-us/library/vstudio/dd633440%28v=vs.120%29.aspx
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sh"></param>
        /// <param name="limit">Limitation to containers. Aborts if reached.</param>
        /// <exception cref="LimitException"></exception>
        /// <returns></returns>
        private protected string ContainerIn(string data, StringHandler sh, uint limit)
        {
            Regex con   = RPattern.ContainerInCompiled;
            int maxRep  = 1; // rule of depth, e.g.: $(p1 = $(Platform))$(p2 = $(p1))$(p2)
                             //TODO: it's slowest but fully compatible with classic rules with minimal programming.. so, improve performance

            references.Clear();
            uint step = 0;
            do
            {
                if(step++ > limit) {
                    sh.Flush();
                    throw new LimitException($"Restriction of supported containers '{limit}' reached. Aborted.", limit);
                }

                data = con.Replace
                (
                    data,
                    (Match m) =>
                    {
                        string raw = m.Groups[1].Value;
                        LSender.Send(this, $"containerIn: raw - `{raw}`", MsgLevel.Trace);

                        return Evaluate(Prepare(sh.Recovery(raw)));
                    },
                    maxRep
                );

                // protect before new checking
                data = sh.ProtectEscContainer(sh.ProtectMixedQuotes(data));

            } while(con.IsMatch(data));

            return data;
        }

        /// <param name="ident"></param>
        /// <returns>never null</returns>
        private protected Project GetProject(string ident)
        {
            try
            {
                LSender.Send(this, $"Trying to get a project instance: '{ident}'", MsgLevel.Trace);
                return env.GetProject(ident) ?? new Project();
            }
            catch(Exception ex)
            {
                LSender.Send(this, $"Use empty project by default due to: {ex.Message}", MsgLevel.Debug);
                return new Project();
            }
        }

        private protected string DefineVariable(Analysis prepared, string evaluated)
        {
            evaluated = VSignOperation(prepared, evaluated);

            LSender.Send(this, $"Evaluate: ready to define variable - '{prepared.variable.name}':'{prepared.variable.scope}'");

            UVars.SetVariable(prepared.variable.name, prepared.variable.scope, evaluated);
            UVars.Evaluate(prepared.variable.name, prepared.variable.scope, new EvaluatorBlank(), true);

            TSignOperation(prepared, ref evaluated);
            return string.Empty;
        }

        private protected string VSignOperation(Analysis prepared, string val)
        {
            if(prepared.variable.vSign == VSignType.Default) {
                return val;
            }

            var left        = UVars.GetValue(prepared.variable.name, prepared.variable.scope) ?? "0";
            bool isNumber   = RPattern.IsNumber.IsMatch(left);

            LSender.Send(this, $"vSignOperation: '{prepared.variable.vSign}'; `{left}` (isNumber: {isNumber})", MsgLevel.Trace);

            if(prepared.variable.vSign == VSignType.Increment)
            {
                if(!isNumber) {
                    // equiv.: $(var = $([System.String]::Concat($(var), "str")) )
                    return left + val;
                }

                // $(var = $([MSBuild]::Add($(var), 1)) )
                // TODO: additional check for errors from right string $(i += 'test') ... this correct: $(i += $(i))
                return Obtain($"$([MSBuild]::Add('{left}', '{val}'))", prepared.variable.scope);
            }

            if(prepared.variable.vSign == VSignType.Decrement)
            {
                if(!isNumber) {
                    throw new ArgumentException($"Argument `{val}` is not valid for operation '-=' or it is not supported yet.");
                }

                // $(var = $([MSBuild]::Subtract($(var), 1)) )
                return Obtain($"$([MSBuild]::Subtract('{left}', '{val}'))", prepared.variable.scope);
            }

            return val;
        }

        private protected void TSignOperation(Analysis prepared, ref string evaluated)
        {
#if DEBUG
            LSender.Send(this, $"tSignOperation: '{prepared.variable.tSign}'", MsgLevel.Trace);
#endif

            if(prepared.variable.tSign == TSignType.DefProperty) {
                DefProperty(prepared.variable, evaluated);
            }
            else if(prepared.variable.tSign == TSignType.UndefProperty) {
                UndefProperty(prepared.variable);
            }
        }

        private protected bool SetGlobalProperty(Project project, string name, string value)
        {
            if(name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            if(value == null) {
                value = string.Empty;
            }

            try
            {
                if(project == null) {
                    ProjectCollection.GlobalProjectCollection.SetGlobalProperty(name, value);
                    return true;
                }

                return project.SetGlobalProperty(name, value);
            }
            finally
            {
                Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
                GlobalPropertyChanged(this, new PropertyArgs(project, name, value));
            }
        }

        private protected bool RemoveGlobalProperty(Project project, string name)
        {
            if(name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            try
            {
                if(project == null) {
                    return ProjectCollection.GlobalProjectCollection.RemoveGlobalProperty(name);
                }

                return project.RemoveGlobalProperty(name);
            }
            finally
            {
                Environment.SetEnvironmentVariable(name, null, EnvironmentVariableTarget.Process);
                GlobalPropertyChanged(this, new PropertyArgs(project, name, null));
            }
        }

        private protected void DefProperty(TVariable uvar, Project project)
        {
            if(uvar.status != ValStatus.Started) {
                SetGlobalProperty(project, uvar.ident, GetUVarValue(uvar.ident));
                return;
            }

            if(uvar.prev != null && ((TVariable)uvar.prev).unevaluated != null)
            {
                TVariable prev = (TVariable)uvar.prev;
                SetGlobalProperty(project, uvar.ident, prev.evaluated ?? prev.unevaluated);
            }
        }

        private protected void DefProperty(AVariable variable, string evaluated)
        {
            LSender.Send(this, $"Set property: `{variable.name}`:`{variable.scope}`");

            //defProperty(uvariable.getVariable(variable.name, variable.scope), getProject(variable.scope));
            SetGlobalProperty(GetProject(variable.scope), variable.name, evaluated);
        }

        private protected bool UndefProperty(AVariable variable)
        {
            LSender.Send(this, $"Unset property: `{variable.name}`:`{variable.scope}`");
            Project project = GetProject(variable.scope);

            //uvariable.unset(variable.name, variable.scope); //leave for sbe-scripts
            return RemoveGlobalProperty(project, variable.name);
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
        private protected string Unlooping(string data, Analysis prepared, bool eqEscape)
        {
            string pname = prepared.property.name;

            if(pname == null || data.IndexOf("$(") == -1) {
                return data; // all clear
            }

            HashSet<string> pchecked = new HashSet<string>();

            void unlink(string _name)
            {
                if(!pchecked.Add(_name)) {
#if DEBUG
                    LSender.Send(this, $"unlooping-unlink: has been protected with `{_name}`");
#endif
                    return;
                    //throw new MismatchException("");
                }

                if(!references.ContainsKey(_name)) {
                    return;
                }

                if(references[_name].Contains(pname)) {
                    throw new LimitException($"Found looping for used properties: `{_name}` <-> `{pname}`");
                }

                foreach(string refvar in references[_name]) {
                    unlink(refvar);
                }
            }

            data = RPattern.ContainerInNamedCompiled.Replace(data, (Match m) =>
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

        private string WrapProperty(ref string data)
        {
            if(data.StartsWith("$(")) {
                return data;
            }

            LSender.Send(this, $"wrap: '{data}'", MsgLevel.Trace);
            return $"$({data})";
        }
    }
}
