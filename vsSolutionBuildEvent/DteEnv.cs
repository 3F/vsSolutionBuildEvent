/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
