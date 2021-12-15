/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Extensions;

namespace net.r_eg.vsSBE.Clients
{
    public class ClientLibrary: IClientLibrary
    {
        /// <summary>
        /// The file name of client library
        /// </summary>
        public const string NAME = "client.vssbe.dll";

        /// <summary>
        /// Name of client library with full path.
        /// </summary>
        public string FullName
        {
            get { return Dllpath + NAME; }
        }

        /// <summary>
        /// Absolute path to client library.
        /// </summary>
        public string Dllpath
        {
            get {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).DirectoryPathFormat();
            }
        }

        /// <summary>
        /// Checking existence of client library.
        /// </summary>
        public bool Exists
        {
            get {
                return File.Exists(FullName);
            }
        }

        /// <summary>
        /// Access to IEvent2 in client library.
        /// </summary>
        public IEvent2 Event
        {
            get { return cevent; }
        }
        protected IEvent2 cevent = new SEvent2Empty();

        /// <summary>
        /// Access to IBuild in client library.
        /// </summary>
        public IBuild Build
        {
            get { return cbuild; }
        }
        protected IBuild cbuild = new SBuildEmpty();

        /// <summary>
        /// Unspecified EnvDTE80.DTE2 from EnvDTE80.dll
        /// </summary>
        protected object dte2;

        /// <summary>
        /// Full path to solution file
        /// </summary>
        protected string solutionFile;

        /// <summary>
        /// Global properties for solution
        /// </summary>
        protected Dictionary<string, string> properties;

        /// <summary>
        /// Entry point of core library.
        /// </summary>
        protected IEntryPointCore core;

        /// <summary>
        /// Object synch.
        /// </summary>
        private Object _lock = new Object();


        /// <summary>
        /// Trying of loading for DTE2 context
        /// </summary>
        /// <param name="core">Entry point of core library</param>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <returns>true value if library exists and successfully loaded</returns>
        public bool tryLoad(IEntryPointCore core, object dte2)
        {
            this.core   = core;
            this.dte2   = dte2;

            return init(true);
        }

        /// <summary>
        /// Trying of loading for Isolated environment
        /// </summary>
        /// <param name="core">Entry point of core library</param>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <returns>true value if library exists and successfully loaded</returns>
        public bool tryLoad(IEntryPointCore core, string sln, Dictionary<string, string> properties)
        {
            this.core       = core;
            solutionFile    = sln;
            this.properties = properties;

            return init(true);
        }

        /// <summary>
        /// Initialize library
        /// </summary>
        /// <returns></returns>
        protected bool init()
        {
            if(!Exists) {
                Log.Debug(String.Format("The Client library '{0}' is not found.", FullName));
                return false;
            }

            Assembly lib            = prepare(FullName);
            IEntryPointClient epc   = Instance<IEntryPointClient>.from(lib);

            epc.Version = new API.Version();
            epc.Core    = core;

            switch(epc.Type)
            {
                case ClientType.Dte2: {
                    epc.load(dte2);
                    break;
                }
                case ClientType.Isolated: {
                    epc.load(solutionFile, properties);
                    break;
                }
                case ClientType.Empty: {
                    epc.load();
                    break;
                }
            }

            cevent = new SEvent2(epc.Event);
            cbuild = new SBuild(epc.Build);

            return true;
        }

        protected bool init(bool safe)
        {
            if(!safe) {
                return init();
            }

            try {
                return init();
            }
            catch(Exception ex) {
                Log.Warn(String.Format("Client library: problem with initialization '{0}'", ex.ToString()));
                return false;
            }
        }

        /// <param name="path">Path to library</param>
        /// <returns></returns>
        protected Assembly prepare(string path)
        {
            lock(_lock) {
                AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
                AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;
            }

            return Assembly.LoadFile(FullName);
        }

        private Assembly assemblyResolver(object sender, ResolveEventArgs args)
        {
            if(args.RequestingAssembly == null || !args.RequestingAssembly.Location.EndsWith(NAME)) {
                return null;
            }
            
            if(String.IsNullOrEmpty(args.Name)) {
                return null;
            }

            Log.Trace("Assembly resolver for client library: '{0}' /requesting from '{1}'", args.Name, args.RequestingAssembly.FullName);
            try {
                int split = args.Name.IndexOf(",");
                return Assembly.LoadFrom(String.Format("{0}{1}.dll",
                                                        Dllpath, 
                                                        args.Name.Substring(0, (split == -1)? args.Name.Length : split)));
            }
            catch(Exception ex) {
                Log.Debug("Use other resolver for '{0}' :: {1}", args.Name, ex.Message);
            }

            return null;
        }
    }
}