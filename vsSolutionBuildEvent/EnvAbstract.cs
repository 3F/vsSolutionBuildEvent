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
using System.IO;
using System.Linq;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;
using net.r_eg.MvsSln.Exceptions;
using net.r_eg.vsSBE.API.Commands;
using BuildType = net.r_eg.vsSBE.Bridge.BuildType;
using EProject = Microsoft.Build.Evaluation.Project;
using ProjectItem = net.r_eg.MvsSln.Core.ProjectItem;

namespace net.r_eg.vsSBE
{
    public abstract class EnvAbstract
    {
        /// <summary>
        /// Project by default or "StartUp Project".
        /// </summary>
        public abstract string StartupProjectString { get; protected set; }

        /// <summary>
        /// Full path to solution file.
        /// </summary>
        public abstract string SolutionFile { get; protected set; }

        protected abstract void UpdateSlnEnv(ISlnResult sln);

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
        /// Access to parsed solution data.
        /// </summary>
        protected ISlnResult Sln
        {
            get => UpdateSln();
            set => _sln = value;

        } protected ISlnResult _sln;

        /// <summary>
        /// Activated environment for projects processing.
        /// </summary>
        protected IXProjectEnv SlnEnv
        {
            get
            {
                UpdateSln();
                return _slnEnv;
            }
            set => _slnEnv = value;

        } protected IXProjectEnv _slnEnv;

        /// <summary>
        /// Get Project instance for work with data inside specified scope.
        /// </summary>
        /// <param name="ident">Abstract identifier of the specified scope. It can be a GUID, or FullPath, or project name, etc.</param>
        /// <returns>Expected the instance that is associated with the identifier or any default instance if not found any related to pushed ident.</returns>
        public EProject GetProject(object ident)
        {
            return getProject(ident?.ToString());
        }

        /// <summary>
        /// Get instance of the Build.Evaluation.Project for accessing to properties etc.
        /// </summary>
        /// <param name="name">Specified project name. null value will use the name from startup-project.</param>
        /// <returns>Found relevant Microsoft.Build.Evaluation.Project.</returns>
        public virtual EProject getProject(string name = null)
        {
            // NOTE: Do not use ProjectCollection.GlobalProjectCollection from EnvDTE Environment because it can be empty.
            //       https://github.com/3F/vsSolutionBuildEvent/issues/8
            //       Either use DTE projects collection to refer to MBE projects, or use MvsSln's GetOrLoadProject

            Log.Trace($"getProject: started with '{name}' /{StartupProjectString}");

            if(String.IsNullOrEmpty(name)) {
                name = StartupProjectString;
            }

            ProjectItem project = Sln.ProjectItems.FirstOrDefault(p => p.name == name);
            if(project.fullPath == null) {
                throw new NotFoundException($"Project '{name}' was not found. ['{project.name}', '{project.pGuid}']");
            }

            return SlnEnv?.GetOrLoadProject(project);
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

        protected string formatCfg(string name, string platform = null)
        {
            return ConfigItem.Format(name, platform ?? name);
        }

        private ISlnResult UpdateSln()
        {
            var input = SolutionFile;

            if(input == null) {
                throw new ArgumentNullException(nameof(SolutionFile));
            }

            if(_sln?.SolutionFile == input) {
                return _sln;
            }

            if(!File.Exists(input)) { // may not exist at this invoking when creating new solution)
                throw new NotFoundException($"Sln file does not exist: {input}.");
            }

            Log.Debug($"Updating sln data: {input}");

            _sln = new SlnParser().Parse
            (
                input,
                SlnItems.Projects | SlnItems.SolutionConfPlatforms | SlnItems.ProjectConfPlatforms
            );

            UpdateSlnEnv(_sln);
            return _sln;
        }
    }
}
