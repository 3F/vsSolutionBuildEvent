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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class UserVariable: IUserVariable, IUserVariableDebug
    {
        /// <summary>
        /// Exposes the enumerable for defined names of user-variables
        /// </summary>
        public IEnumerable<string> Definitions
        {
            get {
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
            get {
                foreach(KeyValuePair<string, TUserVariable> def in definitions.ToArray()) {
                    yield return def.Value;
                }
            }
        }

        /// <summary>
        /// Contains the all defined user-variables.
        /// 
        /// Note: ConcurrentDictionary used Nodes! order is unpredictable - see m_tables & internal adding
        /// http://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs
        /// https://bitbucket.org/3F/vssolutionbuildevent/commits/34cdc43df67#comment-1330734
        /// 
        /// Also variant use the both SynchronizedCollection/BlockingCollection + ConcurrentDictionary for O(1) operations
        /// </summary>
        protected Dictionary<string, TUserVariable> definitions = new Dictionary<string, TUserVariable>();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Getting value of user-variable by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>evaluated value of variable or null if variable not defined</returns>
        public string get(string name, string project)
        {
            return get(defIndex(name, project));
        }

        /// <summary>
        /// Getting value of user-variable by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Evaluated value of variable</returns>
        public string get(string ident)
        {
            lock(_lock)
            {
                if(!definitions.ContainsKey(ident)) {
                    return null;
                }
                string evaluated = definitions[ident].evaluated;

                if(evaluated == null) {
                    Log.nlog.Debug("getValue: evaluated value of '{0}' is null", ident);
                    evaluated = String.Empty;
                }
                return evaluated;
            }
        }

        /// <summary>
        /// Defines user-variable
        /// Value setted as unevaluated
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name or null if project is default</param>
        /// <param name="unevaluated">mixed string. Converted to empty string if value is null</param>
        public void set(string name, string project, string unevaluated)
        {
            if(!isValidName(name) || !isValidProject(project)) {
                throw new InvalidArgumentException("name - '{0}' or project - '{1}' is not valid for variable", name, project);
            }
            string defindex = defIndex(name, project);

            if(unevaluated == null) {
                unevaluated = String.Empty;
            }

            lock(_lock)
            {
                definitions[defindex] = new TUserVariable() {
                    unevaluated = unevaluated,
                    ident       = defindex,
                    status      = TUserVariable.StatusType.Unevaluated,
                    prev        = (definitions.ContainsKey(defindex))? definitions[defindex] : new TUserVariable(),
                    evaluated   = null
                };
                Log.nlog.Debug("User-variable: defined '{0}' = '{1}'", defindex, unevaluated);
            }
        }

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using scope of project
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="project">Project name</param>
        /// <param name="msbuild">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluating from the unevaluated data if true, otherwise evaluation in the chain of others IEvaluator's</param>
        public void evaluate(string name, string project, IEvaluator evaluator, bool resetting)
        {
            evaluate(defIndex(name, project), evaluator, resetting);
        }

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using unique identification
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="msbuild">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluating from the unevaluated data if true, otherwise evaluation in the chain of others IEvaluator's</param>
        public void evaluate(string ident, IEvaluator evaluator, bool resetting)
        {
            lock(_lock)
            {
                if(!definitions.ContainsKey(ident)) {
                    throw new NotFoundException("Variable '{0}' not found", ident);
                }

                if(evaluator == null) {
                    throw new InvalidArgumentException("evaluation of variable: evaluator is null");
                }

                TUserVariable var = new TUserVariable(definitions[ident]) {
                    status = TUserVariable.StatusType.Started
                };
                definitions[ident] = var;

                if(resetting) {
                    var.evaluated = evaluator.evaluate(var.unevaluated);
                }
                else {
                    var.evaluated = evaluator.evaluate(var.evaluated);
                }
                var.status          = TUserVariable.StatusType.Evaluated;
                definitions[ident]  = var;
                Log.nlog.Debug("Completed evaluation of variable with IEvaluator :: '{0}'", ident);
            }
        }

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        public bool isUnevaluated(string name, string project)
        {
            return isUnevaluated(defIndex(name, project));
        }

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        public bool isUnevaluated(string ident)
        {
            return (definitions[ident].status == TUserVariable.StatusType.Unevaluated);
        }

        /// <summary>
        /// Checking existence of variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        public bool isExist(string name, string project)
        {
            return isExist(defIndex(name, project));
        }

        /// <summary>
        /// Checking existence of variable 
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        public bool isExist(string ident)
        {
            return definitions.ContainsKey(ident);
        }

        /// <summary>
        /// Validation of variable name
        /// </summary>
        /// <param name="name">variable name</param>
        /// <returns>Is valid or not</returns>
        public virtual bool isValidName(string name)
        {
            if(String.IsNullOrEmpty(name)) {
                return false;
            }
            return Regex.Match(name, "^[a-z_][a-z_0-9]*$", RegexOptions.IgnoreCase).Success;
        }

        /// <summary>
        /// Validation of project name
        /// </summary>
        /// <param name="project">project name</param>
        /// <returns>Is valid or not</returns>
        public virtual bool isValidProject(string project)
        {
            if(String.IsNullOrEmpty(project)) {
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
        public void unset(string name, string project)
        {
            unset(defIndex(name, project));
        }

        /// <summary>
        /// Removes user-variable
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        public void unset(string ident)
        {
            lock(_lock)
            {
                if(definitions.Remove(ident)) {
                    Log.nlog.Debug("User-variable is successfully unset '{0}'", ident);
                    return;
                }
            }
            Log.nlog.Debug("Cannot unset the user-variable '{0}'", ident);
        }

        /// <summary>
        /// Remove all user-variables
        /// </summary>
        public void unsetAll()
        {
            lock(_lock) {
                definitions.Clear();
            }
            Log.nlog.Debug("Reseted all User-variables");
        }

        /// <summary>
        /// Re/Defines user-variable with evaluated value.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluated">mixed string with evaluated data</param>
        public void debSetEvaluated(string ident, string evaluated)
        {
            if(evaluated == null) {
                evaluated = String.Empty;
            }

            lock(_lock)
            {
                definitions[ident] = new TUserVariable() {
                    unevaluated = evaluated,
                    ident       = ident,
                    status      = TUserVariable.StatusType.Evaluated,
                    prev        = (definitions.ContainsKey(ident))? definitions[ident] : new TUserVariable(),
                    evaluated   = evaluated
                };
                Log.nlog.Debug("User-variable(Debug service): updated '{0}' with evaluated value '{1}'", ident, evaluated);
            }
        }

        /// <summary>
        /// Used key-index for definitions
        /// </summary>
        protected string defIndex(string name, string project)
        {
            if(String.IsNullOrEmpty(project)) {
                return name;
            }
            return String.Format("{0}_{1}", name, project);
        }
    }
}
