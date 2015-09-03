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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using net.r_eg.vsSBE.Bridge;
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
        public ISBEScript SBEScript
        {
            get;
            protected set;
        }

        /// <summary>
        /// Work with MSBuild
        /// </summary>
        public IMSBuild MSBuild
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used environment
        /// </summary>
        public IEnvironment Env
        {
            get;
            protected set;
        }

        /// <summary>
        /// Specified Event type
        /// </summary>
        public SolutionEventType EventType
        {
            get { return type; }
        }
        protected SolutionEventType type = SolutionEventType.General;

        /// <summary>
        /// Predefined actions.
        /// </summary>
        protected volatile Dictionary<ModeType, IAction> actions = new Dictionary<ModeType, IAction>();


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
            if(evt.BuildType != BuildType.Common && evt.BuildType != Env.BuildType) {
                Log.Debug("Ignored context. Build type '{0}' should be '{1}'", Env.BuildType, evt.BuildType);
                return false;
            }
            this.type = type;

            string cfg = Env.SolutionActiveCfgString;

            if(evt.ToConfiguration != null 
                && evt.ToConfiguration.Length > 0 && evt.ToConfiguration.Where(s => s == cfg).Count() < 1)
            {
                Log.Info("Action '{0}' is ignored for current configuration - '{1}'", evt.Caption, cfg);
                return false;
            }

            if(!confirm(evt)) {
                Log.Debug("Skipped action by user");
                return false;
            }

            Log.Info("Launching action '{0}' :: Configuration - '{1}'", evt.Caption, cfg);
            return actionBy(evt);
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

        /// <param name="env">Used environment</param>
        /// <param name="script">Used SBE-Scripts</param>
        /// <param name="msbuild">Used MSBuild</param>
        public Command(IEnvironment env, ISBEScript script, IMSBuild msbuild)
        {
            Env         = env;
            SBEScript   = script;
            MSBuild     = msbuild;

            actions[ModeType.Operation]     = new ActionOperation(this);
            actions[ModeType.Interpreter]   = new ActionInterpreter(this);
            actions[ModeType.Script]        = new ActionScript(this);
            actions[ModeType.File]          = new ActionFile(this);
            actions[ModeType.Targets]       = new ActionTargets(this);
            actions[ModeType.CSharp]        = new ActionCSharp(this);
        }

        protected bool actionBy(ISolutionEvent evt)
        {
            switch(evt.Mode.Type)
            {
                case ModeType.Operation: {
                    Log.Info("Use Operation Mode");
                    return actionBy(ModeType.Operation, evt);
                }
                case ModeType.Interpreter: {
                    Log.Info("Use Interpreter Mode");
                    return actionBy(ModeType.Interpreter, evt);
                }
                case ModeType.Script: {
                    Log.Info("Use Script Mode");
                    return actionBy(ModeType.Script, evt);
                }
                case ModeType.Targets: {
                    Log.Info("Use Targets Mode");
                    return actionBy(ModeType.Targets, evt);
                }
                case ModeType.CSharp: {
                    Log.Info("Use C# Mode");
                    return actionBy(ModeType.CSharp, evt);
                }
            }
            Log.Info("Use Files Mode");
            return actionBy(ModeType.File, evt);
        }

        protected bool actionBy(ModeType type, ISolutionEvent evt)
        {
            if(evt.Process.Waiting) {
                return actions[type].process(evt);
            }
            
            string marker = null;
            if(Thread.CurrentThread.Name == Events.LoggingEvent.IDENT_TH) {
                marker = Events.LoggingEvent.IDENT_TH;
            }

            (new Task(() => {

                if(marker != null && Thread.CurrentThread.Name != marker) {
                    Thread.CurrentThread.Name = marker;
                }
                Log.Trace("Task for another thread is started for '{0}' /{1}", evt.Name, type);
                actions[type].process(evt);

            })).Start();

            return true;
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
            Log.Debug("Ask user about action [{0}]:{1} '{2}'", type, evt.Name, evt.Caption);

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
    }
}
