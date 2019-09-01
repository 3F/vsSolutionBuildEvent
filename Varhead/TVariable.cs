﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) Varhead contributors: https://github.com/3F/Varhead/graphs/contributors
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

namespace net.r_eg.Varhead
{
    public struct TVariable
    {
        /// <summary>
        /// Contains the evaluated data or escaped variable/property (without escape symbol)
        /// Using from current the unevaluated field
        /// </summary>
        public string evaluated;

        /// <summary>
        /// Contains the unevaluated mixed data
        /// May contain the another user-variable etc.
        /// </summary>
        public string unevaluated;

        /// <summary>
        /// Identifier of current variable.
        /// </summary>
        public string ident;

        /// <summary>
        /// Front-end variable name if used.
        /// </summary>
        public string name;

        /// <summary>
        /// Context of variable if used.
        /// </summary>
        public string scope;

        /// <summary>
        /// Current status of evaluation.
        /// </summary>
        public ValStatus status;

        /// <summary>
        /// Previous TUserVariable if exist.
        /// This probably can be used for self redefinition varname = varname
        /// e.g. for post-processing with MSBuild is required to evaluation of new value etc.
        /// </summary>
        public object prev;

        /// <summary>
        /// Flag of permanent storage.
        /// External containers or ~ .csproj, .vcxproj, ... etc.
        /// </summary>
        /// <remarks>reserved</remarks>
        public bool persistence;

        public TVariable(TVariable origin)
        {
            evaluated       = origin.evaluated;
            unevaluated     = origin.unevaluated;
            ident           = origin.ident;
            name            = origin.name;
            scope           = origin.scope;
            status          = origin.status;
            persistence     = origin.persistence;
            prev            = origin.prev;
        }
    }
}
