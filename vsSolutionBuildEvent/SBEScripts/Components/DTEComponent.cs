/*
 * Copyright (c) 2013-2016  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events.CommandEvents;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// For work with DTE
    /// </summary>
    [Component("DTE", "For work with EnvDTE.\nIs an assembly-wrapped COM library containing the objects and members for Visual Studio core automation.\n- http://msdn.microsoft.com/en-us/library/EnvDTE.aspx")]
    public class DTEComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "DTE "; }
        }

        /// <summary>
        /// Work with DTE-Commands
        /// </summary>
        protected DTEOperation dteo;

        /// <summary>
        /// Last received command from EnvDTE
        /// </summary>
        protected volatile IFilter lastCommandEvent = new Filter();

        /// <summary>
        /// Checks ability to work with CommandEvent
        /// </summary>
        protected bool IsAvaialbleCommandEvent
        {
            get { return env != null && env.Events != null; }
        }

        /// <summary>
        /// Provides command events for automation clients
        /// </summary>
        protected EnvDTE.CommandEvents cmdEvents;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();


        /// <param name="env">Used environment</param>
        public DTEComponent(IEnvironment env)
            : base(env)
        {
            dteo = new DTEOperation(env, Events.SolutionEventType.General);
            attachCommandEvents();
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            switch(subtype)
            {
                case "exec": {
                    return stExec(request);
                }
                case "events": {
                    return stEvents(request);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        /// <summary>
        /// DTE-command to execution
        /// e.g: #[DTE exec: command(arg)]
        /// </summary>
        /// <param name="data"></param>
        /// <returns>found command</returns>
        [Property("exec", "DTE-command to execution, e.g.: exec: command(arg)", CValueType.Void, CValueType.Input)]
        protected string stExec(string data)
        {
            Match m = Regex.Match(data, @"exec\s*:(.+)");
            if(!m.Success) {
                throw new OperandNotFoundException("Failed stExec - '{0}'", data);
            }
            string cmd = m.Groups[1].Value.Trim();
            Log.Debug("Found '{0}' to execution", cmd);

            dteo.exec(new string[] { cmd }, false);
            return Value.Empty;
        }

        /// <summary>
        /// Work with available events
        /// #[DTE events]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Property("events", "Operations with events.", CValueType.Void, CValueType.Void)]
        protected string stEvents(string data)
        {
            Match m = Regex.Match(data, @"events\s*\.\s*
                                                 (                  #1 - full ident
                                                   ([A-Za-z_0-9]+)  #2 - subtype
                                                   .*
                                                 )", RegexOptions.IgnorePatternWhitespace);
            if(!m.Success) {
                throw new OperandNotFoundException("Failed stEvents - '{0}'", data);
            }
            string ident    = m.Groups[1].Value;
            string subtype  = m.Groups[2].Value;

            switch(subtype)
            {
                case "LastCommand": {
                    Log.Trace("stEvents: use stLastCommand");
                    return stLastCommand(ident);
                }
            }
            throw new SubtypeNotFoundException("stEvents: not found subtype - '{0}'", subtype);
        }

        /// <summary>
        /// Last received command from EnvDTE
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Property("LastCommand", "Last received command.", "events", "stEvents", CValueType.Void, CValueType.Void)]
        [Property("Guid", "Scope for Command ID", "LastCommand", "stLastCommand", CValueType.String, CValueType.Void)]
        [Property("Id", "Command ID", "LastCommand", "stLastCommand", CValueType.Integer, CValueType.Void)]
        [Property("CustomIn", "Custom input parameters.", "LastCommand", "stLastCommand", CValueType.String, CValueType.Void)]
        [Property("CustomOut", "Custom output parameters.", "LastCommand", "stLastCommand", CValueType.String, CValueType.Void)]
        [Property("Pre", "Flag of the execution command - Before / After", "LastCommand", "stLastCommand", CValueType.Boolean, CValueType.Void)]
        protected string stLastCommand(string data)
        {
            if(!IsAvaialbleCommandEvent) {
                throw new NotSupportedOperationException("CommandEvents: aren't available for current context. Use full environment.");
            }

            Match m = Regex.Match(data, @"LastCommand\s*\.\s*(.+)\s*");
            if(!m.Success) {
                throw new OperandNotFoundException("Failed stLastCommand - '{0}'", data);
            }
            string operation = m.Groups[1].Value;

            switch(operation)
            {
                case "Guid": {
                    return (lastCommandEvent.Guid)?? Value.Empty;
                }
                case "Id": {
                    return Value.from(lastCommandEvent.Id);
                }
                case "CustomIn": {
                    return Value.pack(lastCommandEvent.CustomIn)?? Value.Empty;
                }
                case "CustomOut": {
                    return Value.pack(lastCommandEvent.CustomOut)?? Value.Empty;
                }
                case "Pre": {
                    return Value.from(lastCommandEvent.Pre); // see commandEvent below
                }
            }
            throw new OperationNotFoundException("stLastCommand: not found operation - '{0}'", operation);
        }

        protected void attachCommandEvents()
        {
            if(!IsAvaialbleCommandEvent) {
                Log.Info("CommandEvents: aren't available for current context.");
                return; //this can be for emulated DTE2 context
            }

            cmdEvents = env.Events.CommandEvents;
            lock(_lock) {
                cmdEvents.BeforeExecute -= commandEventBefore;
                cmdEvents.BeforeExecute += commandEventBefore;
                cmdEvents.AfterExecute  -= commandEventAfter;
                cmdEvents.AfterExecute  += commandEventAfter;
            }
        }

        protected void detachCommandEvents()
        {
            if(cmdEvents == null) {
                return;
            }
            lock(_lock) {
                cmdEvents.BeforeExecute -= commandEventBefore;
                cmdEvents.AfterExecute  -= commandEventAfter;
            }
        }

        protected void commandEventBefore(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            commandEvent(true, guid, id, customIn, customOut);
        }

        protected void commandEventAfter(string guid, int id, object customIn, object customOut)
        {
            commandEvent(false, guid, id, customIn, customOut);
        }

        private void commandEvent(bool pre, string guid, int id, object customIn, object customOut)
        {
            lastCommandEvent = new Filter() {
                Guid        = guid,
                Id          = id,
                CustomIn    = customIn,
                CustomOut   = customOut,
                Pre         = pre // only as flag (Before / After) for DTEComponent
            };
        }
    }
}
