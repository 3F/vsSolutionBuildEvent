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

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("CD46BF40-05AF-40F1-9569-F0A1981D4D93")]
    public enum HashType
    {
        /// <summary>
        /// ~335 MiB/Second	with CPU frequency ~2.194e+09 Hz
        /// </summary>
        MD5,

        /// <summary>
        /// Tiger tree.
        /// ~328 MiB/Second	with CPU frequency ~2.194e+09 Hz
        /// </summary>
        TTH
    }
}
