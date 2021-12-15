/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
    internal sealed class StepRepl: IStep
    {
        /// <summary>
        /// File for replacing data.
        /// </summary>
        public string file = String.Empty;

        /// <summary>
        /// Type of replacement.
        /// </summary>
        public ReplType rtype;

        /// <summary>
        /// Pattern for replacing.
        /// </summary>
        public string pattern = "<Version>[0-9.]+</Version>";

        /// <summary>
        /// Prefix of replacement.
        /// </summary>
        public string prefix = "<Version>";

        /// <summary>
        /// Postfix of replacement.
        /// </summary>
        public string postfix = "</Version>";

        /// <summary>
        /// Source for replacing.
        /// </summary>
        public Fields.Type source;

        /// <summary>
        /// The type of step.
        /// </summary>
        public StepsType Type
        {
            get { return StepsType.DirectRepl; }
        }

        /// <summary>
        /// Checks equality of SCM fields for selected source.
        /// </summary>
        public bool IsSourceSCM
        {
            get
            {
                return (source == Fields.Type.BranchName
                        || source == Fields.Type.BranchSha1
                        || source == Fields.Type.BranchRevCount
                        || source == Fields.Type.Informational
                        || source == Fields.Type.InformationalFull);
            }
        }

        /// <summary>
        /// Checks requiring input number for selected source. 
        /// </summary>
        public bool IsSourceNotRequiresInputNum
        {
            get
            {
                return (source == Fields.Type.BranchName
                        || source == Fields.Type.BranchSha1
                        || source == Fields.Type.BranchRevCount
                        || source == Fields.Type.Null);
            }
        }

        /// <summary>
        /// Available types of replacement methods.
        /// </summary>
        public enum ReplType
        {
            Regex,
            Wildcards,
        }

        /// <summary>
        /// List of available types of replacement methods.
        /// </summary>
        public List<KeyValuePair<ReplType, string>> TypeList
        {
            get;
            private set;
        }

        /// <summary>
        /// List of available fields for using as replacement.
        /// </summary>
        public List<KeyValuePair<Fields.Type, string>> SourceList
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public StepRepl()
        {
            TypeList = Enum.GetValues(typeof(ReplType))
                                .Cast<ReplType>()
                                .Select(v => new KeyValuePair<ReplType, string>(v, v.ToString()))
                                .ToList();

            rtype   = TypeList[0].Key;

            SourceList  = Fields.List.Where(i => i.Key != Fields.Type.Number).ToList();
            source      = SourceList[SourceList.FindIndex(i => i.Key == Fields.Type.NumberString)].Key;
            SourceList.Add(new KeyValuePair<Fields.Type, string>(Fields.Type.Null, "None used"));
        }
    }
}
