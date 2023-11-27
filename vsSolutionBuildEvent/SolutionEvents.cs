/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
