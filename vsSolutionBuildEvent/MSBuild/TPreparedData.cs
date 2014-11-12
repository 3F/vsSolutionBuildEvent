/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Text;

namespace net.r_eg.vsSBE.MSBuild
{
    public struct TPreparedData
    {
        /// <summary>
        /// Dynamically define variables
        /// </summary>
        public TVar variable;
        /// <summary>
        /// Unit of properties
        /// </summary>
        public TProperty property;

        public struct TVar
        {
            /// <summary>
            /// Storage if present
            /// </summary>
            public string name;
            /// <summary>
            /// Specific project where to store.
            /// null value - project by default
            /// </summary>
            public string project;
            /// <summary>
            /// Storing in the projects files ~ .csproj, .vcxproj, ..
            /// </summary>
            /// <remarks>reserved</remarks>
            public bool isPersistence;
        }

        public struct TProperty
        {
            /// <summary>
            /// Complex phrase or simple property
            /// </summary>
            public bool complex;
            /// <summary>
            /// has escaped property
            /// </summary>
            public bool escaped;
            /// <summary>
            /// Contain all prepared data from specific projects. Not complete evaluation!
            /// i.e.: prepares only all $(..:project) data
            /// </summary>
            public string unevaluated;
            /// <summary>
            /// Specific project for unevaluated data
            /// </summary>
            public string project;
            /// <summary>
            /// Step of handling
            /// </summary>
            public bool completed;
            /// <summary>
            /// Raw unprepared data without(in any case) storage variable
            /// </summary>
            public string raw;
            /// <summary>
            /// Contains analysis of nested data
            /// </summary>
            public Nested nested;
        }

        public struct Nested
        {
            /// <summary>
            /// Intermediate data of analysis
            /// </summary>
            public string data;
            /// <summary>
            /// Unevaluated values for present placeholders in data
            /// </summary>
            public Dictionary<int, List<Node>> nodes;
            /// <summary>
            /// true value if one or more nodes contains the Property
            /// </summary>
            public bool hasProperty;

            public struct Node
            {
                /// <summary>
                /// mixed data of arguments
                /// e.g.: simple string or unevaluated complex property
                /// </summary>
                public string data;
                /// <summary>
                /// Specific project if exist or null
                /// </summary>
                public string project;
                /// <summary>
                /// Support for variable of variable.
                /// Contains evaluated data or escaped property (without escape symbol).
                /// the null value if disabled evaluation (e.g. string argument)
                /// </summary>
                public string evaluated;
                /// <summary>
                /// Index of previous node for Left operand
                /// >= 0 / -1 if not used
                /// </summary>
                public int backLinkL;
                /// <summary>
                /// Index of previous node for Right operand
                /// >= 0 / -1 if not used
                /// </summary>
                public int backLinkR;

                public TypeValue type;

                public Node(string data, TypeValue type = TypeValue.Unknown, string project = null, string evaluated = null)
                {
                    this.data       = data;
                    this.project    = project;
                    this.evaluated  = evaluated;
                    this.type       = type;
                    this.backLinkL  = -1;
                    this.backLinkR  = -1;
                }
            }

            public enum TypeValue
            {
                Unknown,
                Property,
                PropertyEscaped,
                String
            }
        }
    }
}
