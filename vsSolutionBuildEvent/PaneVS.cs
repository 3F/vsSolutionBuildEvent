/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using EnvDTE;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Working with a VS-pane
    /// TODO: verbose option
    /// </summary>
    class PaneVS
    {
        public const string ITEM_NAME = "Solution Build-Events";

        /// <summary>
        /// a single instance
        /// </summary>
        public static readonly PaneVS instance = new PaneVS();

        /// <summary>
        /// manipulate of item
        /// TODO:
        /// </summary>
        protected OutputWindowPane Pane
        {
            get
            {
                try {
                    return _dte.ToolWindows.OutputWindow.OutputWindowPanes.Item(ITEM_NAME);
                }
                catch { }
                return _dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(ITEM_NAME);
            }
        }

        /// <summary>
        /// for a top-level functionality
        /// </summary>
        private DTE2 _dte = null;

        public void setDTE(DTE2 dte)
        {
            this._dte = dte;
        }

        public void clear()
        {
            Pane.Clear();
        }

        public void show()
        {
            Pane.Activate();
        }

        public void outputString(string data)
        {
            Pane.OutputString(data);
        }

        protected PaneVS() { }
    }
}
