/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;

namespace net.r_eg.vsSBE.Configuration.User
{
    public class Global: IGlobal
    {
        /// <inheritdoc cref="IGlobal.DebugMode"/>
        public bool DebugMode { get; set; }

        /// <inheritdoc cref="IGlobal.LogIgnoreLevels"/>
        //[JsonProperty(TypeNameHandling = TypeNameHandling.None, ItemTypeNameHandling = TypeNameHandling.All)]
        public Dictionary<string, bool> LogIgnoreLevels { get; set; } = new Dictionary<string, bool>();
    }
}
