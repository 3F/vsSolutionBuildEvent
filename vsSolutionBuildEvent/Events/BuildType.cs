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

using System;
using EnvDTE;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Represents available types of the build for any actions
    /// </summary>
    public enum BuildType
    {
        /// <summary>
        /// A 'build' action is occurring.
        /// </summary>
        Build = vsBuildAction.vsBuildActionBuild,
        /// <summary>
        /// A 'rebuild all' action is occurring.
        /// </summary>
        RebuildAll = vsBuildAction.vsBuildActionRebuildAll,
        /// <summary>
        /// A 'clean' action is occurring.
        /// </summary>
        Clean = vsBuildAction.vsBuildActionClean,
        /// <summary>
        /// A 'deploy' action is occurring.
        /// </summary>
        Deploy = vsBuildAction.vsBuildActionDeploy,
        /// <summary>
        /// Common context
        /// </summary>
        Common = Int32.MaxValue
    }
}
