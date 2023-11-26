/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Configuration.User
{
    public class UserValue: IUserValue
    {
        /// <summary>
        /// Type of link to external value.
        /// </summary>
        public LinkType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Guid of external node.
        /// </summary>
        public string Guid
        {
            get;
            set;
        }

        /// <summary>
        /// Manager of accessing to remote value.
        /// </summary>
        [JsonIgnore]
        public IManager Manager
        {
            get
            {
                if(manager == null) {
                    manager = new Manager(this);
                }
                return manager;
            }
        }
        private IManager manager;


        /// <summary>
        /// Initialize with new Guid and specific LinkType.
        /// </summary>
        /// <param name="Type">Type of link to external value.</param>
        public UserValue(LinkType Type)
        {
            Guid        = System.Guid.NewGuid().ToString();
            this.Type   = Type;
        }

        public UserValue()
        {

        }
    }
}
