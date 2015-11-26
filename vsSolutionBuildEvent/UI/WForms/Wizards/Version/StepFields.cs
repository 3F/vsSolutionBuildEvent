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

using System;
using System.Collections.Generic;
using System.Linq;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal sealed class StepFields: IStep
    {
        /// <summary>
        /// Prefix to global const values.
        /// </summary>
        public const string PREFIX_TO_CONST = "VER_";

        /// <summary>
        /// List of available fields with new settings.
        /// </summary>
        public List<Items> items = new List<Items>();

        /// <summary>
        /// 
        /// </summary>
        public sealed class Items
        {
            /// <summary>
            /// Allowing for using.
            /// </summary>
            public bool disabled;

            /// <summary>
            /// Original name.
            /// </summary>
            public string origin;

            /// <summary>
            /// Original name in upper case.
            /// </summary>
            public string originUpperCase;

            /// <summary>
            /// Original name as global const.
            /// </summary>
            public string originConst;

            /// <summary>
            /// About field.
            /// </summary>
            public string description;

            /// <summary>
            /// New name for filed if not null or not empty.
            /// </summary>
            public string newname;

            /// <summary>
            /// The type of field.
            /// </summary>
            public Fields.Type type;
        }

        /// <summary>
        /// The type of step.
        /// </summary>
        public StepsType Type
        {
            get { return StepsType.Fields; }
        }

        /// <summary>
        /// Checking of alowing field type for used scm type.
        /// </summary>
        /// <param name="type">The type of field.</param>
        /// <param name="scm">The type of SCM.</param>
        /// <returns></returns>
        public bool isAllow(Fields.Type type, StepCfgData.SCMType scm)
        {
            if((scm == StepCfgData.SCMType.None)
                &&
                (type == Fields.Type.BranchName
                    || type == Fields.Type.BranchRevCount
                    || type == Fields.Type.BranchSha1
                    || type == Fields.Type.Informational
                    || type == Fields.Type.InformationalFull))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checking of alowing field type for used revision type.
        /// </summary>
        /// <param name="type">The type of field.</param>
        /// <param name="rev">The type of revision.</param>
        /// <returns></returns>
        public bool isAllow(Fields.Type type, RevNumber.Type rev)
        {
            if(rev == RevNumber.Type.Raw && type == Fields.Type.NumberWithRevString) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checking of alowing field type for selected GenType.
        /// </summary>
        /// <param name="type">The type of field.</param>
        /// <param name="rev">The type of revision.</param>
        /// <returns></returns>
        public bool isAllow(Fields.Type type, GenType gtype)
        {
            if(gtype == GenType.CppDefinitions && type == Fields.Type.Number) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// To reset the disabled property for all items.
        /// </summary>
        public void resetDisabled()
        {
            items.ForEach(i => i.disabled = false);
        }

        public StepFields()
        {
            Func<char, string> upper = delegate (char c) {
                return Char.ToUpper(c).ToString();
            };

            foreach(KeyValuePair<Fields.Type, string> f in Fields.List)
            {
                string name     = f.Key.ToString();
                string nameUC   = String.Concat(name.Select((c, i) => ((i > 0) && Char.IsUpper(c))? "_" + upper(c) : upper(c)));

                items.Add(new Items()
                {
                    disabled        = false,
                    origin          = name.Substring(0, 1).ToLower() + name.Substring(1),
                    originUpperCase = nameUC,
                    originConst     = PREFIX_TO_CONST + nameUC,
                    description     = f.Value,
                    newname         = String.Empty,
                    type            = f.Key,
                });
            }
        }
    }
}
