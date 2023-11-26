/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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

#if SDK15_OR_HIGH
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

#if SDK15_OR_HIGH
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

#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread();
#endif

            IVsSolution sln = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            sln.GetProjectOfUniqueName(dProject.FullName, out IVsHierarchy hr);

            return hr;
        }
    }
}
