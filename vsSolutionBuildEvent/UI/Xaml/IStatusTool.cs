/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.UI.Xaml
{
    internal interface IStatusTool
    {
        /// <summary>
        /// Gets number from Warnings counter
        /// </summary>
        int Warnings { get; }

        /// <summary>
        /// Resets the Warnings counter
        /// </summary>
        void resetCounter();

        /// <summary>
        /// Availability of main panel for user
        /// </summary>
        /// <param name="enabled"></param>
        void enabledPanel(bool enabled);

        /// <summary>
        /// Notification about any warnings
        /// </summary>
        void warn();

        /// <summary>
        /// Updates data for controls
        /// </summary>
        void refresh();
    }
}
