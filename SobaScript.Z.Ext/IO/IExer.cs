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
using System.Runtime.InteropServices;

namespace net.r_eg.SobaScript.Z.Ext.IO
{
    public interface IExer
    {
        /// <summary>
        /// Initial directory for processes.
        /// </summary>
        string BasePath { get; set; }

        /// <summary>
        /// Execute file with arguments. 
        /// Uses synchronous read operations.
        /// </summary>
        /// <param name="file">File to execute.</param>
        /// <param name="args">Arguments to file.</param>
        /// <param name="hidden">Hide process if true.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        /// <returns>stdout</returns>
        /// <exception cref="ExternalException"></exception>
        string Run(string file, string args, bool hidden, int timeout = 0);

        /// <summary>
        /// Execute command with standard command-line interpreter.
        /// Uses asynchronous read operations.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="uid">Unique id for streams.</param>
        /// <param name="waiting">Waiting for completion.</param>
        /// <param name="hidden">Hiding result.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        void UseShell(string cmd, Guid uid, bool waiting, bool hidden, int timeout = 0);

        /// <summary>
        /// Execute command with standard command-line interpreter.
        /// Uses asynchronous read operations and unspecified identifier for streams.
        /// </summary>
        /// <param name="cmd">Command to execute</param>
        /// <param name="waiting">Waiting for completion.</param>
        /// <param name="hidden">Hiding result.</param>
        /// <param name="timeout">How long to wait the execution, in seconds. 0 value - infinitely.</param>
        void UseShell(string cmd, bool waiting, bool hidden, int timeout = 0);

        /// <summary>
        /// Pulls latest async received data from stdout stream.
        /// Each calling resets buffer.
        /// </summary>
        /// <param name="id">Identifier of stream.</param>
        /// <returns>Received data; or null if id is not found or buffer is not initialized.</returns>
        string PullStdOut(Guid id);

        /// <summary>
        /// Pulls latest async received data from stdout stream.
        /// Each calling resets buffer.
        /// </summary>
        /// <param name="id">Identifier of stream.</param>
        /// <returns>Received data; or null if id is not found or buffer is not initialized.</returns>
        string PullStdErr(Guid id);
    }
}
