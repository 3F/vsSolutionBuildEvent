/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace net.r_eg.vsSBE.Logger
{
    [Guid("9B97170B-964D-457E-BBA3-35091708F438")]
    internal interface ILog
    {
        /// <summary>
        /// When message has been received.
        /// </summary>
        event EventHandler<MessageArgs> Received;

        /// <summary>
        /// Getting instance of the NLog logger
        /// </summary>
        NLog.Logger NLog { get; }

        /// <summary>
        /// Initialize OWP by IVsOutputWindow.
        /// </summary>
        /// <param name="name">Name of the pane</param>
        /// <param name="ow"></param>
        /// <param name="dteContext"></param>
        void paneAttach(string name, IVsOutputWindow ow, EnvDTE.DTE dteContext);

        /// <summary>
        /// Initialize OWP by IVsOutputWindowPane.
        /// </summary>
        /// <param name="owp"></param>
        void paneAttach(IVsOutputWindowPane owp);

        /// <summary>
        /// Initialize OWP by DTE2.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dte2"></param>
        void paneAttach(string name, EnvDTE80.DTE2 dte2);

        /// <summary>
        /// Detaching OWP by IVsOutputWindow.
        /// </summary>
        /// <param name="ow"></param>
        /// <returns></returns>
        void paneDetach(IVsOutputWindow ow);

        /// <summary>
        /// Detaching OWP.
        /// </summary>
        void paneDetach();

        /// <summary>
        /// Writes raw message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>self reference</returns>
        ILog raw(string message);

        /// <summary>
        /// Writes raw message + line terminator.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>self reference</returns>
        ILog rawLn(string message);

        /// <summary>
        /// Show messages if it's possible.
        /// </summary>
        void show();

        /// <summary>
        /// To clear all available messages if it's possible.
        /// </summary>
        /// <param name="force">Including undelivered etc.</param>
        void clear(bool force);

        /// <summary>
        /// Checks specific level on error type.
        /// </summary>
        /// <param name="level"></param>
        bool isError(string level);

        /// <summary>
        /// Checks specific level on warning type.
        /// </summary>
        /// <param name="level"></param>
        bool isWarn(string level);
    }
}