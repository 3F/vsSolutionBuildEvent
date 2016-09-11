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

namespace net.r_eg.vsSBE.SBEScripts
{
    /// <summary>
    /// Specification of possible values for components and other places of the core
    /// </summary>
    public enum CValueType
    {
        /// <summary>
        /// Specifies that the:
        /// * Method doesn't return a value and/or takes no parameters
        /// * Property: for setting (readonly) / for getting - i.e. only as setter
        /// It's also used for binding the next Property/Method.
        /// </summary>
        Void,
        /// <summary>
        /// Value of different or untyped / uncertain types
        /// </summary>
        Mixed,
        /// <summary>
        /// Any stream data input.
        /// It's also used for binding the multiline data for:
        /// * Property: #[Component property: multiline data]
        /// * Method: #[Component method("arg"): multiline data]
        /// </summary>
        Input,
        /// <summary>
        /// Predefined data
        /// </summary>
        Enum,
        Const,

        String,
        Boolean,
        Integer,
        Float,

        /// <summary>
        /// Unsigned types
        /// </summary>
        UInteger,
        UFloat,

        /// <summary>
        /// Sequential list of mixed values.
        /// format: 1,2,3,4,5,6,7
        /// </summary>
        List,

        /// <summary>
        /// Object data. Similar as array with mixed data.
        /// Format: { "p1", true, { 12, 'n', -4.5f }, 12d }
        /// </summary>
        Object,

        /// <summary>
        /// Mixed expressions like Conditional Expression etc.
        /// </summary>
        Expression,
    }
}
