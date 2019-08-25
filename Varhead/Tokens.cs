/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) Varhead contributors: https://github.com/3F/Varhead/graphs/contributors
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

using System;
using System.Text.RegularExpressions;
using net.r_eg.Components;

namespace net.r_eg.Varhead
{
    public static class Tokens
    {
        /// <summary>
        /// Handler of the escape-sequences.
        /// 
        ///     hexadecimal-escape-sequence:
        ///         \x   0-0xF  [0-0xF  [0-0xF  [0-0xF]]]
        ///     https://msdn.microsoft.com/en-us/library/aa691087%28v=vs.71%29.aspx
        ///     
        ///     unicode-escape-sequence:
        ///         \u   0-0xF  0-0xF  0-0xF  0-0xF
        ///         \U   0-0xF  0-0xF  0-0xF  0-0xF  0-0xF  0-0xF  0-0xF  0-0xF
        ///     https://msdn.microsoft.com/en-us/library/aa664669%28v=vs.71%29.aspx
        /// </summary>
        /// <param name="data"></param>
        /// <param name="limited">Use limited set of combinations if true.</param>
        /// <returns></returns>
        public static string EscapeCharacters(string data, bool limited = true)
        {
            if(string.IsNullOrEmpty(data)) {
                return string.Empty;
            }

            string ret;

            if(!limited) {
                // https://msdn.microsoft.com/en-us/library/system.text.regularexpressions.regex.unescape.aspx
                ret = Regex.Unescape(data); //inc.: \, *, +, ?, |, {, }, [, ], (,), ^, $,., #, and white space characters
                LSender.Send(typeof(Tokens), $"Tokens: processed characters '{ret}'", MsgLevel.Trace);
                return ret;
            }

            ret = Regex.Replace
            (
                data, 
                string.Format(
                    @"(\\){{1,2}}
                        (?:
                            r|n|t|v|a|b|0|f
                            |x{0}{{1,4}}
                            |u{0}{{4}}
                            |U{0}{{8}}
                        )", "[A-Fa-f0-9]"
                ),
                (Match m) =>
                {
                    if(m.Groups[1].Value.Length > 1) {
                        return m.Value.Substring(1);
                    }
                    return Regex.Unescape(m.Value);
                }, 
                RegexOptions.IgnorePatternWhitespace
            );

            LSender.Send(typeof(Tokens), $"Tokens: limited processed characters '{ret}'", MsgLevel.Trace);
            return ret;
        }

        /// <summary>
        /// Unescape quote symbols from string.
        /// </summary>
        /// <param name="type">The quote character.</param>
        /// <param name="data"></param>
        /// <returns>Final string with an unescaped quote symbols.</returns>
        public static string UnescapeQuotes(char type, string data)
        {
            if(string.IsNullOrWhiteSpace(data)) {
                return string.Empty;
            }

            switch(type)
            {
                case '\'':
                case '"': {
                    return data.Replace("\\" + type, type.ToString());
                }
            }

            throw new NotSupportedException($"The quote symbol ({type}) is not supported.");
        }
    }
}
