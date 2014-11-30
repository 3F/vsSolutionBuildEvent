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

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    public struct NodeIdent
    {
        /// <summary>
        /// Name of the parent specification (property/method/etc.)
        /// </summary>
        public string parent;
        /// <summary>
        /// Actual/real method name of the parent specification
        /// </summary>
        public string method;

        /// <param name="parent">Name of the parent specification (property/method/etc.)</param>
        /// <param name="method">Actual/real method name of the parent specification</param>
        public NodeIdent(string parent, string method)
        {
            this.parent = parent;
            this.method = method;
        }
    }
}
