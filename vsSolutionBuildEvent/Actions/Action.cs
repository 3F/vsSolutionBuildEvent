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

using System;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    public abstract class Action: IAction
    {
        /// <summary>
        /// Command container
        /// </summary>
        protected ICommand cmd;

        /// <summary>
        /// Process for specified event.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <returns>Result of handling.</returns>
        public abstract bool process(ISolutionEvent evt);

        /// <summary>
        /// Access to shell for event data.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="cmd">Formatted command to shell.</param>
        public virtual void shell(ISolutionEvent evt, string cmd)
        {
            Log.nlog.Info("Prepared command: '{0}'", cmd);

            HProcess p = new HProcess(Settings.WorkingPath);
            p.useShell(cmd, evt.Process.Waiting, evt.Process.Hidden, evt.Process.TimeLimit);
        }

        /// <summary>
        /// Access to parsers for event data.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="data">Data to analysing.</param>
        public virtual void parse(ISolutionEvent evt, string data)
        {
            if(evt.SupportSBEScripts) {
                data = cmd.SBEScript.parse(data, evt.SupportMSBuild);
            }

            if(evt.SupportMSBuild) {
                data = cmd.MSBuild.parse(data);
            }
        }

        /// <param name="cmd"></param>
        public Action(ICommand cmd)
        {
            this.cmd = cmd;
        }

        protected string treatNewlineAs(string str, string data)
        {
            if(String.IsNullOrEmpty(data)) {
                return String.Empty;
            }
            return data.Trim(new char[]{ '\r', '\n' }).Replace("\r", "").Replace("\n", str);
        }
    }
}
