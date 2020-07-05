/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Runtime.InteropServices;
using LibVersion = net.r_eg.vsSBE.Bridge.Version;

namespace net.r_eg.vsSBE.Bridge
{
    [Guid("EB5D3D29-C0D2-4CFC-B927-D6F43DFF37AD")]
    public class Info: IVersion
    {
        /// <summary>
        /// Main version number of library
        /// </summary>
        public System.Version Number
        {
            get { return LibVersion.number; }
        }

        /// <summary>
        /// Branch name from SCM
        /// </summary>
        public string BranchName
        {
            get { return LibVersion.B_NAME; }
        }

        /// <summary>
        /// SHA1 revision number from SCM
        /// </summary>
        public string BranchSha1
        {
            get { return LibVersion.B_SHA1; }
        }

        /// <summary>
        /// Count of revisions to used branch from SCM
        /// </summary>
        public string BranchRevCount
        {
            get { return LibVersion.B_REVC; }
        }

        /// <summary>
        /// About Bridge to library if used
        /// </summary>
        public IVersion Bridge
        {
            get { return null; }
        }
    }
}
