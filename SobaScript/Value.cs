/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) SobaScript contributors: https://github.com/3F/Varhead/graphs/contributors
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using net.r_eg.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;

namespace net.r_eg.SobaScript
{
    /// <summary>
    /// TODO: standardization
    /// </summary>
    public static class Value
    {
        public const string TRUE    = "true";
        public const string FALSE   = "false";

        /// <summary>
        /// Separator for array data.
        /// </summary>
        public const string ARRAY_SEPARATOR = ",";

        /// <summary>
        /// Empty value by default.
        /// </summary>
        public static string Empty => string.Empty;

        /// <summary>
        /// A boolean value from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool ToBoolean(string val)
        {
            val = val.Trim();//.ToLower();

            // Boolean.Parse() converts only true/false value from string
            switch(val)
            {
                case "1":
                case "True":
                case "TRUE":
                case TRUE: {
                    return true;
                }

                case "0":
                case "False":
                case "FALSE":
                case FALSE: {
                    return false;
                }
            }

            throw new IncorrectSyntaxException($"Values: incorrect boolean value - '{val}'");
        }

        /// <summary>
        /// Int32 value from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToInt32(string val) => Int32.Parse(val.Trim());

        /// <summary>
        /// Unsigned Int32 value from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static uint ToUInt32(string val) => UInt32.Parse(val.Trim());

        /// <summary>
        /// Floating-point number with single-precision from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float ToFloat(string val) => Single.Parse(val.Trim(), CultureInfo.InvariantCulture);

        /// <summary>
        /// Floating-point number with double-precision from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double ToDouble(string val) => Double.Parse(val.Trim(), CultureInfo.InvariantCulture);

        /// <summary>
        /// A symbol as char from string data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static char ToChar(string val) => char.Parse(val.Trim());

        /// <param name="val"></param>
        /// <returns></returns>
        public static string From(bool val) => val.ToString().ToLower();

        /// <param name="val"></param>
        /// <returns></returns>
        public static string From(List<string> val) => string.Join(ARRAY_SEPARATOR, val);

        /// <param name="val"></param>
        /// <returns></returns>
        public static string From(int val) => val.ToString();

        /// <param name="val"></param>
        /// <returns></returns>
        public static string From(Enum val) => val.ToString();

        /// <param name="val">Including array of data.</param>
        /// <returns></returns>
        public static string From(object val)
        {
            if(val == null) {
                return string.Empty;
            }

            if(val is IEnumerable arr) {
                return string.Join(ARRAY_SEPARATOR, arr.Cast<string>());
            }

            return val.ToString();
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string From(string val) => val ?? string.Empty;

        /// <summary>
        /// Extract SNode.RArguments into system object[] data.
        /// </summary>
        /// <param name="args">SNode arguments.</param>
        /// <returns></returns>
        public static object[] Extract(RArgs args)
        {
            object[] ret = new object[args.Length];
            for(int i = 0; i < args.Length; ++i)
            {
                if(args[i].data is RArgs) {
                    ret[i] = Extract((RArgs)args[i].data);
                    continue;
                }
                ret[i] = args[i].data;
#if DEBUG
                LSender.Send(typeof(Value), $"Value.extract: SNode.Argument - '{ret[i]}'", MsgLevel.Trace);
#endif
            }
            return ret;
        }

        /// <summary>
        /// To pack complex object data in string format.
        /// Ex.: {"str", 123, -1.4, true, "str2", {1.2, "str2", false}, -24.574}
        /// </summary>
        /// <param name="data">Mixed data inc. complex object.</param>
        /// <returns>string with mixed data.</returns>
        public static string Pack(object data)
        {
            if(data == null) {
                return null;
            }

            if(!data.GetType().IsArray) {
                return data.ToString();
            }

            var ret = new List<object>();

            foreach(object val in (object[])data)
            {
                if(val.GetType().IsArray) {
                    ret.Add(Pack(val));
                    continue;
                }

                if(val is string) {
                    ret.Add($"\"{val}\"");
                    continue;
                }

                if(val is bool) {
                    ret.Add(val.ToString().ToLower());
                    continue;
                }

                if(val is char) {
                    ret.Add($"'{val}'");
                    continue;
                }

                if(val is Single) {
                    ret.Add($"{val.ToString().Replace(',', '.')}f");
                    continue;
                }

                if(val is Double) {
                    ret.Add(val.ToString().Replace(',', '.'));
                    continue;
                }

                ret.Add(val);
            }

            return string.Format("{{{0}}}", string.Join(", ", ret));
        }

        /// <summary>
        /// To pack string argument in object.
        /// </summary>
        /// <param name="arg">Argument for packing.</param>
        /// <returns></returns>
        public static object PackArgument(object arg)
        {
            if(arg == null) {
                return null;
            }

            if(!(arg is string) || string.IsNullOrWhiteSpace((string)arg)) {
                return arg;
            }

            IPM pm          = new PM($"_({arg})");
            Argument first  = pm.FirstLevel.Args[0];

            if(first.type != ArgumentType.Object) {
                return arg;
            }
            return Extract((RArgs)first.data);
        }

        /// <summary>
        /// Comparing values.
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <param name="coperator">Operator of comparison</param>
        /// <returns>Result of comparison</returns>
        public static bool Cmp(string left, string right = TRUE, string coperator = "===")
        {
            switch(coperator)
            {
                case "===": {
                    return (left == right);
                }
                case "!==": {
                    return (left != right);
                }
                case "~=": {
                    return (left.Contains(right));
                }
                case "==": {
                    return Equal(left, right);
                }
                case "!=": {
                    return !Equal(left, right);
                }
                case "^=": {
                    return left.StartsWith(right);
                }
                case "=^": {
                    return left.EndsWith(right);
                }
                case ">": {
                    return (ToInt32(left) > ToInt32(right));
                }
                case ">=": {
                    return (ToInt32(left) >= ToInt32(right));
                }
                case "<": {
                    return (ToInt32(left) < ToInt32(right));
                }
                case "<=": {
                    return (ToInt32(left) <= ToInt32(right));
                }
            }

            throw new IncorrectSyntaxException($"Values-comparison: incorrect operator - '{coperator}'");
        }

        /// <summary>
        /// Comparing values by chain: Int32 -> Boolean -> String
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        private static bool Equal(string left, string right)
        {
            if(Int32.TryParse(left, out int lNumber) && Int32.TryParse(right, out int rNumber))
            {
                LSender.Send(typeof(Value), $"Values-isEqual: as numeric '{left}' == '{right}'", MsgLevel.Trace);
                return lNumber == rNumber;
            }

            try
            {
                bool ret = (ToBoolean(left) == ToBoolean(right));
                LSender.Send(typeof(Value), $"Values-isEqual: as boolean '{left}' == '{right}'", MsgLevel.Trace);
                return ret;
            }
            catch(IncorrectSyntaxException)
            {
                LSender.Send(typeof(Value), $"Values-isEqual: as string '{left}' == '{right}'", MsgLevel.Trace);
            }
            return left == right;
        }
    }
}
