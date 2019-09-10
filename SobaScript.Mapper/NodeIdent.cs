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

using net.r_eg.SobaScript.Mapper.Extensions;

namespace net.r_eg.SobaScript.Mapper
{
    public struct NodeIdent
    {
        /// <summary>
        /// Name of the parent specification (property/method/etc.)
        /// </summary>
        public string parent;

        /// <summary>
        /// Actual/real method name of the parent specification.
        /// </summary>
        public string method;

        /// <summary>
        /// Actual class name if information is provided.
        /// </summary>
        public string className;

        public static bool operator ==(NodeIdent a, NodeIdent b)
        {
            return object.ReferenceEquals(a, null) ?
                    object.ReferenceEquals(b, null) : a.Equals(b);
        }

        public static bool operator !=(NodeIdent a, NodeIdent b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if(object.ReferenceEquals(obj, null) || !(obj is NodeIdent)) {
                return false;
            }

            var b = (NodeIdent)obj;

            return parent == b.parent
                    && method == b.method
                    && className == b.className;
        }

        public override int GetHashCode()
        {
            return 0.CalculateHashCode
            (
                parent,
                method,
                className
            );
        }

        /// <param name="parent">Name of the parent specification (property/method/etc.).</param>
        /// <param name="method">Actual/real method name of the parent specification.</param>
        /// <param name="className">Actual class name.</param>
        public NodeIdent(string parent, string method, string className)
        {
            this.parent     = parent;
            this.method     = method;
            this.className  = className;
        }

        /// <param name="parent">Name of the parent specification (property/method/etc.).</param>
        /// <param name="method">Actual/real method name of the parent specification.</param>
        public NodeIdent(string parent, string method)
            : this(parent, method, null)
        {

        }
    }
}
