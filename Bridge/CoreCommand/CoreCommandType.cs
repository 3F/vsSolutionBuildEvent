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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Bridge.CoreCommand
{
    /// <summary>
    /// Represents available commands for core library
    /// </summary>
    [Guid("52F17F05-7097-4E5E-8263-0696C9EA4205")]
    public enum CoreCommandType
    {
        /// <summary>
        /// Command by default
        /// </summary>
        Default = Nop,

        /// <summary>
        /// No Operation
        /// </summary>
        Nop = 0x90,

        /// <summary>
        /// Returns latest pushed command
        /// </summary>
        LastCommand = 0x100,

        /// <summary>
        /// To abort latest command if it's possible
        /// </summary>
        AbortCommand = 0x101,

        /// <summary>
        /// Unspecified raw command
        /// </summary>
        RawCommand = 0x110,

        /// <summary>
        /// Cancel build operation if it's available for abort
        /// </summary>
        BuildCancel = 0x200,
    }
}
