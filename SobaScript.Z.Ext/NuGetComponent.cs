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
using net.r_eg.SobaScript.Z.Ext.NuGet;

namespace net.r_eg.SobaScript.Z.Ext
{
    [Component("NuGet", "NuGet packages via GetNuTool https://github.com/3F/GetNuTool")]
    public class NuGetComponent: ComponentAbstract, IComponent
    {
        private GetNuTool gnt;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "NuGet ";

        public string BasePath
        {
            get => gnt.BasePath;
            set => gnt.BasePath = value;
        }

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            IPM pm = new PM(request, emsbuild);

            switch(subtype)
            {
                case "gnt": {
                    return StGNT(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public NuGetComponent(ISobaScript soba, string basePath)
            : base(soba)
        {
            gnt = new GetNuTool(basePath);
        }

        /// <summary>
        /// Work with gnt node. /GetNuTool
        /// Sample: #[NuGet gnt]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("gnt", "GetNuTool logic - github.com/3F/GetNuTool")]
        protected string StGNT(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "gnt")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.Levels[1]; // level of the gnt property

            if(pm.FinalEmptyIs(1, LevelType.Method, "raw")) {
                return RawMethod(level, pm);
            }

            // TODO: +gnt.get(object list [, string path [, string server]]) + config files
            //       +gnt.pack(string nuspec [, string path])

            throw new IncorrectNodeException(pm, 1);
        }

        /// <summary>
        /// Prepares signatures:
        ///     gnt.raw(string command)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Method("raw",
                "Push raw command to GetNuTool core.",
                "gnt",
                nameof(StGNT),
                new[] { "command" },
                new[] { "Command to execute" },
                CValType.Void,
                CValType.String)]
        protected string RawMethod(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble))
            {
                gnt.Raw((string)level.Args[0].data);
                return Value.Empty;
            }

            throw new PMLevelException(level, "`gnt.raw(string command)`");
        }
    }
}