/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Transmission event of the build-data
    /// </summary>
    [Guid("93EBB5FF-AC62-4511-AB27-F90F580FFD40")]
    public interface ITransmitter: ISolutionEvent 
    {

    }
}