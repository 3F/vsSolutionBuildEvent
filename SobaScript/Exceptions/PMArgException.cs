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
using System.Collections.ObjectModel;
using net.r_eg.SobaScript.SNode;

namespace net.r_eg.SobaScript.Exceptions
{
    [Serializable]
    public class PMArgException: UnspecSobaScriptException
    {
        public PMArgException(Argument arg, string expected)
            : base(GetMessage(arg, expected), arg)
        {

        }

        public PMArgException(RArgs args, string expected)
            : base(GetMessage(args, expected), args)
        {

        }

        protected static string GetMessage(Argument arg, string expected)
            => $"Incorrect argument: ({arg.type.ToString()}){arg.data.ToString()}. Expected `{expected}`";

        protected static string GetMessage(RArgs args, string expected)
        {
            string pNull, pSize;

            if(args == null)
            {
                pNull = "null";
                pSize = "";
            }
            else
            {
                pNull = "not null";
                pSize = " with size " + args.Count;
            }

            return $"Incorrect arguments: {pNull} value{pSize}. Expected `{expected}`";
        }
    }
}
