/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE
{
    public interface IMatchWords
    {
        /// <summary>
        /// Expression for comparison
        /// </summary>
        string Condition { get; set; }

        /// <summary>
        /// How to compare
        /// </summary>
        ComparisonType Type { get; set; }
    }

    public enum ComparisonType
    {
        Default,
        Regexp,
        Wildcards
    }
}
