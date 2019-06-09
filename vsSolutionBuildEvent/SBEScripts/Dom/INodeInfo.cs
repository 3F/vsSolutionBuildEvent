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

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public interface INodeInfo
    {
        /// <summary>
        /// Element name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description for current element
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Technical description of the method/property
        /// </summary>
        string Signature { get; }

        /// <summary>
        /// Displays element over the 'Name' property.
        /// In general this useful for code completion
        /// </summary>
        string Displaying { get; }

        /// <summary>
        /// Aliases for primary name if used
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        /// Element type
        /// </summary>
        InfoType Type { get; }

        /// <summary>
        /// Link to the binding with other node
        /// </summary>
        NodeIdent Link { get; }
    }
}
