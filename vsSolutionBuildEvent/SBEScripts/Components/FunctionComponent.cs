/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

            Log.Trace("FunctionComponent: subtype - `{0}`, request - `{1}`", subtype, request);

            switch(subtype) {
                case "hash": {
                    return stHash(new PM(request));
                }
            }
            throw new SubtypeNotFoundException("FunctionComponent: not found subtype - '{0}'", subtype);
        }

        /// <summary>
        /// Work with hash.
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
            if(!pm.Is(0, LevelType.Property, "hash")) {
                throw new SyntaxIncorrectException("Failed stHash - '{0}' /'{1}'", pm.Levels[0].Data, pm.Levels[0].Type);
            }

            ILevel lvlHash = pm.Levels[1]; // level of the hash property

            // hash.MD5("data")
            if(pm.FinalEmptyIs(1, LevelType.Method, "MD5")) {
                lvlHash.Is("hash.MD5(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).MD5Hash();
            }

            // hash.SHA1("data")
            if(pm.FinalEmptyIs(1, LevelType.Method, "SHA1")) {
                lvlHash.Is("hash.SHA1(string data)", ArgumentType.StringDouble);
                return ((string)lvlHash.Args[0].data).SHA1Hash();
            }

            throw new OperationNotFoundException("stHash: not found - '{0}' /'{1}'", lvlHash.Data, lvlHash.Type);
        }
    }
}
