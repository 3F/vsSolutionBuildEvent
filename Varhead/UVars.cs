/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.Varhead
{
    /// <summary>
    /// [Varhead]
    /// 
    /// Evaluator of user variables and more.
    /// Designed for SobaScript, E-MSBuild, and so on.
    /// https://github.com/3F/Varhead
    /// </summary>
    public class UVars: IUVars, IUVarsExt
    {
        /// <summary>
        /// Contains all defined user-variables.
        /// 
        /// !!! Note: ConcurrentDictionary used Nodes! order is unpredictable - see m_tables and internal adding
        /// http://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs
        /// https://bitbucket.org/3F/vssolutionbuildevent/commits/34cdc43df67#comment-1330734
        /// 
        /// Also variant use the both SynchronizedCollection/BlockingCollection + ConcurrentDictionary for O(1) operations
        /// </summary>
        protected IDictionary<string, TVariable> definitions = new Dictionary<string, TVariable>();

        private readonly object sync = new object();

        /// <summary>
        /// Exposes the enumerable for defined names of user-variables
        /// </summary>
        public IEnumerable<string> Definitions
        {
            get
            {
                foreach(KeyValuePair<string, TVariable> def in definitions.ToArray()) {
                    yield return def.Key;
                }
            }
        }

        /// <summary>
        /// Exposes the enumerable for defined user-variables
        /// </summary>
        public IEnumerable<TVariable> Variables
        {
            get
            {
                foreach(KeyValuePair<string, TVariable> def in definitions.ToArray()) {
                    yield return def.Value;
                }
            }
        }

        /// <summary>
        /// Getting value of user-variable by using specific scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns>Evaluated value of variable or null if variable not defined.</returns>
        public string GetValue(string name, string scope)
        {
            return GetValue(DefIndex(name, scope));
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
        /// Get user-variable struct by using specific scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns>Struct of user-variable.</returns>
        public TVariable GetVariable(string name, string scope)
        {
            return GetVariable(DefIndex(name, scope));
        }

        /// <summary>
        /// Get user-variable struct by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Struct of user-variable</returns>
        public TVariable GetVariable(string ident)
        {
            lock(sync)
            {
                if(definitions.ContainsKey(ident)) {
                    return definitions[ident];
                }
                return default(TVariable);
            }
        }

        /// <summary>
        /// Defines user-variable.
        /// Value setted as unevaluated.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <param name="unevaluated">Mixed string with unevaluated data.</param>
        public void SetVariable(string name, string scope, string unevaluated)
        {
            if(!IsValidName(name) || !IsValidScope(scope)) {
                throw new ArgumentException($"name - '{name}' or scope - '{scope}' is not valid for variable");
            }
            string defindex = DefIndex(name, scope);

            if(unevaluated == null) {
                unevaluated = String.Empty;
            }

            lock(sync)
            {
                definitions[defindex] = new TVariable() {
                    unevaluated = unevaluated,
                    ident       = defindex,
                    name        = name,
                    scope       = scope,
                    status      = ValStatus.Unevaluated,
                    prev        = (definitions.ContainsKey(defindex))? definitions[defindex] : new TVariable(),
                    evaluated   = null
                };
                LSender.Send(this, $"User-variable: defined '{defindex}' = '{unevaluated}'");
            }
        }

        /// <summary>
        /// Evaluate user-variable with IEvaluator by using specific scope.
        /// An evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">To reset IEvaluator chain to initial state if true. Otherwise, evaluation can be in the chain of other evaluators.</param>
        public void Evaluate(string name, string scope, IEvaluator evaluator, bool resetting)
        {
            Evaluate(DefIndex(name, scope), evaluator, resetting);
        }

        /// <summary>
        /// Evaluate user-variable with IEvaluator by using unique identification.
        /// An evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">To reset IEvaluator chain to initial state if true. Otherwise, evaluation can be in the chain of other evaluators.</param>
        public void Evaluate(string ident, IEvaluator evaluator, bool resetting)
        {
            if(evaluator == null) {
                throw new ArgumentNullException(nameof(evaluator));
            }

            lock(sync)
            {
                if(!definitions.ContainsKey(ident)) {
                    throw new DefinitionNotFoundException(ident);
                }

                TVariable var = new TVariable(definitions[ident]) {
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
        /// Is this variable with completed evaluation or not?
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns></returns>
        public bool IsUnevaluated(string name, string scope)
        {
            return IsUnevaluated(DefIndex(name, scope));
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
        /// by using specifc scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns></returns>
        public bool IsExist(string name, string scope)
        {
            return IsExist(DefIndex(name, scope));
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
        /// Validation of scope name.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns>Is valid or not</returns>
        public virtual bool IsValidScope(string scope)
        {
            if(string.IsNullOrEmpty(scope)) {
                return true;
            }

            //TODO:
            return true;
        }

        /// <summary>
        /// Removes user-variable
        /// by using specifc scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <exception cref="ArgumentNullException">key is null</exception>
        public void Unset(string name, string scope)
        {
            Unset(DefIndex(name, scope));
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
                definitions[ident] = new TVariable() {
                    unevaluated = evaluated,
                    ident       = ident,
                    status      = ValStatus.Evaluated,
                    prev        = (definitions.ContainsKey(ident))? definitions[ident] : new TVariable(),
                    evaluated   = evaluated
                };
                LSender.Send(this, $"User-variable(Debug service): updated '{ident}' with evaluated value '{evaluated}'");
            }
        }

        /// <summary>
        /// Used key-index for definitions
        /// </summary>
        protected string DefIndex(string name, string scope)
        {
            if(string.IsNullOrEmpty(scope)) {
                return name;
            }
            return $"{name}_{scope}";
        }
    }
}
