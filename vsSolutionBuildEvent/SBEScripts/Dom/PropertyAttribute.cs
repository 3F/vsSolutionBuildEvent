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

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    /// <summary>
    /// Specification of the hierarchy of property
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PropertyAttribute: Attribute, IAttrDomLevelB
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current property
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Value type for getting
        /// </summary>
        public CValueType Get
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Value type for setting
        /// </summary>
        public CValueType Set
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of the parent specification (property/method/etc.) if exist or null
        /// </summary>
        public string Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Actual/real method name of the parent specification if exist or null
        /// </summary>
        public string Method
        {
            get;
            protected set;
        }

        /// <param name="name">Property name</param>
        /// <param name="description">Description for current property</param>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        public PropertyAttribute(string name, string description, CValueType get = CValueType.Void, CValueType set = CValueType.Void)
        {
            Name            = name;
            Description     = description;
            Get             = get;
            Set             = set;
        }

        /// <param name="name">Property name</param>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        public PropertyAttribute(string name, CValueType get = CValueType.Void, CValueType set = CValueType.Void)
            : this(name, String.Empty, get, set)
        {

        }

        /// <param name="name">Property name</param>
        /// <param name="description">Description for current property</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        public PropertyAttribute(string name, string description, string parent, string method, CValueType get = CValueType.Void, CValueType set = CValueType.Void)
            : this(name, description, get, set)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Property name</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        public PropertyAttribute(string name, string parent, string method, CValueType get = CValueType.Void, CValueType set = CValueType.Void)
            : this(name, String.Empty, parent, method, get, set)
        {

        }
    }
}
