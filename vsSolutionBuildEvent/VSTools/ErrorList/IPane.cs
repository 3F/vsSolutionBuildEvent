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

namespace net.r_eg.vsSBE.VSTools.ErrorList
{
    [Guid("EA256A50-31B6-45A3-A0BA-773E5CBB6165")]
    public interface IPane
    {
        /// <summary>
        /// To add new error in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void error(string message);

        /// <summary>
        /// To add new warning in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void warn(string message);

        /// <summary>
        /// To add new information in ErrorList.
        /// </summary>
        /// <param name="message"></param>
        void info(string message);

        /// <summary>
        /// To clear all messages.
        /// </summary>
        void clear();
    }
}
