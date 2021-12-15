/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
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

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Specifies basic item.
    /// </summary>
    [Guid("38C1F903-2584-4D1D-98A3-922A953280C8")]
    public interface IItem
    {
        /// <summary>
        /// Gets current raw data or sets new.
        /// </summary>
        string Raw { get; set; }

        /// <summary>
        /// Updating raw data.
        /// </summary>
        /// <param name="data"></param>
        void updateRaw(string data);
    }
}
