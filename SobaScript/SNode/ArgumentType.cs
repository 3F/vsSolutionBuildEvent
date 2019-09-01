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

namespace net.r_eg.SobaScript.SNode
{
    /// <summary>
    /// Specifies of possible types for arguments.
    /// </summary>
    public enum ArgumentType
    {
        /// <summary>
        /// Unspecified mixed data.
        /// </summary>
        Mixed,

        /// <summary>
        /// Common string.
        /// </summary>
        String,

        /// <summary>
        /// String from single quotes.
        /// </summary>
        StringSingle,

        /// <summary>
        /// String from double quotes.
        /// </summary>
        StringDouble,

        /// <summary>
        /// Single symbol from single quotes. 
        /// </summary>
        Char,

        /// <summary>
        /// Boolean data.
        /// </summary>
        Boolean,

        /// <summary>
        /// Signed Integer number.
        /// </summary>
        Integer,

        /// <summary>
        /// Signed floating-point number with single-precision.
        /// </summary>
        Float,

        /// <summary>
        /// Signed floating-point number with double-precision.
        /// </summary>
        Double,

        /// <summary>
        /// Unspecified predefined data.
        /// </summary>
        EnumOrConst,

        /// <summary>
        /// Predefined data as Enum.
        /// </summary>
        Enum,

        /// <summary>
        /// Predefined data as Const.
        /// </summary>
        Const,

        /// <summary>
        /// Object data. Similar as array with mixed data.
        /// Format: { "p1", true, { 12, 'n', -4.5f }, 12d }
        /// </summary>
        Object,
    }
}
