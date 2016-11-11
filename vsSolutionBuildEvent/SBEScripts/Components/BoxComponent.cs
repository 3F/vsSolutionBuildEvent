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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts.Components.Condition;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    [Component("Box", "Container of data for operations like a template, repeating, etc.")]
    public class BoxComponent: Component, IComponent
    {
        /// <summary>
        /// Soft limit of iterations for user scripts.
        /// </summary>
        public long iterationLimit = UInt32.MaxValue;

        /// <summary>
        /// Core of conditional expressions.
        /// </summary>
        protected Expression expression;

        /// <summary>
        /// The main storage of the content of data node.
        /// </summary>
        protected Dictionary<string, string> content = new Dictionary<string, string>();

        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "Box"; }
        }

        /// <summary>
        /// Should be located before deepening
        /// </summary>
        public override bool BeforeDeepen
        {
            get { return true; }
        }

        protected sealed class ConditionalExpression: Expression
        {
            private BoxComponent cond;

            protected override string evaluate(string data)
            {
                return cond.evaluate(data);
            }

            public ConditionalExpression(BoxComponent cond, ISBEScript script, IMSBuild msbuild)
                : base(script, msbuild)
            {
                this.cond = cond;
            }
        }

        protected Regex RawExp
        {
            get;
            private set;
        } = new Regex(RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Instance of user-variables</param>
        public BoxComponent(IEnvironment env, IUserVariable uvariable)
            : base(env, uvariable)
        {
            init();
        }

        /// <param name="loader">Initialization with loader</param>
        public BoxComponent(IBootloader loader)
            : base(loader)
        {
            init();
        }

        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            var point       = entryPoint(data);
            string subtype  = point.Key;
            string request  = point.Value;

            Log.Trace("`{0}`: subtype - `{1}`, request - `{2}`", ToString(), subtype, request);

            if(subtype == "repeat" || subtype == "iterate")
            {
                string expression;
                IPM epm = new PM(
                    extractExpression(request, out expression),
                    msbuild
                );

                switch(subtype) {
                    case "repeat": {
                        return stRepeat(epm, expression);
                    }
                    case "iterate": {
                        return stIterate(epm, expression);
                    }
                }
            }

            IPM pm = new PM(request, msbuild);
            switch(subtype) {
                case "operators": {
                    return stOperators(pm);
                }
                case "data": {
                    return stData(pm);
                }
            }

            throw new SubtypeNotFoundException("Subtype `{0}` is not found", subtype);
        }

        /// <summary>
        /// Sample: #[Box repeat(condition [; silent]): content]
        /// </summary>
        [Method("repeat",
                "repeat($(i) < 10) { ... }\n\nExecutes a block until a specified expression evaluates to false.",
                new string[] { "condition", "In" },
                new string[] { "Conditional expression.", "mixed data" },
                CValueType.Void,
                CValueType.Expression, CValueType.Input)]
        [Method("repeat",
                "repeat($(i) < 10; true) { ... }\n\nExecutes in silent mode a block until a specified expression evaluates to false.",
                new string[] { "condition; silent", "In" },
                new string[] { "\n  -> condition - Conditional expression.\n  -> silent - Flag of silent mode.", "mixed data" },
                CValueType.Void,
                CValueType.Expression, CValueType.Input)]
        protected string stRepeat(IPM pm, string expression)
        {
            if(!pm.It(LevelType.Method, "repeat") || !pm.IsRight(LevelType.RightOperandColon)) {
                throw new IncorrectNodeException(pm);
            }

            Argument[] args = (new PM()).arguments(expression, ';');
            if(args == null || args.Length > 2 || args.Length < 1) {
                throw new InvalidArgumentException($"Incorrect arguments: {args?.Length} `{expression}`");
            }

            string condition    = args[0].data.ToString();
            bool silent         = false;

            if(args.Length == 2) {
                if(args[1].type != ArgumentType.Boolean) {
                    throw new InvalidArgumentException($"Incorrect type of argument `silent`: {args[1].type}");
                }
                silent = (bool)args[1].data;
            }

            return doRepeat(condition, pm.FirstLevel.Data, null, silent);
        }

        /// <summary>
        /// Sample: #[Box iterate(i = 0; $(i) &lt; 10; i = $([MSBuild]::Add($(i), 1))): content ]
        /// </summary>
        [Method("iterate",
                "iterate(i = 0; $(i) < 10; $([MSBuild]::Add($(i), 1))) { ... }\n\nExecutes a block when condition is true.",
                new string[] { "initializer; condition; iterator", "In" },
                new string[] { "\n  -> initializer - Optional initial state.\n  -> condition - Conditional expression.\n  -> iterator - Optional operation after each iteration.", "mixed data" },
                CValueType.Void,
                CValueType.Expression, CValueType.Input)]
        protected string stIterate(IPM pm, string expression)
        {
            if(!pm.It(LevelType.Method, "iterate") || !pm.IsRight(LevelType.RightOperandColon)) {
                throw new IncorrectNodeException(pm);
            }

            Argument[] args = (new PM()).arguments(expression, ';');
            if(args == null || args.Length != 3) {
                throw new InvalidArgumentException($"Incorrect arguments `iterate(; condition; )`: {args?.Length} `{expression}`");
            }

            string initializer  = args[0].data.ToString();
            string condition    = args[1].data.ToString();
            string iterator     = args[2].data.ToString();

            if(!String.IsNullOrWhiteSpace(initializer)) {
                evaluate($"$({initializer})");
            }

            iterator = String.IsNullOrWhiteSpace(iterator) ? null : $"$({iterator})";

            return doRepeat(condition, pm.FirstLevel.Data, iterator, false);
        }

        protected virtual string doRepeat(string condition, string block, string iterator, bool silent)
        {
            Log.Debug($"doRepeat: condition `{condition}`; block `{block}`; silent: {silent}");
            string ret = String.Empty;

#if DEBUG
            long idx = 0;
#endif
            while(expression.isTrue(condition))
            {
                if(silent) {
                    evaluate(block);
                }
                else {
                    ret += evaluate(block);
                }

                if(iterator != null) {
                    evaluate(iterator);
                }

#if DEBUG
                if(++idx > iterationLimit) {
                    throw new LimitException($"Iteration Limit of '{iterationLimit}' reached. Aborted.");
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
                "stOperators",
                new string[] { "timeout" },
                new string[] { "Block current thread for a specified time in milliseconds." },
                CValueType.Void,
                CValueType.Integer)]
        protected string stOperators(IPM pm)
        {
            if(!pm.It(LevelType.Property, "operators")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel lvlOp = pm.FirstLevel;

            if(pm.FinalEmptyIs(LevelType.Method, "sleep"))
            {
                lvlOp.Is("sleep(integer timeout)", ArgumentType.Integer);
                Thread.Sleep((int)lvlOp.Args[0].data);

                return String.Empty;
            }

            throw new IncorrectNodeException(pm);
        }

        [Property("data", "Main templates with data.")]
        protected string stData(IPM pm)
        {
            if(!pm.It(LevelType.Property, "data")) {
                throw new IncorrectNodeException(pm);
            }
            ILevel level = pm.FirstLevel;

            if(pm.FinalIs(LevelType.Method, "pack")) {
                return dataPack(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "free")) {
                return dataFree(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "get")) {
                return dataGet(level, pm);
            }

            if(pm.FinalEmptyIs(LevelType.Method, "clone")) {
                return dataClone(level, pm);
            }

            throw new IncorrectNodeException(pm);
        }

        /// <summary>
        /// #[Box data.pack("test", false): content]
        /// </summary>
        [Method("pack",
                "To pack mixed data into container.",
                "data",
                "stData",
                new string[] { "name", "eval", "In" },
                new string[] { "The name of package.", "Flag of evaluation of data before packing.", "mixed data" },
                CValueType.Void,
                CValueType.String, CValueType.Boolean, CValueType.Input)]
        protected string dataPack(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return dataPack(pm.pinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new ArgumentPMException(level, "data.pack(string name, boolean eval)");
        }

        protected string dataPack(IPM pm, string name, bool eval)
        {
            if(pm.FirstLevel.Type != LevelType.RightOperandColon) {
                throw new IncorrectNodeException(pm); // #[Box data.pack(name, eval): content]
            }

            if(content.ContainsKey(name)) {
                throw new LimitException($"Package of data with name '{name}' is already defined before. Use `data.free` to release data and avoid this error.");
            }

            Log.Trace($"`data.pack('{name}', {eval}): {pm.FirstLevel.Data}`");

            content[name] = eval ? evaluate(pm.FirstLevel.Data) : pm.FirstLevel.Data;
            Log.Trace($"packed as `{content[name]}`");

            return String.Empty;
        }

        /// <summary>
        /// #[Box data.free("test")]
        /// </summary>
        [Method("free",
                "To release existing package from container.",
                "data",
                "stData",
                new string[] { "name" },
                new string[] { "The name of package." },
                CValueType.Void,
                CValueType.String)]
        protected string dataFree(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble)) {
                return dataFree(pm.pinTo(1), (string)level.Args[0].data);
            }

            throw new ArgumentPMException(level, "data.free(string name)");
        }

        protected string dataFree(IPM pm, string name)
        {
            if(content.ContainsKey(name)) {
                content.Remove(name);
                Log.Debug($"Package of data with name '{name}' has been removed");
            }

            return String.Empty;
        }

        /// <summary>
        /// #[Box data.get("test", true)]
        /// </summary>
        [Method("get",
                "To get package data.",
                "data",
                "stData",
                new string[] { "name", "forceEval" },
                new string[] { "The name of package.", "To force evaluate data of package before receiving." },
                CValueType.Mixed,
                CValueType.String, CValueType.Boolean)]
        protected string dataGet(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean)) {
                return dataGet(pm.pinTo(1), (string)level.Args[0].data, (bool)level.Args[1].data);
            }

            throw new ArgumentPMException(level, "data.get(string name, boolean forceEval)");
        }

        protected string dataGet(IPM pm, string name, bool forceEval)
        {
            if(content.ContainsKey(name)) {
                return forceEval ? evaluate(content[name]) : content[name];
            }

            throw new NotFoundException($"Package of data with name '{name}' was not found.");
        }

        /// <summary>
        /// #[Box data.clone("test", 10)]
        /// #[Box data.clone("test", 10, true)]
        /// </summary>
        [Method("clone",
                "Multiple getting package data.",
                "data",
                "stData",
                new string[] { "name", "count" },
                new string[] { "The name of package.", "The number of clones." },
                CValueType.Mixed,
                CValueType.String, CValueType.Integer)]
        [Method("clone",
                "Multiple getting package data.",
                "data",
                "stData",
                new string[] { "name", "count", "forceEval" },
                new string[] { "The name of package.", "The number of clones.", "To force evaluate data of package before receiving." },
                CValueType.Mixed,
                CValueType.String, CValueType.Integer, CValueType.Boolean)]
        protected string dataClone(ILevel level, IPM pm)
        {
            if(level.Is(ArgumentType.StringDouble, ArgumentType.Integer)) {
                return dataClone(pm.pinTo(1), (string)level.Args[0].data, (int)level.Args[1].data);
            }

            if(level.Is(ArgumentType.StringDouble, ArgumentType.Integer, ArgumentType.Boolean)) {
                return dataClone(pm.pinTo(1), (string)level.Args[0].data, (int)level.Args[1].data, (bool)level.Args[2].data);
            }

            throw new ArgumentPMException(level, "data.clone(string name, integer count [, boolean forceEval])");
        }

        protected string dataClone(IPM pm, string name, int count, bool forceEval = false)
        {
            if(!content.ContainsKey(name)) {
                throw new NotFoundException($"Package of data with name '{name}' was not found.");
            }

            if(count < 1) {
                return String.Empty;
            }

            string val = forceEval ? evaluate(content[name]) : content[name];

            string ret = String.Empty;
            for(int i = 0; i < count; ++i) {
                ret += val;
            }
            return ret;
        }

        /// <summary>
        /// To extract conditional expression.
        /// </summary>
        protected virtual string extractExpression(string raw, out string expression)
        {
            string ret = String.Empty;

            raw = RawExp.Replace(raw, delegate(Match m)
            {
                ret = m.Groups[1].Value;
                return "(expression)";
            }, 1);

            expression = ret;
            return raw;
        }

        protected void init()
        {
            expression = new ConditionalExpression(this, script, msbuild);
        }
    }
}
