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
using System.ComponentModel;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.Events.Mapping.Json;
using Newtonsoft.Json;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Mode of compilation C# code
    /// </summary>
    public class ModeCSharp: ModeCommand, IMode, IModeCSharp
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
        /// Additional assembly names that are referenced by the source to compile.
        /// </summary>
        [Category("General")]
        [Description(@"Additional assembly names that are referenced by the source to compile. Formats for SmartReferences:
        `EnvDTE.dll`; `C:\WINDOWS\assembly\GAC\EnvDTE\<ver>\EnvDTE.dll`; `EnvDTE`; `EnvDTE, Version=8.0.0.0, PublicKeyToken=b03f5f7f11d50a3a`")]
        public string[] References
        {
            get {
                return references;
            }
            set {
                references = value;
            }
        }
        /// <summary>
        /// The assembly names for user actions by default
        /// </summary>
        private string[] references = new string[] { "System.dll" };

        /// <summary>
        /// Advanced searching of assemblies in 'References' set.
        /// </summary>
        [Category("General")]
        [Description("Use advanced search of assemblies in 'References' set.")]
        public bool SmartReferences
        {
            get { return smartReferences; }
            set { smartReferences = value; }
        }
        private bool smartReferences = true;
        
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
        [Obsolete("Deprecated and will be removed soon. Use CacheData instead.")]
        public long LastTime
        {
            get;
            set;
        }

        /// <summary>
        /// Cache data from user settings.
        /// </summary>
        [Browsable(false)]
        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public IUserValue CacheData
        {
            get {
                return cacheData;
            }

            set
            {
                if(value == null && cacheData != null) {
                    //cacheData.Manager.unset(); // can be from cloned object
                }
                cacheData = value;
            }
        }
        private IUserValue cacheData;
    }
}
