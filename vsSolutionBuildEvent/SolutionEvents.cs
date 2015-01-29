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
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE
{
    [Serializable]
    public class SolutionEvents
    {
        /// <summary>
        /// Additional information for configuration
        /// </summary>
        public Header Header
        {
            get { return header; }
            set { header = value; }
        }
        [NonSerialized]
        private Header header = new Header();

        /// <summary>
        /// 
        /// </summary>
        public Component[] Components
        {
            get { return components; }
            set { components = value; }
        }
        [NonSerialized]
        private Component[] components = null;

        /// <summary>
        /// Before assembling
        /// </summary>
        public SBEEvent[] PreBuild
        {
            get { return preBuild; }
            set { preBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] preBuild = new SBEEvent[] { new SBEEvent() };

        /// <summary>
        /// After assembling
        /// </summary>
        public SBEEvent[] PostBuild
        {
            get { return postBuild; }
            set { postBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] postBuild = new SBEEvent[] { new SBEEvent() };

        /// <summary>
        /// When user cancelled the building or when an error occurs
        /// </summary>
        public SBEEvent[] CancelBuild
        {
            get { return cancelBuild; }
            set { cancelBuild = value; }
        }
        [NonSerialized]
        private SBEEvent[] cancelBuild = new SBEEvent[] { new SBEEvent() };

        /// <summary>
        /// Warnings during assembly processing
        /// </summary>
        public SBEEventEW[] WarningsBuild
        {
            get { return warningsBuild; }
            set { warningsBuild = value; }
        }
        [NonSerialized]
        private SBEEventEW[] warningsBuild = new SBEEventEW[] { new SBEEventEW() };

        /// <summary>
        /// Errors during assembly processing
        /// </summary>
        public SBEEventEW[] ErrorsBuild
        {
            get { return errorsBuild; }
            set { errorsBuild = value; }
        }
        [NonSerialized]
        private SBEEventEW[] errorsBuild = new SBEEventEW[] { new SBEEventEW() };

        /// <summary>
        /// Customization from the Output
        /// </summary>
        public SBEEventOWP[] OWPBuild
        {
            get { return owpBuild; }
            set { owpBuild = value; }
        }
        [NonSerialized]
        private SBEEventOWP[] owpBuild = new SBEEventOWP[] { new SBEEventOWP() };

        /// <summary>
        /// Transmission building-data to outer handler
        /// </summary>
        public SBETransmitter[] Transmitter
        {
            get { return transmitter; }
            set { transmitter = value; }
        }
        [NonSerialized]
        private SBETransmitter[] transmitter = new SBETransmitter[] { new SBETransmitter() };

        /// <summary>
        /// All processes with internal logging
        /// </summary>
        public LoggingEvent[] Logging
        {
            get { return logging; }
            set { logging = value; }
        }
        [NonSerialized]
        private LoggingEvent[] logging = new LoggingEvent[] { 
            new LoggingEvent(){ 
                Process = new EventProcess(){ 
                    Waiting = false // is better for performance
                }
            } 
        };


        /// <summary>
        /// Getting event by type
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotFoundException"></exception>
        public ISolutionEvent[] getEvent(SolutionEventType type)
        {
            switch(type) {
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
                case SolutionEventType.Logging: {
                    return Logging;
                }
            }
            throw new NotFoundException("getEvent: Not found event type - '{0}'", type);
        }
    }
}
