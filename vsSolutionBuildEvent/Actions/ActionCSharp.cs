/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext.Extensions;
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
        public const string MAIN_CLASS = Settings.APP_NAME + ".CSharpMode";

        /// <summary>
        /// Entry point for user code.
        /// </summary>
        public const string ENTRY_POINT = "Init";

        /// <summary>
        /// Prefix to cached bytecode.
        /// </summary>
        protected const string PREFIX_CACHE = Settings.APP_CFG + Settings.APP_CFG_USR + ".";

        /// <summary>
        /// Where to look cache.
        /// </summary>
        protected string BasePathToCache => Settings.WPath;

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

                using TextWriter stream = new StreamWriter(FullPath, false, Encoding.UTF8);
                stream.Write(data);
            }

            #region IDisposable

            private bool disposed;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool _)
            {
                if(!disposed)
                {
                    try
                    {
                        File.Delete(FullPath);
                    }
                    catch(Exception ex)
                    {
                        Debug.Assert(false, $"Failed disposing: {ex.Message}");
                    }
                    disposed = true;
                }
            }

            #endregion
        }

        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            Type type = isRequiresCompilation(evt)
                        ? compileAndGetType(evt)
                        : findOrLoad(evt);

            if(type == null) Log.Error($"Compiled type is null. Something went wrong for C# Action '{evt.Name}'");

            int ret = run(type, cmd, evt);
            if(ret == 0) return true;

            string retmsg = $"Return code '{ret}'";

            if(((IModeCSharp)evt.Mode).TreatWarningsAsErrors) {
                Log.Error(retmsg);
            }
            else {
                Log.Warn(retmsg);
            }
            return false;
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
            IModeCSharp cfg = (IModeCSharp)evt.Mode;

            if(cfg.CacheData == null) return true;
            string cache = fileName(evt);

            Log.Trace($"[Cache] Checks: '{cfg.CachingBytecode}','{evt.Name}','{cache}'");
            if(!cfg.CachingBytecode || string.IsNullOrEmpty(cache)) return true;

            ICacheHeader ch = cfg.CacheData.Manager.CacheHeader;

            if(cfg.GenerateInMemory)
            {
                foreach(Assembly asm in iterateLoaded(evt))
                {
                    string hash = GetAsmHash(asm);
                    if(hash != null && hash == ch.Hash) return false;
                }
                return true;
            }

            FileInfo f = new(outputCacheFile(evt));

            if(!f.Exists || !hashEquals(f.FullName, ch.Hash))
            {
                Log.Info($"[Cache] {f.FullName} or {ch.Hash} is outdated. Compile new.");
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
            mode.CacheData ??= new UserValue();

            // calculate hash only for objects below

            object[] toHash =
            [
                mode.Command,
                mode.References,
                mode.OutputPath,
                mode.CompilerOptions,
                mode.TreatWarningsAsErrors,
                mode.WarningLevel,
                mode.FilesMode
            ];

            mode.CacheData.Manager.CacheHeader.Hash         = toHash.MD5Hash();
            mode.CacheData.Manager.CacheHeader.Algorithm    = HashType.MD5;
            mode.CacheData.Manager.CacheHeader.Updated      = DateTime.Now.ToFileTimeUtc();
            Settings._.Config.Usr.save();
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
        /// Load compiled user's code.
        /// </summary>
        /// <param name="file">Path to module.</param>
        /// <returns>The Type for work with user's code.</returns>
        protected Type load(string file)
        {
            Log.Trace($"[load] {file}");

            if(!File.Exists(file)) throw new MismatchException($"the cache '{file}' has disappeared. Try again.");

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
                return load(compiled.PathToAssembly);
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
                throw new ArgumentException("[Compiler] code is not found. abort;");
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
            if(string.IsNullOrEmpty(hash))
            {
                return provider.CompileAssemblyFromFile(
                    parameters, 
                    filesFromCommand(source).ExtractFiles(Settings.WPath)
                );
            }

            using(TempAssemblyInfo f = new TempAssemblyInfo(hash))
            {
                return provider.CompileAssemblyFromFile(
                    parameters, 
                    filesFromCommand($"{source}\n{f.FullPath}").ExtractFiles(Settings.WPath)
                );
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
                            .Select(r => (msbuild)? cmd.MSBuild.Eval(r) : r)
                            .ToArray();
            }
            
            GAC gac = new GAC();
            return cfg.References
                        .Where(r => !String.IsNullOrEmpty(r))
                        .Select(r =>
                                    gac.getPathToAssembly(
                                            (msbuild)? cmd.MSBuild.Eval(r) : r,
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
            if(!cfg.CachingBytecode) return string.Empty;

            if(cfg.CacheData == null
                || cfg.CacheData.Manager.CacheHeader.Updated == 0
                || string.IsNullOrWhiteSpace(cfg.CacheData.Manager.CacheHeader.Hash))
            {
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

        /// <returns>Returns the found type from already loaded assemblies. Or null if not.</returns>
        protected Type findLoadedType(string ident, string hash)
        {
            Log.Trace($"Find loaded {ident} @{hash}");

            foreach(Assembly asm in iterateLoaded(ident))
            {
                string actualHash = GetAsmHash(asm);
                if(actualHash != null && actualHash == hash)
                {
                    Log.Debug($"[load] use from loaded: '{asm}' == {hash}");
                    return asm.GetType(MAIN_CLASS);
                }
            }

            return null;
        }

        protected Type findOrLoad(ISolutionEvent evt)
        {
            Type type = findLoadedType
            (
                assemblyName(evt),
                ((IModeCSharp)evt.Mode).CacheData?.Manager?.CacheHeader.Hash
            );
            return type ?? load(outputCacheFile(evt));
        }

        protected IEnumerable<Assembly> iterateLoaded(ISolutionEvent evt)
            => iterateLoaded(assemblyName(evt));

        protected IEnumerable<Assembly> iterateLoaded(string ident)
        {
            Assembly[] asm = AppDomain.CurrentDomain.GetAssemblies();
            for(int i = asm.Length - 1; i > 0; --i)
            {
                if(asm[i].GetName()?.Name == ident)
                    yield return asm[i];
            }
        }

        private static string GetAsmHash(Assembly input)
        {
#if !SDK10
            IEnumerable<Attribute> attr = input.GetCustomAttributes(typeof(AssemblyProductAttribute));
#else
            object[] attr = input.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
#endif
            return (attr.FirstOrDefault() as AssemblyProductAttribute)?.Product;
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

        private string outputCacheFile(ISolutionEvent evt)
        {
            IModeCSharp cfg = (IModeCSharp)evt.Mode;

            string path = (cfg.OutputPath)?? String.Empty;
            if(evt.SupportMSBuild) {
                path = cmd.MSBuild.Eval(path);
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