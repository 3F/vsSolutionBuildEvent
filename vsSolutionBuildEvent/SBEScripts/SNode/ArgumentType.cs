/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
    /// Specifies of possible types for arguments.
    /// </summary>
    public enum ArgumentType
    {
        /// <summary>
        /// Unspecified mixed data.
        /// </summary>
        Mixed,

        /// <summary>
        /// Common string.
        /// </summary>
        String,

        /// <summary>
        /// String from single quotes.
        /// </summary>
        StringSingle,

        /// <summary>
        /// String from double quotes.
        /// </summary>
        StringDouble,

        /// <summary>
        /// Boolean data.
        /// </summary>
        Boolean,

        /// <summary>
        /// Signed Integer number.
        /// </summary>
        Integer,

        /// <summary>
        /// Signed floating-point number with single-precision.
        /// </summary>
        Float,

        /// <summary>
        /// Signed floating-point number with double-precision.
        /// </summary>
        Double,

        /// <summary>
        /// Unspecified predefined data.
        /// </summary>
        EnumOrConst,

        /// <summary>
        /// Predefined data as Enum.
        /// </summary>
        Enum,

        /// <summary>
        /// Predefined data as Const.
        /// </summary>
        Const,
    }
}
