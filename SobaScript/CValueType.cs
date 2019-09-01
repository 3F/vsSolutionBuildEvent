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

namespace net.r_eg.SobaScript
{
    /// <summary>
    /// Specification of possible values for components and other places of the core
    /// </summary>
    public enum CValueType
    {
        /// <summary>
        /// Specifies that the:
        /// * Method doesn't return a value and/or takes no parameters
        /// * Property: for setting (readonly) / for getting - i.e. only as setter
        /// It's also used for binding the next Property/Method.
        /// </summary>
        Void,

        /// <summary>
        /// Value of different or untyped / uncertain types
        /// </summary>
        Mixed,

        /// <summary>
        /// Any stream data input.
        /// It's also used for binding the multiline data for:
        /// * Property: #[Component property: multiline data]
        /// * Method: #[Component method("arg"): multiline data]
        /// </summary>
        Input,

        /// <summary>
        /// Predefined data
        /// </summary>
        Enum,

        Const,

        String,

        Boolean,

        Integer,

        Float,

        /// <summary>
        /// Unsigned types
        /// </summary>
        UInteger,

        UFloat,

        /// <summary>
        /// Sequential list of mixed values.
        /// format: 1,2,3,4,5,6,7
        /// </summary>
        List,

        /// <summary>
        /// Object data. Similar as array with mixed data.
        /// Format: { "p1", true, { 12, 'n', -4.5f }, 12d }
        /// </summary>
        Object,

        /// <summary>
        /// Mixed expressions like Conditional Expression etc.
        /// </summary>
        Expression,
    }
}
