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

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Main container of the Solution Events
    /// </summary>
    public interface ISolutionEvent
    {
        /// <summary>
        /// Status of activation
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Optional, unique name for manually identification
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Short header about this
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Support of MSBuild environment variables (properties)
        /// </summary>
        bool SupportMSBuild { get; set; }

        /// <summary>
        /// Support of SBE-Scripts
        /// </summary>
        bool SupportSBEScripts { get; set; }

        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        bool IgnoreIfBuildFailed { get; set; }

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
    }

    /// <summary>
    /// Type of available processing modes
    /// </summary>
    public enum ModeType
    {
        /// <summary>
        /// external commands
        /// </summary>
        File,
        /// <summary>
        /// command script
        /// </summary>
        Interpreter,
        /// <summary>
        /// DTE commands
        /// </summary>
        Operation,
    }
}
