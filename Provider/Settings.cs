/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Provider
{
    /// <summary>
    /// Provider settings
    /// </summary>
    public struct Settings: ISettings
    {
        /// <summary>
        /// Additional details for all provider elements
        /// </summary>
        public bool DebugMode
        {
            get;
            set;
        }

        /// <summary>
        /// Settings for library
        /// </summary>
        public Bridge.ISettings LibSettings
        {
            get;
            set;
        }
    }
}
