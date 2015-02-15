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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    public class HProcess
    {
        // CA1060
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern int GetSystemDefaultLCID();
        }

        /// <summary>
        /// The current OEM code page from the system locale
        /// </summary>
        public static int OEMCodePage
        {
            get {
                CultureInfo inf = CultureInfo.GetCultureInfo(NativeMethods.GetSystemDefaultLCID());
                return inf.TextInfo.OEMCodePage;
            }
        }

        /// <summary>
        /// OEM Encoding from system locale
        /// </summary>
        public static Encoding EncodingOEM
        {
            get
            {
                if(encodingOEM == null) {
                    encodingOEM = Encoding.GetEncoding(OEMCodePage);
                }
                return encodingOEM;
            }
        }
        protected static Encoding encodingOEM;

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
                throw new ComponentException(reEncodeString(errors));
            }

            string ret = reEncodeString(p.StandardOutput.ReadToEnd());
            if(!String.IsNullOrEmpty(ret)) {
                ret = ret.TrimEnd(new char[] { '\r', '\n' });
            }
            return ret;
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
                Log.nlog.Warn("Command executed with error: '{0}'", reEncodeString(e.Data));
            });

            p.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) => {
                if(!hidden) {
                    Log.nlog.Info("Result of command: '{0}'", reEncodeString(e.Data));
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
                throw new ComponentException(reEncodeString(errors));
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
                                                = EncodingOEM;  // by default, and we must to detect a charset for received data later

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

        /// <summary>
        /// Auto-detector to fixing the encoded string.
        /// </summary>
        /// <param name="str">Data for reencoding</param>
        /// <param name="from">Known Encoding for current string</param>
        /// <returns>Reencoded string with auto-detected charset.</returns>
        protected virtual string reEncodeString(string str, Encoding from)
        {
            if(String.IsNullOrEmpty(str)) {
                return str;
            }

            byte[] bytes = from.GetBytes(str);

            Ude.CharsetDetector cdet = new Ude.CharsetDetector();
            cdet.Feed(bytes, 0, bytes.Length);
            cdet.DataEnd();

            if(cdet.Charset == null) {
                Log.nlog.Debug("reEncodeString: Problem with detection... use the original");
                return str;
            }
            Log.nlog.Debug("reEncodeString: charset '{0}' confidence: '{1}'", cdet.Charset, cdet.Confidence);

            if(cdet.Confidence < 0.92f) {
                Log.nlog.Debug("reEncodeString: Confidence < 0.92 /use the original");
                return str;
            }

            Encoding to = Encoding.GetEncoding(cdet.Charset);
            Log.nlog.Debug("reEncodeString: '{0}' -> '{1}'", from.EncodingName, to.EncodingName);
            Log.nlog.Trace("reEncodeString: original - '{0}'", str);
            return to.GetString(bytes);
        }

        protected string reEncodeString(string str)
        {
            return reEncodeString(str, EncodingOEM);
        }
    }
}
