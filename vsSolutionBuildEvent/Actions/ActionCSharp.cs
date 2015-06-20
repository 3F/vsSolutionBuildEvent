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
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
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
        /// Where to look cache.
        /// </summary>
        protected string BasePathToCache
        {
            get { return Settings.WorkingPath; }
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
                Log.nlog.Warn("Return code '{0}'", ret);
                return false;
            }
            return true;
        }

        /// <param name="cmd"></param>
        public ActionCSharp(ICommand cmd)
            : base(cmd)
        {
            lock(_lock) {
                AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolver;
                AppDomain.CurrentDomain.AssemblyResolve += assemblyResolver;
            }
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

            Log.nlog.Trace("[Cache] Checks: '{0}','{1}','{2}','{3}'", cfg.GenerateInMemory, cfg.CachingBytecode, evt.Name, cache);
            if(cfg.GenerateInMemory || !cfg.CachingBytecode || String.IsNullOrEmpty(cache)) {
                return true;
            }
            
            FileInfo f = new FileInfo(outputCacheFile(evt));
            if(!f.Exists) {
                Log.nlog.Info("[Cache] Binary '{0}' is not found in '{1}'.", cache, f.FullName);
                return true;
            }

            if(cfg.LastTime != f.LastWriteTimeUtc.Ticks) {
                Log.nlog.Debug("[Cache] Is outdated '{0}' for '{1}'", cfg.LastTime, f.LastWriteTimeUtc.Ticks);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets assembly name of compiled user code for event.
        /// </summary>
        /// <param name="evt">Specific event.</param>
        /// <returns></returns>
        protected virtual string assemblyName(ISolutionEvent evt)
        {
            return String.Format("cached_{0}.{1}", cmd.EventType, evt.Name);
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
            Log.nlog.Trace("[load] started with: '{0}' /'{1}'", file, find);

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
                        Log.nlog.Trace("[load] use from loaded: '{0}'", a);
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
            Log.nlog.Trace("Uses memory for getting type.");

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
            Log.nlog.Trace("[Compiler] start new compilation.");

            IModeCSharp cfg = (IModeCSharp)evt.Mode;
            string command  = cfg.Command;

            if(String.IsNullOrWhiteSpace(command)) {
                throw new InvalidArgumentException("[Compiler] code is not found. abort;");
            }
            command = parse(evt, command);

            string output = outputCacheFile(evt);
            Log.nlog.Debug("[Compiler] output: '{0}' /GenerateInMemory: {1}", output, cfg.GenerateInMemory);

            if(File.Exists(output)) {
                Log.nlog.Trace("[Compiler] clear cache.");
                File.Delete(output);
            }

            CompilerParameters parameters = new CompilerParameters()
            {
                GenerateExecutable      = false,
                GenerateInMemory        = cfg.GenerateInMemory,
                CompilerOptions         = cfg.CompilerOptions,
                TreatWarningsAsErrors   = cfg.TreatWarningsAsErrors,
                WarningLevel            = cfg.WarningLevel,
                OutputAssembly          = (cfg.GenerateInMemory)? null : output
            };
            
            GAC gac = new GAC();
            if(cfg.References != null) {
                //parameters.ReferencedAssemblies.AddRange(cfg.References);
                parameters.ReferencedAssemblies.AddRange(cfg.References.Select(r => gac.getPathToAssembly(r)).ToArray());
            }
            parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location); //to support ICommand & ISolutionEvent

            // ready to work with provider
            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerResults compiled;
            if(cfg.FilesMode) {
                Log.nlog.Trace("[Compiler] use as list of files with source code.");
                compiled = provider.CompileAssemblyFromFile(parameters, extractFiles(filesFromCommand(command)));
            }
            else {
                compiled = provider.CompileAssemblyFromSource(parameters, command);
            }

            // messages about errors & warnings
            foreach(CompilerError msg in compiled.Errors) {
                Log.nlog.Log((msg.IsWarning)? LogLevel.Warn : LogLevel.Error, "[Compiler] '{0}'", msg.ToString());
            }

            if(compiled.Errors.HasErrors) {
                throw new CompilerException("[Compiler] found errors. abort;");
            }

            if(cfg.CachingBytecode) {
                updateTimeTo(compiled.PathToAssembly, evt);
            }

            return compiled;
        }

        /// <summary>
        /// Should update the LastTime field for current event.
        /// </summary>
        /// <param name="pathToAssembly">Full path to assembly for getting timestamp.</param>
        /// <param name="evt">Where to update.</param>
        protected void updateTimeTo(string pathToAssembly, ISolutionEvent evt)
        {
            FileInfo f = new FileInfo(pathToAssembly);
            if(!f.Exists) {
                Log.nlog.Warn("[Cache] Can't find compiled '{0}' for caching.", pathToAssembly);
                return;
            }

            IModeCSharp cfg = (IModeCSharp)evt.Mode;
            Log.nlog.Debug("[Cache] updating timestamp: '{0}' -> '{1}'", cfg.LastTime, f.LastWriteTimeUtc.Ticks);

            cfg.LastTime = f.LastWriteTimeUtc.Ticks;
            Config._.save();
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
                string fullname = Path.Combine(Settings.WorkingPath, file);

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
            return Path.Combine(BasePathToCache, (cfg.OutputPath)?? String.Empty, fileName(evt));
        }

        private Assembly assemblyResolver(object sender, ResolveEventArgs args)
        {
            string req = (args.RequestingAssembly == null)? "-" : args.RequestingAssembly.FullName;
            Log.nlog.Trace("Assembly resolver: '{0}' /requesting from '{1}'", args.Name, req);

            try {
                return Assembly.LoadFrom(String.Format("{0}\\{1}.dll",
                                                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                        args.Name.Substring(0, args.Name.IndexOf(","))));
            }
            catch { //TODO:
                return Assembly.LoadFile(args.Name);
            }
        }
    }
}