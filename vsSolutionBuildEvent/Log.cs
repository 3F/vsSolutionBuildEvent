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
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Logger;
using NLog;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Main logger for Package
    /// </summary>
    internal class Log: ILog
    {
        /// <summary>
        /// When is receiving message.
        /// </summary>
        public event EventHandler<MessageArgs> Receiving = delegate(object sender, MessageArgs e) { };

        /// <summary>
        /// Get instance of the NLog logger
        /// </summary>
        public NLog.Logger NLog
        {
            get {
                return nlog;
            }
        }
        protected NLog.Logger nlog = LogManager.GetLogger(GuidList.PACKAGE_LOGGER);

        /// <summary>
        /// Thread-safe getting the instance of Log class
        /// </summary>
        public static Log _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Log> _lazy = new Lazy<Log>(() => new Log());

        /// <summary>
        /// DTE context
        /// </summary>
        protected EnvDTE.DTE dte;

        /// <summary>
        /// To displaying messages on the OutputWindowPane by SVsOutputWindow
        /// </summary>
        protected IVsOutputWindowPane _paneCOM = null;

        /// <summary>
        /// To displaying messages on the OutputWindowPane by EnvDTE
        /// </summary>
        protected EnvDTE.OutputWindowPane _paneDTE = null;


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
        public void paneAttach(string name, IVsOutputWindow ow, EnvDTE.DTE dteContext)
        {
            dte = dteContext;
            if(_paneCOM != null || _paneDTE != null) {
                Log.Debug("paneAttach-COM: skipped");
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
        public void paneAttach(IVsOutputWindowPane owp)
        {
            if(_paneCOM != null || _paneDTE != null) {
                Log.Debug("paneAttach-direct: to detach prev. first /skipped");
                return;
            }
            _paneCOM = owp;
        }

        /// <summary>
        /// Initialization of the EnvDTE.OutputWindowPane
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="dte2"></param>
        public void paneAttach(string name, EnvDTE80.DTE2 dte2)
        {
            dte = (EnvDTE.DTE)dte2;
            if(_paneCOM != null || _paneDTE != null) {
                Log.Debug("paneAttach-DTE: skipped");
                return; // currently we work only with one pane
            }

            try {
                _paneDTE = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Item(name);
            }
            catch(ArgumentException) {
                _paneDTE = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add(name);
            }
            catch(Exception ex) {
                Log.Error("Log :: inner exception: '{0}'", ex.ToString());
            }
        }

        /// <summary>
        /// Detaching OWP by IVsOutputWindow.
        /// </summary>
        /// <param name="ow"></param>
        public void paneDetach(IVsOutputWindow ow)
        {
            Guid id;
            if(_paneDTE != null) {
                id = new Guid(_paneDTE.Guid);
                //_paneDTE.Clear();
            }
            else{
                id = GuidList.OWP_SBE;
            }

            if(ow != null) {
                ow.DeletePane(ref id);
            }
            paneDetach();
        }

        /// <summary>
        /// Detaching OWP.
        /// </summary>
        public void paneDetach()
        {
            _paneCOM = null;
            _paneDTE = null;
            dte      = null;
        }

        /// <summary>
        /// Writes raw message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>self reference</returns>
        public ILog raw(string message)
        {
            _.write(message);
            return this;
        }

        /// <summary>
        /// Writes raw message + line terminator.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>self reference</returns>
        public ILog rawLn(string message)
        {
            return raw(message + System.Environment.NewLine);
        }

        /// <summary>
        /// Show messages if it's possible.
        /// </summary>
        public void show()
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
                Log.Debug("Log: error of showing '{0}'", ex.Message);
            }
        }


        /// <summary>
        /// Entry point for NLog messages.
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

            _.write(_.format(level, message, stamp), level);
        }

        /// <summary>
        /// Writes message at the Trace level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Trace(string message, params object[] args)
        {
            _.NLog.Trace(message, args);
        }

        /// <summary>
        /// Writes message at the Debug level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Debug(string message, params object[] args)
        {
            _.NLog.Debug(message, args);
        }

        /// <summary>
        /// Writes message at the Warn level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Warn(string message, params object[] args)
        {
            _.NLog.Warn(message, args);
        }

        /// <summary>
        /// Writes message at the Info level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Info(string message, params object[] args)
        {
            _.NLog.Info(message, args);
        }

        /// <summary>
        /// Writes message at the Error level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Error(string message, params object[] args)
        {
            _.NLog.Error(message, args);
        }

        /// <summary>
        /// Writes message at the Fatal level.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void Fatal(string message, params object[] args)
        {
            _.NLog.Fatal(message, args);
        }

        /// <summary>
        /// Used format for messages.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="stamp"></param>
        /// <returns>formatted</returns>
        protected virtual string format(string level, string message, string stamp)
        {
            return String.Format("{0} [{1}]: {2}{3}",
                                (new DateTime(long.Parse(stamp))).ToString(CultureInfo.CurrentCulture.DateTimeFormat),
                                level,
                                message,
                                System.Environment.NewLine);
        }

        /// <summary>
        /// Where to write.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        protected virtual void write(string message, string level = null)
        {
            if(Thread.CurrentThread.Name != LoggingEvent.IDENT_TH) {
                Receiving(this, new MessageArgs() { Message =  message,  Level = (level)?? String.Empty });
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
            System.Diagnostics.Debug.Write(message);
        }

        private Log() { }
    }
}
