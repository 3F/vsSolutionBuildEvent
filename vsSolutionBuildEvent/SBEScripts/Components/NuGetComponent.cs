/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components.NuGet.GetNuTool;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    [Component("NuGet", "Support of NuGet packages")]
    public class NuGetComponent: Component, IComponent
    {
        /// <summary>
        /// GetNuTool core
        /// </summary>
        protected GNT gnt = new GNT();

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "NuGet "; }
        }

        /// <param name="loader">Initialize with loader</param>
        public NuGetComponent(IBootloader loader)
            : base(loader)
        {

        }

        public NuGetComponent() { }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            IPM pm = new PM(request, msbuild);
            switch(subtype) {
                case "gnt": {
                    return stGNT(pm);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        /// <summary>
        /// Work with gnt node. /GetNuTool
        /// Sample: #[NuGet gnt]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("gnt", "GetNuTool logic - github.com/3F/GetNuTool")]
        protected string stGNT(IPM pm)
        {
            if(!pm.Is(LevelType.Property, "gnt")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.Levels[1]; // level of the gnt property

            if(pm.FinalEmptyIs(1, LevelType.Method, "raw")) {
                return rawMethod(level, pm);
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
                "stGNT",
                new string[] { "command" },
                new string[] { "Command to execute" },
                CValueType.Void,
                CValueType.String)]
        protected string rawMethod(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble)) {
                gnt.raw((string)level.Args[0].data);
                return Value.Empty;
            }

            throw new InvalidArgumentException("Incorrect arguments to `gnt.raw(string command)`");
        }
    }
}