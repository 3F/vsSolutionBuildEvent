/*
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
using System.Collections.Generic;
using net.r_eg.SobaScript.Components;

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public interface IInspector
    {
        /// <summary>
        /// List of the constructed root-data
        /// </summary>
        IEnumerable<INodeInfo> Root { get; }

        /// <summary>
        /// List of constructed data by identification of node
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        IEnumerable<INodeInfo> getBy(NodeIdent ident);

        /// <summary>
        /// List of constructed data by type of component
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IEnumerable<INodeInfo> getBy(Type type);

        /// <summary>
        /// List of constructed data by IComponent
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        IEnumerable<INodeInfo> getBy(IComponent component);
    }
}
