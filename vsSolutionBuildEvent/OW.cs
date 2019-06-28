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
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace net.r_eg.vsSBE
{
    public class OWP: IOW
    {
        /// <summary>
        /// DTE2 context
        /// </summary>
        protected DTE2 dte2;

        /// <summary>
        /// Access to OutputWindow.
        /// </summary>
        public OutputWindow OutputWindow
        {
            get { return dte2.ToolWindows.OutputWindow; }
        }

        /// <summary>
        /// Get item of the output window by name.
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="createIfNotExist">If this value as true: Creates new pane if this item does not exist, otherwise exception.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public OutputWindowPane getByName(string name, bool createIfNotExist)
        {
            OutputWindowPanes panes = dte2.ToolWindows.OutputWindow.OutputWindowPanes;
            if(createIfNotExist) {
                try {
                    return panes.Item(name);
                }
                catch(ArgumentException) {
                    return panes.Add(name);
                }
            }
            return panes.Item(name);
        }

        /// <summary>
        /// Remove pane by name of item.
        /// </summary>
        /// <param name="name"></param>
        public void deleteByName(string name)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            OutputWindowPane pane = getByName(name, false);
            deleteByGuid(new Guid(pane.Guid));
        }

        /// <summary>
        /// Remove pane with selected GUID.
        /// </summary>
        /// <param name="guid"></param>
        public void deleteByGuid(Guid guid)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            IVsOutputWindow ow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
            ow.DeletePane(ref guid);
        }

        /// <param name="dte2">DTE2 context</param>
        public OWP(DTE2 dte2)
        {
            this.dte2 = dte2;
        }
    }
}
