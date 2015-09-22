/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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