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
using System.Linq;

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public class NodeInfo: INodeInfo
    {
        /// <summary>
        /// Element name
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current element
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Technical description of the method/property
        /// </summary>
        public string Signature
        {
            get;
            protected set;
        }

        /// <summary>
        /// Displays element over the 'Name' property.
        /// In general this useful for code completion 
        /// (Can be the same as 'Name' property)
        /// </summary>
        public string Displaying
        {
            get { return (displaying)?? Name; }
        }
        protected string displaying;

        /// <summary>
        /// Aliases for primary name if used
        /// </summary>
        public string[] Aliases
        {
            get;
            protected set;
        }

        /// <summary>
        /// Element type
        /// </summary>
        public InfoType Type
        {
            get { return type; }
        }
        protected InfoType type = InfoType.Unspecified;

        /// <summary>
        /// Link to the binding with other node
        /// </summary>
        public NodeIdent Link
        {
            get;
            protected set;
        }

        /// <param name="name">Element name</param>
        /// <param name="description">Description for current element</param>
        /// <param name="type">Element type</param>
        /// <param name="displaying">Displays element over the 'Name' property</param>
        public NodeInfo(string name, string description, InfoType type = InfoType.Unspecified, string displaying = null)
        {
            Name            = name;
            Description     = description;
            this.type       = type;
            this.displaying = displaying;
        }

        /// <param name="name">Element name</param>
        /// <param name="description">Description for current element</param>
        /// <param name="ident">Link to the binding with other node</param>
        /// <param name="type">Element type</param>
        /// <param name="displaying">Displays element over the 'Name' property</param>
        public NodeInfo(string name, string description, NodeIdent ident, InfoType type = InfoType.Unspecified, string displaying = null)
            : this(name, description, type, displaying)
        {
            Link = ident;
        }

        /// <param name="attr">Attribute of property</param>
        /// <param name="method">Current actual/real method name</param>
        /// <param name="className">Actual class name</param>
        public NodeInfo(PropertyAttribute attr, string method, string className = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method, className), InfoType.Property)
        {
            try {
                Signature   = aboutProperty(attr.Get, attr.Set);
                displaying  = displayProperty(attr.Get, attr.Set);
                Name        = displaying;

                //TODO:
                if(attr.Set == CValueType.Input) {
                    Name += ": ]";
                }
            }
            catch(Exception ex) {
                Log.Warn("NodeInfo-PropertyAttribute: '{0}'", ex.Message);
            }
        }

        /// <param name="attr">Attribute of property</param>
        /// <param name="displaying">Displays element over the 'Name' property</param>
        public NodeInfo(DefinitionAttribute attr, string displaying = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, null), InfoType.Definition, displaying)
        {

        }

        /// <param name="attr">Attribute of property</param>
        /// <param name="displaying">Displays element over the 'Name' property</param>
        public NodeInfo(ComponentAttribute attr, string displaying = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, null), InfoType.Component, displaying)
        {
            Aliases = attr.Aliases;
        }

        /// <param name="attr">Attribute of method</param>
        /// <param name="method">Current actual/real method name</param>
        /// <param name="className">Actual class name</param>
        public NodeInfo(MethodAttribute attr, string method, string className = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method, className), InfoType.Method)
        {
            try {
                Signature   = aboutMethod(attr.Return, attr.Arguments);
                displaying  = displayMethod(attr.Return, attr.Arguments);
                Name        = displaying;

                //TODO:
                if(attr.Arguments != null && attr.Arguments.Length > 0) {
                    if(attr.Arguments[attr.Arguments.Length - 1].type == CValueType.Input) {
                        Name = Name.Substring(0, displaying.LastIndexOf(',')) + "): ]";
                    }
                }
            }
            catch(Exception ex) {
                Log.Warn("NodeInfo-MethodAttribute: '{0}'", ex.Message);
            }
        }

        /// <summary>
        /// Builds the technical description of the property
        /// </summary>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        /// <returns></returns>
        private string aboutProperty(CValueType get, CValueType set)
        {
            return String.Format("Get: {0}\nSet: {1}", _type(get), _type(set));
        }

        /// <summary>
        /// Formatting of the property for displaying
        /// </summary>
        /// <param name="get">Value type for getting</param>
        /// <param name="set">Value type for setting</param>
        /// <returns></returns>
        private string displayProperty(CValueType get, CValueType set)
        {
            return Name;
        }

        /// <summary>
        /// Builds the technical description of the method
        /// </summary>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        /// <returns></returns>
        private string aboutMethod(CValueType ret, MethodAttribute.TArguments[] args)
        {
            string retString = _type(ret);

            if(args == null || args.Length < 1) {
                return String.Format("{0} {1}({2})\n", retString, Name, _type(CValueType.Void));
            }

            if(args[0].name == null) {
                return String.Format("{0} {1}({2})", retString, Name, String.Join(", ", args.Select(p => _type(p.type))));
            }

            string argsString       = String.Join(", ", args.Select(p => String.Format("{0} {1}", _type(p.type), p.name)));
            string argsDescription  = String.Join("\n* ", args.Select(p => String.Format("{0} - {1}", p.name, p.description)));

            return String.Format("{0} {1}({2})\n* {3}", retString, Name, argsString, argsDescription);
        }

        /// <summary>
        /// Formatting of the method for displaying
        /// </summary>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        /// <returns></returns>
        private string displayMethod(CValueType ret, MethodAttribute.TArguments[] args)
        {
            string aStr = (args != null && args.Length > 0)? String.Join(", ", args.Select(p => p.name)) : _type(CValueType.Void);
            return String.Format("{0}({1})", Name, aStr);
        }

        /// <summary>
        /// Formatting the type for displaying
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string _type(CValueType type)
        {
            return type.ToString().ToLower();
        }
    }
}
