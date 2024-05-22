/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    public class Command: ICommand
    {
        /// <summary>
        /// Predefined actions.
        /// </summary>
        protected Dictionary<ModeType, IAction> actions = [];

        public ISobaScript SBEScript { get; protected set; }

        public IEvMSBuild MSBuild { get; protected set; }

        public IEnvironment Env { get; protected set; }

        public SolutionEventType EventType { get; protected set; } = SolutionEventType.General;

        /// <summary>
        /// Current context for actions.
        /// </summary>
        protected BuildType CurrentContext => Env.BuildType;

        public bool exec(ISolutionEvent evt, SolutionEventType type, bool force = false)
        {
            if(!force && !evt.Enabled) return false;
            EventType = type;

            if(!isContext(evt, type)) {
                Log.Debug($"Ignored Context '{CurrentContext}'. Expected '{evt.BuildType}'");
                return false;
            }

            string cfg = Env.SolutionActiveCfgString;

            if(evt.ToConfiguration != null 
                && evt.ToConfiguration.Length > 0 && !evt.ToConfiguration.Any(s => cmpConfig(s, cfg)))
            {
                Log.Info($"Ignore action '{evt.Caption}' for current configuration '{cfg}'");
                return false;
            }

            if(!confirm(evt)) {
                Log.Debug("Skipped action by user");
                return false;
            }

            Log.Info($"Run '{evt.Name}' due to '{type}' for '{cfg}' using {evt.Mode.Type} mode.");
            if(!string.IsNullOrWhiteSpace(evt.Caption)) {
                Log.Info(evt.Caption);
            }
            return actionBy(evt.Mode.Type, evt);
        }

        public bool exec(ISolutionEvent evt, bool force = false)
        {
            return exec(evt, SolutionEventType.General, force);
        }

        /// <param name="env">Used environment</param>
        /// <param name="script">Used SBE-Scripts</param>
        /// <param name="msbuild">Used MSBuild</param>
        public Command(IEnvironment env, ISobaScript script, IEvMSBuild msbuild)
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

        protected bool actionBy(ModeType type, ISolutionEvent evt)
        {
            if(!actions.ContainsKey(type))
            {
                Log.Warn($"{type} is not found as a registered action type");
                actions[type] = new ActionScript(this);
            }

            if(evt.Process.Waiting) {
                return actions[type].process(evt);
            }
            
            string marker = null;
            if(Thread.CurrentThread.Name == LoggingEvent.IDENT_TH) {
                marker = LoggingEvent.IDENT_TH;
            }

            (new Task(() => {

                if(Thread.CurrentThread.Name == null && marker != null) {
                    Thread.CurrentThread.Name = marker;
                }

                Log.Trace($"Task for '{evt.Name}' due to '{type}' ...");
                try {
                    actions[type].process(evt);
                }
                catch(Exception ex) {
                    Log.Error($"Failed task for '{evt.Name}': {ex.Message}");
                    Log.Debug(ex.StackTrace);
                }

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
            Log.Debug("Ask user about action [{0}]:{1} '{2}'", EventType, evt.Name, evt.Caption);

            System.Windows.Forms.DialogResult ret = System.Windows.Forms.MessageBox.Show
            (
                $"[{EventType}] '{evt.Name}'\n{evt.Caption}\n\n*Click [Cancel] to disable this action.",
                "Confirm the action. Execute?", 
                System.Windows.Forms.MessageBoxButtons.YesNoCancel, 
                System.Windows.Forms.MessageBoxIcon.Question
            );

            switch(ret)
            {
                case System.Windows.Forms.DialogResult.Yes: {
                    return true;
                }
                case System.Windows.Forms.DialogResult.Cancel: {
                    evt.Enabled = false;
                    Settings._.Config.Sln.save();
                    throw new UnspecSBEException("Aborted by user");
                }
            }
            return false;
        }

        protected bool isContext(ISolutionEvent evt, SolutionEventType type)
        {
            var cfgContext = evt.BuildType;

            // /LC: #799
            if(type == SolutionEventType.SlnOpened)
            {
                if(cfgContext == BuildType.Common) {
                    cfgContext = BuildType.Before; // consider it as default type
                }

                if(cfgContext != BuildType.Before 
                    && cfgContext != BuildType.After 
                    && cfgContext != BuildType.BeforeAndAfter)
                {
                    return false;
                }

                if(CurrentContext == BuildType.Common // Before & After are not possible at all, e.g. Isolated env etc., thus consider it as any possible
                    || cfgContext == BuildType.BeforeAndAfter)
                {
                    return true;
                }

                if(cfgContext == BuildType.Before || cfgContext == BuildType.After) {
                    return (cfgContext == CurrentContext);
                }
            }

            return (cfgContext == BuildType.Common || cfgContext == CurrentContext);
        }

        /// <summary>
        /// Compare configurations.
        /// 
        /// Compatible format: 'configname'|'platformname'
        /// http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// 
        /// note: both variants 'Any CPU' + 'AnyCPU' as an awesome features from MS - see also Connect Issue #503935.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="anycpuCheck">Special case for checking 'Any CPU' + 'AnyCPU' platform</param>
        /// <returns>same or not</returns>
        private bool cmpConfig(string left, string right, bool anycpuCheck = true)
        {
            if(left.Equals(right, StringComparison.OrdinalIgnoreCase)) {
                return true;
            }

            // 'Any CPU' & 'AnyCPU' platform
            if(anycpuCheck) {
                return cmpConfig(left.Replace(" ", ""), right.Replace(" ", ""), false);
            }
            return false;
        }
    }
}
