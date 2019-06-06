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
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Main container of the 'Solution Events'
    /// </summary>
    [Guid("552AA0E0-9EFC-4394-B18B-41CF2F549FB3")]
    public interface ISolutionEvent
    {
        /// <summary>
        /// Status of activation.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Unique name for identification.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// About event.
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Support of the MSBuild engine.
        /// </summary>
        bool SupportMSBuild { get; set; }

        /// <summary>
        /// Support of the SBE-Scripts engine.
        /// </summary>
        bool SupportSBEScripts { get; set; }

        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        bool IgnoreIfBuildFailed { get; set; }

        /// <summary>
        /// The context of action.
        /// </summary>
        BuildType BuildType { get; set; }

        /// <summary>
        /// User interaction.
        /// Waiting until user presses yes/no etc,
        /// </summary>
        bool Confirmation { get; set; }

        /// <summary>
        /// Run only for a specific configuration of solution
        /// strings format as:
        ///   'configname'|'platformname'
        ///   Compatible with: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        string[] ToConfiguration { get; set; }

        /// <summary>
        /// Run for selected projects with the Execution-Order
        /// </summary>
        IExecutionOrder[] ExecutionOrder { get; set; }

        /// <summary>
        /// Handling process
        /// </summary>
        IEventProcess Process { get; set; }

        /// <summary>
        /// Processing mode
        /// </summary>
        IMode Mode { get; set; }

        /// <summary>
        /// Unique identifier at runtime.
        /// </summary>
        Guid Id { get; }
    }
}
