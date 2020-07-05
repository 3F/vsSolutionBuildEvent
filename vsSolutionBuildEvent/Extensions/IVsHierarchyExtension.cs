/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using DProject = EnvDTE.Project;

namespace net.r_eg.vsSBE.Extensions
{
    public static class IVsHierarchyExtension
    {
        public static Guid GetProjectGuid(this IVsHierarchy pHierProj)
        {
            if(pHierProj == null) {
                return Guid.Empty;
            }

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread();
#endif

            pHierProj.GetGuidProperty(
                (uint)VSConstants.VSITEMID.Root, 
                (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, 
                out Guid id
            );

            return id;
        }

        public static DProject GetEnvDteProject(this IVsHierarchy pHierProj)
        {
            if(pHierProj == null) {
                return null;
            }

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread();
#endif

            pHierProj.GetProperty(
                (uint)VSConstants.VSITEMID.Root, 
                (int)__VSHPROPID.VSHPROPID_ExtObject, 
                out object dteProject
            );

            return (DProject)dteProject;
        }

        public static IVsHierarchy GetIVsHierarchy(this DProject dProject)
        {
            if(dProject == null) {
                return null;
            }

#if VSSDK_15_AND_NEW
            ThreadHelper.ThrowIfNotOnUIThread();
#endif

            IVsSolution sln = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            sln.GetProjectOfUniqueName(dProject.FullName, out IVsHierarchy hr);

            return hr;
        }
    }
}
