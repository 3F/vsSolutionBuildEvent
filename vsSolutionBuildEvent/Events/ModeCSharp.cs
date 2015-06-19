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

using System.ComponentModel;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Mode of compilation C# code
    /// </summary>
    public class ModeCSharp: IMode, IModeCSharp
    {
        /// <summary>
        /// Type of implementation
        /// </summary>
        [Browsable(false)]
        public ModeType Type
        {
            get { return ModeType.CSharp; }
        }

        /// <summary>
        /// Source code or list of files with sources.
        /// </summary>
        [Browsable(false)]
        public string Command
        {
            get { return command; }
            set { command = value; }
        }
        private string command = string.Empty;

        /// <summary>
        /// Additional assembly names that are referenced by the source to compile.
        /// </summary>
        [Category("General")]
        [Description(@"Additional assembly names that are referenced by the source to compile. Formats, for example:
        `EnvDTE.dll`; `C:\WINDOWS\assembly\GAC\EnvDTE\<ver>\EnvDTE.dll`; `EnvDTE`; `EnvDTE, Version=8.0.0.0, PublicKeyToken=b03f5f7f11d50a3a`")]
        public string[] References
        {
            get;
            set;
        }
        
        /// <summary>
        /// Whether to generate the output in memory.
        /// </summary>
        [Category("Output")]
        [Description("Whether to generate the output in memory.")]
        public bool GenerateInMemory
        {
            get { return generateInMemory; }
            set { generateInMemory = value; }
        }
        private bool generateInMemory = true;

        /// <summary>
        /// The output path for binary result.
        /// </summary>
        [Category("Output")]
        [Description("The output path for binary result.")]
        public string OutputPath
        {
            get;
            set;
        }

        /// <summary>
        /// Command-line arguments to use when invoking the compiler.
        /// </summary>
        [Category("General")]
        [Description("Command-line arguments to use when invoking the compiler.")]
        public string CompilerOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to treat warnings as errors.
        /// </summary>
        [Category("Warnings && Errors")]
        [Description("Whether to treat warnings as errors.")]
        public bool TreatWarningsAsErrors
        {
            get;
            set;
        }

        /// <summary>
        /// The warning level at which the compiler aborts compilation.
        /// </summary>
        [Category("Warnings && Errors")]
        [Description("The warning level at which the compiler aborts compilation.")]
        public int WarningLevel
        {
            get { return warningLevel; }
            set { warningLevel = value; }
        }
        private int warningLevel = 4;

        /// <summary>
        /// Switching between source code and list of files with sources for ICommand.
        /// </summary>
        [Description("Switching between source code and list of files with sources for ICommand.")]
        public bool FilesMode
        {
            get { return filesMode; }
            set { filesMode = value; }
        }
        private bool filesMode = false;

        /// <summary>
        /// To cache bytecode if it's possible.
        /// </summary>
        [Category("Caching")]
        [Description("To cache bytecode if it's possible.")]
        public bool CachingBytecode
        {
            get;
            set;
        }

        /// <summary>
        /// When the binary data has been updated.
        /// UTC
        /// </summary>
        [Browsable(false)]
        public long LastTime
        {
            get;
            set;
        }
    }
}
