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

namespace net.r_eg.vsSBE.Events.CommandEvents
{
    /// <summary>
    /// Filters for ICommandEvent
    /// </summary>
    public class Filter: IFilter
    {
        /// <summary>
        /// For work with command ID
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Scope by GUID
        /// </summary>
        public string Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Filter by Custom input parameters
        /// </summary>
        public object CustomIn
        {
            get;
            set;
        }

        /// <summary>
        /// Filter by Custom output parameters
        /// </summary>
        public object CustomOut
        {
            get;
            set;
        }

        /// <summary>
        /// Cancel command if it's possible
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }

        /// <summary>
        /// Use Before executing command
        /// </summary>
        public bool Pre
        {
            get { return pre; }
            set { pre = value; }
        }
        private bool pre = true;

        /// <summary>
        /// Use After executed command
        /// </summary>
        public bool Post
        {
            get { return post; }
            set { post = value; }
        }
        private bool post = false;

        /// <summary>
        /// About filter
        /// </summary>
        public string Description
        {
            get;
            set;
        }
    }
}
