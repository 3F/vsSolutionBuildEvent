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

namespace net.r_eg.vsSBE.Provider
{
    internal class Library: MarshalByRefObject, ILibrary
    {
        public const string NAME = "vsSolutionBuildEvent.dll";

        /// <summary>
        /// Absolute path to used library
        /// </summary>
        public string Dllpath
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of used library with full path
        /// </summary>
        public string FullName
        {
            get { return Dllpath + NAME; }
        }

        /// <summary>
        /// Version of used library
        /// </summary>
        public Bridge.IVersion Version
        {
            get;
            protected set;
        }

        /// <summary>
        /// All public events of used library
        /// </summary>
        public Bridge.IEvent Event
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Build operations of used library
        /// </summary>
        public Bridge.IBuild Build
        {
            get;
            protected set;
        }

        /// <summary>
        /// Settings of used library
        /// </summary>
        public Bridge.ISettings Settings
        {
            get;
            protected set;
        }

        /// <summary>
        /// Helper for getting instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected struct Instance<T>
        {
            public static T from(Assembly asm, params object[] args)
            {
                foreach(Type type in asm.GetTypes()) {
                    if(type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T))) {
                        return (T)Activator.CreateInstance(type, args);
                    }
                }
                throw new DllNotFoundException(String.Format("incorrect library for type '{0}'", typeof(T)));
            }
        }

        /// <summary>
        /// Access to provider settings
        /// </summary>
        protected ISettings PSettings
        {
            get { return Provider.Settings._; }
        }

        /// <summary>
        /// Current Domain
        /// </summary>
        protected AppDomain domain;

        /// <summary>
        /// Checking existence of library for specific path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool existsIn(string path)
        {
            return File.Exists(path + NAME);
        }

        /// <summary>
        /// Init library with full environment
        /// </summary>
        /// <param name="libpath">Path to library</param>
        /// <param name="dte2">DTE2 instance</param>
        public Library(string libpath, object dte2)
        {
            initLib(prepare(libpath), dte2, PSettings.DebugMode);
        }

        /// <summary>
        /// Init library with isolated environment
        /// </summary>
        /// <param name="libpath">Path to library</param>
        /// <param name="solutionFile">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        public Library(string libpath, string solutionFile, Dictionary<string, string> properties)
        {
            initLib(prepare(libpath), solutionFile, properties, PSettings.DebugMode);
        }

        protected void initLib(Assembly lib, params object[] args)
        {
            Event       = Instance<Bridge.IEvent>.from(lib, args);
            Build       = (Bridge.IBuild)Event;
            Settings    = Instance<Bridge.ISettings>.from(lib);
            Version     = Instance<Bridge.IVersion>.from(lib);
        }

        /// <param name="path">Path to library</param>
        /// <returns></returns>
        protected Assembly prepare(string path)
        {
            if(!existsIn(path)) {
                throw new DllNotFoundException(String.Format("Library '{0}' not found in '{1}'", NAME, path));
            }
            Dllpath = path;

            domain = AppDomain.CurrentDomain;
            domain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) =>
            {
                if(String.IsNullOrEmpty(args.Name)) {
                    return null;
                }

                try {
                    int split = args.Name.IndexOf(",");
                    return Assembly.LoadFrom(String.Format("{0}{1}.dll", 
                                                            path, 
                                                            args.Name.Substring(0, (split == -1)? args.Name.Length : split)));
                }
                catch(Exception ex) {
                    if(PSettings.DebugMode) {
                        Console.WriteLine("Use other resolver for '{0}' :: {1}", args.Name, ex.Message);
                    }
                }
                return null;
            });

            return Assembly.LoadFile(FullName);
        }
    }
}