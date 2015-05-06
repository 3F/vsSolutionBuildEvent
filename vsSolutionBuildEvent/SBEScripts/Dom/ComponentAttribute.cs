/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class ComponentAttribute: Attribute, IAttrDomLevelA
    {
        /// <summary>
        /// Component name
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// About component
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of parent specification if exist or null
        /// </summary>
        public string Parent
        {
            get;
            protected set;
        }

        /// <param name="name">Componen name</param>
        /// <param name="description">About component</param>
        public ComponentAttribute(string name, string description)
        {
            Name        = name;
            Description = description;
        }

        /// <param name="name">Componen name</param>
        /// <param name="description">About component</param>
        /// <param name="parent">Name of parent specification if exist or null</param>
        public ComponentAttribute(string name, string description, string parent = null)
        {
            Name        = name;
            Description = description;
            Parent      = parent;
        }
    }
}
