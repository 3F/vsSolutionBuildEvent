/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            pane.Activate();
        }

        /// <summary>
        /// Clears all text from pane.
        /// </summary>
        public void Clear()
        {
#if SDK15_OR_HIGH
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
#if SDK15_OR_HIGH
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

#if SDK15_OR_HIGH
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
