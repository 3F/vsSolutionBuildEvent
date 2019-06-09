/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using net.r_eg.vsSBE.API;
using net.r_eg.vsSBE.Configuration;
using NLog;

namespace net.r_eg.vsSBE.UI.Xaml
{
    [Guid(GuidList.PANEL_STRING)]
    internal class StatusToolWindow: ToolWindowPane, IStatusToolEvents
    {
        /// <summary>
        /// Facade of our status tool
        /// </summary>
        protected IStatusTool tool;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        /// <summary>
        /// Add handler for all events from API.IEventLevel
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents attachEvents(IEventLevel evt)
        {
            lock(_eLock)
            {
                detachEvents(evt);
                evt.OpenedSolution  += onOpenSolution;
                evt.ClosedSolution  += onCloseSolution;
            }
            return this;
        }

        /// <summary>
        /// Remove handler for all events from API.IEventLevel
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents detachEvents(IEventLevel evt)
        {
            evt.OpenedSolution -= onOpenSolution;
            evt.ClosedSolution -= onCloseSolution;
            return this;
        }

        /// <summary>
        /// Add handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents attachEvents(IConfig<ISolutionEvents> evt)
        {
            lock(_eLock)
            {
                detachEvents(evt);
                evt.Updated += onUpdated;
            }
            return this;
        }

        /// <summary>
        /// Remove handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents detachEvents(IConfig<ISolutionEvents> evt)
        {
            evt.Updated -= onUpdated;
            return this;
        }

        /// <summary>
        /// Add handler for all available events
        /// </summary>
        /// <returns>self reference</returns>
        public IStatusToolEvents attachEvents()
        {
            lock(_eLock) {
                detachEvents();
                Log._.Received += onReceiving;
            }
            return this;
        }

        /// <summary>
        /// Remove handler for all available events
        /// </summary>
        /// <returns>self reference</returns>
        public IStatusToolEvents detachEvents()
        {
            Log._.Received -= onReceiving;
            return this;
        }

        public StatusToolWindow()
            : base(null)
        {
            Caption         = Settings.OWP_ITEM_VSSBE;
            base.Content    = new StatusToolControl();
            tool            = (IStatusTool)base.Content;
        }

        public StatusToolWindow(string message)
            : this()
        {

        }

        private void onCloseSolution(object sender, EventArgs e)
        {
            tool.enabledPanel(false);
        }

        private void onOpenSolution(object sender, EventArgs e)
        {
            tool.enabledPanel(true);
        }

        private void onUpdated(object sender, DataArgs<ISolutionEvents> e)
        {
            if(e.Data != null) {
                tool.refresh();
            }
        }

        private void onReceiving(object sender, Logger.MessageArgs e)
        {
            if(String.IsNullOrEmpty(e.Level)) {
                return; // raw message
            }

            LogLevel oLevel = LogLevel.FromString(e.Level);
            if(oLevel < LogLevel.Warn) {
                return;
            }
            
            // Notify about warning
            tool.warn();
        }
    }
}
