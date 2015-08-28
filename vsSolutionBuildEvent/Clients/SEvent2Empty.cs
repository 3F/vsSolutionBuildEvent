/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Clients
{
    /// <summary>
    /// The empty variant of safe wrapper for work with IEvent2 from client.
    /// TODO: use events in EventLevel over IEvent instead of this wrapper for more convenience...
    /// </summary>
    internal sealed class SEvent2Empty: IEvent2
    {
        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            return Codes.Success;
        }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionClosed(object pUnkReserved)
        {
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
            return Codes.Success;
        }

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// When a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCancel()
        {
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
            return Codes.Success;
        }
    }
}