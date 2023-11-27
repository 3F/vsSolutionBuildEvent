/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
#if SDK15_OR_HIGH
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
#if SDK15_OR_HIGH
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
