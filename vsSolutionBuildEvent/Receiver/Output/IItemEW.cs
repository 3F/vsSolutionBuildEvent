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
using System.Runtime.InteropServices;

namespace net.r_eg.vsSBE.Receiver.Output
{
    /// <summary>
    /// Specifies item based on errors/warnings.
    /// </summary>
    [Guid("42B41C50-4BAD-48C2-9120-C43410D9F97C")]
    public interface IItemEW: IItem
    {
        /// <summary>
        /// Count of found errors.
        /// </summary>
        int ErrorsCount { get; }

        /// <summary>
        /// Count of found warnings.
        /// </summary>
        int WarningsCount { get; }

        /// <summary>
        /// Checks errors.
        /// </summary>
        bool IsErrors { get; }

        /// <summary>
        /// Checks warnings.
        /// </summary>
        bool IsWarnings { get; }

        /// <summary>
        /// Gets all errors in partial data.
        /// </summary>
        List<string> Errors { get; }

        /// <summary>
        /// Gets all warnings in partial data.
        /// </summary>
        List<string> Warnings { get; }

        /// <summary>
        /// Checking of rule for codes.
        /// </summary>
        /// <param name="type">Type of checking.</param>
        /// <param name="isWhitelist">The rule of whitelist or blacklist.</param>
        /// <param name="codes">List of codes for checking.</param>
        /// <returns>true value if it correct for this rule.</returns>
        bool checkRule(EWType type, bool isWhitelist, List<string> codes);
    }
}
