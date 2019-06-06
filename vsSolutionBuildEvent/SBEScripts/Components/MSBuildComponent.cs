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

using System;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    [Definition("$()", "Advanced evaluation with MSBuild engine.")]
    public class MSBuildComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "$("; }
        }
        
        /// <param name="loader">Initialization with loader</param>
        public MSBuildComponent(IBootloader loader)
            : base(loader)
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[(\$+)\(     # 1
                                              (?'exp'.+) # MSBuild expression
                                               \)\]$", 
                                               RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new SyntaxIncorrectException("'{0}' Failed `{1}`", ToString(), data);
            }

            string type = m.Groups[1].Value;
            string exp  = m.Groups["exp"].Value;

            return msbuild.parse($"{type}({multiline(exp)})");
        }

        protected virtual string multiline(string cmd)
        {
            // var hString = new StringHandler();
            return Regex.Replace(cmd, @"[\r\n]\s*", String.Empty);
        }
    }
}
