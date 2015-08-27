/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Specifies work with core library
    /// </summary>
    [Guid("FF4EA5B6-61F6-43F7-8528-01CF4A482A37")]
    public interface IEntryPointCore
    {
        /// <summary>
        /// Event of core commands.
        /// </summary>
        event CoreCommandHandler CoreCommand;

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="debug">Optional flag of debug mode</param>
        void load(object dte2, bool debug = false);

        /// <summary>
        /// Load with DTE2 context
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        /// <param name="cfg">Specific settings</param>
        void load(object dte2, ISettings cfg);

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="debug">Optional flag of debug mode</param>
        void load(string sln, Dictionary<string, string> properties, bool debug = false);

        /// <summary>
        /// Load with isolated environment
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        /// <param name="cfg">Specific settings</param>
        void load(string sln, Dictionary<string, string> properties, ISettings cfg);
    }
}
