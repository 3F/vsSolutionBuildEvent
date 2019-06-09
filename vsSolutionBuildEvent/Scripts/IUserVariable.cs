/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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

using System.Collections.Generic;

namespace net.r_eg.vsSBE.Scripts
{
    public interface IUserVariable
    {
        /// <summary>
        /// Exposes the enumerable for defined names of user-variables
        /// </summary>
        IEnumerable<string> Definitions { get; }

        /// <summary>
        /// Exposes the enumerable for defined user-variables
        /// </summary>
        IEnumerable<TUserVariable> Variables { get; }

        /// <summary>
        /// Getting value of user-variable by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>evaluated value of variable</returns>
        string get(string name, string project);

        /// <summary>
        /// Getting value of user-variable by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Evaluated value of variable</returns>
        string get(string ident);

        /// <summary>
        /// Get user-variable struct by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <returns>Struct of user-variable</returns>
        TUserVariable getVariable(string name, string project);

        /// <summary>
        /// Get user-variable struct by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns>Struct of user-variable</returns>
        TUserVariable getVariable(string ident);

        /// <summary>
        /// Defines user-variable
        /// Value setted as unevaluated
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        /// <param name="unevaluated">mixed string with unevaluated data</param>
        void set(string name, string project, string unevaluated);

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using scope of project
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="name">Variable name for evaluating</param>
        /// <param name="project">Project name</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluation can be in the chain of others IEvaluator's, this flag should reset this to initial state</param>
        void evaluate(string name, string project, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Evaluation user-variable with IEvaluator by using unique identification
        /// Evaluated value should be updated for variable.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <param name="evaluator">IEvaluator objects for evaluating</param>
        /// <param name="resetting">Evaluation can be in the chain of others IEvaluator's, this flag should reset this to initial state</param>
        void evaluate(string ident, IEvaluator evaluator, bool resetting);

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        bool isUnevaluated(string name, string project);

        /// <summary>
        /// Checking for variable - completed evaluation or not
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool isUnevaluated(string ident);

        /// <summary>
        /// Checking existence of variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <returns></returns>
        bool isExist(string name, string project);

        /// <summary>
        /// Checking existence of variable 
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        bool isExist(string ident);

        /// <summary>
        /// Removes user-variable
        /// by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        void unset(string name, string project);

        /// <summary>
        /// Removes user-variable
        /// by using unique identification
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        void unset(string ident);

        /// <summary>
        /// Removes all user-variables
        /// </summary>
        void unsetAll();
    }
}
