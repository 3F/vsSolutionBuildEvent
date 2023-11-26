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
    /// Action for Operation Mode
    /// </summary>
    public class ActionOperation: Action, IAction
    {
        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            IModeOperation operation = (IModeOperation)evt.Mode;
            if(operation.Command == null || operation.Command.Length < 1) {
                return true;
            }

            string[] parsed = new string[operation.Command.Length];

            for(int i = 0; i < operation.Command.Length; ++i) {
                parsed[i] = parse(evt, operation.Command[i]);
            }
            (new DTEOperation(cmd.Env, cmd.EventType)).exec(parsed, operation.AbortOnFirstError);

            return true;
        }

        /// <param name="cmd"></param>
        public ActionOperation(ICommand cmd)
            : base(cmd)
        {

        }
    }
}
