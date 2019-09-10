/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Mapper contributors: https://github.com/3F/SobaScript.Mapper/graphs/contributors
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Linq;
using net.r_eg.Components;

namespace net.r_eg.SobaScript.Mapper
{
    public class NodeInfo: INodeInfo
    {
        protected string _overname;

        /// <summary>
        /// Element name.
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current element.
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }

        /// <summary>
        /// Technical description of the method/property.
        /// </summary>
        public string Signature
        {
            get;
            protected set;
        }

        /// <summary>
        /// Displays element over the 'Name' property.
        /// Useful for code completion.
        /// </summary>
        public string Overname => _overname ?? Name;

        /// <summary>
        /// Aliases to primary name if used.
        /// </summary>
        public string[] Aliases
        {
            get;
            protected set;
        }

        /// <summary>
        /// Element type.
        /// </summary>
        public NodeType Type
        {
            get;
            protected set;
        } = NodeType.Unspecified;

        /// <summary>
        /// Binding with other node.
        /// </summary>
        public NodeIdent Link
        {
            get;
            protected set;
        }

        /// <param name="name">Element name.</param>
        /// <param name="description">Description for current element.</param>
        /// <param name="type">Element type.</param>
        /// <param name="overname">Displays element over the 'Name' property.</param>
        public NodeInfo(string name, string description, NodeType type = NodeType.Unspecified, string overname = null)
        {
            Name            = name;
            Description     = description;
            Type            = type;
            _overname       = overname;
        }

        /// <param name="name">Element name.</param>
        /// <param name="description">Description for current element.</param>
        /// <param name="ident">Binding with other node.</param>
        /// <param name="type">Element type.</param>
        /// <param name="overname">Displays element over the 'Name' property.</param>
        public NodeInfo(string name, string description, NodeIdent ident, NodeType type = NodeType.Unspecified, string overname = null)
            : this(name, description, type, overname)
        {
            Link = ident;
        }

        /// <param name="attr">Attribute of property.</param>
        /// <param name="method">Current actual/real method name.</param>
        /// <param name="className">Actual class name.</param>
        public NodeInfo(PropertyAttribute attr, string method, string className = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method, className), NodeType.Property)
        {
            try
            {
                Signature   = AboutProperty(attr.Get, attr.Set);
                _overname   = DisplayProperty(attr.Get, attr.Set);
                Name        = _overname;

                //TODO:
                if(attr.Set == CValType.Input) {
                    Name += ": ]";
                }
            }
            catch(Exception ex) {
                LSender.Send(this, $"NodeInfo-PropertyAttribute: '{ex.Message}'", MsgLevel.Warn);
            }
        }

        /// <param name="attr">Attribute of property.</param>
        /// <param name="overname">Displays element over the 'Name' property.</param>
        public NodeInfo(DefinitionAttribute attr, string overname = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, null), NodeType.Definition, overname)
        {

        }

        /// <param name="attr">Attribute of property.</param>
        /// <param name="overname">Displays element over the 'Name' property.</param>
        public NodeInfo(ComponentAttribute attr, string overname = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, null), NodeType.Component, overname)
        {
            Aliases = attr.Aliases;
        }

        /// <param name="attr">Attribute of method.</param>
        /// <param name="method">Current actual/real method name.</param>
        /// <param name="className">Actual class name.</param>
        public NodeInfo(MethodAttribute attr, string method, string className = null)
            : this(attr.Name, attr.Description, new NodeIdent(attr.Name, method, className), NodeType.Method)
        {
            try
            {
                Signature   = AboutMethod(attr.Return, attr.Arguments);
                _overname   = DisplayMethod(attr.Return, attr.Arguments);
                Name        = _overname;

                //TODO:
                if(attr.Arguments != null && attr.Arguments.Length > 0)
                {
                    if(attr.Arguments[attr.Arguments.Length - 1].type == CValType.Input) {
                        Name = Name.Substring(0, _overname.LastIndexOf(',')) + "): ]";
                    }
                }
            }
            catch(Exception ex) {
                LSender.Send(this, $"NodeInfo-MethodAttribute: '{ex.Message}'", MsgLevel.Warn);
            }
        }

        internal NodeInfo(INodeInfo info, string description = null)
        {
            if(info != null)
            {
                Name        = info.Name;
                Description = info.Description;
                Signature   = info.Signature;
                _overname   = info.Overname;
                Aliases     = info.Aliases;
                Type        = info.Type;
                Link        = info.Link;
            }

            if(description != null) {
                Description = description;
            }
        }

        /// <summary>
        /// Builds the technical description of the property.
        /// </summary>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        /// <returns></returns>
        private string AboutProperty(CValType get, CValType set)
            => $"Get: {_t(get)}\nSet: {_t(set)}";

        /// <summary>
        /// Formatting of the property.
        /// </summary>
        /// <param name="get">Value type for getting.</param>
        /// <param name="set">Value type for setting.</param>
        /// <returns></returns>
        private string DisplayProperty(CValType get, CValType set)
        {
            return Name;
        }

        /// <summary>
        /// Builds the technical description of the method.
        /// </summary>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        /// <returns></returns>
        private string AboutMethod(CValType ret, TArgument[] args)
        {
            string retString = _t(ret);

            if(args == null || args.Length < 1) {
                return string.Format("{0} {1}({2})\n", retString, Name, _t(CValType.Void));
            }

            if(args[0].name == null) {
                return string.Format("{0} {1}({2})", retString, Name, string.Join(", ", args.Select(p => _t(p.type))));
            }

            string argsString       = string.Join(", ", args.Select(p => string.Format("{0} {1}", _t(p.type), p.name)));
            string argsDescription  = string.Join("\n* ", args.Select(p => string.Format("{0} - {1}", p.name, p.description)));

            return string.Format("{0} {1}({2})\n* {3}", retString, Name, argsString, argsDescription);
        }

        /// <summary>
        /// Formatting of the method.
        /// </summary>
        /// <param name="ret">Return value.</param>
        /// <param name="args">Arguments of method.</param>
        /// <returns></returns>
        private string DisplayMethod(CValType ret, TArgument[] args)
        {
            string aStr = (args != null && args.Length > 0)? string.Join(", ", args.Select(p => p.name)) : _t(CValType.Void);
            return $"{Name}({aStr})";
        }

        /// <summary>
        /// Formatting of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string _t(CValType type) => type.ToString().ToLower();
    }
}
