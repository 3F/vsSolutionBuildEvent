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

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Support of the Errors & Warnings Event type
    /// </summary>
    public interface ISolutionEventEW: ISolutionEvent
    {
        /// <summary>
        /// List of monitored codes
        /// Format: [any text] {error | warning} code####: localizable string
        /// http://msdn.microsoft.com/en-us/library/yxkt8b26%28v=vs.120%29.aspx
        /// </summary>
        List<string> Codes { get; set; }

        /// <summary>
        /// Whitelist or Blacklist for current codes
        /// </summary>
        bool IsWhitelist { get; set; }
    }
}