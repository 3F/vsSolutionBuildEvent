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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    using LInfo = ConcurrentDictionary<NodeIdent, List<INodeInfo>>;

    public class Inspector: IInspector
    {
        /// <summary>
        /// List of the constructed root-data
        /// </summary>
        public IEnumerable<INodeInfo> Root
        {
            get { return getBy(); }
        }

        /// <summary>
        /// Used loader
        /// </summary>
        protected IBootloader bootloader;

        /// <summary>
        /// Main storage
        /// </summary>
        protected LInfo data = new LInfo();

        /// <summary>
        /// List of constructed data by identification of node
        /// </summary>
        /// <param name="ident">Identificator of node</param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> getBy(NodeIdent ident = new NodeIdent())
        {
            if(data.ContainsKey(ident))
            {
                foreach(INodeInfo elem in data[ident].OrderBy(p => p.Name)) {
                    if(isEnabled(elem.Name)) {
                        yield return elem;
                    }
                }
            }
        }

        /// <summary>
        /// List of constructed data by type of component
        /// </summary>
        /// <param name="type">Type of component</param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> getBy(Type type)
        {
            return getBy(new NodeIdent(getComponentName(type), null));
        }

        /// <summary>
        /// List of constructed data by IComponent
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> getBy(IComponent component)
        {
            return getBy(component.GetType());
        }

        public static bool isComponent(Type type)
        {
            if(type.IsClass && type.GetInterfaces().Contains(typeof(IComponent)) 
                && type.Name.EndsWith("Component")) //TODO:
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Construct data from used components with IBootloader
        /// </summary>
        /// <param name="bootloader"></param>
        public Inspector(IBootloader bootloader)
        {
            this.bootloader = bootloader;
            foreach(IComponent c in bootloader.Registered) {
                Log.nlog.Trace("Inspector: extracting from '{0}'", c.GetType().Name);
                extract(c, data);
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
        protected void extract(IComponent c, LInfo data)
        {
            Type type = c.GetType();

            foreach(Attribute attr in type.GetCustomAttributes(true)) {
                inspectLevelA(type, attr, data);
            }

            foreach(MethodInfo minf in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
                foreach(Attribute attr in Attribute.GetCustomAttributes(minf)) {
                    inspectLevelB(type, attr, minf, data);
                }
            }
        }

        /// <param name="type">Type of element</param>
        /// <param name="attr">Found attribute</param>
        /// <param name="method">Found method</param>
        /// <param name="data"></param>
        protected void inspectLevelB(Type type, Attribute attr, MethodInfo method, LInfo data)
        {
            if(!isComponent(type)
                || (attr.GetType() != typeof(MethodAttribute) && attr.GetType() != typeof(PropertyAttribute)))
            {
                return;
            }
            
            IAttrDomLevelB levB = (IAttrDomLevelB)attr;
            string className    = method.DeclaringType.FullName;
            NodeIdent ident     = new NodeIdent((levB.Parent)?? getComponentName(type), levB.Method, (levB.Method == null)? null : className);

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

        /// <param name="type">Type of element</param>
        /// <param name="attr">Found attribute</param>
        /// <param name="data"></param>
        protected void inspectLevelA(Type type, Attribute attr, LInfo data)
        {
            if(!isComponent(type) 
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
                aliasesToNodeLevelA(node, data[ident]);
                return;
            }
        }

        /// <summary>
        /// Work with aliases to components and to definitions.
        /// </summary>
        /// <param name="node">Node with aliases</param>
        /// <param name="data">All nodes</param>
        protected void aliasesToNodeLevelA(INodeInfo node, List<INodeInfo> data)
        {
            if(node.Aliases == null || node.Aliases.Length < 1) {
                return;
            }
            InfoType aliasType = (node.Type == InfoType.Component)? InfoType.AliasToComponent : InfoType.AliasToDefinition;
            
            foreach(string alias in node.Aliases)
            {
                data.Add(new NodeInfo(alias, 
                                        String.Format("Alias to '{0}' {1}\n{2}", node.Name, node.Type, node.Description), 
                                        new NodeIdent(node.Name, null), 
                                        aliasType));
            }
        }

        /// <summary>
        /// Checking the enabled status for element
        /// </summary>
        /// <param name="elementName">Element name from storage</param>
        /// <returns></returns>
        protected bool isEnabled(string elementName)
        {
            foreach(IComponent c in bootloader.Registered)
            {
                Type type = c.GetType();
                
                if(getComponentName(type) == elementName) {
                    if(!c.Enabled) {
                        return false;
                    }
                    continue;
                }

                string[] defs = getDefinitionsNames(type);
                if(defs != null && defs.Any(def => def == elementName)) {
                    if(!c.Enabled) {
                        return false;
                    }
                }

                // aliases
                object attr = getCustomAttribute(type, typeof(ComponentAttribute), false);
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

        /// <summary>
        /// Gets names of definitions
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Is what is specified with all the DefinitionAttribute</returns>
        protected string[] getDefinitionsNames(Type type)
        {
            object[] attr = type.GetCustomAttributes(typeof(DefinitionAttribute), false);
            if(attr == null) {
                return null;
            }
            return attr.Select(p => ((DefinitionAttribute)p).Name).ToArray();
        }

        /// <summary>
        /// Getting component name
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Original name by class name or what is specified with the ComponentAttribute</returns>
        protected string getComponentName(Type type)
        {
            object attr = getCustomAttribute(type, typeof(ComponentAttribute), false);
            if(attr != null) {
                return ((ComponentAttribute)attr).Name;
            }
            return type.Name;
        }

        /// <summary>
        /// Get the first custom attribute.
        /// note: the GetCustomAttribute - allowed only with v4.5 from CustomAttributeExtensions
        /// </summary>
        /// <param name="attributeType"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private object getCustomAttribute(Type type, Type attributeType, bool inherit)
        {
            object[] attr = type.GetCustomAttributes(attributeType, inherit);
            if(attr == null || attr.Length < 1) {
                return null;
            }
            return attr[0];
        }
    }
}
