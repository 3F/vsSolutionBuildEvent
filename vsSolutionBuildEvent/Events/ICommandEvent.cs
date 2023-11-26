﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events.CommandEvents;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Specifies work with CommandEvents from EnvDTE
    /// </summary>
    public interface ICommandEvent: ISolutionEvent 
    {
        /// <summary>
        /// Conditions of work Commands
        /// </summary>
        IFilter[] Filters { get; set; }
    }
}