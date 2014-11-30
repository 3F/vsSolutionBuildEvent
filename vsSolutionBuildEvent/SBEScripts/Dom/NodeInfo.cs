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
            get { return name; }
        }
        protected string name;

        /// <summary>
        /// Description for current element
        /// </summary>
        public string Description
        {
            get { return description; }
        }
        protected string description;

        /// <summary>
        /// Technical description of the method/property
        /// </summary>
        public string Signature
        {
            get { return signature; }
        }
        protected string signature;

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
            get { return link; }
        }
        protected NodeIdent link;

        /// <param name="name">Element name</param>
        /// <param name="description">Description for current element</param>
        public NodeInfo(string name, string description)
        {
            this.name           = name;
            this.description    = description;
        }

        /// <param name="name">Element name</param>
        /// <param name="description">Description for current element</param>
        /// <param name="method">Link to the binding with other node</param>
        public NodeInfo(string name, string description, NodeIdent ident)
            : this(name, description)
        {
            this.link = ident;
        }

        /// <param name="attr">Attribute of property</param>
        /// <param name="method">Current actual/real method name</param>
        public NodeInfo(PropertyAttribute attr, string method)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method))
        {
            signature   = aboutProperty(attr.Get, attr.Set);
            type        = InfoType.Property;
        }

        /// <param name="attr">Attribute of property</param>
        /// <param name="method">Current actual/real method name</param>
        public NodeInfo(DefinitionAttribute attr)
            : this(attr.Name, attr.Description)
        {
            type = InfoType.Component;
        }

        /// <param name="attr">Attribute of method</param>
        /// <param name="method">Current actual/real method name</param>
        public NodeInfo(MethodAttribute attr, string method)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method))
        {
            signature   = aboutMethod(attr.Return, attr.Arguments);
            type        = InfoType.Method;
        }

        protected virtual string aboutProperty(CValueType get, CValueType set)
        {
            return String.Format("{0} - Get: {1} / Set: {2}\n  ({1}){0}\n  {0} = {2}", name, get, set);
        }

        protected virtual string aboutMethod(CValueType ret, MethodAttribute.TArguments[] args)
        {
            string retString = _printType(ret);

            if(args == null || args.Length < 1) {
                return String.Format("{0} {1}({2})\n", retString, name, _printType(CValueType.Void));
            }

            if(args[0].name == null) {
                return String.Format("{0} {1}({2})", retString, name, String.Join(", ", args.Select(arg => _printType(arg.type))));
            }

            string argsString       = String.Join(", ", args.Select((arg) => String.Format("{0} {1}", _printType(arg.type), arg.name)));
            string argsDescription  = String.Join("\n* ", args.Select((arg) => String.Format("{0} - {1}", arg.name, arg.description)));

            return String.Format("{0} {1}({2})\n* {3}", retString, name, argsString, argsDescription);
        }

        private string _printType(CValueType type)
        {
            return type.ToString().ToLower();
        }
    }
}
