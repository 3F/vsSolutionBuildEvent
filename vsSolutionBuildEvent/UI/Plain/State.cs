/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Globalization;
using System.Linq;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI.Plain
{
    /// <summary>
    /// TODO: temporary... need to move the all UI in additional subproject
    /// </summary>
    public class State
    {
        private static string Timestamp
        {
            get {
                return DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + ".ffff");
            }
        }

        public static void Print(ISolutionEvents data)
        {
            Func<ISolutionEvent[], string, string> about = delegate(ISolutionEvent[] evt, string caption)
            {
                if(evt == null) {
                    return String.Format("\n    -- /--] {0} :: Not Initialized", caption);
                }

                System.Text.StringBuilder info = new System.Text.StringBuilder();
                info.Append(String.Format("\n    {0,2} /{1,2}] {2} :: ", evt.Where(i => i.Enabled).Count(), evt.Length, caption));
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
            sb.Append(about(data.SlnOpened,     "Sln-Opened    "));
            sb.Append(about(data.SlnClosed,     "Sln-Closed    "));
            sb.Append(about(data.Transmitter,   "Transmitter   "));
            sb.Append(about(data.Logging,       "Logging       "));
            sb.Append("\n---\n");
            Log._.raw(sb.ToString());
        }

        public static void BuildBegin()
        {
            Log._.raw(
                String.Format(
                    "{0}========== [{1}] Build-Events started =========={0}",
                    System.Environment.NewLine,
                    Timestamp
                )
            );
        }

        public static void BuildEnd()
        {
            Log._.raw(
                String.Format(
                    "========== [{1}] Build-Events completed =========={0}{0}",
                    System.Environment.NewLine,
                    Timestamp
                )
            );
        }
    }
}
