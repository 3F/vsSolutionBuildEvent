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

namespace net.r_eg.vsSBE.Events.Commands
{
    /// <summary>
    /// Specifies basic fields for command.
    /// </summary>
    [Guid("F80C5586-6157-408A-9029-80FE2FC851B3")]
    public interface ICommand<T>
    {
        /// <summary>
        /// Main command for handling.
        /// </summary>
        T Command { get; set; }
    }

    [Guid("E87D1386-1A8A-40E5-9379-D7790863FC90")]
    public interface ICommand: ICommand<string>
    {

    }
}
