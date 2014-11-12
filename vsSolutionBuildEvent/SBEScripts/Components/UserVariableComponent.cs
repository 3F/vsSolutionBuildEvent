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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// For work with User-Variables
    /// </summary>
    public class UserVariableComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "[var "; }
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of used user-variables</param>
        public UserVariableComponent(IEnvironment env, IUserVariable uvariable): base(env, uvariable)
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[var
                                              \s+
                                              ([A-Za-z_0-9]+)  #1 - name 
                                              (?:
                                                :([^=\]]+)     #2 - project (optional)
                                              )?
                                              \s*
                                              (?:
                                                =\s*
                                                (.+)           #3 - mixed data for definition (optional)
                                              )?
                                           \]$", // #3 - greedy, however it's controlled by main container of SBE-Script
                                           RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed UserVariableComponent - '{0}'", data);
            }

            string name     = m.Groups[1].Value;
            string project  = (m.Groups[2].Success)? m.Groups[2].Value.Trim() : null;
            string value    = (m.Groups[3].Success)? m.Groups[3].Value : null;
            
            if(value != null) {
                set(name, project, value);
                return String.Empty;
            }
            return get(name, project);
        }

        /// <summary>
        /// Setting user-variable with the scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <param name="value">Mixed value for variable</param>
        protected void set(string name, string project, string value)
        {
            uvariable.set(name, project, value);
            evaluate(name, project);
        }

        /// <summary>
        /// Setting user-variable only with simple name
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Mixed value for variable</param>
        protected void set(string name, string value)
        {
            set(name, null, value);
        }

        /// <summary>
        /// Getting value from user-value
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">scope of project</param>
        /// <returns>evaluated value from found variable</returns>
        /// <exception cref="NotFoundException">if not found</exception>
        protected string get(string name, string project = null)
        {
            if(!uvariable.isExist(name, project)) {
                throw new NotFoundException("UVariable '{0}:{1}' not found", name, project);
            }

            if(uvariable.isUnevaluated(name, project)) {
                evaluate(name, project);
            }
            return uvariable.get(name, project);
        }

        protected virtual void evaluate(string name, string project = null)
        {
            uvariable.evaluate(name, project, (IEvaluator)script, true);
            if(postProcessingMSBuild) {
                uvariable.evaluate(name, project, (IEvaluator)msbuild, false);
            }
        }
    }
}
