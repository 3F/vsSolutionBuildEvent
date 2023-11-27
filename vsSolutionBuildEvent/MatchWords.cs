/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE
{
    public class MatchWords: IMatchWords
    {
        /// <summary>
        /// Expression for comparison
        /// </summary>
        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }
        private string condition;

        /// <summary>
        /// How to compare
        /// </summary>
        public ComparisonType Type
        {
            get { return type; }
            set { type = value; }
        }
        private ComparisonType type;
    }
}
