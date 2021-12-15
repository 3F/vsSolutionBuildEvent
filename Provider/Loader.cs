/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
        /// <summary>
        /// Access to library connector
        /// </summary>
        public ILibrary Library
        {
            get;
            protected set;
        }

        /// <summary>
        /// Minimum requirements for library
        /// </summary>
        public System.Version MinVersion
        {
            get {
                return new System.Version(0, 12, 3);
            }
        }

        /// <summary>
        /// Provider settings
        /// </summary>
        public ISettings Settings
        {
            get;
            set;
        }

        /// <summary>
        /// Additional domain for library if used
        /// </summary>
        protected AppDomain domain = null;


        /// <summary>
        /// Load library with DTE2-context + Add-In
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="pathAddIn">Path to Add-in.</param>
        /// <param name="registryRoot">Search in registry as alternative.</param>
        public ILibrary load(object dte2, string pathAddIn, string registryRoot = null)
        {
            string path = extractPath(pathAddIn);
            if(!Provider.Library.existsIn(path)) {
                path = findWithRegistry(registryRoot);
            }
            this.Library = new Library(path, dte2, Settings);
            return Library;
        }

        /// <summary>
        /// Load library with DTE2-context from path.
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="path">Specific path to library.</param>
        /// <param name="createDomain">Create new domain for loading new references into current domain</param>
        public ILibrary load(object dte2, string path, bool createDomain)
        {
            if(createDomain) {
                this.Library = createInNewDomain(path, dte2, Settings);
            }
            else {
                this.Library = new Library(path, dte2, Settings);
            }
            return Library;
        }

        /// <summary>
        /// Load library from path with Isolated Environments.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path to library.</param>
        /// <param name="createDomain">Create new domain for loading new references into current domain</param>
        public ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath, bool createDomain)
        {
            if(createDomain) {
                this.Library = createInNewDomain(libPath, solutionFile, properties, Settings);
            }
            else {
                this.Library = new Library(libPath, solutionFile, properties, Settings);
            }
            return Library;
        }

        /// <summary>
        /// Load library from path with Isolated Environments.
        /// </summary>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="libPath">Specific path to library.</param>
        public ILibrary load(string solutionFile, Dictionary<string, string> properties, string libPath)
        {
            return load(solutionFile, properties, libPath, false);
        }

        /// <summary>
        /// Load library with DTE2-context from path.
        /// </summary>
        /// <param name="dte2">DTE2-context</param>
        /// <param name="path">Specific path to library.</param>
        public ILibrary load(object dte2, string path)
        {
            return load(dte2, path, false);
        }

        /// <summary>
        /// Load library from path with Isolated Environments into domain.
        /// </summary>
        /// <param name="domain">Specific domain</param>
        /// <param name="sln">Full path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="lib">Full path to library.</param>
        public ILibrary loadIn(AppDomain domain, string sln, Dictionary<string, string> properties, string lib)
        {
            throw new NotSupportedException("Not implemented for current version.");
        }

        /// <summary>
        /// Unload library from selected domain.
        /// </summary>
        /// <param name="domain">Specific domain</param>
        public void unload(AppDomain domain)
        {
            throw new NotSupportedException("Not implemented for current version.");
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
            throw new InvalidOperationException("Not available for this instance. Use another domain.");
        }

        public Loader()
        {

        }

        public Loader(ISettings cfg)
        {
            Settings = cfg;
        }

        /// <summary>
        /// TODO: Complete remotable objects
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        protected virtual ILibrary createInDomain(AppDomain domain, params Object[] arguments)
        {
            if(domain == null) {
                throw new Bridge.Exceptions.InitializeException("Domain is empty");
            }

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) =>
            //{
            //    return Assembly.LoadFrom(String.Format("{0}\\{1}.dll", 
            //                                            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
            //                                            args.Name.Substring(0, args.Name.IndexOf(","))));
            //});

            return (ILibrary)domain.CreateInstanceAndUnwrap(
                                        typeof(Library).Assembly.FullName,
                                        typeof(Library).FullName,
                                        false,
                                        BindingFlags.Default,
                                        null,
                                        arguments,
                                        null,
                                        null
                                   );
        }

        protected virtual ILibrary createInNewDomain(params Object[] arguments)
        {
            return createInDomain(
                        AppDomain.CreateDomain(
                            String.Format("Library_{0}", Provider.Library.GUID), 
                            null, 
                            new AppDomainSetup() {
                                ApplicationBase = Environment.CurrentDirectory
                            }
                        ), 
                        arguments);
        }

        protected string findWithRegistry(string root)
        {
            if(String.IsNullOrEmpty(root))
            {
                throw new DllNotFoundException(
                    String.Format(
                        "Not found '{0}' root is empty for search. Use specific path or provide registryRoot",
                        Provider.Library.GUID
                    )
                );
            }

            string keypath = String.Format(@"{0}\ExtensionManager\EnabledExtensions", root);
            using(RegistryKey rk = Registry.CurrentUser.OpenSubKey(keypath))
            {
                string name = rk.GetValueNames().FirstOrDefault(x => x.Contains(Provider.Library.GUID));
                if(!String.IsNullOrEmpty(name)) {
                    return extractPath(rk.GetValue(name).ToString());
                }
            }
            throw new DllNotFoundException(String.Format("Not found '{0}' with Registry", Provider.Library.GUID));
        }

        protected string extractPath(string file)
        {
            string dir = Path.GetDirectoryName(file);

            if(dir[dir.Length - 1] != Path.DirectorySeparatorChar) {
                dir += Path.DirectorySeparatorChar;
            }
            return dir;
        }
    }
}
