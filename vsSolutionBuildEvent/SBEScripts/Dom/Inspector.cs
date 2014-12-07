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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        /// Main storage
        /// </summary>
        protected LInfo data = new LInfo();

        /// <summary>
        /// Construct data from used components with IBootloader
        /// </summary>
        /// <param name="bootloader"></param>
        public Inspector(IBootloader bootloader)
        {
            foreach(IComponent c in bootloader.Components) {
                Log.nlog.Trace("Inspector: extracting from '{0}'", c.GetType().Name);
                extract(c, data);
            }
        }

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
                    yield return elem;
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
            return getBy(new NodeIdent(type.Name, null));
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
            NodeIdent ident     = new NodeIdent((levB.Parent)?? getComponentName(type), levB.Method);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }

            if(attr.GetType() == typeof(PropertyAttribute)) {
                data[ident].Add(new NodeInfo((PropertyAttribute)attr, method.Name));
                return;
            }

            if(attr.GetType() == typeof(MethodAttribute)) {
                data[ident].Add(new NodeInfo((MethodAttribute)attr, method.Name));
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

            //IAttrDomLevelA levA = (IAttrDomLevelA)attr;
            NodeIdent ident = new NodeIdent(null, null);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }

            if(attr.GetType() == typeof(DefinitionAttribute)) {
                data[ident].Add(new NodeInfo((DefinitionAttribute)attr));
                return;
            }

            if(attr.GetType() == typeof(ComponentAttribute)) {
                data[ident].Add(new NodeInfo((ComponentAttribute)attr));
                return;
            }
        }

        /// <summary>
        /// Getting component name
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Original name by class name or what is specified with the ComponentAttribute</returns>
        protected string getComponentName(Type type)
        {
            object[] attr = type.GetCustomAttributes(typeof(ComponentAttribute), false);
            if(attr != null && attr.Length > 0) {
                return ((ComponentAttribute)attr[0]).Name;
            }
            return type.Name;
        }

        protected bool isComponent(Type type)
        {
            if(type.IsClass && type.GetInterfaces().Contains(typeof(IComponent)) 
                && type.Name.EndsWith("Component")) //TODO:
            {
                return true;
            }
            return false;
        }
    }
}
