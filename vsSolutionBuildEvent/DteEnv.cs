/*
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.SobaScript.Z.VS.Dte;
using net.r_eg.vsSBE.Actions;

namespace net.r_eg.vsSBE
{
    internal sealed class DteEnv: IDteEnv
    {
        private IEnvironment env;
        private Lazy<DTEOperation> dteo;

        private EnvDTE.CommandEvents cmdEvents;

        private readonly object sync = new object();

        /// <summary>
        /// Ability of work with DTE Commands.
        /// </summary>
        public bool IsAvaialbleDteCmd => env.Events != null;

        /// <summary>
        /// The last received command from EnvDTE.
        /// </summary>
        public IDteCommand LastCmd
        {
            get;
            private set;
        } = new _DteCommand();

        /// <summary>
        /// Execute command through EnvDTE.
        /// </summary>
        /// <param name="cmd"></param>
        public void Execute(string cmd)
            => dteo.Value.exec(new string[] { cmd }, false);

        public void Dispose() => DetachCommandEvents();

        public DteEnv(IEnvironment env)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));

            dteo = new Lazy<DTEOperation>(() => 
                new DTEOperation(env, Events.SolutionEventType.General)
            );

            AttachCommandEvents();
        }

        private void AttachCommandEvents()
        {
            if(!IsAvaialbleDteCmd || env.Events.CommandEvents == null)
            {
                Log.Info("CommandEvents: aren't available for current context.");
                return; //this can be for emulated DTE2 context
            }

            cmdEvents = env.Events.CommandEvents;

            lock(sync)
            {
                DetachCommandEvents();
                cmdEvents.BeforeExecute += OnCommandEventBefore;
                cmdEvents.AfterExecute  += OnCommandEventAfter;
            }
        }

        private void DetachCommandEvents()
        {
            if(cmdEvents == null) {
                return;
            }

            cmdEvents.BeforeExecute -= OnCommandEventBefore;
            cmdEvents.AfterExecute  -= OnCommandEventAfter;
        }

        private void OnCommandEventBefore(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
            => CommandEvent(true, guid, id, customIn, customOut);

        private void OnCommandEventAfter(string guid, int id, object customIn, object customOut)
            => CommandEvent(false, guid, id, customIn, customOut);

        private void CommandEvent(bool pre, string guid, int id, object customIn, object customOut)
        {
            LastCmd = new _DteCommand()
            {
                Guid        = guid,
                Id          = id,
                CustomIn    = customIn,
                CustomOut   = customOut,
                Pre         = pre
            };
        }

        private sealed class _DteCommand: IDteCommand
        {
            public string Guid { get; set; }

            public int Id { get; set; }

            public object CustomIn { get; set; }

            public object CustomOut { get; set; }

            public bool Cancel { get; set; }

            public bool Pre { get; set; }
        }
    }
}
