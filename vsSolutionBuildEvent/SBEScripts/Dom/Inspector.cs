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

    public class Inspector
    {
        /// <summary>
        /// List of root constructed data
        /// </summary>
        public IEnumerable<INodeInfo> Root
        {
            get { return getBy(); }
        }

        /// <summary>
        /// Thread-safe getting the instance of Inspector class
        /// </summary>
        public static Inspector _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Inspector> _lazy = new Lazy<Inspector>(() => new Inspector());

        /// <summary>
        /// Main storage
        /// </summary>
        protected LInfo data = new LInfo();

        /// <summary>
        /// Construct data from used components with IBootloader
        /// </summary>
        /// <param name="bootloader"></param>
        public void extract(IBootloader bootloader)
        {
            data.Clear();
            foreach(IComponent c in bootloader.Components)
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
                foreach(INodeInfo elem in data[ident]) {
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

        /// <param name="type">Type of component</param>
        /// <param name="attr">Found attribute</param>
        /// <param name="method">Found method</param>
        /// <param name="data"></param>
        protected void inspectLevelB(Type type, Attribute attr, MethodInfo method, LInfo data)
        {
            if(attr.GetType() != typeof(MethodAttribute) && attr.GetType() != typeof(PropertyAttribute)) {
                return;
            }
            
            IAttrDomLevelB levB = (IAttrDomLevelB)attr;
            NodeIdent ident     = new NodeIdent((levB.Parent)?? type.Name, levB.Method);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }

            if(attr.GetType() == typeof(PropertyAttribute)) {
                data[ident].Add(new NodeInfo((PropertyAttribute)levB, method.Name));
            }
            else if(attr.GetType() == typeof(MethodAttribute)) {
                data[ident].Add(new NodeInfo((MethodAttribute)levB, method.Name));
            }
        }

        /// <param name="type">Type of component</param>
        /// <param name="attr">Found attribute</param>
        /// <param name="data"></param>
        protected void inspectLevelA(Type type, Attribute attr, LInfo data)
        {
            if(attr.GetType() != typeof(DefinitionAttribute)) {
                return;
            }
            IAttrDomLevelA levA = (IAttrDomLevelA)attr;
            NodeIdent ident     = new NodeIdent(null, null);

            if(!data.ContainsKey(ident)) {
                data[ident] = new List<INodeInfo>();
            }
            data[ident].Add(new NodeInfo((DefinitionAttribute)levA));
        }

        private Inspector() { }
    }
}
