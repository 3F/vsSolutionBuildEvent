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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;

namespace net.r_eg.SobaScript.Mapper
{
    using LInfo = ConcurrentDictionary<NodeIdent, List<INodeInfo>>;

    public class Inspector: IInspector
    {
        protected const string C_POSTFIX = "Component";

        /// <summary>
        /// Used loader
        /// </summary>
        protected ISobaCLoader cLoader;

        /// <summary>
        /// Main storage.
        /// </summary>
        protected LInfo data = new LInfo();

        /// <summary>
        /// List of constructed root-data.
        /// </summary>
        public IEnumerable<INodeInfo> Root => GetBy();

        /// <summary>
        /// List of constructed data by identification of node.
        /// </summary>
        /// <param name="ident">Identifier of node.</param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> GetBy(NodeIdent ident = new NodeIdent())
        {
            if(data.ContainsKey(ident))
            {
                foreach(INodeInfo elem in data[ident].OrderBy(p => p.Name)) {
                    if(IsEnabled(elem.Name)) {
                        yield return elem;
                    }
                }
            }
        }

        /// <summary>
        /// List of constructed data by type of component.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> GetBy(Type type) => GetBy(new NodeIdent(GetComponentName(type), null));

        /// <summary>
        /// List of constructed data by IComponent
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> GetBy(IComponent component) => GetBy(component.GetType());

        public static bool IsComponent(Type type)
            => type.IsClass && type.GetInterfaces().Contains(typeof(IComponent))
                && type.Name.EndsWith(C_POSTFIX);

        /// <summary>
        /// Construct data from used components with ISobaCLoader.
        /// </summary>
        /// <param name="cLoader"></param>
        public Inspector(ISobaCLoader cLoader)
        {
            this.cLoader = cLoader;
            foreach(IComponent c in cLoader.Registered) {
#if DEBUG
                LSender.Send(this, $"Inspector: extracting from '{c.GetType().Name}'", MsgLevel.Trace);
#endif
                Extract(c, data);
            }

            if(data.Count < 1) {
                return;
            }

            // Aliases to components
            foreach(var root in data[new NodeIdent()])
            {
                if(root.Aliases == null) {
                    continue;
                }
                foreach(string alias in root.Aliases) {
                    data[new NodeIdent(alias, root.Link.method, root.Link.className)] = data[root.Link]; //shallow copies
                }
            }
        }

        /// <param name="c">From</param>
        /// <param name="data">To</param>
        protected void Extract(IComponent c, LInfo data)
        {
            Type type = c.GetType();

            foreach(Attribute attr in type.GetCustomAttributes(true)) {
                InspectLevelA(type, attr, data);
            }

            foreach(MethodInfo minf in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach(Attribute attr in Attribute.GetCustomAttributes(minf)) {
                    InspectLevelB(type, attr, minf, data);
                }
            }
        }

        /// <param name="type">Type of element.</param>
        /// <param name="attr">Found attribute.</param>
        /// <param name="method">Found method.</param>
        /// <param name="data"></param>
        protected void InspectLevelB(Type type, Attribute attr, MethodInfo method, LInfo data)
        {
            if(!IsComponent(type)
                || (attr.GetType() != typeof(MethodAttribute) && attr.GetType() != typeof(PropertyAttribute)))
            {
                return;
            }
            
            IAttrDomLevelB levB = (IAttrDomLevelB)attr;
            string className    = method.DeclaringType.FullName;
            NodeIdent ident     = new NodeIdent(levB.Parent ?? GetComponentName(type), levB.Method, (levB.Method == null)? null : className);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }

            className = (method.Name == null)? null : className;

            if(attr.GetType() == typeof(PropertyAttribute)) {
                data[ident].Add(new NodeInfo((PropertyAttribute)attr, method.Name, className));
                return;
            }

            if(attr.GetType() == typeof(MethodAttribute)) {
                data[ident].Add(new NodeInfo((MethodAttribute)attr, method.Name, className));
                return;
            }
        }

        /// <param name="type">Type of element.</param>
        /// <param name="attr">Found attribute.</param>
        /// <param name="data"></param>
        protected void InspectLevelA(Type type, Attribute attr, LInfo data)
        {
            if(!IsComponent(type) 
                || (attr.GetType() != typeof(DefinitionAttribute) && attr.GetType() != typeof(ComponentAttribute)))
            {
                return;
            }

            IAttrDomLevelA levA = (IAttrDomLevelA)attr;
            NodeIdent ident     = new NodeIdent(levA.Parent, null);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }

            if(attr.GetType() == typeof(DefinitionAttribute)) {
                data[ident].Add(new NodeInfo((DefinitionAttribute)attr));
                return;
            }

            if(attr.GetType() == typeof(ComponentAttribute))
            {
                INodeInfo node = new NodeInfo((ComponentAttribute)attr);
                data[ident].Add(node);
                AliasesToNodeLevelA(node, data[ident]);
                return;
            }
        }

        /// <summary>
        /// Work with aliases to components and to definitions.
        /// </summary>
        /// <param name="node">Node with aliases</param>
        /// <param name="data">All nodes</param>
        protected void AliasesToNodeLevelA(INodeInfo node, List<INodeInfo> data)
        {
            if(node.Aliases == null || node.Aliases.Length < 1) {
                return;
            }
            NodeType aliasType = (node.Type == NodeType.Component)? NodeType.AliasToComponent : NodeType.AliasToDefinition;
            
            foreach(string alias in node.Aliases)
            {
                data.Add(new NodeInfo
                (
                    alias, 
                    $"Alias to '{node.Name}' {node.Type}\n{node.Description}", 
                    new NodeIdent(node.Name, null), 
                    aliasType
                ));
            }
        }

        /// <summary>
        /// Checking the enabled status for element.
        /// </summary>
        /// <param name="elementName">Element name from storage.</param>
        /// <returns></returns>
        protected bool IsEnabled(string elementName)
        {
            foreach(IComponent c in cLoader.Registered)
            {
                Type type = c.GetType();
                
                if(GetComponentName(type) == elementName) {
                    if(!c.Enabled) {
                        return false;
                    }
                    continue;
                }

                string[] defs = GetDefinitionsNames(type);
                if(defs != null && defs.Any(def => def == elementName)) {
                    if(!c.Enabled) {
                        return false;
                    }
                }

                // aliases
                object attr = GetCustomAttribute(type, typeof(ComponentAttribute), false);
                if(attr == null) {
                    continue;
                }

                string[] aliases = ((ComponentAttribute)attr).Aliases;
                if(aliases != null && aliases.Any(a => a == elementName))
                {
                    if(!c.Enabled) {
                        return false;
                    }
                }
            }
            return true;
        }

        protected string[] GetDefinitionsNames(Type type)
        {
            object[] attr = type.GetCustomAttributes(typeof(DefinitionAttribute), false);
            if(attr == null) {
                return null;
            }
            return attr.Select(p => ((DefinitionAttribute)p).Name).ToArray();
        }

        /// <param name="type"></param>
        /// <returns>Original name by its class name or from ComponentAttribute</returns>
        protected string GetComponentName(Type type)
        {
            object attr = GetCustomAttribute(type, typeof(ComponentAttribute), false);
            if(attr != null) {
                return ((ComponentAttribute)attr).Name;
            }
            return type.Name;
        }

        /// <summary>
        /// Get the first custom attribute.
        /// note: CustomAttribute is allowed only with v4.5 from CustomAttributeExtensions
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private object GetCustomAttribute(Type type, Type attributeType, bool inherit)
        {
            object[] attr = type.GetCustomAttributes(attributeType, inherit);
            if(attr == null || attr.Length < 1) {
                return null;
            }
            return attr[0];
        }
    }
}
