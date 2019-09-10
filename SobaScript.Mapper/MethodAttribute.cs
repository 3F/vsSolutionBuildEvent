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
using System.Linq;
using net.r_eg.SobaScript.Exceptions;

namespace net.r_eg.SobaScript.Mapper
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodAttribute: Attribute, IAttrDomLevelB
    {
        /// <summary>
        /// Method name.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current method.
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// The return value for current method.
        /// </summary>
        public CValType Return
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Arguments of method.
        /// </summary>
        public TArgument[] Arguments
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

        /// <param name="name">Method name.</param>
        /// <param name="description">Description for current method.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string description, CValType ret = CValType.Void, params CValType[] args)
        {
            Name        = name;
            Description = description;
            Return      = ret;
            Arguments   = args.Select(arg => new TArgument(arg)).ToArray();
        }

        /// <param name="name">Method name.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, CValType ret = CValType.Void, params CValType[] args)
            : this(name, string.Empty, ret, args)
        {

        }

        /// <param name="name">Method name.</param>
        /// <param name="description">Description for current method.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string description, string parent, string method, CValType ret = CValType.Void, params CValType[] args)
            : this(name, description, ret, args)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Method name.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string parent, string method, CValType ret = CValType.Void, params CValType[] args)
            : this(name, string.Empty, parent, method, ret, args)
        {

        }

        /// <param name="name">Method name.</param>
        /// <param name="description">Description for current method.</param>
        /// <param name="argsName">Arguments of method by name.</param>
        /// <param name="argsDesc">Description for arguments.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string description, string[] argsName, string[] argsDesc, CValType ret, params CValType[] args)
            : this(name, description, ret, args)
        {
            if(argsName == null || argsDesc == null) {
                throw new ArgumentException($"null value is not valid for {nameof(argsName)}/{nameof(argsDesc)}");
            }

            if(args.Length != argsName.Length || args.Length != argsDesc.Length) {
                throw new MismatchException($"CValueType[] is not equal by count with argsName/argsDesc :: {name}");
            }

            Arguments = args.Select((arg, i) => new TArgument(arg, argsName[i], argsDesc[i])).ToArray();
        }

        /// <param name="name">Method name.</param>
        /// <param name="argsName">Arguments of method by name.</param>
        /// <param name="argsDesc">Description for arguments.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string[] argsName, string[] argsDesc, CValType ret, params CValType[] args)
            : this(name, string.Empty, argsName, argsDesc, ret, args)
        {

        }

        /// <param name="name">Method name.</param>
        /// <param name="description">Description for current method.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="argsName">Arguments of method by name.</param>
        /// <param name="argsDesc">Description for arguments.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string description, string parent, string method, string[] argsName, string[] argsDesc, CValType ret, params CValType[] args)
            : this(name, description, argsName, argsDesc, ret, args)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Method name.</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exists or null.</param>
        /// <param name="method">Actual/real method name of the parent specification if exists or null.</param>
        /// <param name="argsName">Arguments of method by name.</param>
        /// <param name="argsDesc">Description for arguments.</param>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        public MethodAttribute(string name, string parent, string method, string[] argsName, string[] argsDesc, CValType ret, params CValType[] args)
            : this(name, string.Empty, parent, method, argsName, argsDesc, ret, args)
        {

        }
    }
}
