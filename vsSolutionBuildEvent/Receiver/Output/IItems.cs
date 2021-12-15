/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Specifies operations with items.
    /// </summary>
    [Guid("962E70D3-FBA4-4019-82AD-2473C45F7D7B")]
    public interface IItems
    {
        /// <summary>
        /// Get item for type and identifier.
        /// </summary>
        /// <param name="type">Type of item.</param>
        /// <param name="ident">Identifier of item.</param>
        /// <returns>Unspecified common item.</returns>
        IItem get(ItemType type, Ident ident);

        /// <summary>
        /// Get EW item for identifier.
        /// </summary>
        /// <param name="ident">Identifier of item.</param>
        /// <returns>EW item.</returns>
        IItemEW getEW(Ident ident);
    }
}
