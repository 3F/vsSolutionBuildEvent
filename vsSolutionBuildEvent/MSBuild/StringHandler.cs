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
using net.r_eg.Varhead;

namespace net.r_eg.vsSBE.MSBuild
{
    public class StringHandler: StringProtector
    {
        /// <summary>
        /// Specific format of double quotes with content
        /// </summary>
        public override string DoubleQuotesContentFull
        {
            get { return RPattern.DoubleQuotesContentFull; }
        }

        /// <summary>
        /// Specific format of single quotes with content
        /// </summary>
        public override string SingleQuotesContentFull
        {
            get { return RPattern.SingleQuotesContentFull; }
        }

        /// <summary>
        /// Protects escaped MSBuild data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectEscContainer(string data)
        {
            lock(sync)
            {
                return Regex.Replace(data, RPattern.ContainerEscOuter, delegate(Match m)
                {
                    uint ident      = IdentNext;
                    strings[ident]  = "$" + m.Groups[1].Value;
                    Log.Trace("StringHandler: protect the escaped outer container '{0}' :: '{1}'", strings[ident], ident);
                    return ReplacementIn(ident);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }
    }
}
