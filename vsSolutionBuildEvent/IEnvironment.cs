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
using System.Runtime.InteropServices;
using net.r_eg.EvMSBuild;
using net.r_eg.vsSBE.API.Commands;
using net.r_eg.vsSBE.Bridge;
using DProject = EnvDTE.Project;
using EProject = Microsoft.Build.Evaluation.Project;

namespace net.r_eg.vsSBE
{
    [Guid("27F04A53-A0B9-431B-83FE-827AC09FB127")]
    public interface IEnvironment: IEvEnv
    {
        /// <summary>
        /// List of EnvDTE projects.
        /// </summary>
        IEnumerable<DProject> ProjectsDTE { get; }

        /// <summary>
        /// List of Microsoft.Build.Evaluation projects.
        /// </summary>
        IEnumerable<EProject> ProjectsMBE { get; }

        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        List<string> ProjectsList { get; }

        /// <summary>
        /// Active configuration for current solution.
        /// </summary>
        EnvDTE80.SolutionConfiguration2 SolutionActiveCfg { get; }

        /// <summary>
        /// Formatted string with an active configuration for current solution.
        /// </summary>
        string SolutionActiveCfgString { get; }

        /// <summary>
        /// Current context for actions.
        /// </summary>
        BuildType BuildType { get; set; }

        /// <summary>
        /// All configurations for current solution.
        /// </summary>
        IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations { get; }

        /// <summary>
        /// Project Name by default or "StartUp Project".
        /// </summary>
        string StartupProjectString { get; }

        /// <summary>
        /// DTE2 context.
        /// </summary>
        EnvDTE80.DTE2 Dte2 { get; }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        EnvDTE.Events Events { get; }

        /// <summary>
        /// Get status of opened solution.
        /// </summary>
        bool IsOpenedSolution { get; }

        /// <summary>
        /// Sender of the core commands.
        /// </summary>
        IFireCoreCommand CoreCmdSender { get; set; }

        /// <summary>
        /// Full path to directory where placed solution file.
        /// </summary>
        string SolutionPath { get; }

        /// <summary>
        /// Full path to solution file.
        /// </summary>
        string SolutionFile { get; }

        /// <summary>
        /// Name of used solution file without extension
        /// </summary>
        string SolutionFileName { get; }

        /// <summary>
        /// Contains all of the commands in the environment
        /// </summary>
        EnvDTE.Commands Commands { get; }

        /// <summary>
        /// Access to OutputWindowPane through IOW
        /// </summary>
        IOW OutputWindowPane { get; }

        /// <summary>
        /// Get instance of the Build.Evaluation.Project for accessing to properties etc.
        /// </summary>
        /// <param name="name">Specified project name. null value will use the name from startup-project.</param>
        /// <returns>Found relevant Microsoft.Build.Evaluation.Project.</returns>
        EProject getProject(string name = null);

        /// <summary>
        /// Returns formatted configuration from the SolutionConfiguration2
        /// </summary>
        string SolutionCfgFormat(EnvDTE80.SolutionConfiguration2 cfg);

        /// <summary>
        /// Getting an unified property for all existing projects. 
        /// Aka "Solution property".
        /// </summary>
        /// <param name="name">Property name</param>
        string getSolutionProperty(string name);

        /// <summary>
        /// Execution command with DTE
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="args">Command arguments</param>
        void exec(string name, string args = "");

        /// <summary>
        /// To update the Project Name by default aka "StartUp Project".
        /// </summary>
        /// <param name="name">Uses default behavior if empty or null.</param>
        void updateStartupProject(string name);
    }
}
