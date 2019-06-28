/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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

#if VSSDK_15_AND_NEW
using System.Threading.Tasks;
#endif

namespace net.r_eg.vsSBE
{
    internal interface IPkg
    {
        CancellationToken CancellationToken { get; }

#if VSSDK_15_AND_NEW

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        Task<ToolWindowPane> getToolWindowAsync(Type type, int id);

        /// <param name="type">service type.</param>
        /// <returns></returns>
        Task<object> getSvcAsync(Type type);

#else

        /// <summary>
        /// Finds or creates tool window.
        /// </summary>
        /// <param name="type">tool window type</param>
        /// <param name="id">tool window id</param>
        /// <returns></returns>
        ToolWindowPane getToolWindow(Type type, int id);

        /// <param name="type">service type.</param>
        /// <returns></returns>
        object getSvc(Type type);

#endif

    }
}
