/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Bridge;

namespace net.r_eg.vsSBE.Devenv
{
    internal struct LibSettings: ISettings
    {
        /// <summary>
        /// Control of debug mode.
        /// </summary>
        public bool DebugMode
        {
            get;
            set;
        }
    }
}
