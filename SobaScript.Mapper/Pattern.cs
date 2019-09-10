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

namespace net.r_eg.SobaScript.Mapper
{
    internal static class Pattern
    {
        // TODO: this are trivial (complexity) patterns but probably we need an precompiled versions

        internal static string EvMLeft => @"\$\(\s*\w*$";

        internal static string SobaLeft 
            => @"\#\[
                 (?:
                    (?'data'\S+)
                  |
                    \s*
                 )$";

        internal static string SobaMiddle
            => @"^\#\[
                  \s*
                  (\S+)     #1 - Component
                  \s*
                  (.+)?     #2 - properties/methods etc. (optional)";

        internal static string Finalization
            => @"(?:
                   \s+
                  |
                   \([^.)]*?
                  |
                   \)
                 )$";
    }
}