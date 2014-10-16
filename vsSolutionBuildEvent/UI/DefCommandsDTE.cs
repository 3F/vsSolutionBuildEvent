/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI
{
    /// <summary>
    /// Predefined operations for gui
    /// TODO: move to SBE ~
    /// </summary>
    internal class DefCommandsDTE
    {
        public static List<ModeOperation> operations()
        {
            List<ModeOperation> dte = new List<ModeOperation>();

            dte.Add(new ModeOperation(new string[]{"Build.Cancel"}, "Stop building"));
            dte.Add(new ModeOperation(new string[]{"Build.Cancel", "Build.RebuildSolution"}, "Rebuild Solution"));
            dte.Add(new ModeOperation(new string[]{"Test.RunAllTestsInSolution"}, "Run all Unit-Tests"));
            dte.Add(new ModeOperation(new string[]{"Test.DebugAllTestsInSolution"}, "Debug all Unit-Tests"));
            dte.Add(new ModeOperation(new string[]{"Debug.Start"}, "Run project"));
            dte.Add(new ModeOperation(new string[]{"Debug.StartWithoutDebugging"}, "Run Without Debugging"));

            return dte;
        }
    }
}
