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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class StringHandler
    {
        /// <summary>
        /// Maximum of nesting level to recovery operation
        /// </summary>
        protected const int RECOVERY_LIMIT = 50;

        /// <summary>
        /// Storage of protected strings
        /// Contains the all protected strings for recovery operations.
        /// </summary>
        protected ConcurrentDictionary<uint, string> strings = new ConcurrentDictionary<uint, string>();

        /// <summary>
        /// Current level of nesting recovery operation.
        /// Aborting if reached limit
        /// </summary>
        private volatile uint _recoveryLevel = 0;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Protects data containing the quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectQuotes(string data)
        {
            lock(_lock)
            {
                uint ident = (uint)strings.Count;
                return Regex.Replace(data, String.Format(@"({0}|{1})",                     // #1 - mixed
                                                            RPattern.DoubleQuotesContent,  // #2 -  ".." 
                                                            RPattern.SingleQuotesContent), // #3 -  '..'
                delegate(Match m)
                {
                    strings[ident] = m.Groups[1].Value;
                    Log.nlog.Trace("StringHandler: protect string '{0}' :: '{1}'", strings[ident], ident);
                    return replacementIn(ident++);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Protects data containing the MSBuild/SBE-Scripts containers.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectCores(string data)
        {
            lock(_lock)
            {
                uint ident = (uint)strings.Count;
                return Regex.Replace(data, String.Format(@"({0}|{1})",                          // #1 - mixed
                                                @"\#{1,2}" + RPattern.SquareBracketsContent,    // #2 -  #[..]
                                                @"\${1,2}" + RPattern.RoundBracketsContent),    // #3 -  $(..)
                delegate(Match m)
                {
                    strings[ident] = m.Groups[1].Value;
                    Log.nlog.Trace("StringHandler: protect cores '{0}' :: '{1}'", strings[ident], ident);
                    return replacementIn(ident++);
                },
                RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Restores the all protected data for strings.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string recovery(string data)
        {
            Debug.Assert(strings != null);

            if(_recoveryLevel >= RECOVERY_LIMIT) {
                _recoveryLevel = 0;
                throw new LimitException("StringHandler->recovery: Nesting level of '{0}' reached. Aborted.", RECOVERY_LIMIT);
            }

            lock(_lock)
            {
                string ret = Regex.Replace(data, replacementOut(), delegate(Match m)
                {
                    string removed;
                    uint index = uint.Parse(m.Groups[1].Value);
                    strings.TryRemove(index, out removed); // deallocate protected string

                    Log.nlog.Trace("StringHandler: recovery string '{0}' :: '{1}' /level: {2}", removed, index, _recoveryLevel);
                    return removed;
                });

                if(Regex.IsMatch(ret, replacementOut()))
                {
                    Log.nlog.Trace("StringHandler->recovery: found the new protected data - '{0}'", ret);
                    ++_recoveryLevel;
                    ret = recovery(ret);
                    --_recoveryLevel;
                }
                return ret;
            }
        }

        /// <summary>
        /// Flushes internal storage for protected strings
        /// </summary>
        public void flush()
        {
            strings.Clear();
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

        /// <summary>
        /// How to protect data in strings
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        protected string replacementIn(uint ident)
        {
            return replacementFormat(ident.ToString());
        }

        /// <summary>
        /// How to recover data from strings
        /// </summary>
        /// <returns></returns>
        protected string replacementOut()
        {
            return replacementFormat(@"(\d+)");
        }

        /// <summary>
        /// Format of protection
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual string replacementFormat(string format)
        {
            // no conflict, because all variants with '!' as argument is not possible without quotes.
            return String.Format("!s{0}!", format);
        }
    }
}
