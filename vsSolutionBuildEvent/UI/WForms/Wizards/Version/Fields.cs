/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
