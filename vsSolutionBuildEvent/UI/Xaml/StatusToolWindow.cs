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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace net.r_eg.vsSBE.UI.Xaml
{
    [Guid(GuidList.PANEL_STRING)]
    internal class StatusToolWindow: ToolWindowPane
    {
        public static readonly StatusToolControl control = new StatusToolControl();

        private Object _eLock = new Object();
        public StatusToolWindow(): base(null)
        {
            this.Caption = "Solution Build-Events";
            base.Content = control;

            ConfigEventHandler cfgEvent = new ConfigEventHandler(control.updateData);
            LogEventHandler logEvent    = new LogEventHandler(control.notify);
            lock(_eLock) {
                Config._.Update -= cfgEvent;
                Config._.Update += cfgEvent;
                Log.Receive -= logEvent;
                Log.Receive += logEvent;
            }
        }
    }
}
