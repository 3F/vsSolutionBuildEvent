/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    public class HProcess
    {
        /// <summary>
        /// Unspecified identifier for streams.
        /// </summary>
        public readonly Guid STDS = new Guid("{97FC36E2-2751-4CCC-A2EC-80FFB5FAF3C4}");

        /// <summary>
        /// Initial directory
        /// </summary>
        protected string initDir;

        /// <summary>
        /// Container for standard stream data.
        /// TODO:
        /// </summary>
        private static Dictionary<Guid, TStream> stdstream = new Dictionary<Guid, TStream>();
        
        // CA1060
        internal static class NativeMethods
        {
            [DllImport("kernel32.dll")]
            internal static extern int GetSystemDefaultLCID();
        }

        internal sealed class TStream
        {
            public string stdout;
            public string stderr;
            //public string stdin;
        }

        /// <summary>
        /// The current OEM code page from system locale
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
        private static Encoding encodingOEM;

        /// <summary>
        /// Execute file with arguments. Uses synchronous read operations.
        /// TODO: Serial data for stdin.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        /// <param name="hidden">Hide process if true</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely</param>
        /// <returns></returns>
        public string run(string file, string args, bool hidden, int timeout = 0)
        {
            Process p = prepareProcessFor(file, args, hidden);
            p.Start();
            wait(p, file, timeout);

            if(!p.StartInfo.RedirectStandardError) {
                return String.Empty;
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
        /// Execute command with standard command-line interpreter.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="uid">Unique id for streams.</param>
        /// <param name="waiting">Waiting for completion</param>
        /// <param name="hidden">Hiding result</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely</param>
        public void useShell(string cmd, Guid uid, bool waiting, bool hidden, int timeout = 0)
        {
            Process p = prepareProcessFor("cmd", $"/C \"{cmd}\"", hidden);

            p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if(String.IsNullOrEmpty(e.Data)) {
                    return;
                }

                string sdata                = reEncodeString(e.Data);
                StreamContainer(uid).stderr = sdata;

                Log.Info("stderr: received '{0}'", sdata.Length);
                Log.Warn("The command has been executed with error: `{0}`", sdata);
            };

            p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if(String.IsNullOrEmpty(e.Data)) {
                    return;
                }

                string sdata                = reEncodeString(e.Data);
                StreamContainer(uid).stdout = sdata;

                Log.Info("stdout: received '{0}'", sdata.Length);
                Log.Info("Result of the command: `{0}`", sdata);
            };

            p.Start();

            if(p.StartInfo.RedirectStandardOutput) {
                p.BeginOutputReadLine();
            }

            if(p.StartInfo.RedirectStandardError) {
                p.BeginErrorReadLine();
            }

            if(!waiting) {
                return;
            }
            wait(p, cmd, timeout);
        }

        public void useShell(string cmd, bool waiting, bool hidden, int timeout = 0)
        {
            useShell(cmd, STDS, waiting, hidden, timeout);
        }

        internal static string Stdout(Guid id)
        {
            try {
                return StreamContainer(id).stdout;
            }
            finally {
                StreamContainer(id).stdout = null; //TODO: for clients by uid etc.
            }
        }

        internal static string Stderr(Guid id)
        {
            try {
                return StreamContainer(id).stderr;
            }
            finally {
                StreamContainer(id).stderr = null;
            }
        }

        /// <param name="directory">Initial directory for processes</param>
        public HProcess(string directory)
        {
            initDir = directory;
        }

        public HProcess()
        {
            initDir = Settings.WPath;
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

            p.StartInfo.Arguments           = args;
            p.StartInfo.UseShellExecute     = false;
            p.StartInfo.WorkingDirectory    = initDir;

            if(!hidden) {
                p.StartInfo.WindowStyle     = ProcessWindowStyle.Normal;
                p.StartInfo.CreateNoWindow  = false;
                return p;
            }

            p.StartInfo.WindowStyle             = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow          = true;
            p.StartInfo.RedirectStandardOutput  = true;
            p.StartInfo.RedirectStandardError   = true;
            p.StartInfo.RedirectStandardInput   = true;
            p.StartInfo.StandardErrorEncoding   = 
            p.StartInfo.StandardOutputEncoding  = 
                                                EncodingOEM;  // by default, and we must to detect a charset for received data later
            return p;
        }

        /// <summary>
        /// Destroys process and all its child processes by the pid of parent.
        /// </summary>
        /// <param name="pid"></param>
        protected void killProcessesFor(int pid)
        {
            // The Win32_Process WMI class represents a process on an operating system. :: https://msdn.microsoft.com/en-us/library/aa394372%28v=vs.85%29.aspx
            ManagementObjectCollection moProcesses = (new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + pid)).Get();
            foreach(ManagementObject moProcess in moProcesses) {
                killProcessesFor(Convert.ToInt32(moProcess["ProcessId"]));
            }

            Process p = Process.GetProcessById(pid);
            if(!p.HasExited) {
                Log.Trace("sigkill: -> {0}", pid);
                p.Kill();
            }
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
                Log.Debug("reEncodeString: Problem with detection... use the original");
                return str;
            }
            Log.Debug("reEncodeString: charset '{0}' confidence: '{1}'", cdet.Charset, cdet.Confidence);

            if(cdet.Confidence < 0.92f) {
                Log.Debug("reEncodeString: Confidence < 0.92 /use the original");
                return str;
            }

            Encoding to = Encoding.GetEncoding(cdet.Charset);
            Log.Debug("reEncodeString: '{0}' -> '{1}'", from.EncodingName, to.EncodingName);
            Log.Trace("reEncodeString: original - '{0}'", str);
            return to.GetString(bytes);
        }

        protected string reEncodeString(string str)
        {
            if(String.IsNullOrWhiteSpace(str)) {
                return String.Empty;
            }
            return reEncodeString(str, EncodingOEM);
        }

        private static TStream StreamContainer(Guid id)
        {
            if(!stdstream.ContainsKey(id) || stdstream[id] == null) {
                stdstream[id] = new TStream();
            }
            return stdstream[id];
        }

        private void wait(Process p, string cmd, int timeout)
        {
            if(p.HasExited) {
                return;
            }

            Log.Trace($"HProcess: time limit is '{timeout}'sec");
            if(timeout == 0) {
                p.WaitForExit();
                return;
            }

            p.WaitForExit(timeout * 1000);
            if(!p.HasExited) {
                Log.Warn($"sigkill for `{cmd}`: limit in {timeout}sec for time is reached. Use any other value or 0 for unlimited time.");
                killProcessesFor(p.Id);
            }
        }
    }
}
