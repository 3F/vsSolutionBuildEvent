/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript.Z.Core contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Text.RegularExpressions;
using System.Threading;
using net.r_eg.Components;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.SobaScript.Z.Core.Condition;
using net.r_eg.Varhead.Exceptions;

namespace net.r_eg.SobaScript.Z.Core
{
    [Component("Box", "Container of data for operations like a template, repeating, etc.")]
    public class BoxComponent: ComponentAbstract, IComponent
    {
        /// <summary>
        /// Soft limit of iterations for user scripts.
        /// </summary>
        public long iterationLimit = UInt32.MaxValue;

        /// <summary>
        /// Core of conditional expressions.
        /// </summary>
        protected ExpressionAbstract expression;

        /// <summary>
        /// The main storage of the content of data node.
        /// </summary>
        protected Dictionary<string, string> content = new Dictionary<string, string>();

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "Box";

        /// <summary>
        /// Will be located before deepening if true.
        /// </summary>
        public override bool BeforeDeepening => true;

        private class ConditionalExpression: ExpressionAbstract
        {
            private BoxComponent cond;

            protected override string Evaluate(string data)
            {
                return cond.Evaluate(data);
            }

            public ConditionalExpression(BoxComponent cond, ISobaScript script, IEvMSBuild msbuild)
                : base(script, msbuild)
            {
                this.cond = cond;
            }
        }

        protected Regex RawExp
        {
            get;
            private set;
        } = new Regex(Pattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <summary>
        /// Prepare, Parse, and Evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var point       = EntryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            LSender.Send(this, $"`{ToString()}`: subtype - `{subtype}`, request - `{request}`", MsgLevel.Trace);

            if(subtype == "repeat" || subtype == "iterate")
            {
                IPM epm = new PM(
                    ExtractExpression(request, out string expression),
                    emsbuild
                );

                switch(subtype) {
                    case "repeat": {
                        return StRepeat(epm, expression);
                    }
                    case "iterate": {
                        return StIterate(epm, expression);
                    }
                }
            }

            IPM pm = new PM(request, emsbuild);

            switch(subtype) {
                case "operators": {
                    return StOperators(pm);
                }
                case "data": {
                    return StData(pm);
                }
            }

            throw new SubtypeNotFoundException(subtype);
        }

        public BoxComponent(ISobaScript soba)
            : base(soba)
        {
            Init();
        }

        /// <summary>
        /// Sample: #[Box repeat(condition [; silent]): content]
        /// </summary>
        [Method("repeat",
                "repeat($(i) < 10) { ... }\n\nExecutes a block until a specified expression evaluates to false.",
                new[] { "condition", "In" },
                new[] { "Conditional expression.", "mixed data" },
                CValType.Void,
                CValType.Expression, CValType.Input)]
        [Method("repeat",
                "repeat($(i) < 10; true) { ... }\n\nExecutes in silent mode a block until a specified expression evaluates to false.",
                new[] { "condition; silent", "In" },
                new[] { "\n  -> condition - Conditional expression.\n  -> silent - Flag of silent mode.", "mixed data" },
                CValType.Void,
                CValType.Expression, CValType.Input)]
        protected string StRepeat(IPM pm, string expression)
        {
            if(!pm.It(LevelType.Method, "repeat") || !pm.IsRight(LevelType.RightOperandColon)) {
                throw new IncorrectNodeException(pm);
            }

            var args = new PM().GetArguments(expression, ';');
            if(args == null || args.Count > 2 || args.Count < 1) {
                throw new PMArgException(args, expression);
            }

            string condition    = args[0].data.ToString();
            bool silent         = false;

            if(args.Count == 2)
            {
                if(args[1].type != ArgumentType.Boolean) {
                    throw new PMArgException(args[1], $"bool type for argument `silent`");
                }
                silent = (bool)args[1].data;
            }

            return DoRepeat(condition, pm.FirstLevel.Data, null, silent);
        }

        /// <summary>
        /// Sample: #[Box iterate(i = 0; $(i) &lt; 10; i = $([MSBuild]::Add($(i), 1))): content ]
        /// </summary>
        [Method("iterate",
                "iterate(i = 0; $(i) < 10; $([MSBuild]::Add($(i), 1))) { ... }\n\nExecutes a block when condition is true.",
                new[] { "initializer; condition; iterator", "In" },
                new[] { "\n  -> initializer - Optional initial state.\n  -> condition - Conditional expression.\n  -> iterator - Optional operation after each iteration.", "mixed data" },
                CValType.Void,
                CValType.Expression, CValType.Input)]
        protected string StIterate(IPM pm, string expression)
        {
            if(!pm.It(LevelType.Method, "iterate") || !pm.IsRight(LevelType.RightOperandColon)) {
                throw new IncorrectNodeException(pm);
            }

            var args = new PM().GetArguments(expression, ';');
            if(args == null || args.Count != 3) {
                throw new PMArgException(args, $"iterate(; condition; ) `{expression}`");
            }

            string initializer  = args[0].data.ToString();
            string condition    = args[1].data.ToString();
            string iterator     = args[2].data.ToString();

            if(!string.IsNullOrWhiteSpace(initializer)) {
                Evaluate($"$({initializer})");
            }

            iterator = string.IsNullOrWhiteSpace(iterator) ? null : $"$({iterator})";

            return DoRepeat(condition, pm.FirstLevel.Data, iterator, false);
        }

        protected virtual string DoRepeat(string condition, string block, string iterator, bool silent)
        {
            LSender.Send(this, $"DoRepeat: condition `{condition}`; block `{block}`; silent: {silent}");
            string ret = string.Empty;

#if DEBUG
            long idx = 0;
#endif
            while(expression.IsTrue(condition))
            {
                if(silent) {
                    Evaluate(block);
                }
                else {
                    ret += Evaluate(block);
                }

                if(iterator != null) {
                    Evaluate(iterator);
                }

#if DEBUG
                if(++idx > iterationLimit) {
                    throw new LimitException($"Iteration Limit of '{iterationLimit}' reached. Aborted.", iterationLimit);
                }
#endif
            }

            return ret;
        }

        /// <summary>
        /// #[Box operators.sleep(1000)]
        /// </summary>
        [Property("operators", "Access to common operators.")]
        [Method("sleep",
                "Suspends the current thread for a specified time.",
                "operators",
                nameof(StOperators),
                new[] { "timeout" },
                new[] { "Block current thread for a specified time in milliseconds." },
                CValType.Void,
                CValType.Integer)]
        protected string StOperators(IPM pm)
        {
            if(!pm.It(LevelType.Property, "operators")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel lvlOp = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "sleep"))
            {
                lvlOp.Is("sleep(integer timeout)", ArgumentType.Integer);
                Thread.Sleep((int)lvlOp.Args[0].data);

                return string.Empty;
            }

            throw new IncorrectNodeException(pm);
        }

        [Property("data", "Main templates with data.")]
        protected string StData(IPM pm)
        {
            if(!pm.It(LevelType.Property, "data")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalIs(LevelType.Method, "pack")) {
                return DataPack(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "free")) {
                return DataFree(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "get")) {
                return DataGet(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "clone")) {
                return DataClone(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// #[Box data.pack("test", false): content]
        /// </summary>
        [Method("pack",
                "To pack mixed data into container.",
                "data",
                nameof(StData),
                new[] { "name", "eval", "In" },
                new[] { "The name of package.", "Flag of evaluation of data before packing.", "mixed data" },
                CValType.Void,
                CValType.String, CValType.Boolean, CValType.Input)]
        protected string DataPack(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return DataPack(pm.PinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new PMLevelException(level, "data.pack(string name, boolean eval)");
        }

        protected string DataPack(IPM pm, string name, bool eval)
        {
            if(pm.FirstLevel.Type != LevelType.RightOperandColon) {
                throw new IncorrectNodeException(pm); // #[Box data.pack(name, eval): content]
            }

            if(content.ContainsKey(name)) {
                throw new LimitException($"Package of data with name '{name}' is already defined before. Use `data.free` to release data and avoid this error.", name);
            }

            LSender.Send(this, $"`data.pack('{name}', {eval}): {pm.FirstLevel.Data}`", MsgLevel.Trace);

            content[name] = eval ? Evaluate(pm.FirstLevel.Data) : pm.FirstLevel.Data;
            LSender.Send(this, $"packed as `{content[name]}`", MsgLevel.Trace);

            return string.Empty;
        }

        /// <summary>
        /// #[Box data.free("test")]
        /// </summary>
        [Method("free",
                "To release existing package from container.",
                "data",
                nameof(StData),
                new[] { "name" },
                new[] { "The name of package." },
                CValType.Void,
                CValType.String)]
        protected string DataFree(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble)) {
                return DataFree(pm.PinTo(1), (string)level.Args[0].data);
            }

            throw new PMLevelException(level, "data.free(string name)");
        }

        protected string DataFree(IPM pm, string name)
        {
            if(content.ContainsKey(name)) {
                content.Remove(name);
                LSender.Send(this, $"Package of data with name '{name}' has been removed");
            }

            return string.Empty;
        }

        /// <summary>
        /// #[Box data.get("test", true)]
        /// </summary>
        [Method("get",
                "To get package data.",
                "data",
                nameof(StData),
                new[] { "name", "forceEval" },
                new[] { "The name of package.", "To force Evaluate data of package before receiving." },
                CValType.Mixed,
                CValType.String, CValType.Boolean)]
        protected string DataGet(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return DataGet(pm.PinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new PMLevelException(level, "data.get(string name, boolean forceEval)");
        }

        protected string DataGet(IPM pm, string name, bool forceEval)
        {
            if(content.ContainsKey(name)) {
                return forceEval ? Evaluate(content[name]) : content[name];
            }

            throw new NotFoundException(name);
        }

        /// <summary>
        /// #[Box data.clone("test", 10)]
        /// #[Box data.clone("test", 10, true)]
        /// </summary>
        [Method("clone",
                "Multiple getting package data.",
                "data",
                nameof(StData),
                new[] { "name", "count" },
                new[] { "The name of package.", "The number of clones." },
                CValType.Mixed,
                CValType.String, CValType.Integer)]
        [Method("clone",
                "Multiple getting package data.",
                "data",
                nameof(StData),
                new[] { "name", "count", "forceEval" },
                new[] { "The name of package.", "The number of clones.", "To force Evaluate data of package before receiving." },
                CValType.Mixed,
                CValType.String, CValType.Integer, CValType.Boolean)]
        protected string DataClone(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Integer)) {
                return DataClone(pm.PinTo(1), (string)level.Args[0].data, (int)level.Args[1].data);
            }

            if(level.Is(ArgumentType.StringDouble, ArgumentType.Integer, ArgumentType.Boolean)) {
                return DataClone(pm.PinTo(1), (string)level.Args[0].data, (int)level.Args[1].data, (bool)level.Args[2].data);
            }

            throw new PMLevelException(level, "data.clone(string name, integer count [, boolean forceEval])");
        }

        protected string DataClone(IPM pm, string name, int count, bool forceEval = false)
        {
            if(!content.ContainsKey(name)) {
                throw new NotFoundException(name);
            }

            if(count < 1) {
                return string.Empty;
            }

            string val = forceEval ? Evaluate(content[name]) : content[name];

            string ret = string.Empty;
            for(int i = 0; i < count; ++i) {
                ret += val;
            }
            return ret;
        }

        /// <summary>
        /// To extract conditional expression.
        /// </summary>
        protected virtual string ExtractExpression(string raw, out string expression)
        {
            string ret = string.Empty;

            raw = RawExp.Replace(raw, delegate(Match m)
            {
                ret = m.Groups[1].Value;
                return "(expression)";
            }, 1);

            expression = ret;
            return raw;
        }

        protected void Init()
        {
            expression = new ConditionalExpression(this, soba, emsbuild);
        }
    }
}
