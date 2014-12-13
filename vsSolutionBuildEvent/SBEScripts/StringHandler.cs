/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class StringHandler
    {
        /// <summary>
        /// Storage of protected strings
        /// Contains the all found strings from data
        /// </summary>
        protected ConcurrentDictionary<uint, string> strings = new ConcurrentDictionary<uint, string>();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Protecting the all strings for data
        /// For work with depth, we need protect data from intersections with data from strings
        /// </summary>
        /// <param name="data"></param>
        /// <returns>safety data</returns>
        public string protect(string data)
        {
            lock(_lock)
            {
                uint ident = 0;
                strings.Clear();

                return Regex.Replace(data, String.Format(@"({0}|{1})",                     // #1 - mixed
                                                            RPattern.DoubleQuotesContent,  // #2 -  ".." 
                                                            RPattern.SingleQuotesContent), // #3 -  '..'
                delegate(Match m)
                {
                    strings[ident] = m.Groups[1].Value;
                    Log.nlog.Trace("StringHandler: protect string '{0}' :: '{1}'", strings[ident], ident);

                    // no conflict, because all variants with '!' as argument is not possible without quotes.
                    return String.Format("!s{0}!", ident++);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Restoring protected strings after handling protectStrings()
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string recovery(string data)
        {
            Debug.Assert(strings != null);

            lock(_lock)
            {
                return Regex.Replace(data, @"!s(\d+)!", delegate(Match m)
                {
                    string removed;
                    uint index = uint.Parse(m.Groups[1].Value);
                    strings.TryRemove(index, out removed); // deallocate protected string

                    Log.nlog.Trace("StringHandler: recovery string '{0}' :: '{1}'", removed, index);
                    return removed;
                });
            }
        }

        /// <summary>
        /// Escaping quotes in data
        /// </summary>
        /// <param name="data">mixed string</param>
        /// <returns>data with escaped quotes</returns>
        public static string escapeQuotes(string data)
        {
            // (?<!\\)"
            return Regex.Replace(data, "(?<!\\\\)\"", "\\\"");
        }

        /// <summary>
        /// Normalization data of strings with escaped double quotes etc.
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
        /// Handler for special symbols
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string hSymbols(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }

            return Regex.Replace(data, @"(\\)?\\(r|n|t|x([A-Fa-f0-9]{2}))", delegate(Match m) {

                if(m.Groups[1].Success) {
                    return m.Value.Substring(1);
                }

                if(m.Groups[3].Success) {
                    return ((char)Convert.ToInt32(m.Groups[3].Value, 16)).ToString();
                }

                switch(m.Groups[2].Value) {
                    case "r": { return "\r"; }
                    case "n": { return "\n"; }
                    case "t": { return "\t"; }
                }

                return m.Value;
            });
        }
    }
}
