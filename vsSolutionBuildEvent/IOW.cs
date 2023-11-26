/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE
{
    [Guid("7C0E9E6B-7BDC-4D0F-97D2-6F3F30FBA368")]
    public interface IOW
    {
        /// <summary>
        /// Access to OutputWindow.
        /// </summary>
        EnvDTE.OutputWindow OutputWindow { get; }

        /// <summary>
        /// Getting item of the output window by name.
        /// </summary>
        /// <param name="name">Name of item</param>
        /// <param name="createIfNotExist">Flag of creating. It means ~create new pane if this item does not exist etc.</param>
        /// <returns></returns>
        EnvDTE.OutputWindowPane getByName(string name, bool createIfNotExist);

        /// <summary>
        /// Removes pane by name of item.
        /// </summary>
        /// <param name="name"></param>
        void deleteByName(string name);

        /// <summary>
        /// Removes pane with selected GUID.
        /// </summary>
        /// <param name="guid"></param>
        void deleteByGuid(Guid guid);
    }
}
