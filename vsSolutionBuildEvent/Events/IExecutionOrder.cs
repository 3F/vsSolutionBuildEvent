/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Declaring of the Execution order (or Events order)
    /// </summary>
    public interface IExecutionOrder
    {
        /// <summary>
        /// Project name
        /// </summary>
        string Project { get; set; }

        /// <summary>
        /// Range of execution
        /// </summary>
        ExecutionOrderType Order { get; set; }
    }

    public enum ExecutionOrderType
    {
        /// <summary>
        /// Before1 -> After1|Cancel 
        /// </summary>
        Before,
        /// <summary>
        /// After1 -> POST/Cancel 
        /// </summary>
        After
    }
}
