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

namespace net.r_eg.vsSBE.Events.CommandEvents
{
    /// <summary>
    /// Specifies filters for ICommandEvent
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// For work with command ID
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Scope by GUID
        /// </summary>
        string Guid { get; set; }

        /// <summary>
        /// Filter by Custom input parameters
        /// </summary>
        object CustomIn { get; set; }

        /// <summary>
        /// Filter by Custom output parameters
        /// </summary>
        object CustomOut { get; set; }

        /// <summary>
        /// Cancel command if it's possible
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        /// Use Before executing command
        /// </summary>
        bool Pre { get; set; }

        /// <summary>
        /// Use After executed command
        /// </summary>
        bool Post { get; set; }

        /// <summary>
        /// About filter
        /// </summary>
        string Description { get; set; }
    }
}