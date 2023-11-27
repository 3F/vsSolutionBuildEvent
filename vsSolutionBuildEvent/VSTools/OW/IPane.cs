/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.VSTools.OW
{
    [Guid("9C9CEFB5-BECE-4DB8-87EF-5C38AFA5EBD7")]
    public interface IPane
    {
        /// <summary>
        /// Gets the GUID for the pane.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Moves the focus to the current item.
        /// </summary>
        void Activate();

        /// <summary>
        /// Clears all text from pane.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sends a text string into pane.
        /// </summary>
        /// <param name="text"></param>
        void OutputString(string text);
    }
}
