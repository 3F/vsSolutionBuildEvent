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

namespace net.r_eg.vsSBE.UI.Xaml
{
    internal interface IStatusTool
    {
        /// <summary>
        /// Gets number from Warnings counter
        /// </summary>
        int Warnings { get; }

        /// <summary>
        /// Resets the Warnings counter
        /// </summary>
        void resetCounter();

        /// <summary>
        /// Availability of main panel for user
        /// </summary>
        /// <param name="enabled"></param>
        void enabledPanel(bool enabled);

        /// <summary>
        /// Notification about any warnings
        /// </summary>
        void warn();

        /// <summary>
        /// Updates data for controls
        /// </summary>
        void refresh();
    }
}
