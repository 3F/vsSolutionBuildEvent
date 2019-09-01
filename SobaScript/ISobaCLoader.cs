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
using System.Collections.Generic;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using net.r_eg.Varhead;

namespace net.r_eg.SobaScript
{
    public interface ISobaCLoader
    {
        /// <summary>
        /// Only enabled components from `Registered`.
        /// </summary>
        IEnumerable<IComponent> Components { get; }

        /// <summary>
        /// All registered components.
        /// </summary>
        IEnumerable<IComponent> Registered { get; }

        /// <summary>
        /// Used instance of the E-MSBuild engine.
        /// </summary>
        IEvMSBuild EvMSBuild { get; }

        /// <summary>
        /// Varhead container for user-variables.
        /// </summary>
        IUVars UVars { get; }

        /// <summary>
        /// Get component for specified type.
        /// </summary>
        /// <param name="type">The type of registered component.</param>
        /// <returns>Found instance or null value if this type is not registered.</returns>
        IComponent GetComponent(Type type);

        /// <summary>
        /// To register new component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Success of operation.</returns>
        bool Register(IComponent component);

        /// <summary>
        /// To unregister specific component.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>Success of operation.</returns>
        bool Unregister(IComponent component);

        /// <summary>
        /// To unregister all available components.
        /// </summary>
        void Unregister();
    }
}