/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
        public IStatusToolEvents attachEvents(API.IEventLevel evt)
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
        public IStatusToolEvents detachEvents(API.IEventLevel evt)
        {
            lock(_eLock)
            {
                evt.OpenedSolution -= onOpenSolution;
                evt.ClosedSolution -= onCloseSolution;
            }
            return this;
        }

        /// <summary>
        /// Add handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents attachEvents(Config evt)
        {
            lock(_eLock)
            {
                detachEvents(evt);
                evt.Update += tool.refresh;
            }
            return this;
        }

        /// <summary>
        /// Remove handler for all events from Config
        /// </summary>
        /// <param name="evt"></param>
        /// <returns>self reference</returns>
        public IStatusToolEvents detachEvents(Config evt)
        {
            lock(_eLock)
            {
                evt.Update -= tool.refresh;
            }
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
                Log.Receipt += tool.warn;
            }
            return this;
        }

        /// <summary>
        /// Remove handler for all available events
        /// </summary>
        /// <returns>self reference</returns>
        public IStatusToolEvents detachEvents()
        {
            lock(_eLock) {
                Log.Receipt -= tool.warn;
            }
            return this;
        }

        public StatusToolWindow()
            : base(null)
        {
            Caption         = Settings.OWP_ITEM_VSSBE;
            base.Content    = new StatusToolControl();
            tool            = (IStatusTool)base.Content;
        }

        protected void onCloseSolution(object sender, EventArgs e)
        {
            tool.enabledPanel(false);
        }

        protected void onOpenSolution(object sender, EventArgs e)
        {
            tool.enabledPanel(true);
        }
    }
}
