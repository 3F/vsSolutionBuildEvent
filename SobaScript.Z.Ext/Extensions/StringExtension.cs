/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Ext contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace net.r_eg.SobaScript.Z.Ext.Extensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns full path to directory where the current assembly is running.
        /// </summary>
        /// <param name="_">stub: "".GetExecDir() </param>
        /// <returns></returns>
        public static string GetExecDir(this string _)
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).DirectoryPathFormat();
        }

        /// <summary>
        /// Calculate MD5 hash from string.
        /// </summary>
        /// <param name="str">String for calculating.</param>
        /// <returns>MD5 Hash code.</returns>
        internal static string MD5Hash(this string str)
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
        internal static string SHA1Hash(this string str)
        {
            using(SHA1 sha1 = SHA1.Create()) {
                return sha1.ComputeHash(Encoding.UTF8.GetBytes(str)).BytesToHexView();
            }
        }

        internal static string FormatDirPath(this string dir)
        {
            return dir?.DirectoryPathFormat() ?? throw new ArgumentNullException(nameof(dir));
        }

        #region MvsSln copy-paste

        //* The MIT License (MIT)
        //* Copyright (c) 2013-2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
        //* Copyright (c) MvsSln contributors: https://github.com/3F/MvsSln/graphs/contributors

        /// <summary>
        /// Formatting of the path to directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string DirectoryPathFormat(this string path)
        {
            if(string.IsNullOrWhiteSpace(path)) {
                return Path.DirectorySeparatorChar.ToString();
            }
            path = path.Trim();

            if(!IsDirectoryPath(path)) {
                path += Path.DirectorySeparatorChar;
            }
            
            return path;
        }

        /// <summary>
        /// Check if this is a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryPath(this string path)
        {
            return IsEndSlash(path?.TrimEnd());
        }

        private static bool IsEndSlash(this string path)
        {
            if(path == null || path.Length < 1) {
                return false;
            }

            char c = path[path.Length - 1];
            if(c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) {
                return true;
            }
            return false;
        }

        #endregion
    }
}