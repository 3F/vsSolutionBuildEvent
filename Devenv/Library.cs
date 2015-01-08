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
using System.IO;
using System.Linq;
using System.Reflection;
using EnvDTE80;

namespace net.r_eg.vsSBE.Devenv
{
    public class Library//: MarshalByRefObject
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

        public string FullName
        {
            get { return Dllpath + NAME; }
        }

        /// <summary>
        /// Minimum requirements for library
        /// </summary>
        public static Version MinVersion
        {
            get { return new Version(0, 11); }
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
        /// Current Domain
        /// </summary>
        protected AppDomain domain;

        public Library(DTE2 dte2, string path)
        {
            if(!existsIn(path)) {
                throw new FileNotFoundException(String.Format("File '{0}' not found in '{1}'", NAME, path));
            }
            Dllpath = path;

            domain = AppDomain.CurrentDomain; // protection from GC
            domain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) => {
                return Assembly.LoadFrom(String.Format("{0}{1}.dll", path, args.Name.Substring(0, args.Name.IndexOf(","))));
            });

            // TODO: protection from incompatible dll's /see AppDomain
            Assembly lib = Assembly.LoadFile(FullName);

            Event   = Instance<Bridge.IEvent>.from(lib, dte2);
            Version = Instance<Bridge.IVersion>.from(lib);
        }

        /// <summary>
        /// Checking existence of library for specific path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool existsIn(string path)
        {
            return File.Exists(path + NAME);
        }
    }
}