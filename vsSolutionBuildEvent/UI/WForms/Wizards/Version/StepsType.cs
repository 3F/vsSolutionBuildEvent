/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal enum StepsType
    {
        /// <summary>
        /// To select type of generation.
        /// </summary>
        Gen,

        /// <summary>
        /// To configure struct or class.
        /// </summary>
        Struct,

        /// <summary>
        /// To configure data of struct or class.
        /// </summary>
        CfgData,

        /// <summary>
        /// To configure the direct replacement.
        /// </summary>
        DirectRepl,

        /// <summary>
        /// To reconfigure of available fields.
        /// </summary>
        Fields,

        /// <summary>
        /// Final step with result.
        /// </summary>
        Final,
    }
}
