/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
            Caption         = "Status Panel";
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
