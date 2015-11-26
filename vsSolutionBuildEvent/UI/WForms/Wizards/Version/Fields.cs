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

using System.Collections.Generic;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal static class Fields
    {
        public enum Type
        {
            /// <summary>
            /// None used.
            /// </summary>
            Null,

            /// <summary>
            /// The general number.
            /// </summary>
            Number,

            /// <summary>
            /// The number as string.
            /// </summary>
            NumberString,

            /// <summary>
            /// The number with revision as string. 
            /// </summary>
            NumberWithRevString,

            /// <summary>
            /// SCM. name of branch.
            /// </summary>
            BranchName,

            /// <summary>
            /// SCM. SHA-1 of commit.
            /// </summary>
            BranchSha1,

            /// <summary>
            /// SCM. branch revision number.
            /// </summary>
            BranchRevCount,

            /// <summary>
            /// Informational string of version.
            /// </summary>
            Informational,

            /// <summary>
            /// Full informational string of version.
            /// </summary>
            InformationalFull,
        }

        /// <summary>
        /// List of available fields.
        /// </summary>
        public static List<KeyValuePair<Type, string>> List
        {
            get { return list; }
        }
        private static List<KeyValuePair<Type, string>> list = new List<KeyValuePair<Type, string>>()
        {
            new KeyValuePair<Type, string>(Type.Number, "The general number"),
            new KeyValuePair<Type, string>(Type.NumberString, "The number as string"),
            new KeyValuePair<Type, string>(Type.NumberWithRevString, "The number with revision as string"),
            new KeyValuePair<Type, string>(Type.BranchName, "SCM. name of branch"),
            new KeyValuePair<Type, string>(Type.BranchSha1, "SCM. SHA-1 of commit"),
            new KeyValuePair<Type, string>(Type.BranchRevCount, "SCM. branch revision number"),
            new KeyValuePair<Type, string>(Type.Informational, "Informational string of version"),
            new KeyValuePair<Type, string>(Type.InformationalFull, "Full informational string of version"),
        };
    }
}
