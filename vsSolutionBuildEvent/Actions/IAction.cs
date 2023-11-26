/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    [Guid("3DA65163-3C80-4F75-BDFA-BFA5C1865128")]
    public interface IAction
    {
        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        bool process(ISolutionEvent evt);

        /// <summary>
        /// Access to shell for event data.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="cmd">Formatted command to shell.</param>
        void shell(ISolutionEvent evt, string cmd);

        /// <summary>
        /// Access to parsers for event data.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="data">Data to analysing.</param>
        /// <returns>Parsed data.</returns>
        string parse(ISolutionEvent evt, string data);
    }
}
