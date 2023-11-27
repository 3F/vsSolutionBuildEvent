/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Configuration
{
    /// <summary>
    /// Configure existing components
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Identification by class name
        /// </summary>
        public string ClassName
        {
            get;
            set;
        }

        /// <summary>
        /// Activation status
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }
    }
}
