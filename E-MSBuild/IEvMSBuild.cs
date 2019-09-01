/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) E-MSBuild contributors: https://github.com/3F/E-MSBuild/graphs/contributors
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
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;
using net.r_eg.Varhead;

namespace net.r_eg.EvMSBuild
{
    /// <summary>
    /// [E-MSBuild]
    /// 
    /// Advanced Evaluator of MSBuild scripts aka Advanced MSBuild 
    /// with user-variables support through Varhead and more.
    /// https://github.com/3F/E-MSBuild
    /// </summary>
    [Guid("958B9A32-BE6F-4B74-A98A-AC99099A63A5")]
    public interface IEvMSBuild: IEvaluator
    {
        event EventHandler<PropertyArgs> GlobalPropertyChanged;

        /// <summary>
        /// Container of user-variables through Varhead.
        /// </summary>
        IUVars UVars { get; }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through E-MSBuild supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        string Eval(string data);

        /// <summary>
        /// Get evaluated variable or property value for specified scope.
        /// </summary>
        /// <param name="name">Access to property or variable by its name.</param>
        /// <param name="scope">Where is placed. null value for global or unspecified scope.</param>
        /// <returns>Evaluated value.</returns>
        string GetPropValue(string name, string scope = null);

        /// <summary>
        /// List all properties for specified scope.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        IEnumerable<PropertyItem> ListProperties(string scope = null);

        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="scope">Where to place. null value for global or unspecified scope.</param>
        /// <returns>Returns true if the value has changes, otherwise false.</returns>
        bool SetGlobalProperty(string name, string value, string scope = null);

        /// <param name="name"></param>
        /// <param name="scope">Where is placed. null value for global or unspecified scope.</param>
        /// <returns>Returns true if the property was removed.</returns>
        bool RemoveGlobalProperty(string name, string scope = null);
    }
}
