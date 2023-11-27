/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events
{
    public class ExecutionOrder: IExecutionOrder
    {
        public const string FIRST_PROJECT   = ":?: First Project";
        public const string LAST_PROJECT    = ":?: Last Project";
        public const string FIRST_TYPE      = ":?: First Type";
        public const string LAST_TYPE       = ":?: Last Type";

        /// <summary>
        /// Project name
        /// </summary>
        public string Project
        {
            get;
            set;
        }

        /// <summary>
        /// Range of execution
        /// </summary>
        public ExecutionOrderType Order
        {
            get;
            set;
        }

        /// <summary>
        /// Checks name for special types.
        /// </summary>
        /// <param name="name">Project name.</param>
        /// <returns></returns>
        public static bool IsSpecial(string name)
        {
            return name == FIRST_PROJECT
                    || name == LAST_PROJECT
                    || name == FIRST_TYPE
                    || name == LAST_TYPE;
        }
    }
}
