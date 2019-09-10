/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
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
using net.r_eg.Varhead;

namespace net.r_eg.SobaScript
{
    public class StringHandler: SProtectorAbstract
    {
        /// <summary>
        /// Specific format of double quotes with content
        /// </summary>
        public override string DoubleQuotesContentFull
            => Pattern.DoubleQuotesContentFull;

        /// <summary>
        /// Specific format of single quotes with content
        /// </summary>
        public override string SingleQuotesContentFull
            => Pattern.SingleQuotesContentFull;

        /// <summary>
        /// Protects the MSBuild/SBE-Scripts containers.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string ProtectCores(string data) => Regex.Replace
        (
            data, 
            string.Format
            (
                @"({0}|{1})",                                   // #1 - mixed
                @"\#{1,2}" + Pattern.SquareBracketsContent,     // #2 -  #[..]
                @"\${1,2}" + Pattern.RoundBracketsContent       // #3 -  $(..)
            ), 
            ReplacerIn,
            RegexOptions.IgnorePatternWhitespace
        );

        /// <summary>
        /// Normalize data for strings.
        /// eg. to unescape double quotes etc.
        /// TODO: obsolete
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Normalize(string data)
        {
            if(string.IsNullOrEmpty(data)) {
                return string.Empty;
            }
            return UnescapeQuotes('"', data);
        }

        /// <summary>
        /// Escaping quotes in data
        /// </summary>
        /// <param name="data">mixed string</param>
        /// <returns>data with escaped quotes</returns>
        public static string EscapeQuotes(string data)
        {
            if(string.IsNullOrEmpty(data)) {
                return string.Empty;
            }

            // (?<!\\)"
            return Regex.Replace(data, "(?<!\\\\)\"", "\\\"");
        }

        /// <summary>
        /// Protects ArrayContent data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        internal string ProtectArray(string data)
            => ProtectByPattern(data, $"({Pattern.ObjectContent})");

        /// <summary>
        /// Protects argument list.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        internal string ProtectArguments(string data)
            => ProtectArray(ProtectMixedQuotes(data));

        /// <summary>
        /// Protects data inside &lt;#data&gt; ... &lt;/#data&gt;
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal string ProtectDataSection(string data) // <#data> ... </#data>
            => Regex.Replace(data, @"<#data>(.*?)<\/#data>", ReplacerIn, RegexOptions.Singleline);

        /// <summary>
        /// Protection methods by default.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal string Protect(string data)
            => ProtectMixedQuotes(ProtectDataSection(data));

        /// <summary>
        /// Unescape quote symbols from string.
        /// TODO
        /// </summary>
        /// <param name="type">Quote symbol.</param>
        /// <param name="data"></param>
        /// <returns>String with unescaped quote symbols.</returns>
        internal static string UnescapeQuotes(char type, string data)
            => Tokens.UnescapeQuotes(type, data);
    }
}
