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
using net.r_eg.vsSBE.Configuration;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;
using UserData = net.r_eg.vsSBE.Configuration.User.Data;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
    /// <summary>
    /// Logic for restoring settings.
    /// </summary>
    public sealed class RestoreData
    {
        /// <summary>
        /// Get Configuration data for selected context.
        /// </summary>
        /// <param name="context">Context for using.</param>
        /// <returns></returns>
        public ISolutionEvents getConfig(ContextType context)
        {
            if(configs.ContainsKey(context)) {
                return configs[context];
            }
            return new SolutionEvents();
        }

        /// <summary>
        /// Get User-Configuration data for selected context.
        /// </summary>
        /// <param name="context">Context for using.</param>
        /// <returns></returns>
        public IUserData getUserConfig(ContextType context)
        {
            if(userConfigs.ContainsKey(context)) {
                return userConfigs[context];
            }
            return new UserData();
        }

        /// <summary>
        /// Update Configuration data for specific context.
        /// </summary>
        /// <param name="data">Configuration data.</param>
        /// <param name="context">Specific context.</param>
        public void update(ISolutionEvents data, ContextType context)
        {
            configs[context] = data;
        }

        /// <summary>
        /// Update User-Configuration data for specific context.
        /// </summary>
        /// <param name="data">Configuration data.</param>
        /// <param name="context">Specific context.</param>
        public void update(IUserData data, ContextType context)
        {
            userConfigs[context] = data;
        }

        /// <summary>
        /// All instances of Config.
        /// </summary>
        private Dictionary<ContextType, ISolutionEvents> configs = new Dictionary<ContextType, ISolutionEvents>();

        /// <summary>
        /// All instances of UserConfig.
        /// </summary>
        private Dictionary<ContextType, IUserData> userConfigs = new Dictionary<ContextType, IUserData>();
    }
}