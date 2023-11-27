/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Windows.Forms;

#if !SDK15_OR_HIGH
using System.Drawing;
#endif

namespace net.r_eg.vsSBE.UI
{
    internal static class ComponentExtension
    {
        public static bool Toggle(this ToolStripMenuItem item) => item.Checked = !item.Checked;

        public static bool Toggle(this ToolStripMenuItem item, bool value) => item.Checked = value;

        public static float GetSysDpi(this Control src)
        {
#if SDK15_OR_HIGH
            return src.DeviceDpi;
#else
            using Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            return g.DpiX;
#endif
        }

        /// <param name="src"></param>
        /// <param name="value"></param>
        /// <param name="size">The default, 96 dots per inch.</param>
        /// <returns>Final value according to current DPI via <see cref="GetSysDpi(Control)"/></returns>
        public static int GetValueUsingDpi(this Control src, int value, float size = 96)
        {
            return (int)Math.Ceiling(value / size * src.GetSysDpi());
        }
    }
}
