/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using net.r_eg.SobaScript;
using net.r_eg.vsSBE.Configuration;

namespace net.r_eg.vsSBE.API
{
    public interface IEventLevel: Bridge.IEvent, Bridge.IBuild
    {
        /// <summary>
        /// When the solution has been opened or created
        /// </summary>
        event EventHandler OpenedSolution;

        /// <summary>
        /// When the solution has been closed
        /// </summary>
        event EventHandler ClosedSolution;

        /// <summary>
        /// Used Environment
        /// </summary>
        IEnvironment Environment { get; }

        /// <summary>
        /// Binder of action
        /// </summary>
        Actions.Binder Action { get; }

        /// <summary>
        /// Manager of configurations.
        /// </summary>
        IManager ConfigManager { get; }
    }
}
