/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
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

using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// General types of available events.
    /// </summary>
    [Guid("DA764E72-5823-4DB9-AEB8-FD756BFEF67D")]
    public enum SolutionEventType
    {
        /// <summary>
        /// Unspecified event.
        /// </summary>
        Common = 0x10,

        /// <summary>
        /// Alias to Common.
        /// </summary>
        General = Common,

        /// <summary>
        /// 'Pre-Build' event.
        /// </summary>
        Pre = 0x100, 
        
        /// <summary>
        /// 'Post-Build' event.
        /// </summary>
        Post = 0x101, 
        
        /// <summary>
        /// 'Cancel-Build' event.
        /// </summary>
        Cancel = 0x102, 
        
        /// <summary>
        /// 'Warnings-Build' event.
        /// </summary>
        Warnings = 0x103, 
        
        /// <summary>
        /// 'Errors-Build' event.
        /// </summary>
        Errors = 0x104, 
        
        /// <summary>
        /// 'Output-Build' event.
        /// </summary>
        Output = 0x105,

        /// <summary>
        /// Alias to Output.
        /// </summary>
        OWP = Output,
        
        /// <summary>
        /// Transmitter of the build-data.
        /// </summary>
        Transmitter = 0x106, 

        /// <summary>
        /// All processes with internal logging.
        /// </summary>
        Logging = 0x107,

        /// <summary>
        /// CommandEvents from EnvDTE.
        /// </summary>
        CommandEvent = 0x108,

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        SlnOpened = 0x109,

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        SlnClosed = 0x10A,

        /// <summary>
        /// 'Pre-Build' by individual project.
        /// </summary>
        ProjectPre = 0x200,

        /// <summary>
        /// 'Post-Build' by individual project.
        /// </summary>
        ProjectPost = 0x201,

        /// <summary>
        /// The deferred 'Pre-Build' action for specific project.
        /// </summary>
        DeferredPre = 0x202,

        /// <summary>
        /// Errors + Warnings.
        /// </summary>
        EW = 0x207,

        /// <summary>
        /// Alias to EW.
        /// </summary>
        ErrorsWarnings = EW,
    }
}
