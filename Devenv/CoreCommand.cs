/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using net.r_eg.vsSBE.Bridge.CoreCommand;
using net.r_eg.vsSBE.Provider;

namespace net.r_eg.vsSBE.Devenv
{
    public class CoreCommand
    {
        /// <summary>
        /// Our vsSolutionBuildEvent library
        /// </summary>
        protected ILibrary library;

        /// <summary>
        /// All received CoreCommand from library.
        /// </summary>
        private Stack<ICoreCommand> receivedCommands = new Stack<ICoreCommand>();

        /// <summary>
        /// To abort all processes as soon as possible
        /// </summary>
        private volatile bool abort = false;

        /// <summary>
        /// Object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Checks of abort status.
        /// </summary>
        public bool IsAborted
        {
            get { return abort; }
        }

        /// <summary>
        /// To attach listener for work with CoreCommands.
        /// </summary>
        /// <returns></returns>
        public bool attachCoreCommandListener()
        {
            if(library == null) {
                return false;
            }

            lock(_lock) {
                detachCoreCommandListener();
                library.EntryPoint.CoreCommand += command;
            }
            return true;
        }

        /// <summary>
        /// To detach listener of CoreCommands.
        /// </summary>
        /// <returns></returns>
        public bool detachCoreCommandListener()
        {
            if(library == null) {
                return false;
            }

            lock(_lock) {
                library.EntryPoint.CoreCommand -= command;
            }
            return true;
        }

        /// <param name="library"></param>
        public CoreCommand(ILibrary library)
        {
            this.library = library;
        }
        
        /// <summary>
        /// Handler of core commands.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="c"></param>
        protected void command(object sender, CoreCommandArgs c)
        {
            switch(c.Type)
            {
                case CoreCommandType.AbortCommand: {
                    abortCommand(c);
                    break;
                }
                case CoreCommandType.BuildCancel: {
                    abort = true;
                    break;
                }
                case CoreCommandType.Nop: {
                    break;
                }
            }
            receivedCommands.Push(c);
        }

        /// <summary>
        /// Aborts latest command if it's possible
        /// </summary>
        /// <param name="c"></param>
        protected void abortCommand(ICoreCommand c)
        {
            if(receivedCommands.Count < 1) {
                return;
            }
            ICoreCommand last = receivedCommands.Peek();

            if(last.Type == CoreCommandType.BuildCancel) {
                abort = false;
            }
        }
    }
}