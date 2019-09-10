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
using System.Collections.Generic;
using net.r_eg.SobaScript.Components;

namespace net.r_eg.SobaScript.Mapper
{
    public interface IInspector
    {
        /// <summary>
        /// List of constructed root-data.
        /// </summary>
        IEnumerable<INodeInfo> Root { get; }

        /// <summary>
        /// List of constructed data by identification of node.
        /// </summary>
        /// <param name="ident">Identifier of node.</param>
        /// <returns></returns>
        IEnumerable<INodeInfo> GetBy(NodeIdent ident);

        /// <summary>
        /// List of constructed data by type of component.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<INodeInfo> GetBy(Type type);

        /// <summary>
        /// List of constructed data by IComponent.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IEnumerable<INodeInfo> GetBy(IComponent component);
    }
}
