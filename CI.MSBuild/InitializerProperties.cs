/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    internal struct InitializerProperties
    {
        /// <summary>
        /// Full path to solution file.
        /// </summary>
        public string SolutionFile;

        /// <summary>
        /// Specifies properties for solution.
        /// </summary>
        public IDictionary<string, string> Properties;

        /// <summary>
        /// The targets for msbuild job.
        /// </summary>
        public string Targets;

        /// <summary>
        /// Full path to library.
        /// </summary>
        public string LibraryPath;

        /// <summary>
        /// User-defined arguments to CIM manager.
        /// </summary>
        public KArgs Args;
    }
}
