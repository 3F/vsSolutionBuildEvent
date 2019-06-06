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

using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// Mixed supported functions
    /// </summary>
    [Component("Func", "Mixed functions.")]
    public class FunctionComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "Func "; }
        }

        /// <param name="loader">Initialize with loader</param>
        public FunctionComponent(IBootloader loader)
            : base(loader)
        {

        }

        public FunctionComponent() { }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data.Trim());
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            IPM pm = new PM(request, msbuild);
            switch(subtype) {
                case "hash": {
                    return stHash(pm);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
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
        [Property("hash", "Work with hash.")]
        [Method("MD5",
                "Calculate hash value with MD5.",
                "hash",
                "stHash",
                new string[] { "data" },
                new string[] { "String for calculating." },
                CValueType.String,
                CValueType.String)]
        [Method("SHA1",
                "Calculate hash value with SHA-1.",
                "hash",
                "stHash",
                new string[] { "data" },
                new string[] { "String for calculating." },
                CValueType.String,
                CValueType.String)]
        protected string stHash(IPM pm)
        {
            if(!pm.It(LevelType.Property, "hash")) {
                throw new IncorrectNodeException(pm);
            }

            ILevel lvlHash = pm.FirstLevel; // level of the hash property

            // hash.MD5("data")
            if(pm.FinalEmptyIs(LevelType.Method, "MD5")) {
                lvlHash.Is("hash.MD5(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).MD5Hash();
            }

            // hash.SHA1("data")
            if(pm.FinalEmptyIs(LevelType.Method, "SHA1")) {
                lvlHash.Is("hash.SHA1(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).SHA1Hash();
            }

            throw new IncorrectNodeException(pm);
        }
    }
}
