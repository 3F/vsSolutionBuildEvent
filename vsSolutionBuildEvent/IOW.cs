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

using System;

namespace net.r_eg.vsSBE
{
    public interface IOW
    {
        /// <summary>
        /// Access to OutputWindow.
        /// </summary>
        EnvDTE.OutputWindow OutputWindow { get; }

        /// <summary>
        /// Getting item of the output window by name.
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="createIfNotExist">Flag of creating. It means ~create new pane if this item does not exist etc.</param>
        /// <returns></returns>
        EnvDTE.OutputWindowPane getByName(string name, bool createIfNotExist);

        /// <summary>
        /// Removes pane by name of item.
        /// </summary>
        /// <param name="name"></param>
        void deleteByName(string name);

        /// <summary>
        /// Removes pane with selected GUID.
        /// </summary>
        /// <param name="guid"></param>
        void deleteByGuid(Guid guid);
    }
}
