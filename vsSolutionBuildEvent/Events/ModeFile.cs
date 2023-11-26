/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events.Mapping.Json;

namespace net.r_eg.vsSBE.Events
{
    /// <summary>
    /// Processing with files
    /// </summary>
    public class ModeFile: ModeCommand, IMode, IModeFile
    {
        /// <summary>
        /// Type of implementation
        /// </summary>
        public ModeType Type
        {
            get { return ModeType.File; }
        }
    }
}
