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
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    [Guid("8E774159-2221-4435-A6C9-A40B78A369FD")]
    public interface ICommand
    {
        /// <summary>
        /// SBE-Scripts core
        /// </summary>
        ISobaScript SBEScript { get; }

        /// <summary>
        /// MSBuild core
        /// </summary>
        IEvMSBuild MSBuild { get; }

        /// <summary>
        /// Used environment
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Specified Event type.
        /// </summary>
        SolutionEventType EventType { get; }

        /// <summary>
        /// Find and execute action by specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="type">The type of event.</param>
        /// <returns>true value if it was handled.</returns>
        bool exec(ISolutionEvent evt, SolutionEventType type);

        /// <summary>
        /// Find and execute action with default event type.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>true value if it was handled.</returns>
        bool exec(ISolutionEvent evt);
    }
}
