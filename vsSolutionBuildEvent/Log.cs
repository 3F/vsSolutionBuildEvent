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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using net.r_eg.vsSBE.Exceptions;
using NLog;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// hooking up notifications
    /// </summary>
    internal delegate void LogEventHandler();

    /// <summary>
    /// Notifications with message
    /// </summary>
    internal delegate void LogMessageEvent(string message);

    /// <summary>
    /// Main logger for Package
    /// Uses the OutputWindowPanes as target
    /// </summary>
    internal static class Log
    {
        public const string OWP_ITEM_NAME = "Solution Build-Events";

        /// <summary>
        /// external logic
        /// </summary>
        public static readonly Logger nlog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Notify about received, only as signal
        /// </summary>
        public static event LogEventHandler Receive = delegate { };

        /// <summary>
        /// Notify about received with formatted message
        /// </summary>
        public static event LogMessageEvent ReceiveMessage = delegate(string message) { };
        
        /// <summary>
        /// to display text output, represented by the OutputWindowPane
        /// </summary>
        private static OutputWindowPane _pane = null;

        /// <summary>
        /// NLog :: static "MethodCall"
        /// use with nlog
        /// https://github.com/nlog/nlog/wiki/MethodCall-target
        /// </summary>
        public static void nprint(string level, string message, string stamp)
        {
            LogLevel oLevel = LogLevel.FromString(level);

#if !DEBUG
            if(oLevel < LogLevel.Info && !Settings.debugMode) {
                return;
            }
#endif

            _notify(oLevel);

            string formatted = _format(level, message, stamp);
            ReceiveMessage(formatted);

            print(formatted);
        }

        public static void print(string message)
        {
            if(_pane == null) {
                init();
            }
            _pane.OutputString(message);
        }

        public static void init()
        {
            try {
                _pane = vsSolutionBuildEventPackage.Dte2.ToolWindows.OutputWindow.OutputWindowPanes.Item(OWP_ITEM_NAME);
            }
            catch(ArgumentException) {
                _pane = vsSolutionBuildEventPackage.Dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(OWP_ITEM_NAME);
            }
            catch(Exception ex) {
                throw new ComponentException("Log :: inner exception", ex);
            }
        }

        public static void show()
        {
            if(_pane == null) {
                init();
            }

            try {
                vsSolutionBuildEventPackage.Dte2.ExecuteCommand("View.Output");
                _pane.Activate();
            }
            catch(Exception ex) {
                //not critical because that option for quick access
                Log.nlog.Debug("DTE error 'View.Output' {0}", ex.Message);
            }
        }

        private static string _format(string level, string message, string stamp)
        {
            return String.Format("{0} [{1}]: {2}{3}",
                                (new DateTime(long.Parse(stamp))).ToString(CultureInfo.CurrentCulture.DateTimeFormat),
                                level,
                                message,
                                System.Environment.NewLine);
        }

        private static void _notify(LogLevel level)
        {
            if(level < LogLevel.Warn) {
                return;
            }
            Receive();
        }
    }
}
