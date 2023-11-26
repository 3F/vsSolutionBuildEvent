/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Main container of the 'Solution Events'
    /// </summary>
    [Guid("552AA0E0-9EFC-4394-B18B-41CF2F549FB3")]
    public interface ISolutionEvent
    {
        /// <summary>
        /// Status of activation.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Unique name for identification.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// About event.
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Support of the MSBuild engine.
        /// </summary>
        bool SupportMSBuild { get; set; }

        /// <summary>
        /// Support of the SBE-Scripts engine.
        /// </summary>
        bool SupportSBEScripts { get; set; }

        /// <summary>
        /// Ignore all actions if the build failed
        /// </summary>
        bool IgnoreIfBuildFailed { get; set; }

        /// <summary>
        /// The context of action.
        /// </summary>
        BuildType BuildType { get; set; }

        /// <summary>
        /// User interaction.
        /// Waiting until user presses yes/no etc,
        /// </summary>
        bool Confirmation { get; set; }

        /// <summary>
        /// Run only for a specific configuration of solution
        /// strings format as:
        ///   'configname'|'platformname'
        ///   Compatible with: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivscfg.get_displayname.aspx
        /// </summary>
        string[] ToConfiguration { get; set; }

        /// <summary>
        /// Run for selected projects with the Execution-Order
        /// </summary>
        IExecutionOrder[] ExecutionOrder { get; set; }

        /// <summary>
        /// Handling process
        /// </summary>
        IEventProcess Process { get; set; }

        /// <summary>
        /// Processing mode
        /// </summary>
        IMode Mode { get; set; }

        /// <summary>
        /// Unique identifier at runtime.
        /// </summary>
        Guid Id { get; }
    }
}
