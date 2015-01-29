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

using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.Actions
{
    public interface ICommand
    {
        /// <summary>
        /// SBE-Scripts core
        /// </summary>
        ISBEScript SBEScript { get; }

        /// <summary>
        /// MSBuild core
        /// </summary>
        IMSBuild MSBuild { get; }

        /// <summary>
        /// Used environment
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Specified Event type
        /// </summary>
        SolutionEventType EventType { get; }

        /// <summary>
        /// Specified type of build action
        /// </summary>
        BuildType BuildType { get; }

        /// <summary>
        /// Entry point for execution
        /// </summary>
        /// <param name="evt">Configured event</param>
        /// <param name="type">Type of event</param>
        /// <returns>true value if has been processed</returns>
        bool exec(ISolutionEvent evt, SolutionEventType type);

        /// <summary>
        /// Entry point for execution
        /// </summary>
        /// <param name="evt">Configured event</param>
        /// <returns>true value if has been processed</returns>
        bool exec(ISolutionEvent evt);

        /// <summary>
        /// Updating context with the BuildType
        /// </summary>
        /// <param name="buildType">Type of build action</param>
        void updateContext(BuildType buildType);
    }
}
