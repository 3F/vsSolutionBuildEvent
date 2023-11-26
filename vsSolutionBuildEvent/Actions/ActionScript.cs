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
    /// Action for Script Mode
    /// </summary>
    public class ActionScript: Action, IAction
    {
        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            parse(evt, ((IModeScript)evt.Mode).Command);
            return true;
        }

        /// <param name="cmd"></param>
        public ActionScript(ICommand cmd)
            : base(cmd)
        {

        }
    }
}
