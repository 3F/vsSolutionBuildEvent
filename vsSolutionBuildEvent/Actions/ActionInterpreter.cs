/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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

using System.Linq;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    /// <summary>
    /// Action for Interpreter Mode
    /// </summary>
    public class ActionInterpreter: Action, IAction
    {
        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public override bool process(ISolutionEvent evt)
        {
            if(((IModeInterpreter)evt.Mode).Handler.Trim().Length < 1) {
                throw new CompilerException("Interpreter: Handler is empty or not selected.");
            }

            string script   = ((IModeInterpreter)evt.Mode).Command;
            string wrapper  = ((IModeInterpreter)evt.Mode).Wrapper;

            script = parse(evt, script);
            script = treatNewlineAs(((IModeInterpreter)evt.Mode).Newline, script);

            switch(wrapper.Length) {
                case 1: {
                    script = string.Format("{0}{1}{0}", wrapper, script.Replace(wrapper, "\\" + wrapper));
                    break;
                }
                case 2: {
                    //pair as: (), {}, [] ...
                    //e.g.: (echo str&echo.&echo str) >> out
                    string wL = wrapper[0].ToString();
                    string wR = wrapper[1].ToString();
                    script = string.Format("{0}{1}{2}", wL, script.Replace(wL, "\\" + wL).Replace(wR, "\\" + wR), wR);
                    break;
                }
            }

            string handler = ((IModeInterpreter)evt.Mode).Handler;
            if(evt.SupportMSBuild) {
                handler = cmd.MSBuild.Eval(handler);
            }

            shell(evt, string.Format("{0} {1}", handler, script));
            return true;
        }

        /// <param name="cmd"></param>
        public ActionInterpreter(ICommand cmd)
            : base(cmd)
        {

        }
    }
}
