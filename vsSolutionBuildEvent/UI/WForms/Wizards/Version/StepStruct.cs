/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using net.r_eg.EvMSBuild;
using net.r_eg.MvsSln;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal sealed class StepStruct: IStep
    {
        /// <summary>
        /// Namespace for struct.
        /// </summary>
        public string namspace;

        /// <summary>
        /// Name of struct.
        /// </summary>
        public string name = "Version";

        /// <summary>
        /// Upper case for fields.
        /// </summary>
        public bool upperCase = false;

        /// <summary>
        /// Type field for number.
        /// </summary>
        public NumberType fnumber;

        /// <summary>
        /// The type of step.
        /// </summary>
        public StepsType Type
        {
            get { return StepsType.DirectRepl; }
        }

        /// <summary>
        /// Available types of using the number field.
        /// </summary>
        public enum NumberType
        {
            /// <summary>
            /// System.Version - .NET (CLR)
            /// </summary>
            SystemVersion,

            /// <summary>
            /// Native struct
            /// </summary>
            NativeStruct,
        }

        /// <summary>
        /// List of available types of fields for number.
        /// </summary>
        public List<KeyValuePair<NumberType, string>> NumbersList
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public StepStruct()
        {
            NumbersList = new List<KeyValuePair<NumberType, string>>()
            {
                new KeyValuePair<NumberType, string>(NumberType.SystemVersion, "System.Version - .NET (CLR)"),
                new KeyValuePair<NumberType, string>(NumberType.NativeStruct, "Native struct")
            };
            fnumber = NumbersList[0].Key;
        }

        /// <param name="parser"></param>
        public StepStruct(IEvMSBuild parser)
            : this()
        {
            try {
                namspace = parser.GetPropValue(PropertyNames.PRJ_NAMESPACE);
            }
            catch(Exception ex) {
                Log.Debug("Wizard-Version: RootNamespace failed - `{0}`", ex.Message);
            }

            if(namspace == PropertyNames.UNDEFINED) {
                namspace = "MyNamespace";
            }
        }
    }
}
