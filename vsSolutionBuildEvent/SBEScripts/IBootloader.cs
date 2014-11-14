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

using System.Collections.Generic;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.SBEScripts
{
    public interface IBootloader
    {
        /// <summary>
        /// All registered components
        /// </summary>
        IEnumerable<IComponent> Components { get; }

        /// <summary>
        /// Operations with environment
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Container for user-variables
        /// </summary>
        IUserVariable UVariable { get; }
    }
}
