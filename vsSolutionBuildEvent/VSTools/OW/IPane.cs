/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.VSTools.OW
{
    [Guid("9C9CEFB5-BECE-4DB8-87EF-5C38AFA5EBD7")]
    public interface IPane
    {
        /// <summary>
        /// Gets the GUID for the pane.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Moves the focus to the current item.
        /// </summary>
        void Activate();

        /// <summary>
        /// Clears all text from pane.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sends a text string into pane.
        /// </summary>
        /// <param name="text"></param>
        void OutputString(string text);
    }
}
