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
    public class EventProcess: IEventProcess
    {
        /// <summary>
        /// Waiting completion
        /// </summary>
        public bool Waiting
        {
            get { return waiting; }
            set { waiting = value; }
        }
        private bool waiting = true;

        /// <summary>
        /// Hiding of processing or not
        /// </summary>
        public bool Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }
        private bool hidden = true;

        /// <summary>
        /// Closing the window with result or not
        /// </summary>
        public bool KeepWindow
        {
            get { return keepWindow; }
            set { keepWindow = value; }
        }
        private bool keepWindow = false;
    }
}
