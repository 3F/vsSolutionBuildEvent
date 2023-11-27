/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
