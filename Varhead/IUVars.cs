﻿/*
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

using System.Collections.Generic;

namespace net.r_eg.Varhead
{
    /// <summary>
    /// [Varhead]
    /// 
    /// Evaluator of user variables and more.
    /// Designed for SobaScript, E-MSBuild, and so on.
    /// https://github.com/3F/Varhead
    /// </summary>
    public interface IUVars
    {
        /// <summary>
        /// Exposes the enumerable for defined names of user-variables.
        /// </summary>
        IEnumerable<string> Definitions { get; }

        /// <summary>
        /// Exposes the enumerable for defined user-variables.
        /// </summary>
        IEnumerable<TVariable> Variables { get; }

        /// <summary>
        /// Getting value of user-variable by using specific scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns>Evaluated value of variable or null if variable not defined.</returns>
        string GetValue(string name, string scope);

        /// <summary>
        /// Getting value of user-variable by using unique identification.
        /// </summary>
        /// <param name="ident">Unique identificator.</param>
        /// <returns>Evaluated value of variable.</returns>
        string GetValue(string ident);

        /// <summary>
        /// Get user-variable struct by using specific scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns>Struct of user-variable.</returns>
        TVariable GetVariable(string name, string scope);

        /// <summary>
        /// Get user-variable struct by using unique identification.
        /// </summary>
        /// <param name="ident">Unique identificator.</param>
        /// <returns>Struct of user-variable.</returns>
        TVariable GetVariable(string ident);

        /// <summary>
        /// Defines user-variable.
        /// Value setted as unevaluated.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <param name="unevaluated">Mixed string with unevaluated data.</param>
        void SetVariable(string name, string scope, string unevaluated);

        /// <summary>
        /// Evaluate user-variable with IEvaluator by using specific scope.
        /// An evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">To reset IEvaluator chain to initial state if true. Otherwise, evaluation can be in the chain of other evaluators.</param>
        void Evaluate(string name, string scope, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Evaluate user-variable with IEvaluator by using unique identification.
        /// An evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">To reset IEvaluator chain to initial state if true. Otherwise, evaluation can be in the chain of other evaluators.</param>
        void Evaluate(string ident, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Is this variable with completed evaluation or not?
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns></returns>
        bool IsUnevaluated(string name, string scope);

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool IsUnevaluated(string ident);

        /// <summary>
        /// Checking existence of variable
        /// by using specifc scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        /// <returns></returns>
        bool IsExist(string name, string scope);

        /// <summary>
        /// Checking existence of variable 
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool IsExist(string ident);

        /// <summary>
        /// Removes user-variable
        /// by using specifc scope.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="scope">Specified scope for this variable.</param>
        void Unset(string name, string scope);

        /// <summary>
        /// Removes user-variable
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        void Unset(string ident);

        /// <summary>
        /// Removes all user-variables
        /// </summary>
        void UnsetAll();
    }
}
