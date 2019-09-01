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

using System.Text.RegularExpressions;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// The comments for scripts.
    /// </summary>
    [Definition("\" \"", "The multiline comment.")]
    public class CommentComponent: ComponentAbstract, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition => "\"";

        /// <summary>
        /// Should be located before deepening
        /// </summary>
        public override bool BeforeDeepen => true;

        public CommentComponent()
            : base()
        {

        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[""
                                              .*
                                              ""\]$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);

            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed CommentComponent - '{data}'");
            }

            return Value.Empty; // silent
        }
    }
}
