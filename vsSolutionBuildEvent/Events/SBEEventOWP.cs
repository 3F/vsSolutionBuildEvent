/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Support of the OutputWindowPane
    /// </summary>
    public class SBEEventOWP: SBEEvent, ISolutionEvent, ISolutionEventOWP
    {
        /// <inheritdoc cref="ISolutionEventOWP.Match"/>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All, ItemTypeNameHandling = TypeNameHandling.All)]
        public IMatchWords[] Match { get; set; }
    }
}
