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

namespace net.r_eg.vsSBE.SBEScripts
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
        /// Protects the MSBuild/SBE-Scripts containers.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectCores(string data)
        {
            lock(_lock)
            {
                return Regex.Replace(data, String.Format(@"({0}|{1})",                          // #1 - mixed
                                                @"\#{1,2}" + RPattern.SquareBracketsContent,    // #2 -  #[..]
                                                @"\${1,2}" + RPattern.RoundBracketsContent),    // #3 -  $(..)
                        delegate(Match m)
                        {
                            uint ident      = IdentNext;
                            strings[ident]  = m.Groups[1].Value;
#if DEBUG
                            Log.Trace("StringHandler: protect cores '{0}' :: '{1}'", strings[ident], ident);
#endif
                            return replacementIn(ident);
                        },
                        RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Protects ArrayContent data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectArray(string data)
        {
            return protectByPattern(data, String.Format("({0})", RPattern.ObjectContent));
        }

        /// <summary>
        /// Protects argument list.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectArguments(string data)
        {
            return protectArray(protectMixedQuotes(data));
        }

        /// <summary>
        /// Escaping quotes in data
        /// </summary>
        /// <param name="data">mixed string</param>
        /// <returns>data with escaped quotes</returns>
        public static string escapeQuotes(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            // (?<!\\)"
            return Regex.Replace(data, "(?<!\\\\)\"", "\\\"");
        }

        /// <summary>
        /// Normalize data for strings.
        /// e.g.: unescape double quotes etc.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string normalize(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            return data.Replace("\\\"", "\"");
        }

        /// <summary>
        /// Format of protection
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected override string replacementFormat(string format)
        {
            // no conflict, because all variants with '!' as argument is not possible without quotes.
            return String.Format("!s{0}!", format);
        }
    }
}
