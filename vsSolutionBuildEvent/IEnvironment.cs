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

using System.Collections.Generic;

namespace net.r_eg.vsSBE
{
    public interface IEnvironment
    {
        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        List<string> ProjectsList { get; }

        /// <summary>
        /// Should provide active configuration for current solution
        /// </summary>
        EnvDTE80.SolutionConfiguration2 SolutionActiveCfg { get; }

        /// <summary>
        /// Formatted string with active configuration for current solution
        /// </summary>
        string SolutionActiveCfgString { get; }

        /// <summary>
        /// Should provide all configurations for current solution
        /// </summary>
        IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations { get; }

        /// <summary>
        /// Name from "Set as StartUp Project"
        /// </summary>
        string StartupProjectString { get; }

        /// <summary>
        /// Events in the extensibility model
        /// </summary>
        EnvDTE.Events Events { get; }

        /// <summary>
        /// Solution path from the DTE-context
        /// </summary>
        string SolutionPath { get; }

        /// <summary>
        /// Contains all of the commands in the environment
        /// </summary>
        EnvDTE.Commands Commands { get; }

        /// <summary>
        /// Should provide instance of the Build.Evaluation.Project by project name
        /// </summary>
        Microsoft.Build.Evaluation.Project getProject(string name);

        /// <summary>
        /// Gets configuration for specific format
        /// e.g.: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        string SolutionCfgFormat(EnvDTE80.SolutionConfiguration2 cfg);

        /// <summary>
        /// Provide global property for all existing projects
        /// </summary>
        /// <param name="name">Property name</param>
        string getSolutionProperty(string name);

        /// <summary>
        /// Execution command with DTE
        /// </summary>
        /// <param name="name">Command name</param>
        /// <param name="args">Command arguments</param>
        void exec(string name, string args = "");
    }
}
