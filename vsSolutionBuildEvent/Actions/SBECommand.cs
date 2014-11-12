/*
 * Copyright (c) 2013  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.Actions
{
    public class SBECommand
    {
        const string CMD_DEFAULT = "cmd";

        public class ShellContext
        {
            public string path;
            public string disk;

            public ShellContext(string path)
            {
                this.path = path;
                this.disk = getDisk(path);
            }

            protected string getDisk(string path)
            {
                if(String.IsNullOrEmpty(path)) {
                    throw new SBEException("path is empty or null");
                }
                return path.Substring(0, 1);
            }
        }

        /// <summary>
        /// Work with SBE-Scripts
        /// </summary>
        protected ISBEScript script;

        /// <summary>
        /// Work with MSBuild
        /// </summary>
        protected IMSBuild msbuild;

        /// <summary>
        /// Used environment
        /// </summary>
        protected Environment env;

        /// <summary>
        /// For additional handling
        /// </summary>
        protected SolutionEventType type = SolutionEventType.General;

        /// <summary>
        /// Current working context for scripts or files
        /// </summary>
        protected ShellContext context;

        /// <summary>
        /// basic implementation
        /// </summary>
        /// <param name="evt">provided sbe-events</param>
        public bool basic(ISolutionEvent evt, SolutionEventType type)
        {
            if(!evt.Enabled){
                return false;
            }
            this.type = type;

            string cfg = env.SolutionConfigurationFormat(env.SolutionActiveConfiguration);

            if(evt.ToConfiguration != null 
                && evt.ToConfiguration.Length > 0 && evt.ToConfiguration.Where(s => s == cfg).Count() < 1)
            {
                Log.nlog.Info("Action '{0}' is ignored for current configuration - '{1}'", evt.Caption, cfg);
                return false;
            }

            Log.nlog.Info("Launching action '{0}' :: Configuration - '{1}'", evt.Caption, cfg);
            switch(evt.Mode.Type) {
                case ModeType.Operation: {
                    Log.nlog.Info("Use Operation Mode");
                    return hModeOperation(evt);
                }
                case ModeType.Interpreter: {
                    Log.nlog.Info("Use Interpreter Mode");
                    return hModeScript(evt);
                }
            }
            Log.nlog.Info("Use File Mode");
            return hModeFile(evt);
        }

        /// <param name="env">Used environment</param>
        /// <param name="script">Used SBE-Scripts</param>
        /// <param name="msbuild">Used MSBuild</param>
        public SBECommand(Environment env, ISBEScript script, IMSBuild msbuild)
        {
            this.env        = env;
            this.script     = script;
            this.msbuild    = msbuild;
        }

        public void updateContext(ShellContext context)
        {
            this.context = context;
        }

        protected virtual bool hModeFile(ISolutionEvent evt)
        {
            string cFiles = ((IModeFile)evt.Mode).Command;

            parseScript(evt, ref cFiles);
            useShell(evt, _treatNewlineAs(" & ", cFiles));

            return true;
        }

        protected virtual bool hModeOperation(ISolutionEvent evt)
        {
            IModeOperation operation = (IModeOperation)evt.Mode;
            if(operation.Command == null || operation.Command.Length < 1) {
                return true;
            }
            (new DTEOperation((EnvDTE.DTE)env.DTE2, type)).exec(operation.Command, operation.AbortOnFirstError);
            return true;
        }

        protected virtual bool hModeScript(ISolutionEvent evt)
        {
            string script   = ((IModeInterpreter)evt.Mode).Command;
            string wrapper  = ((IModeInterpreter)evt.Mode).Wrapper;

            parseScript(evt, ref script);
            script = _treatNewlineAs(((IModeInterpreter)evt.Mode).Newline, script);

            switch(wrapper.Length) {
                case 1: {
                    script = string.Format("{0}{1}{0}", wrapper, script.Replace(wrapper, "\\" + wrapper));
                    break;
                }
                case 2: {
                    //pair as: (), {}, [] ...
                    //e.g.: (echo str&echo.&echo str) >> out
                    string wL = wrapper.ElementAt(0).ToString();
                    string wR = wrapper.ElementAt(1).ToString();
                    script = string.Format("{0}{1}{2}", wL, script.Replace(wL, "\\" + wL).Replace(wR, "\\" + wR), wR);
                    break;
                }
            }

            if(((IModeInterpreter)evt.Mode).Handler.Trim().Length < 1) {
                Log.nlog.Debug("Interpreter: ignoring useShell. Handler is not selected.");
                return true;
            }
            useShell(evt, string.Format("{0} {1}", ((IModeInterpreter)evt.Mode).Handler, script));
            return true;
        }

        protected void useShell(ISolutionEvent evt, string cmd)
        {
            ProcessStartInfo psi = new ProcessStartInfo(CMD_DEFAULT);
            if(evt.Process.Hidden) {
                psi.WindowStyle = ProcessWindowStyle.Hidden;
            }
            //psi.StandardErrorEncoding = psi.StandardOutputEncoding = Encoding.GetEncoding(OEMCodePage);

            string args = String.Format("/C cd {0}{1} & {2}",
                                        context.path,
                                        (context.disk != null) ? " & " + context.disk + ":" : "", cmd);

            if(!evt.Process.Hidden && evt.Process.KeepWindow) {
                args += " & pause";
            }

            Log.nlog.Info(cmd);

            //TODO: stdout/stderr capture & add to OWP

            psi.Arguments       = args;
            Process process     = new Process();
            process.StartInfo   = psi;
            process.Start();

            if(evt.Process.Waiting) {
                process.WaitForExit();
            }
        }

        protected void parseScript(ISolutionEvent evt, ref string data)
        {
            if(evt.SupportSBEScripts) {
                data = script.parse(data, evt.SupportMSBuild);
            }

            if(evt.SupportMSBuild) {
                data = msbuild.parse(data);
            }
        }

        private string _treatNewlineAs(string str, string data)
        {
            return data.Trim(new char[]{'\r', '\n'}).Replace("\r", "").Replace("\n", str);
        }
    }
}
