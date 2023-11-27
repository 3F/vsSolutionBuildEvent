/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
