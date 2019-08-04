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

using System.Collections.Generic;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.MvsSln.Types;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE.Extensions
{
    public static class ProjectExtension
    {
        private static IRuleOfConfig cfgRule = new RuleOfConfig();

        public static IXProject GetXProject(this DProject prj, IXProjectEnv env, bool tryLoad = true)
        {
            if(prj == null || env == null) {
                return null;
            }

            var cfg = new ConfigItem(
                prj.GetActiveConfig(),
                prj.GetActivePlatform()
            );

            if(!tryLoad) {
                return env.XProjectByFile(prj.FullName, cfg, false);
            }

            // similar problems -MS Connect Issue #508628:
            // http://connect.microsoft.com/VisualStudio/feedback/details/508628/

            return env.XProjectByFile(prj.FullName, cfg, prj.GetProjectProperties(env));
        }

        public static string GetActiveConfig(this DProject dProject, bool useRule = true)
        {
            string ret = dProject?.ConfigurationManager?.ActiveConfiguration?.ConfigurationName;

            return useRule ? cfgRule.Configuration(ret) : ret;
        }

        public static string GetActivePlatform(this DProject dProject, bool useRule = true)
        {
            string ret = dProject?.ConfigurationManager?.ActiveConfiguration?.PlatformName;

            return useRule ? cfgRule.Platform(ret) : ret;
        }

        public static string GetProjectGuid(this DProject dProject)
        {
            return dProject?.GetIVsHierarchy().GetProjectGuid().SlnFormat();
        }

        public static MvsSln.Core.ProjectItem GetProjectItem(this DProject prj, IXProjectEnv env)
        {
            return new MvsSln.Core.ProjectItem
            (
                prj.GetProjectGuid(), 
                prj.GetProjectName(env), 
                FileExt.GetProjectTypeByFile(prj.FullName)
            )
            {
                fullPath = prj.FullName
            };
        }

        public static string GetProjectName(this DProject prj, IXProjectEnv env, bool tryLoad = true)
        {
            if(env == null)
            {
                return prj.Name; // can be as 'AppName' and 'AppName_2013' for different .sln

                // dteProject.Name - "Devenv_2013"
                // dteProject.UniqueName - "Devenv\\Devenv_2013.csproj"
            }

            return prj.GetXProject(env, tryLoad)?.ProjectName;
        }

        public static IDictionary<string, string> GetProjectProperties(this DProject prj, IXProjectEnv env = null)
        {
            var p = env == null ? new Dictionary<string, string>() : new Dictionary<string, string>(env.Sln.Properties)
            {
                [PropertyNames.CONFIG]      = prj.GetActiveConfig(),
                [PropertyNames.PLATFORM]    = prj.GetActivePlatform(),
                [PropertyNames.PRJ_GUID]    = prj.GetProjectGuid(),
                [PropertyNames.VS_BUILD]    = "true",
            };

            if(!p.ContainsKey(PropertyNames.CODE_ANAL_ORUN)) {
                p[PropertyNames.CODE_ANAL_ORUN] = "false";
            }

            if(!p.ContainsKey(PropertyNames.DEVENV_DIR)) {
                p[PropertyNames.DEVENV_DIR] = "".GetDevEnvDir();
            }

            return p;
        }

        public static DProject GetEProject(this EProject prj, IEnvironment env)
        {
            return env?.ProjectsDTE?.FirstOrDefault(p => p.FullName == prj.FullPath);
        }
    }
}
