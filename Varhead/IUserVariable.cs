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

using System.Collections.Generic;

namespace net.r_eg.Varhead
{
    public interface IUserVariable
    {
        /// <summary>
        /// Exposes the enumerable for defined names of user-variables.
        /// </summary>
        IEnumerable<string> Definitions { get; }

        /// <summary>
        /// Exposes the enumerable for defined user-variables.
        /// </summary>
        IEnumerable<TUserVariable> Variables { get; }

        /// <summary>
        /// Getting value of user-variable by using scope of project.
        /// </summary>
        /// <param name="name">variable name.</param>
        /// <param name="project">project name.</param>
        /// <returns>evaluated value of variable.</returns>
        string GetValue(string name, string project);

        /// <summary>
        /// Getting value of user-variable by using unique identification.
        /// </summary>
        /// <param name="ident">Unique identificator.</param>
        /// <returns>Evaluated value of variable.</returns>
        string GetValue(string ident);

        /// <summary>
        /// Get user-variable struct by using scope of project.
        /// </summary>
        /// <param name="name">variable name.</param>
        /// <param name="project">project name.</param>
        /// <returns>Struct of user-variable.</returns>
        TUserVariable GetVariable(string name, string project);

        /// <summary>
        /// Get user-variable struct by using unique identification.
        /// </summary>
        /// <param name="ident">Unique identificator.</param>
        /// <returns>Struct of user-variable.</returns>
        TUserVariable GetVariable(string ident);

        /// <summary>
        /// Defines user-variable.
        /// Value setted as unevaluated.
        /// </summary>
        /// <param name="name">variable name.</param>
        /// <param name="project">project name.</param>
        /// <param name="unevaluated">mixed string with unevaluated data.</param>
        void SetVariable(string name, string project, string unevaluated);

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using scope of project
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="project">Project name</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluation can be in the chain of others IEvaluator's, this flag should reset this to initial state</param>
        void Evaluate(string name, string project, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using unique identification
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluation can be in the chain of others IEvaluator's, this flag should reset this to initial state</param>
        void Evaluate(string ident, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        bool IsUnevaluated(string name, string project);

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool IsUnevaluated(string ident);

        /// <summary>
        /// Checking existence of variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        bool IsExist(string name, string project);

        /// <summary>
        /// Checking existence of variable 
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool IsExist(string ident);

        /// <summary>
        /// Removes user-variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        void Unset(string name, string project);

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
