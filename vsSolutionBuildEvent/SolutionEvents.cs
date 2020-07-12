/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE
{
    [Serializable]
    public class SolutionEvents: ISolutionEvents
    {
        /// <inheritdoc cref="ISolutionEvents.Header"/>
        public Header Header { get; set; } = new Header();

        /// <inheritdoc cref="ISolutionEvents.Components"/>
        public Component[] Components { get; set; }

        /// <inheritdoc cref="ISolutionEvents.PreBuild"/>
        public SBEEvent[] PreBuild { get; set; } = new[] { new SBEEvent() };

        /// <inheritdoc cref="ISolutionEvents.PostBuild"/>
        public SBEEvent[] PostBuild { get; set; } = new SBEEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.CancelBuild"/>
        public SBEEvent[] CancelBuild { get; set; } = new SBEEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.WarningsBuild"/>
        public SBEEventEW[] WarningsBuild { get; set; } = new SBEEventEW[] { };

        /// <inheritdoc cref="ISolutionEvents.ErrorsBuild"/>
        public SBEEventEW[] ErrorsBuild { get; set; } = new SBEEventEW[] { };

        /// <inheritdoc cref="ISolutionEvents.OWPBuild"/>
        public SBEEventOWP[] OWPBuild { get; set; } = new SBEEventOWP[] { };

        /// <inheritdoc cref="ISolutionEvents.Transmitter"/>
        public SBETransmitter[] Transmitter { get; set; } = new SBETransmitter[] { };

        /// <inheritdoc cref="ISolutionEvents.CommandEvent"/>
        public CommandEvent[] CommandEvent { get; set; } = new CommandEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.Logging"/>
        public LoggingEvent[] Logging { get; set; } = new LoggingEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.SlnOpened"/>
        public SBEEvent[] SlnOpened { get; set; } = new SBEEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.SlnClosed"/>
        public SBEEvent[] SlnClosed { get; set; } = new SBEEvent[] { };

        /// <inheritdoc cref="ISolutionEvents.getEvent"/>
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

            throw new NotFoundException(type);
        }

        public bool ShouldSerializeComponents() => Components?.Length > 0;
        public bool ShouldSerializePreBuild() => PreBuild?.Length > 0;
        public bool ShouldSerializePostBuild() => PostBuild?.Length > 0;
        public bool ShouldSerializeCancelBuild() => CancelBuild?.Length > 0;
        public bool ShouldSerializeWarningsBuild() => WarningsBuild?.Length > 0;
        public bool ShouldSerializeErrorsBuild() => ErrorsBuild?.Length > 0;
        public bool ShouldSerializeOWPBuild() => OWPBuild?.Length > 0;
        public bool ShouldSerializeTransmitter() => Transmitter?.Length > 0;
        public bool ShouldSerializeCommandEvent() => CommandEvent?.Length > 0;
        public bool ShouldSerializeLogging() => Logging?.Length > 0;
        public bool ShouldSerializeSlnOpened() => SlnOpened?.Length > 0;
        public bool ShouldSerializeSlnClosed() => SlnClosed?.Length > 0;
    }
}
