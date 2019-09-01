﻿/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public class DomParser
    {
        /// <summary>
        /// Caching of retrieved properties
        /// </summary>
        private ConcurrentDictionary<string, IEnumerable<PropertyItem>> pcache = new ConcurrentDictionary<string, IEnumerable<PropertyItem>>();

        public enum KeysCommand
        {
            Default,
            /// <summary>
            /// Start of container '#['..
            /// </summary>
            Container,
            /// <summary>
            /// Keys combination - [ctrl] + [space]
            /// </summary>
            CtrlSpace,
            /// <summary>
            /// Level of property/method etc. with '.'
            /// </summary>
            LevelByDot,
            /// <summary>
            /// Space key
            /// </summary>
            Space,
            /// <summary>
            /// Start of container '$('..
            /// </summary>
            MSBuildContainer,
        }

        /// <summary>
        /// Mapper of the available components
        /// </summary>
        public IInspector Inspector
        {
            get;
            protected set;
        }

        public IEvMSBuild MSBuild
        {
            get;
            protected set;
        }

        protected IEnumerable<ICompletionData> ListEmpty
        {
            get { yield break; }
        }

        protected IEnumerable<ICompletionData> ListNull
        {
            get { return null; }
        }

        /// <param name="data">Where to find</param>
        /// <param name="offset">Position in data to begin viewing</param>
        /// <param name="cmd">Pushed command</param>
        /// <returns>Complete list for code completion</returns>
        public IEnumerable<ICompletionData> find(string data, int offset, KeysCommand cmd)
        {
            if(cmd == KeysCommand.Default) {
                return ListEmpty;
            }

            if(cmd == KeysCommand.MSBuildContainer) { // '#[' - root of the MSBuild container
                return listMSBuildProperties(null);
            }

            if(cmd == KeysCommand.Container) { // '#[' - root of the SBE container
                return listComponents(null);
            }

            data = region(data, offset);

            // '$( '<--
            if(Regex.IsMatch(data, @"\$\(\s*\w*$")) {
                return (cmd == KeysCommand.CtrlSpace)? listMSBuildProperties() : ListNull;
            }

            // '#[ '<--
            Match root = Regex.Match(data, @"\#\[
                                             (?:
                                                (?'data'\S+)
                                              |
                                                \s*
                                             )$", RegexOptions.IgnorePatternWhitespace);


            if(root.Success) {
                return (cmd == KeysCommand.CtrlSpace)? listComponents(root.Groups["data"].Value) : ListNull;
            }

            // '#[...' -->
            Match m = Regex.Match(data, @"^\#\[
                                          \s*
                                          (\S+)     #1 - Component
                                          \s*
                                          (.+)?     #2 - properties/methods etc. (optional)", 
                                          RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                return ListNull;
            }

            INodeInfo component = findComponent(data);
            if(component == null) {
                // hidden components:
                    //Match hc = Regex.Match(data, @"^\s*\#\[(\S+)");
                    //if(hc.Groups[1].Success) {
                    //    return list(new NodeIdent(hc.Groups[1].Value, null));
                    //}
                return ListNull;
            }
            return find(cmd, component, (m.Groups[2].Success)? m.Groups[2].Value : null);
        }

        public DomParser(IInspector inspector, IEvMSBuild msbuild = null)
        {
            Inspector   = inspector;
            MSBuild     = msbuild;
        }

        /// <param name="cmd">Pushed command</param>
        /// <param name="component">INodeInfo component for searching</param>
        /// <param name="raw">Full raw string to finding</param>
        /// <returns></returns>
        protected IEnumerable<ICompletionData> find(KeysCommand cmd, INodeInfo component, string raw)
        {
            if(raw == null)
            {
                if(cmd == KeysCommand.CtrlSpace || cmd == KeysCommand.Space) {
                    return list(new NodeIdent(component.Name, null));
                }
                return ListNull;
            }

            if(cmd == KeysCommand.Space) {
                return ListNull;
            }

            string ident = (new StringHandler()).ProtectMixedQuotes(raw.Trim());

            if(_isLatest('.', ident))
            {
                ident = ident.Substring(0, ident.Length - 1);
                if(cmd == KeysCommand.CtrlSpace) {
                    cmd = KeysCommand.LevelByDot;
                }
            }
            
            if(cmd == KeysCommand.CtrlSpace)
            {
                if(Regex.IsMatch(raw, @"(?:
                                          \s+
                                         |
                                          \([^.)]*?
                                         |
                                          \)
                                        )$", RegexOptions.IgnorePatternWhitespace))
                {
                    return ListNull;
                }
            }

            string[] parts = Regex.Replace(ident,
                                            SobaScript.RPattern.RoundBracketsContent, 
                                            "()", 
                                            RegexOptions.IgnorePatternWhitespace
                                           ).Split('.');

            NodeIdent id = new NodeIdent(component.Name, null);
            for(int i = 0; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Trim();

                if(cmd == KeysCommand.CtrlSpace && i == parts.Length - 1) {
                    return list(id, parts[i]);
                }

                INodeInfo info = infoBy(parts[i], id, (cmd == KeysCommand.LevelByDot));
                if(info == null) {
                    return ListEmpty;
                }

                id = info.Link;
            }

            if(cmd == KeysCommand.LevelByDot) {
                return list(id);
            }
            return ListEmpty;
        }

        /// <param name="name">Element name</param>
        /// <param name="ident">Identificator of node</param>
        /// <param name="strict"></param>
        /// <returns>null value if not found</returns>
        protected INodeInfo infoBy(string name, NodeIdent ident, bool strict)
        {
            foreach(INodeInfo info in Inspector.getBy(ident))
            {
                if(String.IsNullOrEmpty(info.Name)) { // hidden property
                    return infoBy(name, info.Link, strict);
                }

                string elem = (new StringHandler()).ProtectMixedQuotes(info.Name);
                elem = Regex.Replace(elem, SobaScript.RPattern.RoundBracketsContent, "()", RegexOptions.IgnorePatternWhitespace);
                
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
        protected string region(string data, int offset)
        {
            string reduced = data.Substring(0, offset);
            int pos = reduced.LastIndexOf("#[");

            if(pos != -1) {
                return data.Substring(pos, offset - pos);
            }
            return reduced;
        }

        /// <param name="name">Component name</param>
        /// <returns></returns>
        protected IEnumerable<ICompletionData> listComponents(string name)
        {
            foreach(INodeInfo info in Inspector.Root) {
                if(!String.IsNullOrEmpty(name) && !info.Name.Contains(name)) {
                    continue;
                }
                yield return new CompletionData(info.Name, info.Description, info.Type);
            }
        }

        protected IEnumerable<ICompletionData> listMSBuildProperties(string project = null)
        {
            foreach(PropertyItem prop in getMSBProperties(project)) {
                if(!String.IsNullOrWhiteSpace(prop.name)) {
                    yield return new CompletionData(prop.name, prop.value, InfoType.Definition);
                }
            }
        }

        protected IEnumerable<ICompletionData> list(NodeIdent ident, string name = null)
        {
            INodeInfo hidden = isHiddenLevel(ident);
            if(hidden != null) {
                foreach(ICompletionData inf in list(hidden.Link, name)) {
                    yield return inf;
                }
            }
            else
            {
                foreach(INodeInfo inf in Inspector.getBy(ident)) {
                    if(!String.IsNullOrEmpty(name) && !inf.Name.Contains(name)) {
                        continue;
                    }

                    yield return new CompletionData(
                                            inf.Name, 
                                            inf.Displaying, 
                                            String.Format("{0}\n{1}\n{2}", inf.Description, new String('_', 20), inf.Signature), 
                                            inf.Type);
                }
            }
        }

        /// <param name="ident"></param>
        /// <returns>node of the hidden level or null value if level is not hidden</returns>
        protected INodeInfo isHiddenLevel(NodeIdent ident)
        {
            INodeInfo ret = null;
            foreach(INodeInfo info in Inspector.getBy(ident)) {
                if(!String.IsNullOrEmpty(info.Name)) {
                    return null;
                }
                ret = info;
            }
            return ret;
        }

        protected INodeInfo findComponent(string data)
        {
            foreach(INodeInfo info in Inspector.Root)
            {
                if(info.Type != InfoType.Component && info.Type != InfoType.AliasToComponent
                    && info.Type != InfoType.Definition && info.Type != InfoType.AliasToDefinition)
                {
                    continue;
                }

                if(data.StartsWith(String.Format("#[{0}", info.Name))) { //TODO: IComponent <- Condition
                    return info;
                }
            }
            return null;
        }

        private IEnumerable<PropertyItem> getMSBProperties(string project)
        {
            string key = project;
            if(String.IsNullOrEmpty(key)) {
                key = "default";
            }

            if(!pcache.ContainsKey(key))
            {
                if(MSBuild == null) {
                    return new List<PropertyItem>();
                }
                pcache[key] = MSBuild.ListProperties(project);
                Log.Debug("Properties has been saved in cache ['{0}']", key);
            }
            return pcache[key];
        }

        private bool _isLatest(char symbol, string data)
        {
            return (!String.IsNullOrEmpty(data) && data[data.Length - 1] == symbol);
        }
    }
}
