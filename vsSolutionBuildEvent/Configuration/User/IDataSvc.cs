/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Configuration.User
{
    [Guid("09660649-B7BC-43D3-8E8F-951308B953D5")]
    internal interface IDataSvc
    {
        /// <summary>
        /// Update Common property.
        /// </summary>
        /// <param name="isLoad">Update for loading or saving.</param>
        void updateCommon(bool isLoad);
    }
}
