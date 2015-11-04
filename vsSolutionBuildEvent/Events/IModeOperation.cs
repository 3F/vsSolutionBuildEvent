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
using net.r_eg.vsSBE.Events.Commands;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with environment of Visual Studio
    /// TODO: string type is obsolete. See variant in vsCE
    /// </summary>
    [Guid("3860EC60-0206-422F-A16E-4228DCEF2B30")]
    public interface IModeOperation: ICommandArray<string>
    {
        /// <summary>
        /// Caption for atomic commands.
        /// TODO: obsolete and should be removed
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Abort operations on first error.
        /// </summary>
        bool AbortOnFirstError { get; set; }
    }
}
