/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI
{
    internal static class ComponentExtension
    {
        public static bool Toggle(this ToolStripMenuItem item) => item.Checked = !item.Checked;

        public static bool Toggle(this ToolStripMenuItem item, bool value) => item.Checked = value;
    }
}