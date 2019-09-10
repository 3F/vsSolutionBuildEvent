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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class DefinitionAttribute: Attribute, IAttrDomLevelA
    {
        /// <summary>
        /// Definition name.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// About the definition.
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of parent specification if exists or null.
        /// </summary>
        public string Parent
        {
            get;
            protected set;
        }

        /// <param name="name">Definition name.</param>
        /// <param name="description">About of the definition</param>
        public DefinitionAttribute(string name, string description)
        {
            Name        = name;
            Description = description;
        }

        /// <param name="name">Definition name.</param>
        /// <param name="description">About the definition.</param>
        /// <param name="parent">Name of parent specification if exists or null.</param>
        public DefinitionAttribute(string name, string description, string parent)
        {
            Name        = name;
            Description = description;
            Parent      = parent;
        }
    }
}
