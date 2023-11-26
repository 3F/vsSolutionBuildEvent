/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Action for Files Mode
    /// </summary>
    public class ActionFile: Action, IAction
    {
        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            string cFiles = ((IModeFile)evt.Mode).Command;

            cFiles = parse(evt, cFiles);
            shell(evt, treatNewlineAs(" & ", cFiles));

            return true;
        }

        /// <param name="cmd"></param>
        public ActionFile(ICommand cmd)
            : base(cmd)
        {

        }
    }
}
