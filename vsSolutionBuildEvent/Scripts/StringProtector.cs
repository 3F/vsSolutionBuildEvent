/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.Scripts
{
    /// <summary>
    /// This variant is based on storing of tokens inside string.
    /// It's simply, but we have a some problems (like with guid) and other inconvenience from final strings.
    /// As alternative for this, we should implement the nodes storing (like SNode) to abstract build of all final strings by tokens etc. Well, currently we're simple :)
    /// </summary>
    public abstract class StringProtector
    {
        /// <summary>
        /// Maximum of nesting level to recovery operation
        /// </summary>
        public const int RECOVERY_LIMIT = 50;

        /// <summary>
        /// Storage of protected strings
        /// Contains the all protected strings for recovery operations.
        /// </summary>
        protected ConcurrentDictionary<uint, string> strings = new ConcurrentDictionary<uint, string>();

        /// <summary>
        /// object synch.
        /// </summary>
        protected Object _lock = new Object();

        /// <summary>
        /// Current level of nesting recovery operation.
        /// Aborting if reached limit
        /// </summary>
        private volatile uint _recoveryLevel;

        /// <summary>
        /// Unsigned identifier of tokens.
        /// </summary>
        private volatile uint _ident = 0;

        /// <summary>
        /// Unique identifier between all protectors.
        /// TODO
        /// </summary>
        private static uint guid = 0;

        /// <summary>
        /// Specific format of double quotes with content
        /// </summary>
        public abstract string DoubleQuotesContentFull { get; }

        /// <summary>
        /// Specific format of single quotes with content
        /// </summary>
        public abstract string SingleQuotesContentFull { get; }

        /// <summary>
        /// Unique identifier for current protector.
        /// </summary>
        public uint UID
        {
            get;
            protected set;
        }

        /// <summary>
        /// Generates and returns the next ident number
        /// </summary>
        protected uint IdentNext
        {
            get { return ++_ident; }
        }

        /// <summary>
        /// Get current ident number
        /// </summary>
        protected uint Ident
        {
            get { return _ident; }
        }

        /// <summary>
        /// Protects data inside mixed quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectMixedQuotes(string data)
        {
            return protectQuotes(ref data, String.Format(@"({0}|{1})",                  // #1 - mixed
                                                            DoubleQuotesContentFull,    // #2 -  ".." 
                                                            SingleQuotesContentFull));  // #3 -  '..'
        }

        /// <summary>
        /// Protects data inside single quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectSingleQuotes(string data)
        {
            return protectQuotes(ref data, SingleQuotesContentFull);
        }

        /// <summary>
        /// Protects data inside double quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string protectDoubleQuotes(string data)
        {
            return protectQuotes(ref data, DoubleQuotesContentFull);
        }

        /// <summary>
        /// Protects data with custom pattern.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pattern">Pattern with first ($1) capture group.</param>
        /// <returns>protected string</returns>
        public string protectByPattern(string data, string pattern)
        {
            lock(_lock) {
                return Regex.Replace(data, pattern, replacerIn, RegexOptions.IgnorePatternWhitespace);
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
                throw new LimitException("StringProtector->recovery: Nesting level of '{0}' reached. Aborted.", RECOVERY_LIMIT);
            }

            string format = replacementOut();
            lock(_lock)
            {
                string ret = Regex.Replace(data, format, delegate(Match m)
                {
                    string removed;
                    uint index = unpackId(m.Groups[1].Value);
                    strings.TryRemove(index, out removed); // deallocate protected string
#if DEBUG
                    Log.Trace("StringProtector: recovery string '{0}' :: '{1}' /level: {2}", removed, index, _recoveryLevel);
#endif
                    return removed;
                });

                if(Regex.IsMatch(ret, format))
                {
#if DEBUG
                    Log.Trace("StringProtector->recovery: found the new protected data - '{0}'", ret);
#endif
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
            _ident = 0;
        }

        protected StringProtector()
        {
            lock (_lock) {
                gcollect();
                UID = guid++;
            }
        }

        protected void gcollect()
        {
            //TODO:
            if(guid == uint.MaxValue) {
                Log.Debug("gcollect: reset id");
                guid = 0;
            }
        }

        /// <summary>
        /// Protects data inside quotes by format.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format">type/s of quotes</param>
        /// <returns>protected string</returns>
        protected string protectQuotes(ref string data, string format)
        {
            lock(_lock) {
                return Regex.Replace(data, format, replacerIn, RegexOptions.IgnorePatternWhitespace);
            }
        }

        /// <summary>
        /// Replacer by default for protection inside strings.
        /// </summary>
        /// <param name="m"></param>
        /// <returns>string with changed data</returns>
        protected string replacerIn(Match m)
        {
            uint ident      = IdentNext;
            strings[ident]  = m.Groups[1].Value;
#if DEBUG
            Log.Trace("StringProtector: protect `{0}` :: '{1}'", strings[ident], ident);
#endif
            return replacementIn(ident);
        }

        /// <summary>
        /// How to protect data.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        protected string replacementIn(uint ident)
        {
            return replacementFormat(packId(ident));
        }

        /// <summary>
        /// How to recover data from strings.
        /// </summary>
        /// <returns></returns>
        protected string replacementOut()
        {
            return replacementFormat(formatId());
        }

        /// <summary>
        /// Format of protection.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual string replacementFormat(string format)
        {
            // no conflict, because all variants with '!' as argument is not possible without quotes.
            return String.Format("!{0}@{1}!", packId(UID), format);
        }

        protected virtual string packId(uint id)
        {
            return id.ToString("x");
        }

        protected virtual uint unpackId(string id)
        {
            return Convert.ToUInt32(id, 16);
        }

        protected virtual string formatId()
        {
            return "([0-9a-f]+)";
        }
    }
}