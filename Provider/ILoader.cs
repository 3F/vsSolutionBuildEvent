/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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
