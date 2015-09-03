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

using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Clients
{
    /// <summary>
    /// Safe wrapper for work with IEvent2 from client.
    /// TODO: use events in EventLevel over IEvent instead of this wrapper for more convenience...
    /// </summary>
    internal sealed class SEvent2: IEvent2
    {
        /// <summary>
        /// Link to IEvent instance of the client library
        /// </summary>
        private IEvent2 link;

        /// <summary>
        /// Solution has been opened.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <param name="fNewSolution">true if the solution is being created. false if the solution was created previously or is being loaded.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionOpened(object pUnkReserved, int fNewSolution)
        {
            try {
                return link.solutionOpened(pUnkReserved, fNewSolution);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed solutionOpened: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// Solution has been closed.
        /// </summary>
        /// <param name="pUnkReserved">Reserved for future use.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int solutionClosed(object pUnkReserved)
        {
            try {
                return link.solutionClosed(pUnkReserved);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed solutionClosed: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'PRE' of the solution.
        /// Before any build actions have begun.
        /// </summary>
        /// <param name="pfCancelUpdate">Pointer to a flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onPre(ref int pfCancelUpdate)
        {
            try {
                return link.onPre(ref pfCancelUpdate);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onPre: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'Cancel/Abort' of the solution.
        /// When a build is being cancelled.
        /// </summary>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onCancel()
        {
            try {
                return link.onCancel();
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onCancel: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onPost(fSucceeded, fModified, fCancelCommand);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onPost: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onProjectPre(pHierProj, pCfgProj, pCfgSln, dwAction, ref pfCancel);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onProjectPre: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <summary>
        /// 'PRE' of Project.
        /// Before a project configuration begins to build.
        /// </summary>
        /// <param name="project">Project name.</param>
        /// <returns>If the method succeeds, it returns Codes.Success. If it fails, it returns an error code.</returns>
        public int onProjectPre(string project)
        {
            try {
                return link.onProjectPre(project);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onProjectPre: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onProjectPost(pHierProj, pCfgProj, pCfgSln, dwAction, fSuccess, fCancel);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onProjectPost: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onProjectPost(project, fSuccess);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onProjectPost: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onCommandDtePre(guid, id, customIn, customOut, ref cancelDefault);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onCommandDtePre: '{0}'", ex.Message);
            }
            return Codes.Failed;
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
            try {
                return link.onCommandDtePost(guid, id, customIn, customOut);
            }
            catch(Exception ex) {
                Log.Error("[Client library] Failed onCommandDtePost: '{0}'", ex.Message);
            }
            return Codes.Failed;
        }

        /// <param name="link">IEvent instance</param>
        public SEvent2(IEvent2 o)
        {
            link = o;
            if(link == null) {
                link = new SEvent2Empty();
            }
        }
    }
}