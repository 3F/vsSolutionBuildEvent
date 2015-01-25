/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
        Version MinVersion { get; }

        /// <summary>
        /// Access to settings
        /// </summary>
        ISettings Settings { get; }

        /// <summary>
        /// Load the library with path from DTE2 & AddIn for alternative path.
        /// </summary>
        /// <param name="dte2">Uses paths from dte2 object for search in registry etc.</param>
        /// <param name="addIn">Uses paths from addIn object to find in place with Add-in.</param>
        ILibrary load(EnvDTE80.DTE2 dte2, EnvDTE.AddIn addIn);

        /// <summary>
        /// Load the library with path from DTE2 & another path as alternative.
        /// </summary>
        /// <param name="dte2">Uses paths from dte2 object for search in registry etc.</param>
        /// <param name="path">Specific path for search this library.</param>
        /// <param name="createDomain">Create new domain for loading new references into this domain</param>
        ILibrary load(EnvDTE80.DTE2 dte2, string path, bool createDomain = false);

        /// <summary>
        /// Load the library from path with isolated environment.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path for search this library.</param>
        /// <param name="createDomain">Create new domain for loading new references into this domain</param>
        ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath, bool createDomain = false);

        /// <summary>
        /// Unload the loaded library.
        /// Some methods of loading may use additional domain for loading new references,
        /// some not.. so this method can also throwing a some exception
        /// </summary>
        void unload();
    }
}
