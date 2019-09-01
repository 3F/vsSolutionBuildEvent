/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
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

namespace net.r_eg.SobaScript.SNode
{
    [Guid("99D0CB1C-ED79-4358-BF2B-67F2A5B1EEFB")]
    public interface ILevel
    {
        /// <summary>
        /// Type of level.
        /// </summary>
        LevelType Type { get; }

        /// <summary>
        /// Data of level.
        /// </summary>
        string Data { get; }

        /// <summary>
        /// Arguments of level.
        /// </summary>
        Argument[] Args { get; }

        /// <summary>
        /// Type of data.
        /// </summary>
        CValueType DataType { get; set; }

        /// <summary>
        /// Checks the argument types.
        /// </summary>
        /// <param name="types">The types that should be for this level.</param>
        /// <returns>True value if the Args contains arguments with specified types.</returns>
        bool Is(params ArgumentType[] types);

        /// <summary>
        /// Checks the argument types.
        /// </summary>
        /// <param name="exception">Use string for exception instead of boolean result.</param>
        /// <param name="types">The types that should be for this level.</param>
        /// <returns>True value if the Args contains arguments with specified types.</returns>
        bool Is(string exception, params ArgumentType[] types);
    }
}
