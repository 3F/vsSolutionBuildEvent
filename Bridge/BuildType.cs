/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Represents available context for actions.
    /// </summary>
    [Guid("70EC2620-D34E-407A-AB2B-6592D4974510")]
    public enum BuildType
    {
        /// <summary>
        /// Common context - any type or type by default
        /// </summary>
        Common = Int32.MaxValue,

        /// <summary>
        /// Unspecified type as Before action.
        /// </summary>
        Before = Common - 10,

        /// <summary>
        /// Unspecified type as After action.
        /// </summary>
        After = Common - 11,

        /// <summary>
        /// Unspecified type as Before and/then After action, 
        /// i.e. double action.
        /// </summary>
        BeforeAndAfter = Common - 12,

        /// <summary>
        /// Reserved type 1
        /// </summary>
        Reserved1 = Common - 13,

        /// <summary>
        /// Reserved type 2
        /// </summary>
        Reserved2 = Common - 14,

        /// <summary>
        /// 'build' action
        /// </summary>
        Build = 100,

        /// <summary>
        /// 'rebuild' action
        /// </summary>
        Rebuild = 101,

        /// <summary>
        /// 'clean' action
        /// </summary>
        Clean = 102,

        /// <summary>
        /// 'deploy' action
        /// </summary>
        Deploy = 103,

        /// <summary>
        /// 'Start Debugging' action
        /// </summary>
        Start = 104,

        /// <summary>
        /// 'Start Without Debugging' action
        /// </summary>
        StartNoDebug = 105,

        /// <summary>
        /// 'Publish' action
        /// </summary>
        Publish = 106,

        /// <summary>
        /// 'build' action for selection
        /// </summary>
        BuildSelection = 200,

        /// <summary>
        /// 'rebuild' action for selection
        /// </summary>
        RebuildSelection = 201,

        /// <summary>
        /// 'clean' action for selection
        /// </summary>
        CleanSelection = 202,

        /// <summary>
        /// 'deploy' action for selection
        /// </summary>
        DeploySelection = 203,

        /// <summary>
        /// 'Publish' action for selection
        /// </summary>
        PublishSelection = 204,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildOnlyProject = 205,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildOnlyProject = 206,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanOnlyProject = 207,

        /// <summary>
        /// 'Compile' action
        /// </summary>
        Compile = 300,

        /// <summary>
        /// 'Link only' action
        /// </summary>
        LinkOnly = 301,

        /// <summary>
        /// 'build' action for project
        /// </summary>
        BuildCtx = 302,

        /// <summary>
        /// 'rebuild' action for project
        /// </summary>
        RebuildCtx = 303,

        /// <summary>
        /// 'clean' action for project
        /// </summary>
        CleanCtx = 304,

        /// <summary>
        /// 'deploy' action for project
        /// </summary>
        DeployCtx = 305,

        /// <summary>
        /// 'Publish' action for project
        /// </summary>
        PublishCtx = 306,
    }
}
