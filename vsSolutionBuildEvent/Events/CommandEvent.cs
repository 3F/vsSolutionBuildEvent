/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events.CommandEvents;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// For work with CommandEvents from EnvDTE
    /// </summary>
    public class CommandEvent: SBEEvent, ISolutionEvent, ICommandEvent
    {
        /// <inheritdoc cref="ICommandEvent.Filters"/>
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IFilter[] Filters { get; set; }
    }
}
