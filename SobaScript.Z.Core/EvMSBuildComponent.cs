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
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;

namespace net.r_eg.SobaScript.Z.Core
{
    [Definition("$()", "Evaluation with E-MSBuild engine - https://github.com/3F/E-MSBuild")]
    public class EvMSBuildComponent: ComponentAbstract, IComponent
    {
        /*
         * TODO: lazy compiled patterns
         */

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "$(";

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
                @"^\[(\$+)\(     # 1
                     (?'exp'.+) # MSBuild expression
                      \)\]$", 
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline
            );

            if(!m.Success) {
                throw new IncorrectSyntaxException($"'{ToString()}' Failed `{data}`");
            }

            string type = m.Groups[1].Value;
            string exp  = m.Groups["exp"].Value;

            return emsbuild.Eval($"{type}({Multiline(exp)})");
        }

        public EvMSBuildComponent(ISobaScript soba)
            : base(soba)
        {

        }

        protected virtual string Multiline(string cmd)
        {
            // var hString = new StringHandler();
            return Regex.Replace(cmd, @"[\r\n]\s*", string.Empty);
        }
    }
}
