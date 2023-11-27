/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;

namespace net.r_eg.vsSBE.Configuration.Sys
{
    public sealed class Data
    {
        /// <summary>
        /// Debug mode for application.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Do not force show output window panel when initializing solution.
        /// </summary>
        /// <remarks>https://github.com/3F/vsSolutionBuildEvent/issues/76#issuecomment-1031551800</remarks>
        public bool SuppressInitOwp { get; set; }

        /// <summary>
        /// TopMost behavior for the main window.
        /// </summary>
        public bool PinMainWindow { get; set; }

        /// <summary>
        /// List of levels for disabling from logger.
        /// </summary>
        //[JsonProperty(TypeNameHandling = TypeNameHandling.None, ItemTypeNameHandling = TypeNameHandling.All)]
        public Dictionary<string, bool> LogIgnoreLevels { get; set; } = new();
    }
}
