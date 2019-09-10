/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) E-MSBuild contributors: https://github.com/3F/E-MSBuild/graphs/contributors
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
using net.r_eg.Varhead;

namespace net.r_eg.EvMSBuild
{
    internal class StringHandler: SProtectorAbstract
    {
        /// <summary>
        /// Specific format of double quotes with content
        /// </summary>
        public override string DoubleQuotesContentFull => Pattern.DoubleQuotesContentFull;

        /// <summary>
        /// Specific format of single quotes with content
        /// </summary>
        public override string SingleQuotesContentFull => Pattern.SingleQuotesContentFull;

        /// <summary>
        /// Protects escaped MSBuild data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string ProtectEscContainer(string data)
        {
            lock(sync)
            {
                return Regex.Replace
                (
                    data, 
                    Pattern.ContainerEscOuter, 
                    (Match m) =>
                    {
                        uint ident      = IdentNext;
                        strings[ident]  = "$" + m.Groups[1].Value;

                        LSender.Send(this, $"StringHandler: protect the escaped outer container '{strings[ident]}' :: '{ident}'", MsgLevel.Trace);
                        return ReplacementIn(ident);
                    },
                    RegexOptions.IgnorePatternWhitespace
                );
            }
        }
    }
}
