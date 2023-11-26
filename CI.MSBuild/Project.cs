/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Collections.Generic;

namespace net.r_eg.vsSBE.CI.MSBuild
{
    public sealed class Project
    {
        /// <summary>
        /// Project name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All available properties from msbuild instance
        /// </summary>
        public IDictionary<object, string> Properties { get; set; }

        /// <summary>
        /// Full path to project file
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Has error/s with some target/s
        /// </summary>
        public bool HasErrors { get; set; }
    }
}
