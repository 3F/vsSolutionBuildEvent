/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.API;
using net.r_eg.vsSBE.Configuration;

namespace net.r_eg.vsSBE.UI.Xaml
{
    internal interface IStatusToolEvents
    {
        /// <summary>
        /// Add handler for all events from API.IEventLevel
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        IStatusToolEvents attachEvents(IEventLevel evt);

        /// <summary>
        /// Remove handler for all events from API.IEventLevel
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        IStatusToolEvents detachEvents(IEventLevel evt);

        /// <summary>
        /// Add handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        IStatusToolEvents attachEvents(IConfig<ISolutionEvents> evt);

        /// <summary>
        /// Remove handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        IStatusToolEvents detachEvents(IConfig<ISolutionEvents> evt);

        /// <summary>
        /// Add handler for all available events
        /// </summary>
        /// <returns>self reference</returns>
        IStatusToolEvents attachEvents();

        /// <summary>
        /// Remove handler for all available events
        /// </summary>
        /// <returns>self reference</returns>
        IStatusToolEvents detachEvents();
    }
}