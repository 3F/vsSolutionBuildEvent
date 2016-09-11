/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.MSBuild
{
    public struct PreparedData
    {
        /// <summary>
        /// Available value types
        /// </summary>
        public enum ValueType
        {
            Unknown,
            /// <summary>
            /// MSBuild properties
            /// </summary>
            Property,
            /// <summary>
            /// Escaped MSBuild properties
            /// </summary>
            PropertyEscaped,
            /// <summary>
            /// String data ".."
            /// </summary>
            StringFromDouble,
            /// <summary>
            /// String data '..'
            /// </summary>
            StringFromSingle,
        }

        public enum TSignType
        {
            Default,

            /// <summary>
            /// Set the global msbuild property.
            /// </summary>
            DefProperty,

            /// <summary>
            /// Unset the global msbuild property.
            /// </summary>
            UndefProperty,
        }

        public enum VSignType
        {
            Default,

            /// <summary>
            /// left += right
            /// </summary>
            Increment,

            /// <summary>
            /// left -= right
            /// </summary>
            Decrement,
        }

        public struct Variable
        {
            /// <summary>
            /// Variable name
            /// </summary>
            public string name;

            /// <summary>
            /// Project context for variable.
            /// or specific project where to store.
            /// </summary>
            public string project;

            /// <summary>
            /// Storing inside project files  ~ .csproj, .vcxproj, ..
            /// </summary>
            public bool persistence;

            /// <summary>
            /// Available types of value
            /// </summary>
            public ValueType type;

            /// <summary>
            /// $({tSign}name = data)
            /// </summary>
            public TSignType tSign;

            /// <summary>
            /// $(name {vSign}= data)
            /// </summary>
            public VSignType vSign;
        }

        public struct Property
        {
            /// <summary>
            /// Complex phrase or simple property
            /// e.g.:
            ///  * MSBuild Property Function: $([System.DateTime]::UtcNow.Ticks)
            ///  * Simple MSBuild Property:   $(Configuration)
            /// </summary>
            public bool complex;

            /// <summary>
            /// Prepared but unevaluated data, i.e. the evaluation should be from this!
            /// </summary>
            public string unevaluated;

            /// <summary>
            /// The left definition of property.
            /// </summary>
            public string name;

            /// <summary>
            /// Specific project for property
            /// </summary>
            public string project;

            /// <summary>
            /// Initial data
            /// </summary>
            public string raw;
        }

        /// <summary>
        /// User-variable of MSBuild core
        /// </summary>
        public Variable variable;

        /// <summary>
        /// Unit of properties
        /// </summary>
        public Property property;
    }
}
