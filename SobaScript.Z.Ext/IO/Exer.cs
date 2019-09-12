/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Ext contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using net.r_eg.Components;
using net.r_eg.SobaScript.Z.Ext.Extensions;

namespace net.r_eg.SobaScript.Z.Ext.IO
{
    public class Exer: IExer
    {
        /// <summary>
        /// Unspecified identifier for streams.
        /// </summary>
        public readonly Guid STDS = new Guid("{97FC36E2-2751-4CCC-A2EC-80FFB5FAF3C4}");

        protected IEncDetector detector;
        private string _basePath;

        /// <summary>
        /// Initial directory for processes.
        /// </summary>
        public string BasePath
        {
            get => _basePath;
            set => _basePath = value.FormatDirPath();
        }

        /// <summary>
        /// The current OEM code page from system locale
        /// </summary>
        protected int OEMCodePage => CultureInfo.GetCultureInfo(NativeMethods.GetSystemDefaultLCID()).TextInfo.OEMCodePage;

        /// <summary>
        /// OEM Encoding from system locale
        /// </summary>
        protected Encoding EncodingOEM => Encoding.GetEncoding(OEMCodePage);

        /// <summary>
        /// Container for standard stream data.
        /// TODO:
        /// </summary>
        private ConcurrentDictionary<Guid, TDataStream> streams = new ConcurrentDictionary<Guid, TDataStream>();

        /// <summary>
        /// Execute file with arguments. 
        /// Uses synchronous read operations.
        /// </summary>
        /// <param name="file">File to execute.</param>
        /// <param name="args">Arguments to file.</param>
        /// <param name="hidden">Hide process if true.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        /// <returns>stdout</returns>
        /// <exception cref="ExternalException"></exception>
        public string Run(string file, string args, bool hidden, int timeout = 0)
        {
            // TODO: Serial data for stdin.

            Process p = PrepareProcessFor(file, args, hidden);
            p.Start();
            Wait(p, file, timeout);

            if(!p.StartInfo.RedirectStandardError) {
                return string.Empty;
            }

            string errors = p.StandardError.ReadToEnd();
            if(errors.Length > 0) {
                throw new ExternalException(ReEncodeString(errors));
            }

            string ret = ReEncodeString(p.StandardOutput.ReadToEnd());

            if(!string.IsNullOrEmpty(ret)) {
                ret = ret.TrimEnd(new[] { '\r', '\n' });
            }
            return ret;
        }

        /// <summary>
        /// Execute command with standard command-line interpreter.
        /// Uses asynchronous read operations.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="uid">Unique id for streams.</param>
        /// <param name="waiting">Waiting for completion.</param>
        /// <param name="hidden">Hiding result.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        public void UseShell(string cmd, Guid uid, bool waiting, bool hidden, int timeout = 0)
        {
            Process p = PrepareProcessFor("cmd", $"/C \"{cmd}\"", hidden);

            p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if(string.IsNullOrEmpty(e.Data)) {
                    return;
                }

                string sdata = ReEncodeString(e.Data);
                LSender.Send(this, $"stderr({sdata.Length}): {sdata}", MsgLevel.Warn);

                GetStreamContainer(uid).stderr += sdata;
            };

            p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
            {
                if(string.IsNullOrEmpty(e.Data)) {
                    return;
                }

                string sdata = ReEncodeString(e.Data);
                LSender.Send(this, $"stdout({sdata.Length}): {sdata}", MsgLevel.Info);

                GetStreamContainer(uid).stdout += sdata;
            };

            GetStreamContainer(uid).Flush();
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
            Wait(p, cmd, timeout);
        }

        /// <summary>
        /// Execute command with standard command-line interpreter.
        /// Uses asynchronous read operations.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="waiting">Waiting for completion.</param>
        /// <param name="hidden">Hiding result.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        public void UseShell(string cmd, bool waiting, bool hidden, int timeout = 0)
        {
            UseShell(cmd, STDS, waiting, hidden, timeout);
        }

        /// <summary>
        /// Pulls latest async received data from stdout stream.
        /// Each calling resets buffer.
        /// </summary>
        /// <param name="id">Identifier of stream.</param>
        /// <returns>Received data; or null if id is not found or buffer is not initialized.</returns>
        public string PullStdOut(Guid id)
        {
            var con = GetStreamContainer(id);
            if(con == null) {
                return null;
            }

            try {
                return con.stdout;
            }
            finally {
                con.stdout = null; //TODO: for clients by uid etc.
            }
        }

        /// <summary>
        /// Pulls latest async received data from stdout stream.
        /// Each calling resets buffer.
        /// </summary>
        /// <param name="id">Identifier of stream.</param>
        /// <returns>Received data; or null if id is not found or buffer is not initialized.</returns>
        public string PullStdErr(Guid id)
        {
            var con = GetStreamContainer(id);
            if(con == null) {
                return null;
            }

            try {
                return con.stderr;
            }
            finally {
                con.stderr = null;
            }
        }

        /// <param name="dir">Initial directory for processes.</param>
        /// <param name="detector"></param>
        public Exer(string dir, IEncDetector detector = null)
        {
            BasePath        = dir.FormatDirPath();
            this.detector   = detector;
        }

        protected virtual void KillProcesses(int pid)
        {
            // Destroys process and all its child processes by the pid of parent.

            // The Win32_Process WMI class represents a process on an operating system. :: https://msdn.microsoft.com/en-us/library/aa394372%28v=vs.85%29.aspx
            //var moProcesses = new ManagementObjectSearcher("SELECT ProcessId FROM Win32_Process WHERE ParentProcessId = " + pid).Get();
            //foreach(ManagementObject moProcess in moProcesses) {
            //    KillProcesses(Convert.ToInt32(moProcess["ProcessId"]));
            //}

            Process p = Process.GetProcessById(pid);
            if(!p.HasExited) {
                LSender.Send(this, $"sigkill: -> {pid}", MsgLevel.Trace);
                p.Kill();
            }
        }

        protected string ReEncodeString(string str, Encoding from)
            => detector?.FixEncoding(str, from, 0.92f) ?? str;

        /// <param name="file">Application to start</param>
        /// <param name="args">Arguments to use when starting the application</param>
        /// <param name="hidden">Silent mode</param>
        /// <returns>Prepared instance</returns>
        protected Process PrepareProcessFor(string file, string args, bool hidden)
        {
            var p = new Process();
            p.StartInfo.FileName = file;

            p.StartInfo.Arguments           = args;
            p.StartInfo.UseShellExecute     = false;
            p.StartInfo.WorkingDirectory    = BasePath;

            if(!hidden)
            {
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

        protected string ReEncodeString(string str)
        {
            if(string.IsNullOrWhiteSpace(str)) {
                return string.Empty;
            }
            return ReEncodeString(str, EncodingOEM);
        }

        private TDataStream GetStreamContainer(Guid id)
        {
            if(!streams.ContainsKey(id) || streams[id] == null) {
                streams[id] = new TDataStream(id);
            }
            return streams[id];
        }

        private void Wait(Process p, string cmd, int timeout)
        {
            if(p.HasExited) {
                return;
            }

            LSender.Send(this, $"time limit is '{timeout}'sec", MsgLevel.Trace);
            if(timeout == 0)
            {
                p.WaitForExit();
                return;
            }

            p.WaitForExit(timeout * 1000);
            if(!p.HasExited)
            {
                LSender.Send(this, $"sigkill for `{cmd}`: limit in {timeout}sec for time is reached. Use any other value or 0 for unlimited time.", MsgLevel.Warn);
                KillProcesses(p.Id);
            }
        }
    }
}
