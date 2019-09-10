/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Mapper contributors: https://github.com/3F/SobaScript.Mapper/graphs/contributors
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

namespace net.r_eg.SobaScript.Mapper
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PropertyAttribute: Attribute, IAttrDomLevelB
    {
        /// <summary>
        /// Property name.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current property.
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Value type for getting.
        /// </summary>
        public CValType Get
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Value type for setting.
        /// </summary>
        public CValType Set
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of the parent specification (property/method/etc.) if exists or null.
        /// </summary>
        public string Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Actual/real method name of the parent specification if exists or null.
        /// </summary>
        public string Method
        {
            get;
            protected set;
        }

        /// <param name="name">Property name.</param>
        /// <param name="description">Description for current property.</param>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        public PropertyAttribute(string name, string description, CValType get = CValType.Void, CValType set = CValType.Void)
        {
            Name        = name;
            Description = description;
            Get         = get;
            Set         = set;
        }

        /// <param name="name">Property name.</param>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        public PropertyAttribute(string name, CValType get = CValType.Void, CValType set = CValType.Void)
            : this(name, string.Empty, get, set)
        {

        }

        /// <param name="name">Property name.</param>
        /// <param name="description">Description for current property.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        public PropertyAttribute(string name, string description, string parent, string method, CValType get = CValType.Void, CValType set = CValType.Void)
            : this(name, description, get, set)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Property name.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        public PropertyAttribute(string name, string parent, string method, CValType get = CValType.Void, CValType set = CValType.Void)
            : this(name, string.Empty, parent, method, get, set)
        {

        }
    }
}
