/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("7631EC6E-A74C-464E-AEA2-19EB6F8A6780")]
    public interface IEvent2: IEventLight, IEventLight2
    {
        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="pHierProj">Unspecified IVsHierarchy (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Unspecified IVsCfg (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Unspecified IVsCfg (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="pfCancel">Pointer to a flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        int onProjectPre(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, ref int pfCancel);

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="pHierProj">Unspecified IVsHierarchy (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Unspecified IVsCfg (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Unspecified IVsCfg (Microsoft.VisualStudio.Shell.Interop.dll) Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <param name="fCancel">Flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        int onProjectPost(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, int fSuccess, int fCancel);
    }
}
