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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.SBEScripts.SNode
{
    [Guid("99D0CB1C-ED79-4358-BF2B-67F2A5B1EEFB")]
    public interface ILevel
    {
        /// <summary>
        /// Type of level.
        /// </summary>
        LevelType Type { get; }

        /// <summary>
        /// Data of level.
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Arguments of level.
        /// </summary>
        Argument[] Args { get; }

        /// <summary>
        /// Type of data.
        /// </summary>
        CValueType DataType { get; set; }
    }
}
