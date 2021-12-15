/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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

using net.r_eg.vsSBE.Bridge;

namespace ClientDemo
{
    public class Event: IEvent2
    {
        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            Log._.info("Entering solutionOpened(object pUnkReserved, int fNewSolution)");
            return Codes.Success;
        }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionClosed(object pUnkReserved)
        {
            Log._.info("Entering solutionClosed(object pUnkReserved)");
            return Codes.Success;
        }

        /// <summary>
        /// 'PRE' of the solution.
        /// Before any build actions have begun.
        /// </summary>
        /// <param name="pfCancelUpdate">Pointer to a flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onPre(ref int pfCancelUpdate)
        {
            Log._.info("Entering onPre(ref int pfCancelUpdate)");
            return Codes.Success;
        }

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// When a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCancel()
        {
            Log._.info("Entering onCancel()");
            return Codes.Success;
        }

        /// <summary>
        /// 'POST' of the solution.
        /// When a build is completed.
        /// </summary>
        /// <param name="fSucceeded">true if no update actions failed.</param>
        /// <param name="fModified">true if any update action succeeded.</param>
        /// <param name="fCancelCommand">true if update actions were canceled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onPost(int fSucceeded, int fModified, int fCancelCommand)
        {
            Log._.info("Entering onPost(int fSucceeded, int fModified, int fCancelCommand)");
            return Codes.Success;
        }

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
        public int onProjectPre(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, ref int pfCancel)
        {
            Log._.info("Entering onProjectPre(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, ref int pfCancel)");
            return Codes.Success;
        }

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPre(string project)
        {
            Log._.info("Entering onProjectPre(string project)");
            return Codes.Success;
        }

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
        public int onProjectPost(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, int fSuccess, int fCancel)
        {
            Log._.info("Entering onProjectPost(object pHierProj, object pCfgProj, object pCfgSln, uint dwAction, int fSuccess, int fCancel)");
            return Codes.Success;
        }

        /// <summary>
        /// 'POST' of Project.
        /// After a project configuration is finished building.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <param name="fSuccess">Flag indicating success.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPost(string project, int fSuccess)
        {
            Log._.info("Entering onProjectPost(string project, int fSuccess)");
            return Codes.Success;
        }

        /// <summary>
        /// Before executing Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <param name="cancelDefault">Whether the command has been cancelled.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            Log._.info("Entering onCommandDtePre(string guid, int id, object customIn, object customOut, ref bool cancelDefault)");
            return Codes.Success;
        }

        /// <summary>
        /// After executed Command ID for EnvDTE.
        /// </summary>
        /// <param name="guid">The GUID.</param>
        /// <param name="id">The command ID.</param>
        /// <param name="customIn">Custom input parameters.</param>
        /// <param name="customOut">Custom output parameters.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCommandDtePost(string guid, int id, object customIn, object customOut)
        {
            Log._.info("Entering onCommandDtePost(string guid, int id, object customIn, object customOut)");
            return Codes.Success;
        }
    }
}
