/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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

using System;
using EnvDTE;

namespace net.r_eg.vsSBE.VSTools.OW
{
    public class PaneDTE: IPane
    {
        /// <summary>
        /// The OutputWindowPane by EnvDTE
        /// </summary>
        protected OutputWindowPane pane = null;

        /// <summary>
        /// Gets the GUID for the pane.
        /// </summary>
        public Guid Guid
        {
            get {
                return new Guid(pane.Guid); //TODO: provide GuidList.OWP_SBE
            }
        }

        /// <summary>
        /// Moves the focus to the current item.
        /// </summary>
        public void Activate()
        {
            pane.Activate();
        }

        /// <summary>
        /// Clears all text from pane.
        /// </summary>
        public void Clear()
        {
            pane.Clear();
        }

        /// <summary>
        /// Sends a text string into pane.
        /// </summary>
        /// <param name="text"></param>
        public void OutputString(string text)
        {
            pane.OutputString(text);
        }

        /// <param name="dte2">DTE2 Context.</param>
        /// <param name="name">Name of the pane.</param>
        public PaneDTE(EnvDTE80.DTE2 dte2, string name)
        {
            if(dte2 == null) {
                throw new ArgumentNullException(nameof(dte2));
            }

            try {
                pane = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Item(name);
            }
            catch(ArgumentException) {
                pane = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(name);
            }
            catch(Exception ex) {
                Log.Error($"Failed PaneDTE init: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }
    }
}
