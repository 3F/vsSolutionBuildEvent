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
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
        /// Formatting path to directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string PathFormat(this string path)
        {
            if(String.IsNullOrWhiteSpace(path)) {
                return String.Empty;
            }
            
            if(path[path.Length - 1] != Path.DirectorySeparatorChar) {
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }
    }
}