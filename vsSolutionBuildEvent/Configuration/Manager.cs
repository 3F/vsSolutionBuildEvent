/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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

using System.Collections.Generic;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE.Configuration
{
    internal class Manager: IManager
    {
        /// <summary>
        /// All instances of Config.
        /// </summary>
        protected Dictionary<ContextType, IConfig<ISolutionEvents>> configs = new Dictionary<ContextType, IConfig<ISolutionEvents>>();

        /// <summary>
        /// All instances of UserConfig.
        /// </summary>
        protected Dictionary<ContextType, IConfig<IUserData>> userConfigs = new Dictionary<ContextType, IConfig<IUserData>>();

        /// <summary>
        /// Settings of application.
        /// </summary>
        protected IAppSettings app;

        /// <summary>
        /// Get Config instance for used context.
        /// </summary>
        public IConfig<ISolutionEvents> Config
        {
            get
            {
                if(!configs.ContainsKey(Context)) {
                    return null;
                }
                return configs[Context];
            }
        }

        /// <summary>
        /// Get UserConfig instance for used context.
        /// </summary>
        public IConfig<IUserData> UserConfig
        {
            get
            {
                if(!userConfigs.ContainsKey(Context)) {
                    return null;
                }
                return userConfigs[Context];
            }
        }

        /// <summary>
        /// Current used context.
        /// </summary>
        public ContextType Context
        {
            get;
            protected set;
        }

        /// <summary>
        /// Switcher of used context.
        /// </summary>
        /// <param name="context">New context for using.</param>
        /// <returns>Self reference.</returns>
        public IManager switchOn(ContextType context)
        {
            if(this.Context != context) {
                Log.Trace("Config-Manager: switched context '{0}'", context.ToString());
            }
            this.Context = context;

            configure(UserConfig.Data);
            return this;
        }

        /// <summary>
        /// Checks existance of configuration for specific context.
        /// </summary>
        /// <param name="context">Context for checking.</param>
        /// <returns>true value if exists in selected context.</returns>
        public bool IsExistCfg(ContextType context)
        {
            return configs.ContainsKey(context);
        }

        /// <summary>
        /// Get Config instance for selected context without an switching.
        /// </summary>
        /// <param name="context">Context for using.</param>
        /// <returns></returns>
        public IConfig<ISolutionEvents> getConfigFor(ContextType context)
        {
            if(configs.ContainsKey(context)) {
                return configs[context];
            }
            return null;
        }

        /// <summary>
        /// Get UserConfig instance for selected context without an switching.
        /// </summary>
        /// <param name="context">Context for using.</param>
        /// <returns></returns>
        public IConfig<IUserData> getUserConfigFor(ContextType context)
        {
            if(userConfigs.ContainsKey(context)) {
                return userConfigs[context];
            }
            return null;
        }

        /// <summary>
        /// Add Config instance for specific context.
        /// </summary>
        /// <param name="cfg">Config instance.</param>
        /// <param name="context">Specific context.</param>
        /// <param name="force">Add with replacement if true.</param>
        /// <returns>true value if successfully added.</returns>
        public bool add(IConfig<ISolutionEvents> cfg, ContextType context, bool force = false)
        {
            if(!force && configs.ContainsKey(context)) {
                return false;
            }
            configs[context] = cfg;
            return true;
        }

        /// <summary>
        /// Add UserConfig instance for specific context.
        /// </summary>
        /// <param name="cfg">UserConfig instance.</param>
        /// <param name="context">Specific context.</param>
        /// <param name="force">Add with replacement if true.</param>
        /// <returns>true value if successfully added.</returns>
        public bool add(IConfig<IUserData> cfg, ContextType context, bool force = false)
        {
            if(!force && userConfigs.ContainsKey(context)) {
                return false;
            }
            userConfigs[context] = cfg;
            return true;
        }

        /// <summary>
        /// Add and use configurations with selected context.
        /// </summary>
        /// <param name="cfg">Config instance.</param>
        /// <param name="userCfg">UserConfig instance.</param>
        /// <param name="context">Context for switching.</param>
        public void addAndUse(IConfig<ISolutionEvents> cfg, IConfig<IUserData> userCfg, ContextType context)
        {
            add(cfg, context, true);
            add(userCfg, context, true);
            switchOn(context);
        }

        /// <summary>
        /// Unset Config instance from selected context.
        /// </summary>
        /// <param name="context">Selected context.</param>
        public void unsetConfig(ContextType context)
        {
            configs.Remove(context);
        }

        /// <summary>
        /// Unset UserConfig instance from selected context.
        /// </summary>
        /// <param name="context">Selected context.</param>
        public void unsetUserConfig(ContextType context)
        {
            userConfigs.Remove(context);
        }

        /// <summary>
        /// Unset all instance from selected context and switch on other.
        /// </summary>
        /// <param name="context">Selected context.</param>
        /// <param name="to">Context for switching.</param>
        public void unsetAndUse(ContextType context, ContextType to)
        {
            configs.Remove(context);
            userConfigs.Remove(context);
            switchOn(to);
        }

        /// <param name="app"></param>
        public Manager(IAppSettings app)
        {
            this.app = app;
        }

        /// <summary>
        /// Apply global configuration.
        /// </summary>
        /// <param name="cfg"></param>
        protected void configure(IUserData cfg)
        {
            if(cfg == null) {
                return;
            }
            app.DebugMode = cfg.Global.DebugMode;
        }
    }
}
