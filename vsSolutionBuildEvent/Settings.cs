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
using System.Reflection;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE
{
    public static class Settings
    {
        /// <summary>
        /// Debug mode for current application
        /// </summary>
        public static bool debugMode = false;

        /// <summary>
        /// Path to this library
        /// </summary>
        public static string LibPath
        {
            get {
                if(String.IsNullOrEmpty(_libPath)) {
                    _libPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar;
                }
                return _libPath;
            }
        }
        private static string _libPath;

        /// <summary>
        /// Root path for all operations
        /// </summary>
        public static string WorkPath
        {
            get
            {
                if(String.IsNullOrEmpty(_workPath)) {
                    throw new SBEException("WorkPath is empty or null");
                }
                return _workPath;
            }
        }
        private static string _workPath = null;

        public static void setWorkPath(string path)
        {
            _workPath = path;
        }
    }
}
