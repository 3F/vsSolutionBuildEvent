/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.r_eg.vsSBE.UI
{
    /// <summary>
    /// Predefined operations for gui
    /// TODO: move to SBE ~
    /// </summary>
    class DefCommandsDTE
    {
        public static List<TOperation> operations()
        {
            List<TOperation> dte = new List<TOperation>();

            dte.Add(new TOperationQ("Build.Cancel", "Stop building"));
            dte.Add(new TOperationQ("Build.Cancel\nBuild.RebuildSolution", "Rebuild Solution"));
            dte.Add(new TOperationQ("Debug.Start", "Run project"));
            dte.Add(new TOperationQ("Debug.StartWithoutDebugging", "Run Without Debugging"));

            return dte;
        }

        /// <summary>
        /// TOperation cannot contain a constructor to serialize
        /// .. benefits the principle LSP =_=
        /// </summary>
        class TOperationQ: TOperation
        {
            public TOperationQ(string cmd, string caption)
            {
                this.cmd     = cmd;
                this.caption = caption;
            }
        }
    }
}
