/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.VS contributors: https://github.com/3F/Varhead/graphs/contributors
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
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.VS.Dte;

namespace net.r_eg.SobaScript.Z.VS
{
    [Component("DTE", "For work with EnvDTE.\nAssembly-wrapped COM library containing the objects and members for Visual Studio core automation.\n- http://msdn.microsoft.com/en-us/library/EnvDTE.aspx")]
    public class DteComponent: ComponentAbstract, IComponent
    {
        protected IDteEnv env;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "DTE ";

        /// <summary>
        /// Prepare, parse, and evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            switch(subtype)
            {
                case "exec": {
                    return StExec(new PM(request));
                }
                case "events": {
                    return StEvents(new PM(request));
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public DteComponent(ISobaScript soba, IDteEnv env)
            : base(soba)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        /// <summary>
        /// #[DTE exec: command(arg)]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns>found command</returns>
        [Property("exec", "To execute DTE-command; exec: command(arg)", CValType.Void, CValType.Input)]
        protected string StExec(IPM pm)
        {
            if(!pm.It(LevelType.Property, "exec") || !pm.IsRight(LevelType.RightOperandColon)) {
                throw new IncorrectNodeException(pm);
            }

            string cmd = pm.FirstLevel.Data.Trim();

            if(string.IsNullOrWhiteSpace(cmd)) {
                throw new ArgumentException("The command cannot be empty.");
            }

            LSender.Send(this, $"Execute command `{cmd}`");

            env.Execute(cmd);
            return Value.Empty;
        }

        /// <summary>
        /// For work with available events.
        /// #[DTE events]
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("events", "Operations with events.", CValType.Void, CValType.Void)]
        protected string StEvents(IPM pm)
        {
            if(!pm.It(LevelType.Property, "events")) {
                throw new IncorrectNodeException(pm);
            }

            if(pm.It(LevelType.Property, "LastCommand")) {
                return StLastCommand(pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// Last received command from EnvDTE
        /// </summary>
        /// <param name="pm"></param>
        /// <returns></returns>
        [Property("LastCommand", "The last received command.", "events", "StEvents")]
        [Property("Guid", "Scope of Command ID", "LastCommand", nameof(StLastCommand), CValType.String)]
        [Property("Id", "Command ID", "LastCommand", nameof(StLastCommand), CValType.Integer)]
        [Property("CustomIn", "Custom input parameters.", "LastCommand", nameof(StLastCommand), CValType.Object)]
        [Property("CustomOut", "Custom output parameters.", "LastCommand", nameof(StLastCommand), CValType.Object)]
        [Property("Pre", "Flag of execution of the command - Before / After", "LastCommand", nameof(StLastCommand), CValType.Boolean)]
        protected string StLastCommand(IPM pm)
        {
            if(!env.IsAvaialbleDteCmd) {
                throw new NotSupportedOperationException("DTE commands aren't available for current context. Use full environment.");
            }

            if(pm.FinalEmptyIs(LevelType.Property, "Guid")) {
                return (env.LastCmd.Guid) ?? Value.Empty;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "Id")) {
                return Value.From(env.LastCmd.Id);
            }

            if(pm.FinalEmptyIs(LevelType.Property, "CustomIn")) {
                return Value.Pack(env.LastCmd.CustomIn) ?? Value.Empty;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "CustomOut")) {
                return Value.Pack(env.LastCmd.CustomOut) ?? Value.Empty;
            }

            if(pm.FinalEmptyIs(LevelType.Property, "Pre")) {
                return Value.From(env.LastCmd.Pre);
            }

            throw new IncorrectNodeException(pm);
        }
    }
}
