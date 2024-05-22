/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.Events.Commands;

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
        /// Advanced searching of assemblies in 'References' set.
        /// </summary>
        bool SmartReferences { get; set; }

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
        /// Cache data from user settings.
        /// </summary>
        IUserValue CacheData { get; set; }
    }
}
