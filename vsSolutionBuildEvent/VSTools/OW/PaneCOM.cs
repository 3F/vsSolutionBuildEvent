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

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace net.r_eg.vsSBE.VSTools.OW
{
    public class PaneCOM: IPane
    {
        /// <summary>
        /// The OutputWindowPane by SVsOutputWindow
        /// </summary>
        protected IVsOutputWindowPane pane = null;

        /// <summary>
        /// Gets the GUID for the pane.
        /// </summary>
        public Guid Guid
        {
            get;
            protected set;
        }

        /// <summary>
        /// Moves the focus to the current item.
        /// </summary>
        public void Activate()
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            pane.Activate();
        }

        /// <summary>
        /// Clears all text from pane.
        /// </summary>
        public void Clear()
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            pane.Clear();
        }

        /// <summary>
        /// Sends a text string into pane.
        /// </summary>
        /// <param name="text"></param>
        public void OutputString(string text)
        {
#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread();
            pane.OutputStringThreadSafe(text);
#else
            pane.OutputString(text);
#endif
        }

        /// <param name="ow"></param>
        /// <param name="name">Name of the pane</param>
        public PaneCOM(IVsOutputWindow ow, string name)
        {
            if(ow == null) throw new ArgumentNullException(nameof(ow));

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            Guid id = GuidList.OWP_SBE;
            ow.CreatePane(ref id, name, 1, 1);
            ow.GetPane(ref id, out pane);

            this.Guid = id;
        }

        /// <param name="owp"></param>
        public PaneCOM(IVsOutputWindowPane owp)
        {
            pane = owp;
        }
    }
}
