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
using net.r_eg.Varhead;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class StringHandler: SProtectorAbstract
    {
        /// <summary>
        /// Specific format of double quotes with content
        /// </summary>
        public override string DoubleQuotesContentFull
            => RPattern.DoubleQuotesContentFull;

        /// <summary>
        /// Specific format of single quotes with content
        /// </summary>
        public override string SingleQuotesContentFull
            => RPattern.SingleQuotesContentFull;

        /// <summary>
        /// Protects the MSBuild/SBE-Scripts containers.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectCores(string data) => Regex.Replace
        (
            data, 
            string.Format
            (
                @"({0}|{1})",                                   // #1 - mixed
                @"\#{1,2}" + RPattern.SquareBracketsContent,    // #2 -  #[..]
                @"\${1,2}" + RPattern.RoundBracketsContent      // #3 -  $(..)
            ), 
            ReplacerIn,
            RegexOptions.IgnorePatternWhitespace
        );

        /// <summary>
        /// Protects ArrayContent data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectArray(string data)
        {
            return ProtectByPattern(data, $"({RPattern.ObjectContent})");
        }

        /// <summary>
        /// Protects argument list.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectArguments(string data)
        {
            return protectArray(ProtectMixedQuotes(data));
        }

        /// <summary>
        /// Protects data inside &lt;#data&gt; ... &lt;/#data&gt;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string protectDataSection(string data) // <#data> ... </#data>
            => Regex.Replace(data, @"<#data>(.*?)<\/#data>", ReplacerIn, RegexOptions.Singleline);

        /// <summary>
        /// Protection methods by default.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string protect(string data)
        {
            return ProtectMixedQuotes(protectDataSection(data));
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
        /// Unescape quote symbols from string.
        /// TODO
        /// </summary>
        /// <param name="type">Quote symbol.</param>
        /// <param name="data"></param>
        /// <returns>String with unescaped quote symbols.</returns>
        public static string unescapeQuotes(char type, string data)
        {
            return Tokens.UnescapeQuotes(type, data);
        }

        /// <summary>
        /// Normalize data for strings.
        /// e.g.: unescape double quotes etc.
        /// TODO: obsolete
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string normalize(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            return unescapeQuotes('"', data);
        }
    }
}
