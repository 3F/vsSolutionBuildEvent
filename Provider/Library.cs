﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Provider
{
    internal class Library: MarshalByRefObject, ILibrary
    {
        /// <summary>
        /// The Guid of main Package
        /// </summary>
        public const string GUID = "94ecd13f-15f3-4f51-9afd-17f0275c6266";

        /// <summary>
        /// The file name of library
        /// </summary>
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
        public IEvent Event
        {
            get;
            protected set;
        }

        /// <summary>
        /// The Build operations of used library
        /// </summary>
        public IBuild Build
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
        /// Entry point for core library
        /// </summary>
        public IEntryPointCore EntryPoint
        {
            get;
            protected set;
        }

        /// <summary>
        /// Current Domain
        /// </summary>
        protected AppDomain domain;

        /// <summary>
        /// Provider settings
        /// </summary>
        protected Provider.ISettings config;

        /// <summary>
        /// Object synch.
        /// </summary>
        private Object _lock = new Object();


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
            config = new Settings();

            initLib(prepare(libpath));
            EntryPoint.load(dte2);
        }

        /// <summary>
        /// Init library with full environment
        /// </summary>
        /// <param name="libpath">Path to library</param>
        /// <param name="dte2">DTE2 instance</param>
        /// <param name="cfg">Provider settings</param>
        public Library(string libpath, object dte2, ISettings cfg)
        {
            config = cfg;
            initLib(prepare(libpath));

            if(cfg.LibSettings != null) {
                EntryPoint.load(dte2, cfg.LibSettings);
            }
            else {
                EntryPoint.load(dte2);
            }
        }

        /// <summary>
        /// Init library with isolated environment
        /// </summary>
        /// <param name="lib">Path to library</param>
        /// <param name="sln">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        public Library(string lib, string sln, Dictionary<string, string> properties)
        {
            config = new Settings();

            initLib(prepare(lib));
            EntryPoint.load(sln, properties);
        }

        /// <summary>
        /// Init library with isolated environment
        /// </summary>
        /// <param name="lib">Path to library</param>
        /// <param name="sln">Path to .sln file</param>
        /// <param name="properties">Solution properties</param>
        /// <param name="cfg">Provider settings</param>
        public Library(string lib, string sln, Dictionary<string, string> properties, ISettings cfg)
        {
            config = cfg;
            initLib(prepare(lib));

            if(cfg.LibSettings != null) {
                EntryPoint.load(sln, properties, cfg.LibSettings);
            }
            else {
                EntryPoint.load(sln, properties);
            }
        }

        /// <summary>
        /// Initialize library
        /// </summary>
        /// <param name="lib"></param>
        protected void initLib(Assembly lib)
        {
            EntryPoint  = Instance<IEntryPointCore>.from(lib);
            Event       = (IEvent)EntryPoint;
            Build       = (IBuild)EntryPoint;
            Settings    = config.LibSettings;
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
            lock(_lock) {
                domain.AssemblyResolve -= asmResolver;
                domain.AssemblyResolve += asmResolver;
            }

            return Assembly.LoadFile(FullName);
        }

        private Assembly asmResolver(object sender, ResolveEventArgs args)
        {
            if(String.IsNullOrEmpty(args.Name)) {
                return null;
            }

            try {
                int split = args.Name.IndexOf(",");
                return Assembly.LoadFrom(String.Format("{0}{1}.dll",
                                                        Dllpath, 
                                                        args.Name.Substring(0, (split == -1)? args.Name.Length : split)));
            }
            catch(Exception ex)
            {
                //TODO: logger
                if(config.DebugMode) {
                    Console.WriteLine("Use other resolver for '{0}' :: {1}", args.Name, ex.Message);
                }
            }

            return null;
        }
    }
}