/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
