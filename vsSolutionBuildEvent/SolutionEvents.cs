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
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE
{
    [Serializable]
    public class SolutionEvents: ISolutionEvents
    {
        /// <summary>
        /// Header of information.
        /// </summary>
        public Header Header
        {
            get { return header; }
            set { header = value; }
        }
        [NonSerialized]
        private Header header = new Header();

        /// <summary>
        /// Configuration of components.
        /// </summary>
        public Component[] Components
        {
            get;
            set;
        }

        /// <summary>
        /// Before assembling.
        /// </summary>
        public SBEEvent[] PreBuild
        {
            get { return preBuild; }
            set { preBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] preBuild = new SBEEvent[] { new SBEEvent() };

        /// <summary>
        /// After assembling.
        /// </summary>
        public SBEEvent[] PostBuild
        {
            get { return postBuild; }
            set { postBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] postBuild = new SBEEvent[] { };

        /// <summary>
        /// When build has been canceled by user or when occurs error.
        /// </summary>
        public SBEEvent[] CancelBuild
        {
            get { return cancelBuild; }
            set { cancelBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] cancelBuild = new SBEEvent[] { };

        /// <summary>
        /// Warnings during assembly processing.
        /// </summary>
        public SBEEventEW[] WarningsBuild
        {
            get { return warningsBuild; }
            set { warningsBuild = value; }
        }
        [NonSerialized]
        private SBEEventEW[] warningsBuild = new SBEEventEW[] { };

        /// <summary>
        /// Errors during assembly processing.
        /// </summary>
        public SBEEventEW[] ErrorsBuild
        {
            get { return errorsBuild; }
            set { errorsBuild = value; }
        }
        [NonSerialized]
        private SBEEventEW[] errorsBuild = new SBEEventEW[] { };

        /// <summary>
        /// Customization from the Output.
        /// </summary>
        public SBEEventOWP[] OWPBuild
        {
            get { return owpBuild; }
            set { owpBuild = value; }
        }
        [NonSerialized]
        private SBEEventOWP[] owpBuild = new SBEEventOWP[] { };

        /// <summary>
        /// Transmission of the build-data to outer handler.
        /// </summary>
        public SBETransmitter[] Transmitter
        {
            get { return transmitter; }
            set { transmitter = value; }
        }
        [NonSerialized]
        private SBETransmitter[] transmitter = new SBETransmitter[] { };

        /// <summary>
        /// Provides command events from EnvDTE.
        /// </summary>
        public CommandEvent[] CommandEvent
        {
            get { return commandEvent; }
            set { commandEvent = value; }
        }
        [NonSerialized]
        private CommandEvent[] commandEvent = new CommandEvent[] { };

        /// <summary>
        /// All processes with internal logging.
        /// </summary>
        public LoggingEvent[] Logging
        {
            get { return logging; }
            set { logging = value; }
        }
        [NonSerialized]
        private LoggingEvent[] logging = new LoggingEvent[] { };

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        public SBEEvent[] SlnOpened
        {
            get { return slnOpened; }
            set { slnOpened = value; }
        }
        [NonSerialized]
        private SBEEvent[] slnOpened = new SBEEvent[] { };

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        public SBEEvent[] SlnClosed
        {
            get { return slnClosed; }
            set { slnClosed = value; }
        }
        [NonSerialized]
        private SBEEvent[] slnClosed = new SBEEvent[] { };


        /// <summary>
        /// The event by type.
        /// </summary>
        /// <param name="type">Available event.</param>
        /// <exception cref="NotFoundException"></exception>
        public ISolutionEvent[] getEvent(SolutionEventType type)
        {
            switch(type)
            {
                case SolutionEventType.Pre: {
                    return PreBuild;
                }
                case SolutionEventType.Post: {
                    return PostBuild;
                }
                case SolutionEventType.Cancel: {
                    return CancelBuild;
                }
                case SolutionEventType.Warnings: {
                    return WarningsBuild;
                }
                case SolutionEventType.Errors: {
                    return ErrorsBuild;
                }
                case SolutionEventType.OWP: {
                    return OWPBuild;
                }
                case SolutionEventType.Transmitter: {
                    return Transmitter;
                }
                case SolutionEventType.CommandEvent: {
                    return CommandEvent;
                }
                case SolutionEventType.Logging: {
                    return Logging;
                }
                case SolutionEventType.SlnOpened: {
                    return SlnOpened;
                }
                case SolutionEventType.SlnClosed: {
                    return SlnClosed;
                }
            }

            throw new NotFoundException("getEvent: the event type '{0}' is not found.", type);
        }
    }
}
