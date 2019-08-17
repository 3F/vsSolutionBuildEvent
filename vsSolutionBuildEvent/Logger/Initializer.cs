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

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

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

            lock(_lock)
            {
                LogManager.ConfigurationChanged -= onCfgLoggerChanged;
                LogManager.ConfigurationChanged += onCfgLoggerChanged;

                initLoggerCfg();

                LSender.SReceived -= onSReceived;
                LSender.SReceived += onSReceived;
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
    }
}
