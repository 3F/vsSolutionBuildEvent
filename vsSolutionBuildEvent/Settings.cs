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
        /// Name of vsSBE log item in OutputWindowPane
        /// </summary>
        public const string OWP_ITEM_VSSBE = "Solution Build-Events";

        /// <summary>
        /// Debug mode for this application
        /// </summary>
        public static bool debugMode = false;

        /// <summary>
        /// Ignores all actions if value as true
        /// Support of cycle control, e.g.: PRE -> POST [recursive DTE: PRE -> POST] -> etc.
        /// </summary>
        public static volatile bool silentModeActions = false;

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
        public static string WorkingPath
        {
            get
            {
                if(String.IsNullOrEmpty(_workingPath)) {
                    throw new SBEException("WorkingPath is empty or null");
                }
                return _workingPath;
            }
        }
        private static string _workingPath = null;

        public static void setWorkingPath(string path)
        {
            _workingPath = path;
        }
    }
}
