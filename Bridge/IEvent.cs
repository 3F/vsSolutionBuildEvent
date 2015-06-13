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
using Microsoft.VisualStudio.Shell.Interop;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("FD4E35DB-9509-4353-9F2A-C31B7B1E63B8")]
    public interface IEvent
    {
        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int solutionOpened(object pUnkReserved, int fNewSolution);

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int solutionClosed(object pUnkReserved);

        /// <summary>
        /// 'PRE' of the solution.
        /// Before any build actions have begun.
        /// </summary>
        /// <param name="pfCancelUpdate">Pointer to a flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onPre(ref int pfCancelUpdate);

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// When a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onCancel();

        /// <summary>
        /// 'POST' of the solution.
        /// When a build is completed.
        /// </summary>
        /// <param name="fSucceeded">true if no update actions failed.</param>
        /// <param name="fModified">true if any update action succeeded.</param>
        /// <param name="fCancelCommand">true if update actions were canceled.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onPost(int fSucceeded, int fModified, int fCancelCommand);

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="pHierProj">Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="pfCancel">Pointer to a flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onProjectPre(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel);

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onProjectPre(string project);

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="pHierProj">Pointer to a hierarchy project object.</param>
        /// <param name="pCfgProj">Pointer to a configuration project object.</param>
        /// <param name="pCfgSln">Pointer to a configuration solution object.</param>
        /// <param name="dwAction">Double word containing the action.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <param name="fCancel">Flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onProjectPost(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel);

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onProjectPost(string project, int fSuccess);

        /// <summary>
        /// Before executing Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <param name="cancelDefault">Whether the command has been cancelled.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault);

        /// <summary>
        /// After executed Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <returns>If the method succeeds, it returns VSConstants.S_OK. If it fails, it returns an error code.</returns>
        int onCommandDtePost(string guid, int id, object customIn, object customOut);
    }
}
