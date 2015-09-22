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
    [Guid("09660649-B7BC-43D3-8E8F-951308B953D5")]
    internal interface IDataSvc
    {
        /// <summary>
        /// Update Common property.
        /// </summary>
        /// <param name="isLoad">Update for loading or saving.</param>
        void updateCommon(bool isLoad);
    }
}
