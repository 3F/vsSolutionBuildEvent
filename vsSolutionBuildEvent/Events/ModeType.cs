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

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Type of available processing modes
    /// </summary>
    [Guid("F7E15CE6-F7B1-44A6-AB55-62F1C8BEDE26")]
    public enum ModeType
    {
        /// <summary>
        /// External handling with files.
        /// </summary>
        File,

        /// <summary>
        /// Processing with external interpreter.
        /// generally, it's a stream processor etc.
        /// </summary>
        Interpreter,

        /// <summary>
        /// DTE-commands - operations with EnvDTE.
        /// </summary>
        Operation,

        /// <summary>
        /// Script processing.
        /// generally, it's internal handling with MSBuild / SBE-Scripts cores, and similar
        /// </summary>
        Script,

        /// <summary>
        /// MSBuild targets
        /// </summary>
        Targets,

        /// <summary>
        /// C# code
        /// </summary>
        CSharp,
    }
}
