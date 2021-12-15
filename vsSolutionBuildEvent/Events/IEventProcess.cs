﻿/*
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

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Declaring of handling process
    /// </summary>
    public interface IEventProcess
    {
        /// <summary>
        /// Waiting completion
        /// </summary>
        bool Waiting { get; set; }

        /// <summary>
        /// Hiding of processing or not
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        /// How long to wait the execution, in seconds. 0 value - infinitely
        /// </summary>
        int TimeLimit { get; set; }
    }
}
