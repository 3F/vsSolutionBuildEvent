/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
