/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Specifies types of available items.
    /// </summary>
    [Guid("3DD1EA27-A02E-4982-B71C-F72329CAC723")]
    public enum ItemType
    {
        /// <summary>
        /// The item based on errors/warnings container.
        /// </summary>
        EW,
    }
}
