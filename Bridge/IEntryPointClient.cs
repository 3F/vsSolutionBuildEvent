/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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

namespace net.r_eg.vsSBE.Bridge
{
    /// <summary>
    /// Specifies work with client library
    /// </summary>
    [Guid("7586B777-5104-4BE4-9CA3-E0F7A5E2CE7A")]
    public interface IEntryPointClient
    {
        /// <summary>
        /// Type of implementation.
        /// </summary>
        ClientType Type { get; }

        /// <summary>
        /// Entry point of core library.
        /// Use this for additional work with core library.
        /// </summary>
        IEntryPointCore Core { set; }

        /// <summary>
        /// Version of core library.
        /// Use this for internal settings in client if needed.
        /// </summary>
        IVersion Version { set; }

        /// <summary>
        /// Should provide instance for handling IEvent2 by client from core library.
        /// </summary>
        IEvent2 Event { get; }

        /// <summary>
        /// Should provide instance for handling IBuild by client from core library.
        /// </summary>
        IBuild Build { get; }

        /// <summary>
        /// Load with DTE2 context.
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        void load(object dte2);

        /// <summary>
        /// Load with isolated environment.
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        void load(string sln, Dictionary<string, string> properties);

        /// <summary>
        /// Load with empty environment.
        /// </summary>
        void load();
    }
}
