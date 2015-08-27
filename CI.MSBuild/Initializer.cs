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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Build.Framework;
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
            get
            {
                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if(path[path.Length - 1] != Path.DirectorySeparatorChar) {
                    path += Path.DirectorySeparatorChar;
                }
                return path;
            }
        }

        /// <summary>
        /// Used logger
        /// </summary>
        internal ILog log;


        /// <param name="parameters">User-defined parameters</param>
        /// <param name="logger"></param>
        /// <param name="showHeader">To show header message if true</param>
        public Initializer(string parameters, ILog logger, bool showHeader = true)
        {
            log = logger;

            if(showHeader) {
                header();
            }
            Properties = extractProperties(parameters);

            setCulture(Properties.Args);
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

            ILoader loader = new Loader(
                                    new Provider.Settings()
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
                ILibrary library = loader.load(prop.SolutionFile, prop.Properties, prop.LibraryPath);
                log.info("Library: loaded from '{0}' :: v{1} [{2}] API: v{3} /'{4}':{5}", 
                                    library.Dllpath, 
                                    library.Version.Number.ToString(), 
                                    library.Version.BranchSha1,
                                    library.Version.Bridge.Number.ToString(2),
                                    library.Version.BranchName,
                                    library.Version.BranchRevCount);

                return library;
            }
            catch(DllNotFoundException ex)
            {
                log.info(ex.Message);
                log.info(new String('.', 80));
                log.info("How about:");

                log.info("");
                log.info("* Define path to library, for example: /l:CI.MSBuild.dll;lib=<path_to_vsSolutionBuildEvent.dll>");
                log.info("* Or install the vsSolutionBuildEvent as plugin for Visual Studio.");
                log.info("* Or manually place the 'vsSolutionBuildEvent.dll' with dependencies into: '{0}\\'", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                log.info("");

                log.info("See documentation for more details:");
                log.info("- http://vssbe.r-eg.net");
                log.info("- http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
                log.info("");

                log.info("Minimum requirements: vsSolutionBuildEvent.dll v{0}", loader.MinVersion.ToString());
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
                log.info("Error with loading: '{0}'", ex.ToString());
            }

            throw new LoggerException("Fatal error");
        }

        protected void header()
        {
            log.info(new String('=', 60));
            log.info("[[ vsSolutionBuildEvent CI.MSBuild ]] Welcomes You!");
            log.info(new String('=', 60));
            log.info("Version: v{0}", System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);
            log.info("Feedback: entry.reg@gmail.com | vssbe.r-eg.net");
            log.info(new String('_', 60));
        }

        /// <summary>
        /// Find .sln file
        /// </summary>
        /// <param name="targets">Return the targets key if exists or null value</param>
        /// <param name="property">Return the property key if exists or null value</param>
        /// <returns>Full path to .sln file</returns>
        protected string findSln(out string targets, out string property)
        {
            string sln  = null;
            targets     = null;
            property    = null;

            // multi-keys support: `/p:prop1=val1 /p:prop2=val2` same as `/p:prop1=val1;prop2=val2`
            Func<string, string, string> _concat = delegate(string key, string val) {
                return (key != null)? String.Format("{0};{1}", key, val) : val;
            };

            foreach(string arg in Environment.GetCommandLineArgs())
            {
                if(sln == null && arg.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)) {
                    sln = arg;
                    log.debug("findSln: .sln - '{0}'", sln);
                    continue;
                }

                int vpos = arg.IndexOf(':', 2);
                if(vpos == -1) {
                    continue;
                }

                if(arg.StartsWith("/target:", StringComparison.OrdinalIgnoreCase)
                    || arg.StartsWith("/t:", StringComparison.OrdinalIgnoreCase))
                {
                    targets = _concat(targets, arg.Substring(vpos + 1));
                    log.debug("findSln: targets - '{0}'", targets);
                    continue;
                }

                if(arg.StartsWith("/property:", StringComparison.OrdinalIgnoreCase)
                    || arg.StartsWith("/p:", StringComparison.OrdinalIgnoreCase))
                {
                    property = _concat(property, arg.Substring(vpos + 1));
                    log.debug("findSln: property - '{0}'", property);
                    continue;
                }
            }
            return sln;
        }

        /// <summary>
        /// Find & get path to main library
        /// </summary>
        protected string findLibraryPath(Dictionary<string, string> args)
        {
            if(args.ContainsKey(LKEY_LIB)) {
                return Path.Combine(RootPath, args[LKEY_LIB]);
            }
            return RootPath;
        }

        /// <summary>
        /// To redefinition Configuration and Platform from user switches if exists, etc.
        /// </summary>
        /// <param name="cmd">Property of command line</param>
        /// <param name="init">Initial properties</param>
        /// <returns></returns>
        protected Dictionary<string, string> finalizeProperties(string cmd, Dictionary<string, string> init)
        {
            Dictionary<string, string> pKeys = extractArguments(cmd);
            Func<string, string> pValue = delegate(string key) {
                return pKeys.FirstOrDefault(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
            };

            string Configuration    = pValue("Configuration");
            string Platform         = pValue("Platform");

            if(Configuration != null) {
                init["Configuration"] = Configuration;
            }

            if(Platform != null) {
                init["Platform"] = Platform;
            }

            if(log.IsDiagnostic)
            {
                foreach(KeyValuePair<string, string> p in init) {
                    log.info(String.Format("Solution property def: ['{0}' => '{1}']", p.Key, p.Value));
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
        protected Dictionary<string, string> getSolutionProperties(IEnumerable properties)
        {
            Dictionary<string, string> _properties = properties.OfType<DictionaryEntry>().ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

            Dictionary<string, string> ret = new Dictionary<string, string>();
            if(_properties.ContainsKey("Configuration")) {
                ret["Configuration"] = _properties["Configuration"];
            }
            if(_properties.ContainsKey("Platform")) {
                ret["Platform"] = _properties["Platform"];
            }
            if(_properties.ContainsKey("SolutionDir")) {
                ret["SolutionDir"] = _properties["SolutionDir"];
            }
            if(_properties.ContainsKey("SolutionName")) {
                ret["SolutionName"] = _properties["SolutionName"];
            }
            if(_properties.ContainsKey("SolutionFileName")) {
                ret["SolutionFileName"] = _properties["SolutionFileName"];
            }
            if(_properties.ContainsKey("SolutionExt")) {
                ret["SolutionExt"] = _properties["SolutionExt"];
            }
            if(_properties.ContainsKey("SolutionPath")) {
                ret["SolutionPath"] = _properties["SolutionPath"];
            }

            if(log.IsDiagnostic)
            {
                foreach(KeyValuePair<string, string> p in ret) {
                    log.info(String.Format("SolutionProperty found: ['{0}' => '{1}']", p.Key, p.Value));
                }
            }
            return ret;
        }

        /// <param name="parameters">User-defined parameters</param>
        protected InitializerProperties extractProperties(string parameters)
        {
            string targets  = null;
            string property = null;
            string sln      = findSln(out targets, out property);

            if(sln == null) {
                throw new LoggerException("We can't detect .sln file in arguments.");
            }
            sln = Path.Combine(Environment.CurrentDirectory, sln);
            log.debug("Solution file is detected: '{0}'", sln);

            SolutionProperties.Result slnData   = (new SolutionProperties(log)).parse(sln);
            Dictionary<string, string> args     = extractArguments(parameters);

            return new InitializerProperties()
            {
                SolutionFile    = sln,
                Properties      = finalizeProperties(property, slnData.properties),
                PropertyCmdRaw  = property,
                Targets         = (targets)?? "Build", // 'Build' for targets by default
                LibraryPath     = findLibraryPath(args),
                Args            = args,
            };
        }

        /// <param name="keys">Defined cultures</param>
        protected void setCulture(Dictionary<string, string> keys)
        {
            string culture      = null;
            string cultureUI    = (keys.ContainsKey(LKEY_CULTURE_UI))? keys[LKEY_CULTURE_UI] : "en-US";

            if(keys.ContainsKey(LKEY_CULTURE)) {
                culture = keys[LKEY_CULTURE];
            }
            setCulture(cultureUI, culture);
        }

        /// <summary>
        /// Extracts arguments by format: 
        /// name1=value1;name2=value2;name3=value3
        /// http://msdn.microsoft.com/en-us/library/ms164311.aspx
        /// </summary>
        /// <param name="raw">Multiple properties separated by semicolon</param>
        /// <returns></returns>
        protected Dictionary<string, string> extractArguments(string raw)
        {
            if(String.IsNullOrEmpty(raw)) {
                return new Dictionary<string, string>();
            }

            return raw.Split(';')
                        .Select(p => p.Split('='))
                        .Select(p => new KeyValuePair<string, string>(p[0], (p.Length < 2)? null : p[1]))
                        .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}
