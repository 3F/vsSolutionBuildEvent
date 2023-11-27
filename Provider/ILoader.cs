/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Provider
{
    [Guid("371E873E-A4EC-4844-92AB-E5835B86CC67")]
    public interface ILoader
    {
        /// <summary>
        /// Should provide instance of loaded library.
        /// </summary>
        ILibrary Library { get; }

        /// <summary>
        /// Minimum requirements for library.
        /// </summary>
        System.Version MinVersion { get; }

        /// <summary>
        /// Access to provider settings
        /// </summary>
        ISettings Settings { get; set; }

        /// <summary>
        /// Load library with DTE2-context + Add-In
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="pathAddIn">Path to Add-in.</param>
        /// <param name="registryRoot">Search in registry as alternative.</param>
        ILibrary load(object dte2, string pathAddIn, string registryRoot = null);
        //ILibrary load(EnvDTE80.DTE2 dte2, EnvDTE.AddIn addIn); // deprecated - heavy dependencies
        
        /// <summary>
        /// Load library with DTE2-context from path.
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="path">Specific path to library.</param>
        /// <param name="createDomain">Create new domain for loading new references into current domain</param>
        [Obsolete("Deprecated and can be removed in new versions.")]
        ILibrary load(object dte2, string path, bool createDomain);
        ////ILibrary load(EnvDTE80.DTE2 dte2, string path, bool createDomain = false); // deprecated - heavy dependencies

        /// <summary>
        /// Load library with DTE2-context from path.
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="path">Specific path to library.</param>
        ILibrary load(object dte2, string path);

        /// <summary>
        /// Load library from path with Isolated Environments.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path to library.</param>
        /// <param name="createDomain">Create new domain for loading new references into current domain</param>
        [Obsolete("Deprecated and can be removed in new versions.")]
        ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath, bool createDomain);

        /// <summary>
        /// Load library from path with Isolated Environments.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path to library.</param>
        ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath);

        /// <summary>
        /// Load library from path with Isolated Environments into domain.
        /// </summary>
        /// <param name="domain">Specific domain</param>
        /// <param name="sln">Full path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="lib">Full path to library.</param>
        ILibrary loadIn(AppDomain domain, string sln, Dictionary<string, string> properties, string lib);

        /// <summary>
        /// Unload of the loaded library.
        /// Some methods of loading may use additional domain for loading new references,
        /// some not.. so this method is also should throw some exception
        /// </summary>
        void unload();

        /// <summary>
        /// Unload library from selected domain.
        /// </summary>
        /// <param name="domain">Specific domain</param>
        void unload(AppDomain domain);
    }
}
