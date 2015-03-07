/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using vsSBEPackage = net.r_eg.vsSBE.vsSolutionBuildEventPackage;

namespace net.r_eg.vsSBE.UI.Xaml
{
    [Guid(GuidList.PANEL_STRING)]
    internal class StatusToolWindow: ToolWindowPane
    {
        /// <summary>
        /// Main control for this ToolWindow
        /// </summary>
        protected StatusToolControl control;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        public StatusToolWindow()
            : base(null)
        {
            Caption = "Solution Build-Events";
            control = new StatusToolControl();

            base.Content = control;

            lock(_eLock)
            {
                vsSBEPackage.Event.OpenedSolution -= new EventHandler(onOpenSolution);
                vsSBEPackage.Event.OpenedSolution += new EventHandler(onOpenSolution);
                vsSBEPackage.Event.ClosedSolution -= new EventHandler(onCloseSolution);
                vsSBEPackage.Event.ClosedSolution += new EventHandler(onCloseSolution);

                Config._.Update -= new Config.UpdateEvent(control.updateData);
                Config._.Update += new Config.UpdateEvent(control.updateData);
                Log.Receipt -= new Log.ReceiptEvent(control.notify);
                Log.Receipt += new Log.ReceiptEvent(control.notify);
            }
        }

        protected void onCloseSolution(object sender, EventArgs e)
        {
            control.logic.resetWarnings();
            control.enabledPanel(false);
        }

        protected void onOpenSolution(object sender, EventArgs e)
        {
            control.enabledPanel(true);
        }
    }
}
