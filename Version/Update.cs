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
        public System.Version Version
        {
            get { return version;  }
        }
        protected System.Version version;

        /// <param name="from">Version number(Major.Minor.Patch) for updates</param>
        /// <param name="gitDir">.git directory</param>
        /// <param name="tpl">Template of Version.cs</param>
        /// <param name="cs">Original Version.cs</param>
        /// <param name="manifest">Original source.extension.vsixmanifest</param>
        public Update(string from, string gitDir, string tpl, string cs, string manifest)
        {
            version = generateRevision(loadVersionFromFile(from));

            tVersion(gitDir, tpl, cs);
            tVsixmanifest(manifest);
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

        protected void tVsixmanifest(string manifest)
        {
            _write(manifest, 
                Regex.Replace(_read(manifest), 
                                @"<Version>[0-9\.]+</Version>",
                                string.Format("<Version>{0}.{1}.{2}.{3}</Version>", version.Major, version.Minor, version.Build, version.Revision), 
                                RegexOptions.IgnoreCase
                )
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
