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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using net.r_eg.EvMSBuild;

namespace net.r_eg.SobaScript.Mapper
{
    /// <summary>
    /// Mapper for SobaScript components and their nodes.
    /// https://github.com/3F/SobaScript.Mapper
    /// 
    /// Please note: initially it was part of https://github.com/3F/vsSolutionBuildEvent
    /// </summary>
    public class SbMapper: ISbMapper
    {
        /// <summary>
        /// Mapper of available components.
        /// </summary>
        public IInspector Inspector
        {
            get;
            protected set;
        }

        /// <summary>
        /// Used instance of E-MSBuild engine.
        /// </summary>
        public IEvMSBuild EvMSBuild
        {
            get;
            protected set;
        }

        protected IEnumerable<INodeInfo> ListEmpty
        {
            get { yield break; }
        }

        protected IEnumerable<INodeInfo> ListNull => null;

        /// <param name="name">Component name</param>
        /// <returns></returns>
        public IEnumerable<INodeInfo> ListComponents(string name)
        {
            foreach(INodeInfo info in Inspector.Root)
            {
                if(!string.IsNullOrEmpty(name) && !info.Name.Contains(name)) {
                    continue;
                }

                yield return info;
            }
        }

        /// <param name="data">Where to look.</param>
        /// <param name="offset">Position in data to start searching.</param>
        /// <param name="cmd">Specified command for this search.</param>
        /// <returns>Found items.</returns>
        public IEnumerable<INodeInfo> Find(string data, int offset, KDataCommand cmd)
        {
            if(cmd == KDataCommand.Default) {
                return ListEmpty;
            }

            if(cmd == KDataCommand.MSBuildContainer) { // '$(' - root 
                return ListMSBuildProperties(null);
            }

            if(cmd == KDataCommand.Container) { // '#[' - root 
                return ListComponents(null);
            }

            data = Region(data, offset);

            // '$( '<--
            if(Regex.IsMatch(data, Pattern.EvMLeft)) {
                return (cmd == KDataCommand.CtrlSpace)? ListMSBuildProperties() : ListNull;
            }

            // '#[ '<--
            Match root = Regex.Match(data, Pattern.SobaLeft, RegexOptions.IgnorePatternWhitespace);


            if(root.Success) {
                return (cmd == KDataCommand.CtrlSpace)? ListComponents(root.Groups["data"].Value) : ListNull;
            }

            // '#[...' -->
            Match m = Regex.Match(data, Pattern.SobaMiddle, RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                return ListNull;
            }

            INodeInfo component = FindComponent(data);
            if(component == null)
            {
                // hidden components:
                    //Match hc = Regex.Match(data, @"^\s*\#\[(\S+)");
                    //if(hc.Groups[1].Success) {
                    //    return list(new NodeIdent(hc.Groups[1].Value, null));
                    //}
                return ListNull;
            }

            return Find(cmd, component, (m.Groups[2].Success)? m.Groups[2].Value : null);
        }

        public SbMapper(IInspector inspector, IEvMSBuild emsbuild = null)
        {
            Inspector   = inspector ?? throw new ArgumentNullException(nameof(inspector));
            EvMSBuild   = emsbuild;
        }

        protected virtual IEnumerable<PropertyItem> GetMSBProperties(string project)
        {
            if(EvMSBuild == null) {
                return new List<PropertyItem>();
            }
            return EvMSBuild.ListProperties(project);
        }

        /// <param name="cmd"></param>
        /// <param name="component">INodeInfo component for searching.</param>
        /// <param name="raw">Full raw string to search.</param>
        /// <returns></returns>
        protected IEnumerable<INodeInfo> Find(KDataCommand cmd, INodeInfo component, string raw)
        {
            if(raw == null)
            {
                if(cmd == KDataCommand.CtrlSpace || cmd == KDataCommand.Space) {
                    return ListInfo(new NodeIdent(component.Name, null));
                }
                return ListNull;
            }

            if(cmd == KDataCommand.Space) {
                return ListNull;
            }

            string ident = new StringHandler().ProtectMixedQuotes(raw.Trim());

            if(IsLatest('.', ident))
            {
                ident = ident.Substring(0, ident.Length - 1);
                if(cmd == KDataCommand.CtrlSpace) {
                    cmd = KDataCommand.LevelByDot;
                }
            }
            
            if(cmd == KDataCommand.CtrlSpace)
            {
                if(Regex.IsMatch(raw, Pattern.Finalization, RegexOptions.IgnorePatternWhitespace)) {
                    return ListNull;
                }
            }

            string[] parts = Regex.Replace
            (
                ident,
                SobaScript.Pattern.RoundBracketsContent, 
                "()", 
                RegexOptions.IgnorePatternWhitespace
            )
            .Split('.');

            NodeIdent id = new NodeIdent(component.Name, null);
            for(int i = 0; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Trim();

                if(cmd == KDataCommand.CtrlSpace && i == parts.Length - 1) {
                    return ListInfo(id, parts[i]);
                }

                INodeInfo info = InfoBy(parts[i], id, (cmd == KDataCommand.LevelByDot));
                if(info == null) {
                    return ListEmpty;
                }

                id = info.Link;
            }

            if(cmd == KDataCommand.LevelByDot) {
                return ListInfo(id);
            }
            return ListEmpty;
        }

        /// <param name="name">Element name.</param>
        /// <param name="ident">Identificator of node.</param>
        /// <param name="strict"></param>
        /// <returns>null value if not found</returns>
        protected INodeInfo InfoBy(string name, NodeIdent ident, bool strict)
        {
            foreach(INodeInfo info in Inspector.GetBy(ident))
            {
                if(string.IsNullOrEmpty(info.Name)) { // hidden property
                    return InfoBy(name, info.Link, strict);
                }

                string elem = new StringHandler().ProtectMixedQuotes(info.Name);
                elem = Regex.Replace(elem, SobaScript.Pattern.RoundBracketsContent, "()", RegexOptions.IgnorePatternWhitespace);
                
                if((strict && elem == name)
                    || (!strict && elem.Contains(name)))
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets working region for data
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected string Region(string data, int offset)
        {
            string reduced = data.Substring(0, offset);
            int pos = reduced.LastIndexOf("#[");

            if(pos != -1) {
                return data.Substring(pos, offset - pos);
            }
            return reduced;
        }

        protected IEnumerable<INodeInfo> ListMSBuildProperties(string project = null)
        {
            foreach(PropertyItem prop in GetMSBProperties(project))
            {
                if(!string.IsNullOrWhiteSpace(prop.name)) {
                    yield return new NodeInfo(prop.name, prop.value, NodeType.Definition);
                }
            }
        }

        protected IEnumerable<INodeInfo> ListInfo(NodeIdent ident, string name = null)
        {
            INodeInfo hidden = IsHiddenLevel(ident);
            if(hidden != null)
            {
                foreach(INodeInfo inf in ListInfo(hidden.Link, name)) {
                    yield return inf;
                }
            }
            else
            {
                foreach(INodeInfo inf in Inspector.GetBy(ident))
                {
                    if(!string.IsNullOrEmpty(name) && !inf.Name.Contains(name)) {
                        continue;
                    }

                    yield return new NodeInfo(inf, $"{inf.Description}\n{new string('_', 20)}\n{inf.Signature}");
                }
            }
        }

        /// <param name="ident"></param>
        /// <returns>node of the hidden level or null value if level is not hidden</returns>
        protected INodeInfo IsHiddenLevel(NodeIdent ident)
        {
            INodeInfo ret = null;
            foreach(INodeInfo info in Inspector.GetBy(ident)) {
                if(!string.IsNullOrEmpty(info.Name)) {
                    return null;
                }
                ret = info;
            }
            return ret;
        }

        protected INodeInfo FindComponent(string data)
        {
            foreach(INodeInfo info in Inspector.Root)
            {
                if(info.Type != NodeType.Component && info.Type != NodeType.AliasToComponent
                    && info.Type != NodeType.Definition && info.Type != NodeType.AliasToDefinition)
                {
                    continue;
                }

                if(data.StartsWith(string.Format("#[{0}", info.Name))) { //TODO: IComponent <- Condition
                    return info;
                }
            }

            return null;
        }

        private bool IsLatest(char symbol, string data)
        {
            return (!string.IsNullOrEmpty(data) && data[data.Length - 1] == symbol);
        }
    }
}
