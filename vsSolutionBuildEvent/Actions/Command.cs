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
using System.Linq;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.Actions
{
    public class Command: ICommand
    {
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
        protected IEnvironment env;

        /// <summary>
        /// For additional handling
        /// </summary>
        protected SolutionEventType type = SolutionEventType.General;

        /// <summary>
        /// Type of build action
        /// </summary>
        protected BuildType buildType;

        /// <summary>
        /// Entry point for execution
        /// </summary>
        /// <param name="evt">Configured event</param>
        /// <param name="type">Type of event</param>
        /// <returns>true value if has been processed</returns>
        public bool exec(ISolutionEvent evt, SolutionEventType type)
        {
            if(!evt.Enabled){
                return false;
            }
            if(evt.BuildType != BuildType.Common && evt.BuildType != buildType) {
                Log.nlog.Debug("Ignored context. Build type '{0}'", buildType);
                return false;
            }
            this.type = type;

            string cfg = env.SolutionActiveCfgString;

            if(evt.ToConfiguration != null 
                && evt.ToConfiguration.Length > 0 && evt.ToConfiguration.Where(s => s == cfg).Count() < 1)
            {
                Log.nlog.Info("Action '{0}' is ignored for current configuration - '{1}'", evt.Caption, cfg);
                return false;
            }

            if(!confirm(evt)) {
                Log.nlog.Debug("Skipped action by user");
                return false;
            }

            Log.nlog.Info("Launching action '{0}' :: Configuration - '{1}'", evt.Caption, cfg);
            switch(evt.Mode.Type)
            {
                case ModeType.Operation: {
                    Log.nlog.Info("Use Operation Mode");
                    return hModeOperation(evt);
                }
                case ModeType.Interpreter: {
                    Log.nlog.Info("Use Interpreter Mode");
                    return hModeInterpreter(evt);
                }
                case ModeType.Script: {
                    Log.nlog.Info("Use Script Mode");
                    return hModeScript(evt);
                }
            }
            Log.nlog.Info("Use File Mode");
            return hModeFile(evt);
        }

        /// <summary>
        /// Entry point for execution
        /// </summary>
        /// <param name="evt">Configured event</param>
        /// <returns>true value if has been processed</returns>
        public bool exec(ISolutionEvent evt)
        {
            return exec(evt, SolutionEventType.General);
        }

        /// <summary>
        /// Updating context with the BuildType
        /// </summary>
        /// <param name="buildType">Type of build action</param>
        public void updateContext(BuildType buildType)
        {
            this.buildType = buildType;
        }

        /// <param name="env">Used environment</param>
        /// <param name="script">Used SBE-Scripts</param>
        /// <param name="msbuild">Used MSBuild</param>
        public Command(IEnvironment env, ISBEScript script, IMSBuild msbuild)
        {
            this.env        = env;
            this.script     = script;
            this.msbuild    = msbuild;
        }

        protected bool hModeFile(ISolutionEvent evt)
        {
            string cFiles = ((IModeFile)evt.Mode).Command;

            parse(evt, ref cFiles);
            useShell(evt, treatNewlineAs(" & ", cFiles));

            return true;
        }

        protected bool hModeOperation(ISolutionEvent evt)
        {
            IModeOperation operation = (IModeOperation)evt.Mode;
            if(operation.Command == null || operation.Command.Length < 1) {
                return true;
            }
            (new DTEOperation(env, type)).exec(operation.Command, operation.AbortOnFirstError);
            return true;
        }

        protected bool hModeInterpreter(ISolutionEvent evt)
        {
            if(((IModeInterpreter)evt.Mode).Handler.Trim().Length < 1) {
                throw new NotFoundException("Interpreter: Handler is empty or not selected.");
            }

            string script   = ((IModeInterpreter)evt.Mode).Command;
            string wrapper  = ((IModeInterpreter)evt.Mode).Wrapper;

            parse(evt, ref script);
            script = treatNewlineAs(((IModeInterpreter)evt.Mode).Newline, script);

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
            useShell(evt, string.Format("{0} {1}", ((IModeInterpreter)evt.Mode).Handler, script));
            return true;
        }

        protected bool hModeScript(ISolutionEvent evt)
        {
            string script = ((IModeScript)evt.Mode).Command;
            parse(evt, ref script);
            return true;
        }

        protected virtual void useShell(ISolutionEvent evt, string cmd)
        {
            Log.nlog.Info("Prepared command: '{0}'",  cmd);

            HProcess p = new HProcess(Settings.WorkPath);
            p.useShell(cmd, evt.Process.Waiting, evt.Process.Hidden);
        }

        protected virtual void parse(ISolutionEvent evt, ref string data)
        {
            if(evt.SupportSBEScripts) {
                data = script.parse(data, evt.SupportMSBuild);
            }

            if(evt.SupportMSBuild) {
                data = msbuild.parse(data);
            }
        }

        /// <summary>
        /// Supports the user interaction.
        /// Waiting until user presses yes/no or cancel
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>true value if need to execute</returns>
        protected bool confirm(ISolutionEvent evt)
        {
            if(!evt.Confirmation) {
                return true;
            }
            Log.nlog.Debug("Ask the user about action [{0}]:{1} '{2}'", type, evt.Name, evt.Caption);

            string msg = String.Format("Execute the next action ?\n  [{0}]:{1} '{2}'\n\n* Cancel - to disable current action", 
                                        type, evt.Name, evt.Caption);

            System.Windows.Forms.DialogResult ret = System.Windows.Forms.MessageBox.Show(msg,
                                                                                        "Confirm the action", 
                                                                                        System.Windows.Forms.MessageBoxButtons.YesNoCancel, 
                                                                                        System.Windows.Forms.MessageBoxIcon.Question);

            switch(ret) {
                case System.Windows.Forms.DialogResult.Yes: {
                    return true;
                }
                case System.Windows.Forms.DialogResult.Cancel: {
                    evt.Enabled = false;
                    Config._.save();
                    throw new SBEException("Aborted by user");
                }
            }
            return false;
        }

        protected string treatNewlineAs(string str, string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            return data.Trim(new char[]{'\r', '\n'}).Replace("\r", "").Replace("\n", str);
        }
    }
}
