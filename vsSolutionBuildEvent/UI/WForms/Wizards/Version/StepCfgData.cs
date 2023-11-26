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
    internal sealed class StepCfgData: IStep
    {
        /// <summary>
        /// Type of revision number.
        /// </summary>
        public RevNumber.Type revType = RevNumber.Type.DeltaTime;

        /// <summary>
        /// Value of revision number.
        /// </summary>
        public RevNumber.IRevNumber revVal;

        /// <summary>
        /// Type of input number.
        /// </summary>
        public InputNumberType inputNumberType = InputNumberType.File;

        /// <summary>
        /// Input version number.
        /// </summary>
        public string inputNumber = String.Empty;

        /// <summary>
        /// Type of SCM.
        /// </summary>
        public SCMType scm = SCMType.None;

        /// <summary>
        /// Output file.
        /// </summary>
        public string output = String.Empty;

        /// <summary>
        /// The type of step.
        /// </summary>
        public StepsType Type
        {
            get { return StepsType.CfgData; }
        }

        /// <summary>
        /// Available types of how and where to get number of version.
        /// </summary>
        public enum InputNumberType
        {
            /// <summary>
            /// External file with raw data like {major}.{minor}.{patch} and similar.
            /// </summary>
            File,

            /// <summary>
            /// MSBuild Property like $(Version) and similar.
            /// </summary>
            MSBuildProp,
        }

        /// <summary>
        /// Available types of SCM.
        /// </summary>
        public enum SCMType
        {
            /// <summary>
            /// None used.
            /// </summary>
            None,

            /// <summary>
            /// Git version control system.
            /// </summary>
            Git,
        }

        /// <summary>
        /// List of available types of methods for using as revision number.
        /// </summary>
        public List<KeyValuePair<RevNumber.Type, string>> RevTypeList
        {
            get;
            private set;
        }

        /// <summary>
        /// List of available types of how and where to get number of version.
        /// </summary>
        public List<KeyValuePair<InputNumberType, string>> InputNumberTypeList
        {
            get;
            private set;
        }

        /// <summary>
        /// List of available types of SCM.
        /// </summary>
        public List<KeyValuePair<SCMType, string>> SCMTypeList
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public StepCfgData()
        {
            RevTypeList = new List<KeyValuePair<RevNumber.Type, string>>()
            {
                new KeyValuePair<RevNumber.Type, string>(RevNumber.Type.Raw, "Raw - from $(Revision)"),
                new KeyValuePair<RevNumber.Type, string>(RevNumber.Type.DeltaTime, "Delta of time")
            };

            InputNumberTypeList = new List<KeyValuePair<InputNumberType, string>>()
            {
                new KeyValuePair<InputNumberType, string>(InputNumberType.File, "External file"),
                new KeyValuePair<InputNumberType, string>(InputNumberType.MSBuildProp, "MSBuild property as $(name)")
            };

            SCMTypeList = Enum.GetValues(typeof(SCMType))
                                .Cast<SCMType>()
                                .Select(v => new KeyValuePair<SCMType, string>(v, v.ToString()))
                                .ToList();
        }
    }
}
