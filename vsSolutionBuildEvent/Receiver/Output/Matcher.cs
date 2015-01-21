/*
 * Copyright (c) 2013  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Receiver.Output
{
    public class Matcher
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters">terms of user</param>
        /// <param name="raw"></param>
        /// <returns>matched if at least one of conditions are true</returns>
        public bool match(IMatchWords[] filters, string raw)
        {
            if(filters == null) {
                return false;
            }

            foreach(IMatchWords filter in filters)
            {
                switch(filter.Type) {
                    case ComparisonType.Default: {
                        if(mDefault(filter.Condition, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                    case ComparisonType.Regexp: {
                        if(mRegexp(filter.Condition, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                    case ComparisonType.Wildcards: {
                        if(mWildcards(filter.Condition, ref raw)) {
                            return true;
                        }
                        continue;
                    }
                }
            }
            return false;
        }

        protected bool mRegexp(string pattern, ref string raw)
        {
            try {
                return Regex.Match(raw, pattern/*, RegexOptions.IgnoreCase*/).Success;
            }
            catch(Exception ex) {
                // all incorrect syntax should be simply false
                Log.nlog.Warn("OWPMatcher: {0}", ex.Message);
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
    }
}
