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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.Varhead
{
    /// <summary>
    /// This variant is based on storing of tokens inside string.
    /// It's simply, but we have a some problems (like with guid) and other inconvenience from final strings.
    /// As alternative for this, we should implement the nodes storing (like SobaScript SNode) to abstract build of all final strings by tokens etc. 
    /// Well, currently we're simple :)
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

        protected object sync = new object();

        /// <summary>
        /// Current level of nesting recovery operation.
        /// Aborting if reached limit
        /// </summary>
        private volatile uint recoveryLevel;

        /// <summary>
        /// Unsigned identifier of tokens.
        /// </summary>
        private volatile uint ident = 0;

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
        protected uint IdentNext => ++ident;

        /// <summary>
        /// Get current ident number
        /// </summary>
        protected uint Ident => ident;

        /// <summary>
        /// Protects data inside mixed quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string ProtectMixedQuotes(string data) => ProtectQuotes
        (
            ref data, 
            string.Format
            (
                @"({0}|{1})",               // #1 - mixed
                DoubleQuotesContentFull,    // #2 -  ".." 
                SingleQuotesContentFull     // #3 -  '..'
            )
        );

        /// <summary>
        /// Protects data inside single quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string ProtectSingleQuotes(string data)
        {
            return ProtectQuotes(ref data, SingleQuotesContentFull);
        }

        /// <summary>
        /// Protects data inside double quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>protected string</returns>
        public string ProtectDoubleQuotes(string data)
        {
            return ProtectQuotes(ref data, DoubleQuotesContentFull);
        }

        /// <summary>
        /// Protects data with custom pattern.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pattern">Pattern with first ($1) capture group.</param>
        /// <returns>protected string</returns>
        public string ProtectByPattern(string data, string pattern)
            => Regex.Replace(data, pattern, ReplacerIn, RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Restores the all protected data for strings.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Recovery(string data)
        {
            Debug.Assert(strings != null);

            if(recoveryLevel >= RECOVERY_LIMIT) {
                recoveryLevel = 0;
                throw new LimitException($"StringProtector->recovery: Nesting level of '{RECOVERY_LIMIT}' reached. Aborted.", RECOVERY_LIMIT);
            }

            string format = ReplacementOut();
            lock(sync)
            {
                string ret = Regex.Replace(data, format, delegate(Match m)
                {
                    uint index = UnpackId(m.Groups[1].Value);
                    strings.TryRemove(index, out string removed); // deallocate protected string
#if DEBUG
                    LSender.Send(this, $"StringProtector: recovery string '{removed}' :: '{index}' /level: {recoveryLevel}", MsgLevel.Trace);
#endif
                    return removed;
                });

                if(Regex.IsMatch(ret, format))
                {
#if DEBUG
                    LSender.Send(this, $"StringProtector->recovery: found the new protected data - '{ret}'", MsgLevel.Trace);
#endif
                    ++recoveryLevel;
                    ret = Recovery(ret);
                    --recoveryLevel;
                }
                return ret;
            }
        }

        /// <summary>
        /// Flushes internal storage for protected strings
        /// </summary>
        public void Flush()
        {
            strings.Clear();
            ident = 0;
        }

        /// <summary>
        /// Format of protection.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        protected virtual string ReplacementFormat(string format)
        {
            // no conflict, because all variants with '!' as argument is not possible without quotes.
            return $"!{PackId(UID)}@{format}!";
        }

        protected virtual string PackId(uint id) => id.ToString("x");

        protected virtual uint UnpackId(string id) => Convert.ToUInt32(id, 16);

        protected virtual string FormatId() => "([0-9a-f]+)";

        protected StringProtector()
        {
            lock(sync)
            {
                Collect();
                UID = guid++;
            }
        }

        protected void Collect()
        {
            //TODO:
            if(guid == uint.MaxValue) {
                LSender.Send(this, "Collect: reset id");
                guid = 0;
            }
        }

        /// <summary>
        /// Protects data inside quotes by format.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format">type/s of quotes</param>
        /// <returns>protected string</returns>
        protected string ProtectQuotes(ref string data, string format)
            => Regex.Replace(data, format, ReplacerIn, RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// Replacer by default for protection inside strings.
        /// </summary>
        /// <param name="m"></param>
        /// <returns>string with changed data</returns>
        protected string ReplacerIn(Match m)
        {
            lock(sync)
            {
                uint ident = IdentNext;
                strings[ident] = m.Groups[1].Value;
#if DEBUG
                LSender.Send(this, $"StringProtector: protect `{strings[ident]}` :: '{ident}'", MsgLevel.Trace);
#endif
                return ReplacementIn(ident);
            }
        }

        /// <summary>
        /// How to protect data.
        /// </summary>
        /// <param name="ident">Unique identificator</param>
        /// <returns></returns>
        protected string ReplacementIn(uint ident) => ReplacementFormat(PackId(ident));

        /// <summary>
        /// How to recover data from strings.
        /// </summary>
        /// <returns></returns>
        protected string ReplacementOut() => ReplacementFormat(FormatId());
    }
}