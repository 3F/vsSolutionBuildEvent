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

using System.IO;
using System.Text;

namespace net.r_eg.SobaScript.Z.Ext
{
    public interface IEncDetector
    {
        /// <summary>
        /// Detects encoding for specified stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="confidence">Detected confidence.</param>
        /// <returns>null if can't be detected.</returns>
        Encoding Detect(Stream stream, out float confidence);

        /// <summary>
        /// Detects encoding for specified stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>null if can't be detected.</returns>
        Encoding Detect(Stream stream);

        /// <summary>
        /// Try to fix the wrong encoded string.
        /// </summary>
        /// <param name="input">Input data.</param>
        /// <param name="container">Known information about bytes.</param>
        /// <param name="confidence">To limit accepted confidence.</param>
        /// <returns>Returns null if detected confidence less than input limit. Otherwise, re-encoded string.</returns>
        string FixEncoding(string input, Encoding container, float confidence = 0.92f);
    }
}
