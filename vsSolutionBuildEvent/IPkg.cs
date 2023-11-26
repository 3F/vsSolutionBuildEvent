/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Threading;
using Microsoft.VisualStudio.Shell;

#if SDK15_OR_HIGH
using System.Threading.Tasks;
#endif

namespace net.r_eg.vsSBE
{
    internal interface IPkg
    {
        CancellationToken CancellationToken { get; }

#if SDK15_OR_HIGH

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="create">try to create tool when true</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        Task<ToolWindowPane> getToolWindowAsync(Type type, bool create = true, int id = 0);

        /// <param name="type">service type.</param>
        /// <returns></returns>
        Task<object> getSvcAsync(Type type);

#else

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="create">try to create tool when true</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        ToolWindowPane getToolWindow(Type type, bool create = true, int id = 0);

        /// <param name="type">service type.</param>
        /// <returns></returns>
        object getSvc(Type type);

#endif

    }
}
