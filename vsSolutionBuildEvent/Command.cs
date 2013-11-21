/*
    * The MIT License (MIT)
    * 
    * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace reg.ext.vsSolutionBuildEvent
{
    class SBECommand
    {
        /// <summary>
        /// working directory for commands
        /// </summary>
        private SBEContext _context = null;

        public bool basic(ISolutionEvent evt)
        {
            if(!evt.enabled){
                return false;
            }

            if(evt.modeScript){
                return hModeScript(evt);
            }
            return hModeFile(evt);
        }

        public SBECommand()
        {
            string path = Config.getWorkPath();
            _context    = new SBEContext(path, _letDisk(path));
        }

        protected bool hModeFile(ISolutionEvent evt)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            if(evt.processHide){
                psi.WindowStyle = ProcessWindowStyle.Hidden;
            }

            //TODO: [optional] capture message...

            string args = string.Format(
                "/C cd {0}{1} & {2}",
                _context.path, 
                (_context.disk != null) ? " & " + _context.disk + ":" : "",
                _treatNewlineAs(" & ", _modifySlash(evt.command)));

            if(!evt.processHide && evt.processKeep){
                args += " & pause";
            }

            psi.Arguments       = args;
            Process process     = new Process();
            process.StartInfo   = psi;
            process.Start();

            if(evt.waitForExit){
                process.WaitForExit(); //TODO: !replace it on handling build
            }
            return true;
        }

        //TODO:
        protected bool hModeScript(ISolutionEvent evt)
        {
            if(evt.interpreter.Trim().Length < 1){
                return false;
            }
            //new ProcessStartInfo(evt.interpreter);

            string script = evt.command;

            script = _treatNewlineAs(evt.newline, script);

            if(evt.wrapper.Length > 0){
                script = evt.wrapper + script.Replace(evt.wrapper, "\\" + evt.wrapper) + evt.wrapper;
            }

            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            if(evt.processHide) {
                psi.WindowStyle = ProcessWindowStyle.Hidden;
            }

            string args = string.Format("/C cd {0}{1} & {2} {3}",
                                        _context.path,
                                        (_context.disk != null) ? " & " + _context.disk + ":" : "",
                                        evt.interpreter, //TODO: optional manually..
                                        script);

            if(!evt.processHide && evt.processKeep) {
                args += " & pause";
            }

            Debug.WriteLine(args);

            psi.Arguments       = args;
            Process process     = new Process();
            process.StartInfo   = psi;
            process.Start();

            //TODO: [optional] capture message...

            if(evt.waitForExit) {
                process.WaitForExit(); //TODO: !replace it on handling build
            }
            return true;
        }

        private string _modifySlash(string data)
        {
            return data.Replace("/", "\\");
        }

        private string _treatNewlineAs(string str, string data)
        {
            return data.Replace("\r", "").Replace("\n", str);
        }

        private static string _letDisk(string path)
        {
            if(path.Length < 1){
                return null;
            }
            return path.Substring(0, 1);
        }
    }

    class SBEContext
    {
        public string path;
        public string disk;

        public SBEContext(string path, string disk)
        {
            this.path = path;
            this.disk = disk;
        }
    }
}
