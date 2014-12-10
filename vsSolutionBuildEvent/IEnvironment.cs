/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
        /// Provides projects from EnvDTE
        /// </summary>
        IEnumerable<EnvDTE.Project> DTEProjects { get; }

        /// <summary>
        /// Simple list of names from EnvDTE projects
        /// </summary>
        List<string> DTEProjectsList { get; }

        /// <summary>
        /// Should provide the Build.Evaluation.Project by project name
        /// </summary>
        Microsoft.Build.Evaluation.Project getProject(string project);

        /// <summary>
        /// Should provide active configuration for current solution
        /// </summary>
        EnvDTE80.SolutionConfiguration2 SolutionActiveConfiguration { get; }

        /// <summary>
        /// Should provide all configurations for current solution
        /// </summary>
        IEnumerable<EnvDTE80.SolutionConfiguration2> SolutionConfigurations { get; }

        /// <summary>
        /// Gets configuration for specific format
        /// e.g.: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        string SolutionConfigurationFormat(EnvDTE80.SolutionConfiguration2 cfg);

        /// <summary>
        /// Name from "Set as SturtUp Project"
        /// </summary>
        string StartupProjectString { get; }

        /// <summary>
        /// Provide global property for all existing projects
        /// </summary>
        /// <param name="name">Property name</param>
        string getSolutionGlobalProperty(string name);

        /// <summary>
        /// DTE context
        /// </summary>
        EnvDTE80.DTE2 Dte2 { get; }
    }
}
