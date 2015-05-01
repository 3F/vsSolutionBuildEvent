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

namespace net.r_eg.vsSBE.Scripts
{
    public static class Tokens
    {
        /// <summary>
        /// Handler of the escape-sequence.
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
        /// <param name="limited">a limited set of combinations if true</param>
        /// <returns></returns>
        public static string characters(string data, bool limited = true)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            string ret;

            if(!limited) {
                ret = Regex.Unescape(data);
            }

            ret = Regex.Replace(data, String.Format(@"(\\){{1,2}}
                                                      (?:
                                                         r|n|t|v|a|b|0|f
                                                         |x{0}{{1,4}}
                                                         |u{0}{{4}}
                                                         |U{0}{{8}}
                                                      )", "[A-Fa-f0-9]"),
                                delegate(Match m)
                                {
                                    if(m.Groups[1].Value.Length > 1) {
                                        return m.Value.Substring(1);
                                    }
                                    return Regex.Unescape(m.Value);
                                }, 
                                RegexOptions.IgnorePatternWhitespace);

            Log.nlog.Trace("Tokens: processed characters '{0}'", ret);
            return ret;
        }
    }
}
