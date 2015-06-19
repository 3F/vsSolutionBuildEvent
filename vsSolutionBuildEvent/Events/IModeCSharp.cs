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

using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Mode of compilation C# code
    /// </summary>
    [Guid("25EA6943-800A-4D6E-BE14-9E26DB084172")]
    public interface IModeCSharp: ICommand
    {
        /// <summary>
        /// Additional assembly names that are referenced by the source to compile.
        /// </summary>
        string[] References { get; set; }

        /// <summary>
        /// Whether to generate the output in memory.
        /// </summary>
        bool GenerateInMemory { get; set; }

        /// <summary>
        /// The output path for binary result.
        /// </summary>
        string OutputPath { get; set; }

        /// <summary>
        /// Command-line arguments to use when invoking the compiler.
        /// </summary>
        string CompilerOptions { get; set; }

        /// <summary>
        /// Whether to treat warnings as errors.
        /// </summary>
        bool TreatWarningsAsErrors { get; set; }

        /// <summary>
        /// The warning level at which the compiler aborts compilation.
        /// </summary>
        int WarningLevel { get; set; }

        /// <summary>
        /// Switching between source code and list of files with source code for ICommand.
        /// </summary>
        bool FilesMode { get; set; }

        /// <summary>
        /// To cache bytecode if it's possible.
        /// </summary>
        bool CachingBytecode { get; set; }

        /// <summary>
        /// When the binary data has been updated.
        /// UTC
        /// </summary>
        long LastTime { get; set; }
    }
}
