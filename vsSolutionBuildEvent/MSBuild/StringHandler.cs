/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.MSBuild
{
    public class StringHandler: Scripts.StringProtector
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
            lock(_lock)
            {
                return Regex.Replace(data, RPattern.ContainerEscOuter, delegate(Match m)
                {
                    uint ident      = IdentNext;
                    strings[ident]  = "$" + m.Groups[1].Value;
                    Log.nlog.Trace("StringHandler: protect the escaped outer container '{0}' :: '{1}'", strings[ident], ident);
                    return replacementIn(ident);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Format of protection
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected override string replacementFormat(string format)
        {
            // no conflict, because all variants with '!' as argument is not possible without quotes.
            return String.Format("!p{0}!", format);
        }
    }
}
