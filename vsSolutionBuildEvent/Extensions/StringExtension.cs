﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Extensions;

namespace net.r_eg.vsSBE.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Calculate MD5 hash from string.
        /// </summary>
        /// <param name="str">String for calculating.</param>
        /// <returns>MD5 Hash code.</returns>
        public static string MD5Hash(this string str)
        {
            using(MD5 md5 = MD5.Create()) {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(str)).BytesToHexView();
            }
        }

        /// <summary>
        /// Calculate SHA-1 hash from string.
        /// </summary>
        /// <param name="str">String for calculating.</param>
        /// <returns>SHA-1 Hash code.</returns>
        public static string SHA1Hash(this string str)
        {
            using(SHA1 sha1 = SHA1.Create()) {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(str)).BytesToHexView();
            }
        }

        /// <summary>
        /// BSTR. Directory where visual studio executable was installed.
        /// http://technet.microsoft.com/en-us/microsoft.visualstudio.shell.interop.__vsspropid%28v=vs.71%29.aspx
        /// </summary>
        /// <param name="ptr">stub.</param>
        /// <returns></returns>
        public static string GetDevEnvDir(this string ptr)
        {
#if SDK15_OR_HIGH
            ThreadHelper.ThrowIfNotOnUIThread(); //TODO: upgrade to 15
#endif

            IVsShell shell = (IVsShell)Package.GetGlobalService(typeof(SVsShell));
            shell.GetProperty((int)__VSSPROPID.VSSPROPID_InstallDirectory, out object dirObject);

            string dir = (string)dirObject;

            return string.IsNullOrEmpty(dir) ? PropertyNames.UNDEFINED : dir.DirectoryPathFormat();
        }

        /// <summary>
        /// Comparing string Guids.
        /// </summary>
        /// <param name="g1">Guid 1</param>
        /// <param name="g2">Guid 2</param>
        /// <returns>equality or not</returns>
        public static bool CompareGuids(this string g1, string g2)
        {
            if(g1 == g2) {
                return true; // checked 'as is' inc. null values
            }

            if(String.IsNullOrEmpty(g1) || String.IsNullOrEmpty(g2)) {
                return false;
            }

            // {AF0B53BF-2F92-482C-B3D5-F3E804813100} & AF0B53BF-2F92-482C-B3D5-F3E804813100
            const int CURLY = 38;

            if(g1.Length == CURLY) {
                g1 = g1.Substring(1, g1.Length - 2);
            }

            if(g2.Length == CURLY) {
                g2 = g2.Substring(1, g2.Length - 2);
            }

            return g1.Equals(g2, StringComparison.OrdinalIgnoreCase);
        }
    }
}