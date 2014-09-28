/* 
 * Copyright (c) 2014 Developed by reg <entry.reg@gmail.com>
 * 
 * Distributed under the MIT license
 * (see accompanying file LICENSE or a copy at http://opensource.org/licenses/MIT)
 * 
 * helper for vsSolutionBuildEvent
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.Version
{
    internal class Update
    {
        public struct Data
        {
            /// <summary>
            /// Version number(Major.Minor.Patch) for updates
            /// </summary>
            public string version;
            /// <summary>
            /// .git directory
            /// </summary>
            public string git;
            /// <summary>
            /// Template of Version.cs
            /// </summary>
            public string tpl;
            /// <summary>
            /// Original Version.cs
            /// </summary>
            public string cs;
            /// <summary>
            /// Original source.extension.vsixmanifest
            /// </summary>
            public string manifest;
        }

        public System.Version Version
        {
            get { return version;  }
        }
        protected System.Version version;

        /// <param name="data"></param>
        /// <param name="revForManifest">Write or not the revision number for vsixmanifest</param>
        public Update(Data data, bool revForManifest)
        {
            version = generateRevision(loadVersionFromFile(data.version));

            tVersion(data.git, data.tpl, data.cs);
            tVsixmanifest(data.manifest, revForManifest);
        }

        protected void tVersion(string gitDir, string template, string original)
        {
            _write(original, _read(template).Replace("%Version%",
                                string.Format("{0}, {1}, {2}, {3}", version.Major, version.Minor, version.Build, version.Revision)
                             )
                             .Replace("%VersionRevString%", string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision))
                             .Replace("%VersionString%",    string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build))
                             .Replace("%branchSha1%",       _cmdGit("rev-parse --short HEAD", gitDir))
                             .Replace("%branchName%",       _cmdGit("rev-parse --abbrev-ref HEAD", gitDir))
                             .Replace("%branchRevCount%",   _cmdGit("rev-list HEAD --count", gitDir))
            );
        }

        protected void tVsixmanifest(string manifest, bool showRevision)
        {
            string versionString = String.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            if(showRevision) {
                versionString += String.Format(".{0}", version.Revision);
            }

            _write(manifest, 
                Regex.Replace(_read(manifest), 
                                @"<Version>[0-9\.]+</Version>", 
                                String.Format("<Version>{0}</Version>", versionString), 
                                RegexOptions.IgnoreCase)
            );
        }

        protected System.Version loadVersionFromFile(string name)
        {
            return new System.Version(_read(name));
        }

        protected virtual System.Version generateRevision(System.Version ver)
        {
            return new System.Version(ver.Major, ver.Minor, ver.Build, 
                                     (int)((DateTime.UtcNow - new DateTime(2014, 7, 7)).TotalMinutes));
        }

        private string _cmdGit(string command, string path)
        {
            if(!Directory.Exists(path)) {
                return "?";
            }

            Process p = new Process();
            p.StartInfo.FileName = "git";

            p.StartInfo.Arguments               = string.Format("--git-dir=\"{0}\" {1}", path, command);
            p.StartInfo.UseShellExecute         = false;
            p.StartInfo.RedirectStandardOutput  = true;
            p.StartInfo.RedirectStandardError   = true;
            p.Start();

            string errors = p.StandardError.ReadToEnd();
            if(errors.Length > 0) {
                throw new IOException(errors);
            }
            return p.StandardOutput.ReadLine();
        }

        /// <exception cref="*">all unhandled exceptions from StreamReader</exception>
        private string _read(string fname)
        {
            using(StreamReader reader = new StreamReader(fname)) {
                return reader.ReadToEnd();
            }
        }

        private void _write(string file, string data)
        {
            using(TextWriter stream = new StreamWriter(file)) {
                stream.Write(data);
            }
        }
    }
}
