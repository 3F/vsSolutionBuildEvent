/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.SobaScript.Z.Ext;
using net.r_eg.SobaScript.Z.Ext.IO;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Actions
{
    public abstract class Action: IAction
    {
        /// <summary>
        /// Command container
        /// </summary>
        protected ICommand cmd;

        protected IExer exer;

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
            Log.Info($"Prepared command: '{cmd}'");
            exer.UseShell
            (
                cmd, 
                evt.Id, 
                evt.Process.Waiting, 
                evt.Process.Hidden, 
                evt.Process.TimeLimit
            );
        }

        /// <summary>
        /// Access to parsers for event data.
        /// </summary>
        /// <param name="evt">Configured event.</param>
        /// <param name="data">Data to analysing.</param>
        /// <returns>Parsed data.</returns>
        public virtual string parse(ISolutionEvent evt, string data)
        {
            if(evt.SupportSBEScripts) {
                data = cmd.SBEScript.Eval(data, evt.SupportMSBuild);
            }

            if(evt.SupportMSBuild) {
                data = cmd.MSBuild.Eval(data);
            }

            return data;
        }

        /// <param name="cmd"></param>
        protected Action(ICommand cmd)
        {
            this.cmd = cmd;

            if(Bootloader._?.Soba.GetComponent(typeof(FileComponent)) is FileComponent fc) {
                exer = fc.Exer;
                return;
            }

            Log.Trace("Use new Exer instead of FileComponent");
            exer = new Exer(Settings.WPath, new EncDetector());

            Settings._.WorkPathUpdated +=
                (object sender, DataArgs<string> e) => exer.BasePath = e.Data;
        }

        protected string treatNewlineAs(string str, string data)
        {
            if(string.IsNullOrEmpty(data)) {
                return string.Empty;
            }
            return data.Trim(new char[]{ '\r', '\n' }).Replace("\r", "").Replace("\n", str);
        }
    }
}
