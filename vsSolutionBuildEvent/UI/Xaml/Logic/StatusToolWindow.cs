/*
 * Copyright (c) 2013-2014 Developed by reg [Denis Kuzmin] <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
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

namespace net.r_eg.vsSBE.UI
{
    [Guid(GuidList.PANEL_STRING)]
    internal class StatusToolWindow: ToolWindowPane
    {
        public static readonly StatusControl control = new StatusControl();

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
