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

using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.vsSBE.API.Commands;
using BuildType = net.r_eg.vsSBE.Bridge.BuildType;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE
{
    public abstract class EnvAbstract
    {
        protected IRuleOfConfig cfgRule = new RuleOfConfig();

        /// <summary>
        /// Current context for actions.
        /// </summary>
        public BuildType BuildType
        {
            get;
            set;
        } = BuildType.Common;

        /// <summary>
        /// Sender of the core commands.
        /// </summary>
        public IFireCoreCommand CoreCmdSender
        {
            get;
            set;
        }

        /// <summary>
        /// Returns formatted configuration from the SolutionConfiguration2
        /// </summary>
        public string SolutionCfgFormat(EnvDTE80.SolutionConfiguration2 cfg)
        {
            if(cfg == null) {
                return formatCfg(PropertyNames.UNDEFINED);
            }
            return formatCfg(cfg.Name, cfg.PlatformName);
        }

        /// <summary>
        /// Gets project name from Microsoft.Build.Evaluation.Project
        /// </summary>
        /// <param name="eProject"></param>
        /// <returns></returns>
        protected virtual string getProjectNameFrom(EProject eProject)
        {
            //NOTE: this property can also define an unified project name between various .sln files (_2010.sln, _2017.sln)
            return eProject.GetPropertyValue(PropertyNames.PRJ_NAME);
        }

        protected string formatCfg(string name, string platform = null)
        {
            return ConfigItem.Format(name, platform ?? name);
        }
    }
}
