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
using Microsoft.Build.Evaluation;

namespace net.r_eg.EvMSBuild
{
    internal sealed class EvEnvBlank: IEvEnv, IEvMin
    {
        private readonly IEvMin ev;
        private readonly Project prj;

        /// <summary>
        /// An unified unscoped and out of Project instance the property value by its name.
        /// Remarks: Any property values cannot be null.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <returns>Found non-null property value or null if not.</returns>
        public string GetMutualPropValue(string name) => ev?.GetMutualPropValue(name);

        /// <summary>
        /// Get Project instance for work with data inside specified scope.
        /// </summary>
        /// <param name="ident">Abstract identifier of the specified scope. It can be a GUID, or FullPath, or project name, etc.</param>
        /// <returns>Expected the instance that is associated with the identifier or any default instance if not found any related to pushed ident.</returns>
        public Project GetProject(object ident) => prj;

        public EvEnvBlank()
        {
            prj = new Project();
        }

        public EvEnvBlank(IEvMin ev)
            : this()
        {
            this.ev = ev ?? throw new ArgumentNullException(nameof(ev));
        }
    }
}