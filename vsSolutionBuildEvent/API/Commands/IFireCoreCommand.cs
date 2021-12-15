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
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace net.r_eg.vsSBE.API.Commands
{
    [Guid("28286176-B1A7-4725-AD38-384D58BC1C29")]
    public interface IFireCoreCommand
    {
        /// <summary>
        /// Send the core command for all clients.
        /// </summary>
        /// <param name="c"></param>
        void fire(CoreCommandArgs c);
    }
}