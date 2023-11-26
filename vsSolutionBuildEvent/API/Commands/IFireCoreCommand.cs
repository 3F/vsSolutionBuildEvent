/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace net.r_eg.vsSBE.API.Commands
{
    [Guid("28286176-B1A7-4725-AD38-384D58BC1C29")]
    public interface IFireCoreCommand
    {
        /// <summary>
        /// Send the core command for all clients.
        /// </summary>
        /// <param name="c"></param>
        void fire(CoreCommandArgs c);
    }
}