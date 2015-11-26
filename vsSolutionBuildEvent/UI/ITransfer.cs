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

namespace net.r_eg.vsSBE.UI
{
    public interface ITransfer
    {
        /// <summary>
        /// Various commands such as a DTE, etc.
        /// </summary>
        void command(string data);

        /// <summary>
        /// Basic view of property by name/project.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        void property(string name, string project = null);

        /// <summary>
        /// Provides the action by event type.
        /// </summary>
        /// <param name="type">The type of event.</param>
        /// <param name="cfg">The event configuration for action.</param>
        void action(SolutionEventType type, ISolutionEvent cfg);

        /// <summary>
        /// EnvDTE command.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="id"></param>
        /// <param name="customIn"></param>
        /// <param name="customOut"></param>
        /// <param name="description"></param>
        //void command(string guid, int id, object customIn, object customOut, string description);
    }
}
