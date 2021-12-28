/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
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
