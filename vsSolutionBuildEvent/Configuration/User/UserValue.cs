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
        public string Guid { get; set; }

        [JsonIgnore]
        public IManager Manager { get; set; }

        public UserValue()
        {
            Guid = System.Guid.NewGuid().ToString();
            Manager = new Manager(this);
        }
    }
}
