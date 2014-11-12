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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.r_eg.vsSBE.OWP
{
    /// <summary>
    /// Provides the available items from OutputWindowPane
    /// </summary>
    public sealed class Items: IItems
    {
        public BuildItem Build
        {
            get { return build; }
        }
        private BuildItem build = new BuildItem();

        /// <summary>
        /// Thread-safe getting the instance of Items
        /// </summary>
        public static Items _
        {
            get { return _lazy.Value; }
        }
        private static readonly Lazy<Items> _lazy = new Lazy<Items>(() => new Items());

        private Items(){}
    }
}
