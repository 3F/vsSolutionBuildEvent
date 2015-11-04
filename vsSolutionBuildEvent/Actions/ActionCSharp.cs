/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Extensions;
using NLog;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Action for C# Mode
    /// </summary>
    public class ActionCSharp: Action, IAction
    {
        /// <summary>
        /// Main class for user code.
        /// </summary>
        public const string MAIN_CLASS = "vsSolutionBuildEvent.CSharpMode";

        /// <summary>
        /// Entry point for user code.
        /// </summary>
        public const string ENTRY_POINT = "Init";

        /// <summary>
        /// Prefix to cached bytecode.
        /// </summary>
        protected const string PREFIX_CACHE = "cached_vssbe.";

        /// <summary>
        /// Where to look cache.
        /// </summary>
        protected string BasePathToCache
        {
            get { return Settings.WPath; }
        }

        /// <summary>
        /// Generate a temporary AssemblyInfo.
        /// </summary>
        protected sealed class TempAssemblyInfo: IDisposable
        {
            /// <summary>
            /// Full path to AssemblyInfo.
            /// </summary>
            public string FullPath { get; private set; }

            public TempAssemblyInfo(string data)
            {
                FullPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

                using(TextWriter stream = new StreamWriter(FullPath, false, Encoding.UTF8)) {
                    stream.Write(data);
                }
            }

            public void Dispose()
            {
                try {
                    File.Delete(FullPath);
                }
                catch { /* we work in temp directory with unique name, so it's not important */ }
            }
        }

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();


        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            Type type;
            if(isRequiresCompilation(evt)) {
                type = compileAndGetType(evt);
            }
            else {
                type = load(outputCacheFile(evt), assemblyName(evt));
            }

            int ret = run(type, cmd, evt);
            if(ret != 0)
            {
                Log.Warn("Return code '{0}'", ret);
                return false;
            }
            return true;
        }

        /// <param name="cmd"></param>
        public ActionCSharp(ICommand cmd)
            : base(cmd)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
            AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;
        }

        /// <summary>
        /// Checks requirement of compiling source code.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>true value if needed compilation, otherwise we can use compiled version from cache.</returns>
        protected bool isRequiresCompilation(ISolutionEvent evt)
        {
            IModeCSharp cfg = ((IModeCSharp)evt.Mode);
            string cache    = fileName(evt);

            Log.Trace("[Cache] Checks: '{0}','{1}','{2}','{3}'", cfg.GenerateInMemory, cfg.CachingBytecode, evt.Name, cache);
            if(cfg.GenerateInMemory || !cfg.CachingBytecode || String.IsNullOrEmpty(cache)) {
                return true;
            }

            if(cfg.CacheData == null) {
                Log.Trace("[Cache] hash data is empty.");
                return true;
            }

            FileInfo f = new FileInfo(outputCacheFile(evt));
            if(!f.Exists) {
                Log.Info("[Cache] Binary '{0}' is not found in '{1}'. Compile new.", cache, f.FullName);
                return true;
            }

            string actual = cfg.CacheData.Manager.CacheHeader.Hash;
            if(!hashEquals(f.FullName, actual)) {
                Log.Info("[Cache] hash code '{0}' is invalid. Compile new.", actual);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Recalculate hash for IModeCSharp mode and save in user settings.
        /// </summary>
        /// <param name="mode"></param>
        protected void updateHash(IModeCSharp mode)
        {
            if(mode.CacheData == null) {
                mode.CacheData = new UserValue(LinkType.CacheHeader);
            }

            // calculate hash only for objects below

            object[] toHash = new object[]
            {
                mode.Command,
                mode.References,
                mode.OutputPath,
                mode.CompilerOptions,
                mode.TreatWarningsAsErrors,
                mode.WarningLevel,
                mode.FilesMode
            };

            mode.CacheData.Manager.CacheHeader.Hash         = toHash.MD5Hash();
            mode.CacheData.Manager.CacheHeader.Algorithm    = HashType.MD5;
            mode.CacheData.Manager.CacheHeader.Updated      = DateTime.Now.ToFileTimeUtc();
            Settings.CfgManager.UserConfig.save();
        }

        /// <summary>
        /// Gets assembly name of compiled user code for event.
        /// </summary>
        /// <param name="evt">Specific event.</param>
        /// <returns></returns>
        protected virtual string assemblyName(ISolutionEvent evt)
        {
            return String.Format("{0}{1}.{2}", PREFIX_CACHE, cmd.EventType, evt.Name);
        }

        /// <summary>
        /// Gets unique file name of compiled user code for event.
        /// </summary>
        /// <param name="evt">Specific event.</param>
        /// <returns></returns>
        protected virtual string fileName(ISolutionEvent evt)
        {
            return String.Format("{0}.dll", assemblyName(evt));
        }

        /// <summary>
        /// Loading of compiled user code if exists.
        /// </summary>
        /// <param name="file">Compiled code.</param>
        /// <param name="find">Search in already loaded assemblies. Use null value if needed to update assembly.</param>
        /// <returns>The Type for work with user code.</returns>
        protected Type load(string file, string find = null)
        {
            Log.Trace("[load] started with: '{0}' /'{1}'", file, find);

            if(find != null)
            {
                Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
                for(int i = asm.Length - 1; i > 0; --i)
                {
                    string a = asm[i].FullName;
                    if(String.IsNullOrWhiteSpace(a)) {
                        continue;
                    }
                    
                    if(a.Substring(0, a.IndexOf(',')) == find) {
                        Log.Trace("[load] use from loaded: '{0}'", a);
                        return asm[i].GetType(MAIN_CLASS);
                    }
                }
            }

            if(!File.Exists(file)) {
                throw new MismatchException("oops., something went wrong... while we thought, cache '{0}' disappeared. Try again.", file);
            }

            // into memory without blocking - it's important for CSharpCodeProvider with GenerateInMemory = false
            return Assembly.Load(File.ReadAllBytes(file)).GetType(MAIN_CLASS);
        }

        /// <summary>
        /// Runs user code.
        /// </summary>
        /// <param name="type">The Type for work with user code.</param>
        /// <param name="cmd">Push net.r_eg.vsSBE.Actions.ICommand into user code.</param>
        /// <param name="evt">Push net.r_eg.vsSBE.Events.ISolutionEvent into user code.</param>
        /// <returns>Result from user code.</returns>
        protected int run(Type type, ICommand cmd, ISolutionEvent evt)
        {
            MethodInfo method = type.GetMethod(ENTRY_POINT);
            var exec = (Func<ICommand, ISolutionEvent, int>)Delegate.CreateDelegate(typeof(Func<ICommand, ISolutionEvent, int>), method);
            return exec(cmd, evt);
        }

        /// <summary>
        /// Compiling user code with getting the result type for next step.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>The Type for work with user code.</returns>
        protected Type compileAndGetType(ISolutionEvent evt)
        {
            IModeCSharp cfg = (IModeCSharp)evt.Mode;
            if(!cfg.GenerateInMemory) {
                CompilerResults compiled = onlyCompile(evt);
                return load(compiled.PathToAssembly, null);
            }
            Log.Trace("Uses memory for getting type.");

            // be careful, this should automatically load assembly with blocking file if not used GenerateInMemory
            // therefore, use this only with GenerateInMemory == true
            return onlyCompile(evt).CompiledAssembly.GetType(MAIN_CLASS);
        }

        /// <summary>
        /// Only compiling user code.
        /// Use the safe compileAndGetType for work with the end-type.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Compiled user code.</returns>
        protected CompilerResults onlyCompile(ISolutionEvent evt)
        {
            Log.Trace("[Compiler] start new compilation.");

            IModeCSharp cfg = (IModeCSharp)evt.Mode;
            string command  = cfg.Command;

            if(String.IsNullOrWhiteSpace(command)) {
                throw new InvalidArgumentException("[Compiler] code is not found. abort;");
            }
            command = parse(evt, command);

            string output = outputCacheFile(evt);
            Log.Debug("[Compiler] output: '{0}' /GenerateInMemory: {1}", output, cfg.GenerateInMemory);

            if(File.Exists(output)) {
                Log.Trace("[Compiler] clear cache.");
                File.Delete(output);
            }

            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable      = false,
                GenerateInMemory        = cfg.GenerateInMemory,
                CompilerOptions         = cfg.CompilerOptions,
                TreatWarningsAsErrors   = cfg.TreatWarningsAsErrors,
                WarningLevel            = cfg.WarningLevel,

                // use prefix from fileName() to avoid random names if used GenerateInMemory
                OutputAssembly = (!cfg.GenerateInMemory)? output : Path.Combine(Path.GetTempPath(), fileName(evt))
            };

            // Assembly references

            string[] references = constructReferences(cfg, evt.SupportMSBuild);
            Log.Trace("[Compiler] final references: '{0}'", String.Join("; ", references));

            parameters.ReferencedAssemblies.AddRange(references);
            parameters.ReferencedAssemblies.Add(typeof(ISolutionEvent).Assembly.Location); // to support ICommand & ISolutionEvent
            parameters.ReferencedAssemblies.Add(typeof(Bridge.IEvent).Assembly.Location); // to support Bridge

            // ready to work with provider
            CompilerResults compiled = toBinary(command, parameters, cfg);

            // messages about errors & warnings
            foreach(CompilerError msg in compiled.Errors) {
                Log._.NLog.Log((msg.IsWarning)? LogLevel.Warn : LogLevel.Error, "[Compiler] '{0}'", msg.ToString());
            }

            if(compiled.Errors.HasErrors) {
                throw new CompilerException("[Compiler] found errors. abort;");
            }

            return compiled;
        }

        /// <summary>
        /// Final step to generate binary.
        /// </summary>
        /// <param name="source">Source code or list of files.</param>
        /// <param name="parameters">Compiler settings.</param>
        /// <param name="cfg">Settings of IModeCSharp.</param>
        /// <returns></returns>
        protected CompilerResults toBinary(string source, CompilerParameters parameters, IModeCSharp cfg)
        {
            string hash = formatHashForAsm(cfg);
            CSharpCodeProvider provider = new CSharpCodeProvider();
            
            if(!cfg.FilesMode) {
                return provider.CompileAssemblyFromSource(parameters, source, hash);
            }

            Log.Trace("[Compiler] use as list of files with source code.");
            if(String.IsNullOrEmpty(hash)) {
                return provider.CompileAssemblyFromFile(parameters, extractFiles(filesFromCommand(source)));
            }

            using(TempAssemblyInfo f = new TempAssemblyInfo(hash)) {
                return provider.CompileAssemblyFromFile(parameters, extractFiles(filesFromCommand(String.Format("{0}\n{1}", source, f.FullPath))));
            }
        }

        /// <param name="cfg">IModeCSharp configuration</param>
        /// <param name="msbuild">Flag of supporting MSBuild properties.</param>
        /// <returns>Prepared list of references</returns>
        protected string[] constructReferences(IModeCSharp cfg, bool msbuild)
        {
            if(cfg.References == null) {
                return new string[] { };
            }

            if(!cfg.SmartReferences)
            {
                return cfg.References
                            .Where(r => !String.IsNullOrEmpty(r))
                            .Select(r => (msbuild)? cmd.MSBuild.parse(r) : r)
                            .ToArray();
            }
            
            GAC gac = new GAC();
            return cfg.References
                        .Where(r => !String.IsNullOrEmpty(r))
                        .Select(r =>
                                    gac.getPathToAssembly(
                                            (msbuild)? cmd.MSBuild.parse(r) : r,
                                            true
                                    )
                                )
                        .ToArray();
        }

        /// <summary>
        /// How to store hash value in assembly.
        /// </summary>
        /// <param name="cfg">IModeCSharp configuration.</param>
        /// <returns>Formatted hash value for assembly.</returns>
        protected virtual string formatHashForAsm(IModeCSharp cfg)
        {
            if(!cfg.CachingBytecode) {
                return String.Empty;
            }

            if(cfg.CacheData == null || String.IsNullOrEmpty(cfg.CacheData.Manager.CacheHeader.Hash)) {
                updateHash(cfg);
            }

            string hash = cfg.CacheData.Manager.CacheHeader.Hash;
            if(String.IsNullOrEmpty(hash)) {
                return String.Empty;
            }
            return String.Format("[assembly: System.Reflection.AssemblyProduct(\"{0}\")]", hash);
        }

        /// <summary>
        /// Compare hash values from assembly file and actual.
        /// </summary>
        /// <param name="file">Full path to assembly file.</param>
        /// <param name="actual">Actual hash value for comparison.</param>
        /// <returns>Equals or not.</returns>
        protected virtual bool hashEquals(string file, string actual)
        {
            string h1 = FileVersionInfo.GetVersionInfo(file).ProductName;
            if(String.IsNullOrEmpty(h1)) {
                Log.Trace("[Cache] hash code from '{0}' is null or empty", file);
                return false;
            }
            Log.Trace("[Cache] compare hash values '{0}' / '{1}'", h1, actual);
            return h1.Equals(actual, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets list of the file names for compiling from data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>An array of the file names.</returns>
        private string[] filesFromCommand(string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return new string[]{ };
            }
            return data.Replace("\r\n", "\n").Split('\n');
        }

        /// <summary>
        /// Gets absolute paths to files and works with mask (*.*, *.cs, ..)
        /// </summary>
        /// <param name="files">List of files.</param>
        /// <returns></returns>
        private string[] extractFiles(string[] files)
        {
            List<string> ret = new List<string>();
            foreach(string file in files)
            {
                string mask     = Path.GetFileName(file);
                string fullname = Path.Combine(Settings.WPath, file);

                if(mask.IndexOf('*') != -1) {
                    ret.AddRange(Directory.GetFiles(Path.GetDirectoryName(fullname), mask));
                    continue;
                }
                ret.Add(fullname);
            }
            return ret.ToArray();
        }

        private string outputCacheFile(ISolutionEvent evt)
        {
            IModeCSharp cfg = (IModeCSharp)evt.Mode;

            string path = (cfg.OutputPath)?? String.Empty;
            if(evt.SupportMSBuild) {
                path = cmd.MSBuild.parse(path);
            }

            return Path.Combine(BasePathToCache, path, fileName(evt));
        }

        private Assembly assemblyResolver(object sender, ResolveEventArgs args)
        {
            if(args.RequestingAssembly == null || !args.RequestingAssembly.FullName.StartsWith(PREFIX_CACHE)) {
                return null;
            }

            Log.Trace("Assembly resolver: '{0}' /requesting from '{1}'", args.Name, args.RequestingAssembly.FullName);
            try {
                return Assembly.LoadFrom(String.Format("{0}\\{1}.dll",
                                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                        args.Name.Substring(0, args.Name.IndexOf(","))));
            }
            catch {
                return null;
            }
        }
    }
}