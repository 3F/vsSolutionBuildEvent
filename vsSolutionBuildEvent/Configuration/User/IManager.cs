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

namespace net.r_eg.vsSBE.Configuration.User
{
    /// <summary>
    /// Specifies basic manager of accessing to remote value.
    /// </summary>
    [Guid("7E740672-7D27-4180-A5F7-0A21A0329D3A")]
    public interface IManager
    {
        /// <summary>
        /// Unspecified raw value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Trying to get value as ICacheHeader.
        /// </summary>
        ICacheHeader CacheHeader { get; }

        /// <summary>
        /// To erase current value from common data.
        /// </summary>
        void unset();

        /// <summary>
        /// Reset data from value.
        /// </summary>
        void reset();
    }
}
