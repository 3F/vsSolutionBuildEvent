/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Core contributors: https://github.com/3F/Varhead/graphs/contributors
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

using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.SobaScript.Z.Core
{
    [Definition("var", "Dynamic User-Variables through Varhead - https://github.com/3F/Varhead")]
    [Definition("name", "Get data from variable 'name'", "var")]
    [Definition("name = ", "Set mixed data for variable 'name'", "var")]
    [Definition("-name", "Unset variable 'name'", "var")]
    [Definition("+name", "Default value for variable 'name'", "var")]
    public class UserVariableComponent: ComponentAbstract, IComponent
    {
        /*
         * TODO: lazy compiled patterns
         */

        /// <summary>
        /// Default value for user-variables.
        /// The '*Undefined*' as value for compatibility with MSBuild core.
        /// </summary>
        public const string UVARIABLE_VALUE_DEFAULT = EvMSBuilder.UNDEF_VAL;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "var ";

        /// <summary>
        /// Prepare, Parse, and Evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
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
                return Std(data);
            }
            
            string op       = m.Groups[1].Value;
            string name     = m.Groups[2].Value;
            string project  = m.Groups[3].Success ? m.Groups[3].Value.Trim() : null;

            LSender.Send(this, $"`{ToString()}`: found `{op}` as operation", MsgLevel.Trace);
            switch(op)
            {
                case "+": {
                    LSender.Send(this, $"UVariable: Set default value for variable - '{name}':'{project}'");
                    Set(name, project, UVARIABLE_VALUE_DEFAULT);
                    return Value.Empty;
                }
                case "-": {
                    LSender.Send(this, $"UVariable: Unset variable - '{name}':'{project}'");
                    Unset(name, project);
                    return Value.Empty;
                }
            }

            throw new SubtypeNotFoundException(op, $"data: {data}");
        }

        public UserVariableComponent(ISobaScript soba)
            : base(soba)
        {

        }

        /// <summary>
        /// Standard operations with variables
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        protected string Std(string data)
        {
            LSender.Send(this, "UVariable: use Std handler", MsgLevel.Trace);
            Match m = Regex.Match
            (
                data, 
                @"^\[var
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
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline
            );

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed UserVariableComponent - '{data}'");
            }

            string name     = m.Groups[1].Value;
            string project  = (m.Groups[2].Success)? m.Groups[2].Value.Trim() : null;
            string value    = (m.Groups[3].Success)? m.Groups[3].Value : null;

            LSender.Send(this, $"UVariable: found '{name}':'{project}' = '{value}'", MsgLevel.Trace);
            if(value != null) {
                Set(name, project, value);
                return Value.Empty;
            }
            return Get(name, project);
        }

        /// <summary>
        /// Setting user-variable with the scope of project
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="project">Project name</param>
        /// <param name="value">Mixed value for variable</param>
        protected void Set(string name, string project, string value)
        {
            uvars.SetVariable(name, project, value);
            Evaluate(name, project);
        }

        /// <summary>
        /// Setting user-variable only with simple name
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="value">Mixed value for variable</param>
        protected void Set(string name, string value)
        {
            Set(name, null, value);
        }

        /// <summary>
        /// Removes user-variable by using scope of project
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="project">project name</param>
        protected void Unset(string name, string project = null)
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
        protected string Get(string name, string project = null)
        {
            if(!uvars.IsExist(name, project)) {
                throw new DefinitionNotFoundException($"{name}:{project}");
            }

            if(uvars.IsUnevaluated(name, project)) {
                Evaluate(name, project);
            }
            return uvars.GetValue(name, project);
        }

        protected virtual void Evaluate(string name, string project = null)
        {
            uvars.Evaluate(name, project, soba, true);
            if(PostProcessingMSBuild) {
                uvars.Evaluate(name, project, emsbuild, false);
            }
        }
    }
}
