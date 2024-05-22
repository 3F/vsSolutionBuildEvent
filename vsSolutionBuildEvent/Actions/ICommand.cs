/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    public interface ICommand
    {
        /// <summary>
        /// SBE-Scripts engine.
        /// </summary>
        ISobaScript SBEScript { get; }

        /// <summary>
        /// E-MSBuild engine.
        /// </summary>
        IEvMSBuild MSBuild { get; }

        /// <summary>
        /// Used environment.
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Event type of the last action.
        /// </summary>
        SolutionEventType EventType { get; }

        /// <summary>
        /// Find and execute action using specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="type">The type of event.</param>
        /// <param name="force">If true, ignore <see cref="ISolutionEvent.Enabled"/> option.</param>
        /// <returns>true if the action was executed.</returns>
        bool exec(ISolutionEvent evt, SolutionEventType type, bool force = false);

        /// <inheritdoc cref="exec(ISolutionEvent, SolutionEventType, bool)"/>
        bool exec(ISolutionEvent evt, bool force = false);
    }
}
