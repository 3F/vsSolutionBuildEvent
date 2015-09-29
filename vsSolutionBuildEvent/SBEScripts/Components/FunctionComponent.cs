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

using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
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
            Match m = Regex.Match(data.Trim(), @"^\[Func
                                                    \s+
                                                    (                  #1 - full ident
                                                      ([A-Za-z_0-9]+)  #2 - subtype
                                                      .*
                                                    )
                                                 \]$", 
                                                 RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed FunctionComponent - '{0}'", data);
            }
            string ident    = m.Groups[1].Value;
            string subtype  = m.Groups[2].Value;

            switch(subtype) {
                case "hash": {
                    Log.Trace("FunctionComponent: use stHash - '{0}'", ident);
                    return stHash(ident);
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
        /// <param name="data"></param>
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
        protected string stHash(string data)
        {
            IPM pm = new PM(data);

            if(!pm.Is(0, LevelType.Property, "hash")) {
                throw new SyntaxIncorrectException("Failed stHash - '{0}'", data);
            }

            // hash.MD5("data")
            if(pm.FinalEmptyIs(1, LevelType.Method, "MD5"))
            {
                Argument[] args = pm.Levels[1].Args;
                if(args.Length != 1 || args[0].type != ArgumentType.StringDouble) {
                    throw new InvalidArgumentException("stHash: incorrect arguments to `hash.MD5(string data)`");
                }
                return ((string)args[0].data).MD5Hash();
            }

            // hash.SHA1("data")
            if(pm.FinalEmptyIs(1, LevelType.Method, "SHA1"))
            {
                Argument[] args = pm.Levels[1].Args;
                if(args.Length != 1 || args[0].type != ArgumentType.StringDouble) {
                    throw new InvalidArgumentException("stHash: incorrect arguments to `hash.SHA1(string data)`");
                }
                return ((string)args[0].data).SHA1Hash();
            }

            throw new OperationNotFoundException("stHash: not found - '{0}' /'{1}'", pm.Levels[1].Data, pm.Levels[1].Type);
        }
    }
}
