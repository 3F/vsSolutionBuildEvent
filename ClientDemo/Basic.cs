/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace ClientDemo
{
    /// <summary>
    /// Simple variant of implementing the IEntryPointClient
    /// </summary>
    public class Basic: IEntryPointClient
    {
        /// <summary>
        /// Type of implementation.
        /// </summary>
        public ClientType Type
        {
            get { return ClientType.Isolated; }
        }

        /// <summary>
        /// Entry point of core library.
        /// Use this for additional work with core library.
        /// </summary>
        public IEntryPointCore Core
        {
            get;
            set;
        }

        /// <summary>
        /// Version of core library.
        /// Use this for internal settings in client if needed.
        /// </summary>
        public IVersion Version
        {
            get;
            set;
        }

        /// <summary>
        /// Should provide instance for handling IEvent2 by client from core library.
        /// </summary>
        public IEvent2 Event
        {
            get;
            protected set;
        }

        /// <summary>
        /// Should provide instance for handling IBuild by client from core library.
        /// </summary>
        public IBuild Build
        {
            get;
            protected set;
        }

        /// <summary>
        /// Internal Reporter
        /// </summary>
        internal ILog log = Log._.init(new StatusFrm());

        /// <summary>
        /// Object synch.
        /// </summary>
        private Object _lock = new Object();


        /// <summary>
        /// Load with DTE2 context.
        /// </summary>
        /// <param name="dte2">Unspecified EnvDTE80.DTE2 from EnvDTE80.dll</param>
        public void load(object dte2)
        {
            log.info("Entering load(object dte2)");
            init();
        }

        /// <summary>
        /// Load with isolated environment.
        /// </summary>
        /// <param name="sln">Full path to solution file</param>
        /// <param name="properties">Global properties for solution</param>
        public void load(string sln, Dictionary<string, string> properties)
        {
            log.info("Entering load(string sln, Dictionary<string, string> properties)");
            init();
        }

        /// <summary>
        /// Load with empty environment.
        /// </summary>
        public void load()
        {
            log.info("Entering load()");
            init();
        }

        protected void init()
        {
            log.info(
                "Version of core library: v{0} [{1}] API: v{2} /'{3}':{4}",
                Version.Number.ToString(),
                Version.BranchSha1,
                Version.Bridge.Number.ToString(2),
                Version.BranchName,
                Version.BranchRevCount
            );

            this.Event = new Event();
            this.Build = new Build();

            attachCoreCommandListener();
            log.show();
        }

        protected void attachCoreCommandListener()
        {
            lock(_lock) {
                detachCoreCommandListener();
                Core.CoreCommand += command;
            }
        }

        protected void detachCoreCommandListener()
        {
            lock(_lock) {
                Core.CoreCommand -= command;
            }
        }

        private void command(object sender, CoreCommandArgs e)
        {
            log.info("CoreCommand: '{0}'", e.Type);
        }
    }
}
