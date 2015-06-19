/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
