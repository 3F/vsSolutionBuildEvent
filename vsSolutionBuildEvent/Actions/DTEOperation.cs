/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EnvDTE;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.Actions
{
    public class DTEOperation
    {
        /// <summary>
        /// Aggregation of prepared data for DTE
        /// </summary>
        public struct DTEPrepared
        {
            /// <summary>
            /// The name of the command to invoke.
            /// </summary>
            public string name;
            /// <summary>
            /// A string containing the arguments for DTE-command.
            /// see format with _DTE.ExecuteCommand
            /// </summary>
            public string args;

            public DTEPrepared(string name, string args)
            {
                this.name = name;
                this.args = args;
            }
        }

        /// <summary>
        /// Support recursive DTE-commands with level protection
        /// e.g.:
        ///   exec - "Debug.Start"
        ///   exec - "Debug.Start"
        ///   exec - "File.Print"
        /// </summary>
        public sealed class TQueue
        {
            public volatile uint level;
            public Queue<DTEPrepared> cmd;
        }

        /// <summary>
        /// Current type for recursive DTE commands
        /// </summary>
        protected SolutionEventType type;
        protected TQueue queue
        {
            get { return queues[type]; }
            set
            {
                if(!queues.ContainsKey(type)) {
                    queues[type] = value;
                }
            }
        }

        protected IEnvironment env;

        /// <summary>
        /// splitted by event type
        /// </summary>
        private static ConcurrentDictionary<SolutionEventType, TQueue> queues = new ConcurrentDictionary<SolutionEventType, TQueue>();

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _eLock = new Object();

        public virtual DTEPrepared parse(string line)
        {
            Match m = Regex.Match(line.Trim(), @"^
                                                   ([A-Za-z_0-9.]+) #1 - Command
                                                   (?:
                                                       \s*
                                                       \(
                                                          ([^)]+)   #2 - Arguments (optional)
                                                       \)           ## http://msdn.microsoft.com/en-us/library/envdte._dte.executecommand.aspx
                                                       \s*
                                                   )?
                                                 $", RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                Log.Debug("Operation '{0}' is not correct", line);
                throw new IncorrectSyntaxException("prepare failed - '{0}'", line);
            }
            return new DTEPrepared(m.Groups[1].Value, m.Groups[2].Success ? m.Groups[2].Value.Trim() : String.Empty);
        }

        public Queue<DTEPrepared> parse(string[] raw)
        {
            Queue<DTEPrepared> pRaw = new Queue<DTEPrepared>();

            foreach(string rawLine in raw) {
                pRaw.Enqueue(parse(rawLine));
            }
            return pRaw;
        }

        public void exec(DTEPrepared command)
        {
            exec(command.name, command.args);
        }

        public virtual void exec(string[] commands, bool abortOnFirstError)
        {
            exec(parse(format(ref commands)), abortOnFirstError);
        }

        public void exec(Queue<DTEPrepared> commands, bool abortOnFirstError)
        {
            if(queue.level < 1 && commands.Count < 1) {
                return;
            }

            lock(_eLock)
            {
                if(queue.level == 0) {
                    Log.Debug("DTE: init the queue");
                    queue.cmd = commands;
                    Settings._.IgnoreActions = true;
                }

                if(queue.cmd.Count < 1) {
                    Log.Debug("DTE recursion: all pushed :: level {0}", queue.level);
                    return;
                }
                ++queue.level;

                DTEPrepared current     = queue.cmd.Dequeue();
                string progressCaption  = String.Format("({0}/{1})", queue.level, queue.level + queue.cmd.Count);
                Exception terminated    = null;
                try {
                    // also error if command not available at current time
                    // * +causes recursion with Debug.Start, Debug.StartWithoutDebugging, etc.,
                    Log.Info("DTE exec {0}: '{1}' [{2}]", progressCaption, current.name, current.args);
                    exec(current.name, current.args);
                    Log.Debug("DTE exec {0}: done.", progressCaption);
                }
                catch(Exception ex) {
                    Log.Debug("DTE fail {0}: {1} :: '{2}'", progressCaption, ex.Message, current.name);
                    terminated = ex;
                }

                if(queue.cmd.Count > 0)
                {
                    // remaining commands
                    if(terminated != null && abortOnFirstError) {
                        Log.Info("DTE exec {0}: Aborted", progressCaption);
                    }
                    else {
                        Log.Debug("DTE {0}: step into", progressCaption);
                        exec((Queue<DTEPrepared>)null, abortOnFirstError);
                    }
                }

                --queue.level;

                if(queue.level < 1)
                {
                    Log.Debug("DTE: all completed");
                    Settings._.IgnoreActions = false;
                    if(terminated != null) {
                        throw new ComponentException(terminated.Message, terminated);
                    }
                }
            }
        }

        public virtual void exec(string name, string args = "")
        {
            env.exec(name, args);
        }

        public void flushQueue()
        {
            queue = new TQueue();
        }

        public void initQueue(SolutionEventType type)
        {
            if(queues.ContainsKey(type)) {
                return;
            }
            queues[type] = new TQueue();
        }

        public DTEOperation(IEnvironment env, SolutionEventType type)
        {
            this.env    = env;
            this.type   = type;
            initQueue(type);
        }

        protected virtual string[] format(ref string[] data)
        {
            return data.Where(s => s.Trim().Length > 0).ToArray();
        }
    }
}
