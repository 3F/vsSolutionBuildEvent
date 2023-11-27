/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Reflection;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.vsSBE.Configuration;

namespace net.r_eg.vsSBE
{
    internal sealed class Settings: IAppSettings
    {
        /// <summary>
        /// VS Output Window Pane - `application`
        /// </summary>
        public const string OWP_ITEM_VSSBE = APP_NAME;

        /// <summary>
        /// VS Output Window Pane - `Build`
        /// </summary>
        public const string OWP_ITEM_BUILD = "Build";

        public const string APP_NAME = "vsSolutionBuildEvent";

        /// <summary>
        /// Short name of <see cref="APP_NAME"/>
        /// </summary>
        public const string APP_NAME_SHORT = "vsSBE";

        internal const string APP_CFG = ".vssbe";

        internal const string APP_CFG_USR = ".user";

        private static readonly Lazy<Settings> _lazy = new(() => new Settings());

        private bool debugMode = false;

        private string libPath, workPath;

        public event EventHandler<DataArgs<bool>> DebugModeUpdated = delegate(object sender, DataArgs<bool> e) { };

        public event EventHandler<DataArgs<string>> WorkPathUpdated = delegate (object sender, DataArgs<string> e) { };

        public static IAppSettings _ => _lazy.Value;

        public bool DebugMode
        {
            get => debugMode;
            set
            {
                debugMode = value;
                DebugModeUpdated(this, new DataArgs<bool>() { Data = debugMode });
            }
        }

        public bool IgnoreActions { get; set; } = false;

        public string CommonPath { get; } = GetCommonPath();

        public string LibPath
        {
            get
            {
                if(string.IsNullOrWhiteSpace(libPath))
                {
                    libPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).DirectoryPathFormat();
                }
                return libPath;
            }
        }

        public string WorkPath
            //NOTE: First call is possible from Bootloader before EventLevel.solutionOpened
            => string.IsNullOrWhiteSpace(workPath) ? setWorkPath(GetCommonPath()) : workPath;

        public ConfManager Config { get; } = new();

        /// <summary>
        /// Static alias. <see cref="WorkPath"/>
        /// </summary>
        public static string WPath => _.WorkPath;

        /// <summary>
        /// Static alias. <see cref="LibPath"/>
        /// </summary>
        public static string LPath => _.LibPath;

        public string setWorkPath(string path)
        {
            workPath = path.DirectoryPathFormat();
            Log.Trace($"{nameof(workPath)} now is {workPath}");

            WorkPathUpdated(this, new DataArgs<string>() { Data = workPath });
            return workPath;
        }

        internal static string GetCommonPath()
        {
            //NOTE: do not use `VisualStudioDir`, L-711
            string root = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(root ?? Path.GetTempPath(), APP_NAME).DirectoryPathFormat();

            if(!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch(Exception ex) when 
                (ex is UnauthorizedAccessException || ex is PathTooLongException)
                {
                    path = Path.GetTempPath();
                }
            }
            return path;
        }

        private Settings() { }
    }
}
