/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Framework;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Provider;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    internal class Initializer
    {
        /// <summary>
        /// The key for logger: Path to library.
        /// </summary>
        public const string LKEY_LIB = "lib";

        /// <summary>
        /// The key for logger: 
        /// Culture for the current thread.
        /// </summary>
        public const string LKEY_CULTURE = "culture";

        /// <summary>
        /// The key for logger: 
        /// Culture used by the Resource Manager to look up culture-specific resources at run time. 
        /// For example - console messages from msbuild engine etc.
        /// </summary>
        public const string LKEY_CULTURE_UI = "cultureUI";

        internal KArgs kargs;

        internal ILog log;

        /// <summary>
        /// Defined properties for initializer
        /// </summary>
        public InitializerProperties Properties
        {
            get;
            protected set;
        }

        /// <summary>
        /// Root path for our CIM manager.
        /// It can be in different path from SolutionDir.
        /// </summary>
        public string RootPath
        {
            get => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                        .DirectoryPathFormat();
        }

        /// <param name="parameters">User-defined parameters</param>
        /// <param name="logger"></param>
        /// <param name="showHeader">To show header message if true</param>
        public Initializer(string parameters, ILog logger, bool showHeader = true)
        {
            log = logger;

            kargs = new KArgs();

            if(showHeader) {
                header();
            }
            Properties = extractProperties(parameters);

            setCulture(kargs[KArgType.CIM]);
        }

        public void setCulture(string ui, string general = null)
        {
            try
            {
                if(ui != null) {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(ui);
                }
                if(general != null) {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(general);
                }
            }
            catch(Exception ex) {
                log.info(ex.Message);
            }
        }

        /// <summary>
        /// Loads the library with default properties
        /// </summary>
        /// <returns></returns>
        public ILibrary load()
        {
            return load(Properties);
        }

        /// <summary>
        /// Loads the library with specific properties
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public ILibrary load(InitializerProperties prop)
        {
            log.debug("Loading: '{0}' /'{1}'", prop.SolutionFile, prop.LibraryPath);

            ILoader loader = new Loader
            (
                new Settings()
                {
                    DebugMode = log.IsDiagnostic,
                    LibSettings = new LibSettings()
                    {
                        DebugMode = log.IsDiagnostic,
                    },
                }
            );

            try
            {
                ILibrary library = loader.load
                (
                    prop.SolutionFile, 
                    new Dictionary<string, string>(prop.Properties), //TODO: API to IDictionary for more abstract cases
                    prop.LibraryPath
                );

                log.info($" --# Core library: {library.Version.Number}+{library.Version.BranchSha1} API {library.Version.Bridge.Number.ToString(2)}: '{library.Dllpath}'");

                return library;
            }
            catch(DllNotFoundException ex)
            {
                log.info(ex.Message);
                log.info(new String('.', 80));
                log.info($"Required vsSolutionBuildEvent.dll v{loader.MinVersion} (min.) was not found.");

                log.info("");
                log.info("* Try to define path to library, for example: /l:CI.MSBuild.dll;lib=<path_to_vsSolutionBuildEvent.dll>");
                log.info("* Or install the vsSolutionBuildEvent as plugin for Visual Studio.");
                log.info($"* Or place manually 'vsSolutionBuildEvent.dll' with dependencies into: '{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\'");
                log.info("* Or please report here: https://github.com/3F/vsSolutionBuildEvent");
                log.info("");

                log.info(new String('.', 80));
            }
            catch(ReflectionTypeLoadException ex)
            {
                log.info(ex.ToString());
                log.info(new String('.', 80));

                foreach(FileNotFoundException le in ex.LoaderExceptions) {
                    log.info("{2} {0}{3} {0}{0}{4} {0}{1}",
                                        Environment.NewLine, 
                                        new String('~', 80),
                                        le.FileName, 
                                        le.Message, 
                                        le.FusionLog);
                }
            }
            catch(Exception ex) {
                log.info($"Error when loading: '{ex.ToString()}'");
            }

            throw new LoggerException("Fatal error");
        }

        protected void header()
        {
            log.info($"{Environment.NewLine}  vsSolutionBuildEvent");
            log.info( "  Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F");
            log.info($"  CI.MSBuild: {System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            log.info($"  https://github.com/3F/vsSolutionBuildEvent {Environment.NewLine}");
        }

        /// <summary>
        /// Find .sln file
        /// </summary>
        protected string findSln()
        {
            foreach(string arg in kargs.GetKeys(KArgType.Common))
            {
                if(arg.TrimEnd().EndsWith(".sln", StringComparison.OrdinalIgnoreCase)) {
                    log.debug($"{nameof(findSln)}: .sln '{arg}'");
                    return arg;
                }
            }

            return Directory.GetFiles(Environment.CurrentDirectory, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
        }

        /// <summary>
        /// Find + get path to main library
        /// </summary>
        protected string findLibraryPath(IDictionary<string, string> args)
        {
            if(args.ContainsKey(LKEY_LIB)) {
                return Path.Combine(RootPath, args[LKEY_LIB] ?? string.Empty);
            }
            return RootPath;
        }

        /// <summary>
        /// Finalize properties using user switches if exists.
        /// </summary>
        protected IDictionary<string, string> finalizeProperties(IDictionary<string, string> properties, IDictionary<string, string> init)
        {
            string pValue(string key) {
                return properties.FirstOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            }

            string userConfig   = pValue(PropertyNames.CONFIG);
            string userPlatform = pValue(PropertyNames.PLATFORM);

            if(userConfig != null) {
                init[PropertyNames.CONFIG] = userConfig;
            }

            if(userPlatform != null) {
                init[PropertyNames.PLATFORM] = userPlatform;
            }

            if(log.IsDiagnostic)
            {
                foreach(var p in init) {
                    log.info($"Solution property def: ['{p.Key}' => '{p.Value}']");
                }
            }

            return init;
        }

        /// <summary>
        /// Gets solution properties (global properties for all projects)
        /// 
        /// Note: ProjectStartedEventArgs.GlobalProperties available only with .NET 4.5
        /// http://msdn.microsoft.com/en-us/library/microsoft.build.framework.projectstartedeventargs.globalproperties%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="properties">DictionaryEntry properties</param>
        /// <returns></returns>
        protected IDictionary<string, string> getSolutionProperties(IEnumerable properties)
        {
            var _props = properties.OfType<DictionaryEntry>()
                            .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

            var ret = new Dictionary<string, string>()
            {
                [PropertyNames.CONFIG]      = _props.GetOrDefault(PropertyNames.CONFIG),
                [PropertyNames.PLATFORM]    = _props.GetOrDefault(PropertyNames.PLATFORM),
                [PropertyNames.SLN_DIR]     = _props.GetOrDefault(PropertyNames.SLN_DIR),
                [PropertyNames.SLN_NAME]    = _props.GetOrDefault(PropertyNames.SLN_NAME),
                [PropertyNames.SLN_FNAME]   = _props.GetOrDefault(PropertyNames.SLN_FNAME),
                [PropertyNames.SLN_EXT]     = _props.GetOrDefault(PropertyNames.SLN_EXT),
                [PropertyNames.SLN_PATH]    = _props.GetOrDefault(PropertyNames.SLN_PATH),
            };

            if(log.IsDiagnostic)
            {
                foreach(var p in ret) {
                    log.info($"SolutionProperty found: ['{p.Key}' => '{p.Value}']");
                }
            }

            return ret;
        }

        /// <param name="parameters">User-defined parameters</param>
        protected InitializerProperties extractProperties(string parameters)
        {
            string slnFile = findSln();
            if(slnFile == null) {
                throw new LoggerException("We can't detect .sln file in arguments.");
            }

            slnFile = Path.Combine(Environment.CurrentDirectory, slnFile);
            log.debug("Solution file is detected: '{0}'", slnFile);

            ISlnResult sln = new SlnParser().Parse
            (
                slnFile,
                SlnItems.Projects | SlnItems.SolutionConfPlatforms | SlnItems.ProjectConfPlatforms
            );

            return new InitializerProperties()
            {
                SolutionFile    = slnFile,
                Properties      = finalizeProperties(kargs[KArgType.Properties], sln.Properties.ExtractDictionary),
                Targets         = !kargs.Exists(KArgType.Targets) ? "Build" : string.Join(";", kargs.GetKeys(KArgType.Targets)),
                LibraryPath     = findLibraryPath(kargs[KArgType.CIM]),
                Args            = kargs,
            };
        }

        /// <param name="keys">Defined cultures</param>
        protected void setCulture(IDictionary<string, string> keys)
        {
            string culture      = null;
            string cultureUI    = (keys.ContainsKey(LKEY_CULTURE_UI))? keys[LKEY_CULTURE_UI] : "en-US";

            if(keys.ContainsKey(LKEY_CULTURE)) {
                culture = keys[LKEY_CULTURE];
            }
            setCulture(cultureUI, culture);
        }
    }
}
