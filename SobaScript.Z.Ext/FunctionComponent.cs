/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Ext contributors: https://github.com/3F/Varhead/graphs/contributors
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

using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Ext.Extensions;

namespace net.r_eg.SobaScript.Z.Ext
{
    [Component("Func", "Mixed functions.")]
    public class FunctionComponent: ComponentAbstract, IComponent
    {
        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "Func ";

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data.Trim());
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            IPM pm = new PM(request, emsbuild);

            switch(subtype)
            {
                case "hash": {
                    return StHash(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public FunctionComponent(ISobaScript soba)
            : base(soba)
        {

        }

        /// <summary>
        /// The hash node.
        /// 
        /// Samples:
        ///     #[Func hash.MD5("test")]
        ///     #[Func hash.SHA1("test")]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("hash", "Work with hash functions.")]
        [Method("MD5",
                "Calculate hash value with MD5.",
                "hash",
                nameof(StHash),
                new[] { "data" },
                new[] { "String for calculating." },
                CValType.String,
                CValType.String)]
        [Method("SHA1",
                "Calculate hash value with SHA-1.",
                "hash",
                nameof(StHash),
                new[] { "data" },
                new[] { "String for calculating." },
                CValType.String,
                CValType.String)]
        protected string StHash(IPM pm)
        {
            if(!pm.It(LevelType.Property, "hash")) {
                throw new IncorrectNodeException(pm);
            }

            ILevel lvlHash = pm.FirstLevel; // level of the hash property

            // hash.MD5("data")
            if(pm.FinalEmptyIs(LevelType.Method, "MD5"))
            {
                lvlHash.Is("hash.MD5(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).MD5Hash();
            }

            // hash.SHA1("data")
            if(pm.FinalEmptyIs(LevelType.Method, "SHA1"))
            {
                lvlHash.Is("hash.SHA1(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).SHA1Hash();
            }

            throw new IncorrectNodeException(pm);
        }
    }
}
