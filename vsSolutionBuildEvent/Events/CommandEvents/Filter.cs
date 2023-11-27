/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
