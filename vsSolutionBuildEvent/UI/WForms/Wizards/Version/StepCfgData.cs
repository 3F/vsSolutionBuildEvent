/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
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
