/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Linq;
using net.r_eg.MvsSln.Log;
using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;

namespace net.r_eg.vsSBE.Logger
{
    /// <summary>
    /// Initialize logger with bug fixes from other assemblies if exists:
    /// *  1. if another used the NLog.Config.SimpleConfigurator.
    /// *  2. if target from another has been configured as "*"
    /// </summary>
    public class Initializer
    {
        /// <summary>
        /// Entry point for messages.
        /// </summary>
        protected MethodCallTarget target;

        /// <summary>
        /// Filter for disabling logger
        /// </summary>
        protected ConditionBasedFilter selfLogger = new ConditionBasedFilter() {
                                                            Condition   = String.Format("(logger=='{0}')", GuidList.PACKAGE_LOGGER), 
                                                            Action      = FilterResult.Ignore,
                                                        };

        private readonly object sync = new object();

        /// <summary>
        /// Configure logger by default.
        /// </summary>
        public void configure()
        {
            var t = new MethodCallTarget() {
                ClassName   = typeof(Log).AssemblyQualifiedName,
                MethodName  = "nprint"
            };

            t.Parameters.Add(new MethodCallParameter("${level:uppercase=true}"));
            t.Parameters.Add(new MethodCallParameter("${message}"));
            t.Parameters.Add(new MethodCallParameter("${ticks}"));

            configure(t);
        }

        /// <summary>
        /// Configure logger by MethodCallTarget
        /// </summary>
        /// <param name="target"></param>
        public void configure(MethodCallTarget target)
        {
            this.target = target;

            lock(sync)
            {
                LogManager.ConfigurationChanged -= onCfgLoggerChanged;
                LogManager.ConfigurationChanged += onCfgLoggerChanged;

                initLoggerCfg();

                LSender.SReceived -= onSReceived;
                LSender.SReceived += onSReceived;

                Components.LSender.Sent -= onLSenderSent;
                Components.LSender.Sent += onLSenderSent;
            }
            
            Log.Trace($"Log('{target.ClassName}') is configured for: '{GuidList.PACKAGE_LOGGER}'");
        }

        /// <summary>
        /// To disable our logger for other assemblies.
        /// </summary>
        /// <param name="cfg">Configuration of logger</param>
        protected void fixLoggerCfg(LoggingConfiguration cfg)
        {
            fixLoggerCfg(cfg, selfLogger);
        }

        /// <summary>
        /// To disable logger for other assemblies.
        /// </summary>
        /// <param name="cfg">Configuration of logger</param>
        /// <param name="filter">Custom filter</param>
        protected void fixLoggerCfg(LoggingConfiguration cfg, ConditionBasedFilter filter)
        {
            if(cfg == null) {
                return;
            }

            LoggingRule rule = cfg.LoggingRules.FirstOrDefault(p => p.LoggerNamePattern == "*");
            if(rule == null) {
                return;
            }

            if(!rule.Filters.Contains(filter)) {
                rule.Filters.Add(filter); 
            }
        }

        /// <summary>
        /// Initialize logger
        /// </summary>
        private void initLoggerCfg()
        {
            LoggingConfiguration config = LogManager.Configuration;
            if(config == null) {
                config = new LoggingConfiguration();
            }

            NLog.Targets.Target t = config.FindTargetByName(GuidList.PACKAGE_LOGGER);
            if(t != null) {
                return; // the config is already contains our logger
            }

            // configure entry point

            config.AddTarget(GuidList.PACKAGE_LOGGER, target);
            config.LoggingRules.Add(new LoggingRule(GuidList.PACKAGE_LOGGER, LogLevel.Trace, target));

            // update configuration
            
            LogManager.Configuration = config;
            LogManager.Configuration.Reload();
        }

        /// <summary>
        /// Protects from NLog.Config.SimpleConfigurator from others assemblies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onCfgLoggerChanged(object sender, LoggingConfigurationChangedEventArgs e)
        {
            fixLoggerCfg(e.ActivatedConfiguration /*NLog PR#1897: OldConfiguration*/); // if we're at the point after first initialization
            fixLoggerCfg(e.DeactivatedConfiguration /*NLog PR#1897: NewConfiguration*/); // if this was raised from others

            initLoggerCfg(); // we also should be ready to SimpleConfigurator from other assemblies etc.
        }

        private void onSReceived(object sender, Message e)
        {
            Log._.NLog.Log
            (
                LogLevel.FromOrdinal
                (
                    (int)(e.type == Message.Level.Info ? Message.Level.Debug : e.type)
                ), 
                $"{nameof(MvsSln)}: {e.content}"
            );
        }

        private void onLSenderSent(object sender, Components.MsgArgs e)
        {
            if(!e.At(Settings.APP_NAME)) {
                return;
            }

            Log._.NLog.Log
            (
                LogLevel.FromOrdinal
                (
                    (int)(e.level == Components.MsgLevel.Info ? Components.MsgLevel.Debug : e.level)
                ),
                $"{sender.GetType().Name}: {e.content}"
            );
        }
    }
}
