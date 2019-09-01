﻿/*
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

using System.Text.RegularExpressions;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Works with User-Variables
    /// </summary>
    [Definition("var", "Dynamic User-Variables")]
    [Definition("name", "Get data from variable 'name'", "var")]
    [Definition("name = ", "Set mixed data for variable 'name'", "var")]
    [Definition("-name", "Unset variable 'name'", "var")]
    [Definition("+name", "Default value for variable 'name'", "var")]
    public class UserVariableComponent: ComponentAbstract, IComponent
    {
        /// <summary>
        /// Default value for user-variables.
        /// The '*Undefined*' as value for compatibility with MSBuild core.
        /// </summary>
        public const string UVARIABLE_VALUE_DEFAULT = EvMSBuilder.UNDEF_VAL;

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition => "var ";

        public UserVariableComponent(ISobaScript soba)
            : base(soba)
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match
            (
                data, 
                @"^\[var
                    \s+
                    (\+|-)           #1 - operation
                    ([A-Za-z_0-9]+)  #2 - name
                    (?:
                    :([^=\]]+)     #3 - project (optional)
                    )?
                    \s*
                \]$",
                RegexOptions.IgnorePatternWhitespace
            );

            if(!m.Success) {
                return std(data);
            }
            
            string op       = m.Groups[1].Value;
            string name     = m.Groups[2].Value;
            string project  = (m.Groups[3].Success)? m.Groups[3].Value.Trim() : null;

            Log.Trace($"`{ToString()}`: found `{op}` as operation");
            switch(op)
            {
                case "+": {
                    Log.Debug("UVariable: set default value for variable - '{0}':'{1}'", name, project);
                    set(name, project, UVARIABLE_VALUE_DEFAULT);
                    return Value.Empty;
                }
                case "-": {
                    Log.Debug("UVariable: unset variable - '{0}':'{1}'", name, project);
                    unset(name, project);
                    return Value.Empty;
                }
            }

            throw new SubtypeNotFoundException(op, $"data: {data}");
        }

        /// <summary>
        /// Standard operations with variables
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        protected string std(string data)
        {
            Log.Trace("UVariable: use std handler");
            Match m = Regex.Match(data, @"^\[var
                                              \s+
                                              ([A-Za-z_0-9]+)  #1 - name 
                                              (?:
                                                :([^=\]]+)     #2 - project (optional)
                                              )?
                                              \s*
                                              (?:
                                                =\s*
                                                (.*)           #3 - mixed data for definition (optional)
                                              )?
                                           \]$", // #3 - greedy, however it's controlled by main container of SBE-Script
                                           RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed UserVariableComponent - '{data}'");
            }

            string name     = m.Groups[1].Value;
            string project  = (m.Groups[2].Success)? m.Groups[2].Value.Trim() : null;
            string value    = (m.Groups[3].Success)? m.Groups[3].Value : null;

            Log.Trace("UVariable: found '{0}':'{1}' = '{2}'", name, project, value);
            if(value != null) {
                set(name, project, value);
                return Value.Empty;
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
            uvars.SetVariable(name, project, value);
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
        /// Removes user-variable by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        protected void unset(string name, string project = null)
        {
            uvars.Unset(name, project);
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
            if(!uvars.IsExist(name, project)) {
                throw new DefinitionNotFoundException($"{name}:{project}");
            }

            if(uvars.IsUnevaluated(name, project)) {
                evaluate(name, project);
            }
            return uvars.GetValue(name, project);
        }

        protected virtual void evaluate(string name, string project = null)
        {
            uvars.Evaluate(name, project, (IEvaluator)soba, true);
            if(PostProcessingMSBuild) {
                uvars.Evaluate(name, project, (IEvaluator)msbuild, false);
            }
        }
    }
}
