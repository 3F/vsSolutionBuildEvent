/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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

namespace net.r_eg.vsSBE.SBEScripts.SNode
{
    /// <summary>
    /// Available statuses for evaluation with MSBuild and other engines.
    /// </summary>
    public enum EvalType
    {
        None = 0x0,

        /// <summary>
        /// String argument from double quotes.
        /// </summary>
        ArgStringD = 0x01,

        /// <summary>
        /// String argument from single quotes.
        /// </summary>
        ArgStringS = 0x02,

        /// <summary>
        /// Standard right operand via '='
        /// </summary>
        RightOperandStd = 0x04,

        /// <summary>
        /// Right operand via ':'
        /// </summary>
        RightOperandColon = 0x08,
    }
}
