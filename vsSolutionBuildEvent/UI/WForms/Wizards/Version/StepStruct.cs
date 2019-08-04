/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
        public StepStruct(MSBuild.Parser parser)
            : this()
        {
            try {
                namspace = parser.getProperty(PropertyNames.PRJ_NAMESPACE);
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
