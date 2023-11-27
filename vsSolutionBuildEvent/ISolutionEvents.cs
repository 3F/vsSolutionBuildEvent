/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE
{
    [Guid("8DFEA125-47CC-46A5-9A70-8202372A0680")]
    public interface ISolutionEvents
    {
        /// <summary>
        /// Header of information.
        /// </summary>
        Header Header { get; set; }

        /// <summary>
        /// Configuration of components.
        /// </summary>
        Component[] Components { get; set; }

        /// <summary>
        /// Before assembling.
        /// </summary>
        SBEEvent[] PreBuild { get; set; }

        /// <summary>
        /// After assembling.
        /// </summary>
        SBEEvent[] PostBuild  { get; set; }

        /// <summary>
        /// When build has been canceled by user or when occurs error.
        /// </summary>
        SBEEvent[] CancelBuild { get; set; }

        /// <summary>
        /// Warnings during assembly processing.
        /// </summary>
        SBEEventEW[] WarningsBuild { get; set; }

        /// <summary>
        /// Errors during assembly processing.
        /// </summary>
        SBEEventEW[] ErrorsBuild { get; set; }

        /// <summary>
        /// Customization from the Output.
        /// </summary>
        SBEEventOWP[] OWPBuild { get; set; }

        /// <summary>
        /// Transmission of the build-data to outer handler.
        /// </summary>
        SBETransmitter[] Transmitter { get; set; }

        /// <summary>
        /// Provides command events from EnvDTE.
        /// </summary>
        CommandEvent[] CommandEvent { get; set; }

        /// <summary>
        /// All processes with internal logging.
        /// </summary>
        LoggingEvent[] Logging { get; set; }

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        SBEEvent[] SlnOpened { get; set; }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        SBEEvent[] SlnClosed { get; set; }

        /// <summary>
        /// Getting event by type.
        /// </summary>
        /// <param name="type">Available event.</param>
        ISolutionEvent[] getEvent(SolutionEventType type);
    }
}
