/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Linq;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Extensions;
using EOWP = net.r_eg.vsSBE.Events.OWP;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// TODO: this old matcher from v0.4 or less, need refact for new structure
    /// </summary>
    public class Matcher
    {
        /// <summary>
        /// Checking with IMatching.
        /// </summary>
        /// <param name="filters">Specific filter for checking.</param>
        /// <param name="raw"></param>
        /// <param name="guid">Guid string of pane</param>
        /// <param name="item">Name of item pane</param>
        /// <returns>matched if one of conditions is true</returns>
        public bool match(Events.OWP.IMatching[] filters, string raw, string guid, string item)
        {
            if(raw == null || filters == null || filters.Length < 1) {
                return false;
            }

            foreach(Events.OWP.IMatching filter in filters)
            {
                //if(!checkPane(filter, guid, item)) { //TODO: the guid & item currently used only in vsCE
                //    continue;
                //}

                switch(filter.Type)
                {
                    case Events.OWP.ComparisonType.Default: {
                        if(mDefault(filter.Phrase, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                    case Events.OWP.ComparisonType.Regexp: {
                        if(mRegexp(filter.Phrase, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                    case Events.OWP.ComparisonType.Wildcards: {
                        if(mWildcards(filter.Phrase, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checking with IMatchWords.
        /// </summary>
        /// <param name="filters">Specific filter for checking.</param>
        /// <param name="raw"></param>
        /// <param name="guid">Guid string of pane</param>
        /// <param name="item">Name of item pane</param>
        /// <returns>matched if one of conditions is true</returns>
        public bool match(IMatchWords[] filters, string raw, string guid, string item)
        {
            var nf = filters.Select(f => new EOWP.Condition()
                                         {
                                                Phrase = f.Condition,
                                                Type = (EOWP.ComparisonType)f.Type
                                         })
                                         .ToArray();

            return match(nf, raw, guid, item);
        }

        protected bool mRegexp(string pattern, ref string raw)
        {
            try {
                return Regex.Match(raw, pattern/*, RegexOptions.IgnoreCase*/).Success;
            }
            catch(Exception ex) {
                // all incorrect syntax should be simply false
                Log.Warn("OWPMatcher: {0}", ex.Message);
            }
            return false;
        }

        protected bool mWildcards(string pattern, ref string raw)
        {
            //TODO: rapid alternative https://bitbucket.org/3F/sandbox/src/master-C%2B%2B/cpp/text/wildcards/wildcards/versions/essential/AlgorithmEss.h
            //_
            string stub = Regex.Escape(pattern).Replace("\\*", ".*?").Replace("\\+", ".+?").Replace("\\?", ".");
            return mRegexp(stub, ref raw);
        }

        protected bool mDefault(string pattern, ref string raw)
        {
            return raw.Contains(pattern);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="guid">Guid string of pane</param>
        /// <param name="item">Name of item pane</param>
        /// <returns></returns>
        protected bool checkPane(Events.OWP.IMatching filter, string guid, string item)
        {
            if(guid.CompareGuids(GuidList.OWP_SBE_STRING) || item == Settings.OWP_ITEM_VSSBE) {
                return false; // TODO: it may be as a Logging event from vsSBE later
            }

            if(String.IsNullOrEmpty(filter.PaneGuid) && String.IsNullOrEmpty(filter.PaneName)) {
                return false;
            }

            if(!String.IsNullOrEmpty(filter.PaneGuid) && guid.CompareGuids(filter.PaneGuid)) {
                return true;
            }

            if(!String.IsNullOrEmpty(filter.PaneName) && filter.PaneName == item) {
                return true;
            }

            return false;
        }
    }
}
