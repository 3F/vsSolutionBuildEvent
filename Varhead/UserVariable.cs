/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) Varhead contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.Components;

namespace net.r_eg.Varhead
{
    public class UserVariable: IUserVariable, IUserVariableExt
    {
        /// <summary>
        /// Contains all defined user-variables.
        /// 
        /// Note: ConcurrentDictionary used Nodes! order is unpredictable - see m_tables and internal adding
        /// http://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs
        /// https://bitbucket.org/3F/vssolutionbuildevent/commits/34cdc43df67#comment-1330734
        /// 
        /// Also variant use the both SynchronizedCollection/BlockingCollection + ConcurrentDictionary for O(1) operations
        /// </summary>
        protected Dictionary<string, TUserVariable> definitions = new Dictionary<string, TUserVariable>();

        private readonly object sync = new object();

        /// <summary>
        /// Exposes the enumerable for defined names of user-variables
        /// </summary>
        public IEnumerable<string> Definitions
        {
            get
            {
                foreach(KeyValuePair<string, TUserVariable> def in definitions.ToArray()) {
                    yield return def.Key;
                }
            }
        }

        /// <summary>
        /// Exposes the enumerable for defined user-variables
        /// </summary>
        public IEnumerable<TUserVariable> Variables
        {
            get
            {
                foreach(KeyValuePair<string, TUserVariable> def in definitions.ToArray()) {
                    yield return def.Value;
                }
            }
        }

        /// <summary>
        /// Getting value of user-variable by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>evaluated value of variable or null if variable not defined</returns>
        public string GetValue(string name, string project)
        {
            return GetValue(DefIndex(name, project));
        }

        /// <summary>
        /// Getting value of user-variable by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Evaluated value of variable</returns>
        public string GetValue(string ident)
        {
            lock(sync)
            {
                if(!definitions.ContainsKey(ident)) {
                    return null;
                }
                string evaluated = definitions[ident].evaluated;

                if(evaluated == null) {
                    LSender.Send(this, $"getValue: evaluated value of '{ident}' is null");
                    evaluated = string.Empty;
                }
                return evaluated;
            }
        }

        /// <summary>
        /// Get user-variable struct by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>Struct of user-variable</returns>
        public TUserVariable GetVariable(string name, string project)
        {
            return GetVariable(DefIndex(name, project));
        }

        /// <summary>
        /// Get user-variable struct by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Struct of user-variable</returns>
        public TUserVariable GetVariable(string ident)
        {
            lock(sync)
            {
                if(definitions.ContainsKey(ident)) {
                    return definitions[ident];
                }
                return default(TUserVariable);
            }
        }

        /// <summary>
        /// Defines user-variable
        /// Value setted as unevaluated
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name or null if project is default</param>
        /// <param name="unevaluated">mixed string. Converted to empty string if value is null</param>
        public void SetVariable(string name, string project, string unevaluated)
        {
            if(!IsValidName(name) || !IsValidProject(project)) {
                throw new ArgumentException($"name - '{name}' or project - '{project}' is not valid for variable");
            }
            string defindex = DefIndex(name, project);

            if(unevaluated == null) {
                unevaluated = String.Empty;
            }

            lock(sync)
            {
                definitions[defindex] = new TUserVariable() {
                    unevaluated = unevaluated,
                    ident       = defindex,
                    name        = name,
                    project     = project,
                    status      = ValStatus.Unevaluated,
                    prev        = (definitions.ContainsKey(defindex))? definitions[defindex] : new TUserVariable(),
                    evaluated   = null
                };
                LSender.Send(this, $"User-variable: defined '{defindex}' = '{unevaluated}'");
            }
        }

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using scope of project
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="project">Project name</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluating from the unevaluated data if true, otherwise evaluation in the chain of others IEvaluator's</param>
        public void Evaluate(string name, string project, IEvaluator evaluator, bool resetting)
        {
            Evaluate(DefIndex(name, project), evaluator, resetting);
        }

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using unique identification
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluating from the unevaluated data if true, otherwise evaluation in the chain of others IEvaluator's</param>
        public void Evaluate(string ident, IEvaluator evaluator, bool resetting)
        {
            if(evaluator == null) {
                throw new ArgumentNullException(nameof(evaluator));
            }

            lock(sync)
            {
                if(!definitions.ContainsKey(ident)) {
                    throw new KeyNotFoundException($"Variable '{ident}' is not found.");
                }

                TUserVariable var = new TUserVariable(definitions[ident]) {
                    status = ValStatus.Started
                };
                definitions[ident] = var;

                if(resetting) {
                    var.evaluated = evaluator.Evaluate(var.unevaluated);
                }
                else {
                    var.evaluated = evaluator.Evaluate(var.evaluated);
                }
                var.status          = ValStatus.Evaluated;
                definitions[ident]  = var;
                LSender.Send(this, $"IEvaluator '{evaluator.GetType().ToString()}': Evaluation of variable '{ident}' is completed.", MsgLevel.Trace);
            }
        }

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        public bool IsUnevaluated(string name, string project)
        {
            return IsUnevaluated(DefIndex(name, project));
        }

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        public bool IsUnevaluated(string ident)
        {
            return (definitions[ident].status == ValStatus.Unevaluated);
        }

        /// <summary>
        /// Checking existence of variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        public bool IsExist(string name, string project)
        {
            return IsExist(DefIndex(name, project));
        }

        /// <summary>
        /// Checking existence of variable 
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        public bool IsExist(string ident)
        {
            return definitions.ContainsKey(ident);
        }

        /// <summary>
        /// Validation of variable name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns>Is valid or not</returns>
        public virtual bool IsValidName(string name)
        {
            if(string.IsNullOrEmpty(name)) {
                return false;
            }
            return Regex.Match(name, "^[a-z_][a-z_0-9]*$", RegexOptions.IgnoreCase).Success;
        }

        /// <summary>
        /// Validation of project name
        /// </summary>
        /// <param name="project">project name</param>
        /// <returns>Is valid or not</returns>
        public virtual bool IsValidProject(string project)
        {
            if(string.IsNullOrEmpty(project)) {
                return true;
            }
            //TODO:
            return true;
        }

        /// <summary>
        /// Remove user-variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <exception cref="ArgumentNullException">key is null</exception>
        public void Unset(string name, string project)
        {
            Unset(DefIndex(name, project));
        }

        /// <summary>
        /// Removes user-variable
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        public void Unset(string ident)
        {
            lock(sync)
            {
                if(definitions.Remove(ident)) {
                    LSender.Send(this, $"User-variable is successfully unset '{ident}'");
                    return;
                }
            }
            LSender.Send(this, $"Cannot unset the user-variable '{ident}'");
        }

        /// <summary>
        /// Remove all user-variables
        /// </summary>
        public void UnsetAll()
        {
            lock(sync) {
                definitions.Clear();
            }
            LSender.Send(this, "Reseted all User-variables", MsgLevel.Trace);
        }

        /// <summary>
        /// Re/Defines user-variable with evaluated value.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluated">mixed string with evaluated data</param>
        public void SetEvaluated(string ident, string evaluated)
        {
            if(evaluated == null) {
                evaluated = String.Empty;
            }

            lock(sync)
            {
                definitions[ident] = new TUserVariable() {
                    unevaluated = evaluated,
                    ident       = ident,
                    status      = ValStatus.Evaluated,
                    prev        = (definitions.ContainsKey(ident))? definitions[ident] : new TUserVariable(),
                    evaluated   = evaluated
                };
                LSender.Send(this, $"User-variable(Debug service): updated '{ident}' with evaluated value '{evaluated}'");
            }
        }

        /// <summary>
        /// Used key-index for definitions
        /// </summary>
        protected string DefIndex(string name, string project)
        {
            if(string.IsNullOrEmpty(project)) {
                return name;
            }
            return $"{name}_{project}";
        }
    }
}
