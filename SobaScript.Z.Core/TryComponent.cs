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
using System.Text.RegularExpressions;
using net.r_eg.Components;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.SNode;
using net.r_eg.Varhead;

namespace net.r_eg.SobaScript.Z.Core
{
    [Definition("try", "try/catch support")]
    [Definition("{ }catch{ }", "try/catch\n\nProtects from errors in try{...} block and handles it in catch{...}", "try")]
    [Definition("{ }catch(err, msg){ }", "try/catch with error type and its message.", "try")]
    public class TryComponent: ComponentAbstract, IComponent
    {
        /*
         * TODO: lazy compiled patterns
         */

        private readonly Lazy<Regex> _crule;

        /// <summary>
        /// Expression when to start processing.
        /// </summary>
        public override string Activator => "try";

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        public override bool PostParse => true;

        /// <summary>
        /// Will be located before deepening if true.
        /// </summary>
        public override bool BeforeDeepening => true;

        /// <summary>
        /// Main rule of container.
        /// </summary>
        protected string Rule => string.Format
        (
            @"^\[\s*
                    try
                    \s*{0}\s*             #1      - try
                    catch
                    (?:\s*
                    \((?'args'.*?)\)    #args   - optional arguments
                    \s*)?
                    \s*{0}\s*             #2      - catch
                \]",
                Pattern.CurlyBracketsContent
        );

        /// <summary>
        /// Compiled rule.
        /// </summary>
        protected Regex CRule => _crule.Value;

        /// <summary>
        /// Prepare, Parse, and Evaluate mixed data through SobaScript supported syntax.
        /// </summary>
        /// <param name="data">Mixed input data.</param>
        /// <returns>Evaluated end value.</returns>
        public override string Eval(string data)
        {
            var hString = new StringHandler();

            Match m = CRule.Match(hString.ProtectMixedQuotes(data.Trim()));
            if(!m.Success) {
                throw new IncorrectSyntaxException($"Failed {nameof(TryComponent)} - '{data}'");
            }

            string blockTry     = hString.Recovery(m.Groups[1].Value);
            string blockCatch   = hString.Recovery(m.Groups[2].Value);
            string args         = (m.Groups["args"].Success)? hString.Recovery(m.Groups["args"].Value) : null;

            try {
                return Evaluate(blockTry);
            }
            catch(Exception ex) {
                return DoCatch(blockCatch, ex, (new PM()).GetArguments(args));
            }
        }

        public TryComponent(ISobaScript soba)
            : base(soba)
        {
            _crule = new Lazy<Regex>(() => new Regex
            (
                Rule,
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.Singleline |
                RegexOptions.Compiled
            ));
        }

        protected string DoCatch(string cmd, Exception ex, RArgs args)
        {
            LSender.Send(this, $"Catched error `{ex.Message}`", MsgLevel.Info);

            if(args == null) {
                return Evaluate(cmd);
            }

            if(args.Count == 2
                && args[0].type == ArgumentType.EnumOrConst
                && args[1].type == ArgumentType.EnumOrConst)
            {
                // try{ }catch(err, msg){ }
                return DoCatch(cmd, ex, args[0].data.ToString(), args[1].data.ToString());
            }

            throw new NotSupportedOperationException("the format of the catch block is incorrect or not supported yet.");
        }

        protected string DoCatch(string cmd, Exception ex, string err, string msg)
        {
            try {
                Setvar(err, ex.GetType().FullName);
                Setvar(msg, ex.Message);

                return Evaluate(cmd);
            }
            finally {
                Delvar(err, msg);
            }
        }

        private void Setvar(string name, string value)
        {
            uvars.SetVariable(name, null, value);
            uvars.Evaluate(name, null, new EvaluatorBlank(), true);
        }

        private void Delvar(params string[] names)
        {
            if(names == null) {
                return;
            }

            foreach(string name in names) {
                uvars.Unset(name, null);
            }
        }
    }
}
