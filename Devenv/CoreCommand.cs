/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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