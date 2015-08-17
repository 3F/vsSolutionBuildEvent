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
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Events;
using NLog;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Main logger for Package
    /// </summary>
    internal static class Log
    {
        /// <summary>
        /// Notification about any receiving
        /// </summary>
        public delegate void ReceiptEvent();

        /// <summary>
        /// Notification about receiving of message
        /// </summary>
        public delegate void MessageEvent(string message, string level);

        /// <summary>
        /// external logic
        /// </summary>
        public static readonly Logger nlog = LogManager.GetLogger(GuidList.PACKAGE_LOGGER);

        /// <summary>
        /// Any receipt - only as signal
        /// </summary>
        public static event ReceiptEvent Receipt = delegate { };

        /// <summary>
        /// Received message
        /// </summary>
        public static event MessageEvent Message = delegate(string message, string level) { };

        /// <summary>
        /// DTE context
        /// </summary>
        private static EnvDTE.DTE dte;
        
        /// <summary>
        /// To displaying messages on the OutputWindowPane by SVsOutputWindow
        /// </summary>
        private static IVsOutputWindowPane _paneCOM = null;

        /// <summary>
        /// To displaying messages on the OutputWindowPane by EnvDTE
        /// </summary>
        private static EnvDTE.OutputWindowPane _paneDTE = null;

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
            print(_format(level, message, stamp), level);
        }

        public static void print(string message, string level = null)
        {
            if(Thread.CurrentThread.Name != LoggingEvent.IDENT_TH) {
                Message(message, (level)?? String.Empty);
            }

            if(_paneDTE != null) {
                _paneDTE.OutputString(message);
                return;
            }

            if(_paneCOM != null) {
                _paneCOM.OutputString(message);
                return;
            }

            Console.Write(message);
            Debug.Write(message);
        }

        /// <summary>
        /// Initialization of the IVsOutputWindowPane
        /// note: probably slow initialization, 
        ///       and be careful with using in Initialize() of package or constructor, 
        ///       may be inner exception for COM object in VS (tested on VS2013 with docked to output panel)
        ///       Otherwise, use the IVsUIShell.FindToolWindow (again, only with __VSFINDTOOLWIN.FTW_fFindFirst)
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="ow"></param>
        /// <param name="dteContext"></param>
        public static void paneAttach(string name, IVsOutputWindow ow, EnvDTE.DTE dteContext)
        {
            dte = dteContext;
            if(_paneCOM != null || _paneDTE != null) {
                Log.nlog.Debug("paneAttach-COM: skipped");
                return; // currently we work only with one pane
            }

            Guid id = GuidList.OWP_SBE;
            ow.CreatePane(ref id, name, 1, 1);
            ow.GetPane(ref id, out _paneCOM);
        }

        /// <summary>
        /// Direct access from existing instance
        /// </summary>
        /// <param name="owp"></param>
        public static void paneAttach(IVsOutputWindowPane owp)
        {
            if(_paneCOM != null || _paneDTE != null) {
                Log.nlog.Debug("paneAttach-direct: to detach prev. first /skipped");
                return;
            }
            _paneCOM = owp;
        }

        /// <summary>
        /// Initialization of the EnvDTE.OutputWindowPane
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="dte2"></param>
        public static void paneAttach(string name, EnvDTE80.DTE2 dte2)
        {
            dte = (EnvDTE.DTE)dte2;
            if(_paneCOM != null || _paneDTE != null) {
                Log.nlog.Debug("paneAttach-DTE: skipped");
                return; // currently we work only with one pane
            }

            try {
                _paneDTE = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Item(name);
            }
            catch(ArgumentException) {
                _paneDTE = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(name);
            }
            catch(Exception ex) {
                Log.nlog.Error("Log :: inner exception: '{0}'", ex.ToString());
            }
        }

        public static void paneDetach(IVsOutputWindow ow)
        {
            Guid id;
            if(_paneDTE != null) {
                id = new Guid(_paneDTE.Guid);
                _paneDTE.Clear();
            }
            else{
                id = GuidList.OWP_SBE;
            }

            if(ow != null) {
                ow.DeletePane(ref id);
            }
            paneDetach();
        }

        public static void paneDetach()
        {
            _paneCOM = null;
            _paneDTE = null;
            dte      = null;
        }

        /// <summary>
        /// Opening the Output window and activate pane
        /// </summary>
        public static void show()
        {
            try
            {
                if(dte != null) {
                    dte.ExecuteCommand("View.Output"); //TODO:
                }

                if(_paneDTE != null) {
                    _paneDTE.Activate();
                }
                else if(_paneCOM != null) {
                    _paneCOM.Activate();
                }
            }
            catch(Exception ex) {
                Log.nlog.Debug("Log: error of the showing {0}", ex.Message);
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
            Receipt();
        }
    }
}
