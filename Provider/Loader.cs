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
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace net.r_eg.vsSBE.Provider
{
    public class Loader: ILoader
    {
        public const string GUID = "94ecd13f-15f3-4f51-9afd-17f0275c6266";

        public ILibrary Library
        {
            get;
            protected set;
        }

        /// <summary>
        /// Minimum requirements for library
        /// </summary>
        public Version MinVersion
        {
            get { return new Version(0, 11); }
        }

        /// <summary>
        /// Switches the debug mode for details about errors with loader/library etc.
        /// </summary>
        public bool DebugMode
        {
            get; set;
        }

        /// <summary>
        /// Additional domain for library if used
        /// </summary>
        protected AppDomain domain = null;

        /// <summary>
        /// Load the library with path from DTE2 & AddIn for alternative path.
        /// </summary>
        /// <param name="dte2">Uses paths from dte2 object for search in registry etc.</param>
        /// <param name="addIn">Uses paths from addIn object to find in place with Add-in.</param>
        public ILibrary load(EnvDTE80.DTE2 dte2, EnvDTE.AddIn addIn)
        {
            string path = extractPath(addIn.SatelliteDllPath);
            if(!Provider.Library.existsIn(path)) {
                path = findWithRegistry(dte2.RegistryRoot);
            }
            this.Library = new Library(path, dte2) { ResolveErrorsShow = DebugMode };
            return Library;
        }

        /// <summary>
        /// Load the library with path from DTE2 & another path as alternative.
        /// </summary>
        /// <param name="dte2">Uses paths from dte2 object for search in registry etc.</param>
        /// <param name="path">Specific path for search this library.</param>
        /// <param name="createDomain">Create new domain for loading new references into this domain</param>
        public ILibrary load(EnvDTE80.DTE2 dte2, string path, bool createDomain = false)
        {
            if(createDomain) {
                this.Library = createInNewDomain(path, dte2);
            }
            else {
                this.Library = new Library(path, dte2) { ResolveErrorsShow = DebugMode };
            }
            return Library;
        }

        /// <summary>
        /// Load the library from path with isolated environment.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path for search this library.</param>
        /// <param name="createDomain">Create new domain for loading new references into this domain</param>
        public ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath, bool createDomain = false)
        {
            if(createDomain) {
                this.Library = createInNewDomain(libPath, solutionFile, properties);
            }
            else {
                this.Library = new Library(libPath, solutionFile, properties) { ResolveErrorsShow = DebugMode };
            }
            return Library;
        }

        /// <summary>
        /// Unload the loaded library.
        /// </summary>
        public void unload()
        {
            if(domain != null) {
                AppDomain.Unload(domain);
                domain = null;
                return;
            }
            throw new InvalidOperationException("Not available for this instance. Use the 'createDomain' flag");
        }

        // TODO:
        protected Library createInNewDomain(params Object[] arguments)
        {
            domain = AppDomain.CreateDomain(String.Format("Library_{0}", GUID), null, new AppDomainSetup() {
                ApplicationBase = Environment.CurrentDirectory
            });

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) => {
                return Assembly.LoadFrom(String.Format("{0}\\{1}.dll", 
                                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                                                        args.Name.Substring(0, args.Name.IndexOf(","))));
            });
            Library lib = (Library)domain.CreateInstanceAndUnwrap(
                                            typeof(Library).Assembly.FullName,
                                            typeof(Library).FullName,
                                            false,
                                            BindingFlags.Default,
                                            null,
                                            arguments,
                                            null,
                                            null
                                          );

            lib.ResolveErrorsShow = DebugMode;
            return lib;
        }

        protected string findWithRegistry(string root)
        {
            string keypath = String.Format(@"{0}\ExtensionManager\EnabledExtensions", root);
            using(RegistryKey rk = Registry.CurrentUser.OpenSubKey(keypath))
            {
                string name = rk.GetValueNames().FirstOrDefault(x => x.Contains(GUID));
                if(!String.IsNullOrEmpty(name)) {
                    return extractPath(rk.GetValue(name).ToString());
                }
            }
            throw new DllNotFoundException(String.Format("Not found '{0}' with Registry", GUID));
        }

        protected string extractPath(string file)
        {
            string dir = Path.GetDirectoryName(file);

            if(dir.ElementAt(dir.Length - 1) != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }
    }
}
