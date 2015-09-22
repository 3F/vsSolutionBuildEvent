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
using System.Linq;
using Microsoft.VisualStudio.Shell;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI.Plain
{
    /// <summary>
    /// TODO: temporary... need to move the all UI in additional subproject
    /// </summary>
    public class State
    {
        public static void print(ISolutionEvents data)
        {
            Func<ISolutionEvent[], string, string> about = delegate(ISolutionEvent[] evt, string caption)
            {
                if(evt == null) {
                    return String.Format("\n\t-- /--] {0} :: Not Initialized", caption);
                }

                System.Text.StringBuilder info = new System.Text.StringBuilder();
                info.Append(String.Format("\n\t{0,2} /{1,2}] {2} :: ", evt.Where(i => i.Enabled).Count(), evt.Length, caption));
                foreach(ISolutionEvent item in evt) {
                    info.Append(String.Format("[{0}]", (item.Enabled) ? "!" : "X"));
                }
                return info.ToString();
            };
            Log._.rawLn("\nReady:");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(about(data.PreBuild,      "Pre-Build     "));
            sb.Append(about(data.PostBuild,     "Post-Build    "));
            sb.Append(about(data.CancelBuild,   "Cancel-Build  "));
            sb.Append(about(data.CommandEvent,  "CommandEvent  "));
            sb.Append(about(data.WarningsBuild, "Warnings-Build"));
            sb.Append(about(data.ErrorsBuild,   "Errors-Build  "));
            sb.Append(about(data.OWPBuild,      "Output-Build  "));
            sb.Append(about(data.Transmitter,   "Transmitter   "));
            sb.Append(about(data.Logging,       "Logging       "));
            sb.Append("\n---\n");
            Log._.raw(sb.ToString());
        }

        public static void lineBegin()
        {
            Log._.raw(String.Format("{0}========== Build-Events started =========={0}", System.Environment.NewLine));
        }

        public static void summaryWarn(ToolWindowPane tool)
        {
            try {
                Log._.raw(String.Format("========== Build-Events completed: {0} Warnings ==========", ((UI.Xaml.IStatusTool)tool.Content).Warnings));
                //Log._.raw(String.Format("{0}{1}", new String('=', 80), System.Environment.NewLine));
                //Log._.raw(String.Format("Warnings: {0}", ((UI.Xaml.IStatusTool)tool.Content).Warnings));
                Log._.raw(String.Format("{0}{0}", System.Environment.NewLine));
            }
            catch(Exception ex) {
                Log.Debug("Failed summaryWarn: '{0}'", ex.ToString());
            }
        }
    }
}
