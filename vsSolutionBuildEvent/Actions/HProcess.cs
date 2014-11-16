/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    public class HProcess
    {
        [DllImport("kernel32.dll")]
        public static extern int GetSystemDefaultLCID();

        /// <summary>
        /// The current OEM code page from the system locale
        /// </summary>
        public static int OEMCodePage
        {
            get {
                CultureInfo inf = CultureInfo.GetCultureInfo(GetSystemDefaultLCID());
                return inf.TextInfo.OEMCodePage;
            }
        }

        /// <summary>
        /// OEM Encoding from system locale
        /// </summary>
        public static Encoding EncodingOEM
        {
            get {
                return Encoding.GetEncoding(OEMCodePage);
            }
        }

        /// <summary>
        /// Initial directory
        /// </summary>
        protected string initDir;

        /// <summary>
        /// Execute file with arguments.
        /// Uses synchronous read operations.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <param name="hidden">Hide process if true</param>
        /// <param name="stdin">Post input for app. Can be null if not used</param>
        /// <returns></returns>
        public string run(string file, string args, bool hidden, string stdin)
        {
            Process p = prepareProcessFor(file, args, hidden);
            p.Start();

            if(!String.IsNullOrEmpty(stdin)) {
                p.StandardInput.Write(stdin);
            }

            string errors = p.StandardError.ReadToEnd();
            if(errors.Length > 0) {
                throw new ComponentException(errors);
            }
            return p.StandardOutput.ReadToEnd().TrimEnd(new char[]{ '\r', '\n' });
        }

        /// <summary>
        /// Executing command with standard command-line interpreter
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="waiting">Waiting for completion</param>
        /// <param name="hidden">Hiding result</param>
        public void useShell(string cmd, bool waiting, bool hidden)
        {
            Process p = prepareProcessFor("cmd", String.Format("/C {0}", cmd), true);

            p.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
                if(String.IsNullOrEmpty(e.Data)) {
                    return;
                }
                Log.nlog.Warn("Command executed with error: '{0}'", e.Data);
            });

            p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
                if(!hidden) {
                    Log.nlog.Info("Result of command: '{0}'", e.Data);
                }
            });

            p.Start();
            p.BeginOutputReadLine();

            if(!waiting) {
                p.BeginErrorReadLine();
                return;
            }

            p.WaitForExit();
            string errors = p.StandardError.ReadToEnd();
            if(errors.Length > 0) {
                throw new ComponentException(errors);
            }
        }

        /// <param name="directory">Initial directory for processes</param>
        public HProcess(string directory)
        {
            initDir = directory;
        }

        public HProcess()
        {
            initDir = Settings.WorkPath;
        }

        /// <summary>
        /// Preparing instance of the Process
        /// </summary>
        /// <param name="file">Application to start</param>
        /// <param name="args">Arguments to use when starting the application</param>
        /// <param name="hidden">Silent mode</param>
        /// <returns>Prepared instance</returns>
        protected Process prepareProcessFor(string file, string args, bool hidden)
        {
            Process p = new Process();
            p.StartInfo.FileName = file;

            p.StartInfo.Arguments               = args;
            p.StartInfo.UseShellExecute         = false;
            p.StartInfo.WorkingDirectory        = initDir;
            p.StartInfo.RedirectStandardOutput  = true;
            p.StartInfo.RedirectStandardError   = true;
            p.StartInfo.RedirectStandardInput   = true;
            p.StartInfo.StandardErrorEncoding   = p.StartInfo.StandardOutputEncoding 
                                                = EncodingOEM;

            if(hidden) {
                p.StartInfo.WindowStyle     = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow  = true;
            }
            else {
                p.StartInfo.WindowStyle     = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow  = false;
            }
            return p;
        }
    }
}
