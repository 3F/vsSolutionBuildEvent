/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Logger;
using net.r_eg.vsSBE.VSTools.OW;
using NLog;

namespace net.r_eg.vsSBE
{
    /// <summary>
    /// Main logger for Package
    /// </summary>
    internal class Log: ILog
    {
        /// <summary>
        /// When message has been received.
        /// </summary>
        public event EventHandler<MessageArgs> Received = delegate(object sender, MessageArgs e) { };

        /// <summary>
        /// DTE context
        /// </summary>
        protected EnvDTE.DTE dte;

        /// <summary>
        /// To displaying messages.
        /// </summary>
        protected IPane upane;

        /// <summary>
        /// Undelivered messages.
        /// </summary>
        protected Queue<string> undelivered = new Queue<string>();

        /// <summary>
        /// Size of buffer for undelivered messages.
        /// </summary>
        protected int undBuffer = 2048;

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
        public static ILog _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Log> _lazy = new Lazy<Log>(() => new Log());

        /// <summary>
        /// Initialization of the IVsOutputWindowPane
        /// note: probably slow, 
        ///       and be careful with using inside `Initialize()` method or constructor of main package, 
        ///       may be inner exception for COM object in VS (tested on VS2013 with docked to output panel)
        ///       Otherwise, use the IVsUIShell.FindToolWindow (again, only with __VSFINDTOOLWIN.FTW_fFindFirst)
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="ow"></param>
        /// <param name="dteContext"></param>
        public void paneAttach(string name, IVsOutputWindow ow, EnvDTE.DTE dteContext)
        {
            dte = dteContext;

            if(upane != null) {
                Log.Debug("paneAttach-COM: pane is already attached.");
                return;
            }
            upane = new PaneCOM(ow, name);
        }

        /// <summary>
        /// Direct access from existing instance
        /// </summary>
        /// <param name="owp"></param>
        public void paneAttach(IVsOutputWindowPane owp)
        {
            if(upane != null) {
                Log.Debug("paneAttach-direct: pane is already attached.");
                return;
            }
            upane = new PaneCOM(owp);
        }

        /// <summary>
        /// Attach pane with EnvDTE.OutputWindowPane
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="dte2"></param>
        public void paneAttach(string name, EnvDTE80.DTE2 dte2)
        {
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread();
#endif
            dte = (EnvDTE.DTE)dte2;
            if(upane != null) {
                Log.Debug("paneAttach-DTE: pane is already attached.");
                return;
            }
            upane = new PaneDTE(dte2, name);
        }

        /// <summary>
        /// Detaching OWP by IVsOutputWindow.
        /// </summary>
        /// <param name="ow"></param>
        public void paneDetach(IVsOutputWindow ow)
        {
            Guid id = (upane != null)? upane.Guid : GuidList.OWP_SBE;
            paneDetach();

#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            if(ow != null) {
                ow.DeletePane(ref id);
            }
        }

        /// <summary>
        /// Detaching OWP.
        /// </summary>
        public void paneDetach()
        {
            upane   = null;
            dte     = null;
        }

        /// <summary>
        /// Writes raw message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>self reference</returns>
        public ILog raw(string message)
        {
            write(message);
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
#if SDK15_OR_HIGH
                ThreadHelper.ThrowIfNotOnUIThread();
#endif
                if(dte != null)
                {
                    dte.ExecuteCommand("View.Output"); //TODO:
                }

                if(upane != null) {
                    upane.Activate();
                }
            }
            catch(Exception ex) {
                Log.Debug("Log: error of showing '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// To clear all available messages if it's possible.
        /// </summary>
        /// <param name="force">Including undelivered etc.</param>
        public void clear(bool force)
        {
            if(force) {
                undelivered.Clear();
            }

            if(upane != null) {
                upane.Clear();
            }
        }

        /// <summary>
        /// Checks specific level on error type.
        /// </summary>
        /// <param name="level"></param>
        public bool isError(string level)
        {
            if(String.IsNullOrWhiteSpace(level)) {
                return false;
            }

            // TODO:
            Func<LogLevel, bool> _is = delegate(LogLevel t) {
                return level.Equals($"{t}", StringComparison.OrdinalIgnoreCase);
            };

            return _is(LogLevel.Error) || _is(LogLevel.Fatal);
        }

        /// <summary>
        /// Checks specific level on warning type.
        /// </summary>
        /// <param name="level"></param>
        public bool isWarn(string level)
        {
            if(String.IsNullOrWhiteSpace(level)) {
                return false;
            }

            // TODO: 
            return level.Equals($"{LogLevel.Warn}", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Entry point for NLog messages.
        /// https://github.com/nlog/nlog/wiki/MethodCall-target
        /// </summary>
        public static void nprint(string level, string message, string stamp, string src, string type)
        {
            if(!Settings._.DebugMode
                && LogLevel.FromString(level) < LogLevel.Info)
            {
                return;
            }

            Log log = _lazy.Value;
            log.write(new(log.format(level, message, stamp), level, src, type, message));
        }

        /// <summary>
        /// Writes message at the Trace level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Trace(string message, params object[] args)
            => Msg(LogLevel.Trace, message, args);

        /// <summary>
        /// Writes message at the Debug level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Debug(string message, params object[] args)
            => Msg(LogLevel.Debug, message, args);

        /// <summary>
        /// Writes message at the Warn level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Warn(string message, params object[] args)
            => Msg(LogLevel.Warn, message, args);

        /// <summary>
        /// Writes message at the Info level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Info(string message, params object[] args)
            => Msg(LogLevel.Info, message, args);

        /// <summary>
        /// Writes message at the Error level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Error(string message, params object[] args)
            => Msg(LogLevel.Error, message, args);

        /// <summary>
        /// Writes message at the Fatal level with <see cref="Fallback"/> if needed.
        /// </summary>
        public static void Fatal(string message, params object[] args)
            => Msg(LogLevel.Fatal, message, args);

        /// <summary>
        /// Writes message at the specified level with <see cref="Fallback"/> if needed.
        /// </summary>
        internal static void Msg(LogLevel level, string message, params object[] args)
        {
            if(!Fallback(level, message))
            {
                LogEventInfo msg = new(level, _.NLog.Name, message);

                if(args.Length == 1 && args[0] is ExecLocator el)
                {
                    msg.Properties.Add("src", el.Evt?.Name);
                    msg.Properties.Add("type", el.EvtType);
                }

                _.NLog.Log(msg);
            }
        }

        /// <remarks>
        /// Some thing, like <see cref="Configuration.SysConfig"/>, can be initialized too early, for example, <see cref="Pkg"/> and its accessing to <see cref="Settings.DebugMode"/> flag) before <see cref="Initializer"/>;
        /// <br/>
        /// If so, only <see cref="raw(string)"/> methods can help to deliver messages after complete <see cref="Initializer"/> (delayed messages).
        /// </remarks>
        /// <returns>true if fallback is applied</returns>
        protected static bool Fallback(LogLevel flevel, string message)
        {
            if(LogManager.Configuration != null)
            {
                return false;
            }
            // NLog configuring is not completed yet

            Log log = _lazy.Value;
            log.write
            (
                log.format(flevel.ToString(), message),
                level: null //TODO: ignoreLevel() inside may cause circular dependency
            );
            return true;
        }

        protected virtual string format(string level, string message, string stamp = null)
        {
            DateTime dt = (stamp == null) ? DateTime.Now : new(long.Parse(stamp));
            return string.Format
            (
                "{0} [{1}]: {2}{3}",
                dt.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + ".ffff"),
                level,
                message,
                System.Environment.NewLine
            );
        }

        /// <summary>
        /// Checks status of ignoring level.
        /// </summary>
        /// <param name="level">Level for cheking.</param>
        /// <returns></returns>
        protected bool ignoreLevel(string level)
        {
            if(String.IsNullOrEmpty(level)) {
                return false;
            }

            var cfg = Settings._.Config.Sys?.Data;

            if(cfg?.LogIgnoreLevels?.ContainsKey(level) == true) {
                return cfg.LogIgnoreLevels[level];
            }
            return false;
        }

        protected void write(MessageArgs args)
            => write(args.Message, args.Level ?? string.Empty, args);

        // TODO: combine MessageArgs and message
        protected virtual void write(string message, string level = null, MessageArgs args = null)
        {
            if(ignoreLevel(level)) { //TODO: extract ignoreLevel() from here due to Fallback use etc.
                return;
            }

            if(Thread.CurrentThread.Name != LoggingEvent.IDENT_TH)
            {
                Received(this, args ?? new() { Message = message, Level = level ?? string.Empty });
            }

            try
            {
                if(deliver(message)) return;
            }
            catch(COMException ex) {
                message = String.Format("Log - COMException '{0}' :: Message - '{1}'", ex.Message, message);
            }

            conwrite(message, level);

#if DEBUG
            System.Diagnostics.Debug.Write(message);
#endif
        }

        protected virtual void conwrite(string message, string level)
        {
            if(Log._.isError(level)) {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if(Log._.isWarn(level)) {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            Console.Write(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Use OWP for write operation.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>true value if message has been sent.</returns>
        protected bool owpSend(string message)
        {
            if(upane != null) {
                upane.OutputString(message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Delivering message.
        /// Including the all undelivered before.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>true value if message has been delivered.</returns>
        protected bool deliver(string message)
        {
            if(upane == null) {
                holdMessage(message);
                return false;
            }
            
            while(undelivered.Count > 0) {
                owpSend(undelivered.Dequeue());
            }

            owpSend(message);
            return true;
        }

        /// <summary>
        /// Hold undelivered message.
        /// </summary>
        /// <param name="msg"></param>
        protected void holdMessage(string msg)
        {
            if(undelivered.Count > undBuffer) {
                undelivered.Dequeue();
            }
            undelivered.Enqueue(msg);
        }

        protected Log() { }
    }
}
