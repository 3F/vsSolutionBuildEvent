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
using net.r_eg.vsSBE.Events.Commands;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with streaming tools
    /// </summary>
    [Guid("DEF30B20-9A0B-41FC-97DD-E95BA770FDB0")]
    public interface IModeInterpreter: ICommand
    {
        /// <summary>
        /// Stream handler
        /// </summary>
        string Handler { get; set; }

        /// <summary>
        /// Treat newline as
        /// </summary>
        string Newline { get; set; }

        /// <summary>
        /// Symbol/s for wrapping of commands
        /// </summary>
        string Wrapper { get; set; }
    }
}
