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
