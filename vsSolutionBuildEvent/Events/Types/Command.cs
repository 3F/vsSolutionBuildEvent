/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events.Types
{
    /// <summary>
    /// Simple commands with format like this:
    /// https://msdn.microsoft.com/en-us/library/envdte._dte.executecommand.aspx
    /// </summary>
    public struct Command
    {
        /// <summary>
        /// Name of command.
        /// </summary>
        public string name;

        /// <summary>
        /// Arguments of command.
        /// </summary>
        public string args;
    }
}
