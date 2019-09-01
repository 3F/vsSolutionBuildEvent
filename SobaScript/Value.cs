﻿/*
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
using System.Collections.Generic;
using System.Globalization;
using net.r_eg.Components;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Extensions;

namespace net.r_eg.SobaScript
{
    /// <summary>
    /// TODO: specification for SBE-Script
    /// </summary>
    public static class Value
    {
        public const string VTRUE   = "true";
        public const string VFALSE  = "false";

        /// <summary>
        /// Separator for array data.
        /// </summary>
        public const string ARRAY_SEPARATOR = ",";

        /// <summary>
        /// Empty value by default.
        /// </summary>
        public static string Empty
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Getting boolean value
        /// Boolean.Parse() - converts only true/false value from string
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool toBoolean(string val)
        {
            val = val.Trim();//.ToLower();
            switch(val) {
                case "1":
                case "True":
                case "TRUE":
                case VTRUE: {
                    return true;
                }
                case "0":
                case "False":
                case "FALSE":
                case VFALSE: {
                    return false;
                }
            }
            throw new IncorrectSyntaxException($"Values: incorrect boolean value - '{val}'");
        }

        /// <summary>
        /// Getting Int32 value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int toInt32(string val)
        {
            return Int32.Parse(val.Trim());
        }

        /// <summary>
        /// Getting Unsigned Int32 value
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static uint toUInt32(string val)
        {
            return UInt32.Parse(val.Trim());
        }

        /// <summary>
        /// Getting of floating-point number with single-precision.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float toFloat(string val)
        {
            return Single.Parse(val.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Getting of floating-point number with double-precision.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double toDouble(string val)
        {
            return Double.Parse(val.Trim(), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Getting of symbol as char.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static char toChar(string val)
        {
            return Char.Parse(val.Trim());
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(bool val)
        {
            return val.ToString().ToLower();
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(List<string> val)
        {
            return string.Join(ARRAY_SEPARATOR, val);
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(int val)
        {
            return val.ToString();
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(Enum val)
        {
            return val.ToString();
        }

        /// <param name="val">Including array of data</param>
        /// <returns></returns>
        public static string from(object val)
        {
            if(val == null) {
                return string.Empty;
            }

            if(val.GetType().IsArray) {
                string[] arr = Array.ConvertAll((object[])val, i => i.ToString());
                return string.Join(ARRAY_SEPARATOR, arr);
            }
            return val.ToString();
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(string val)
        {
            return (val)?? string.Empty;
        }

        /// <summary>
        /// Extract SNode.Argument[] into system object[] data.
        /// </summary>
        /// <param name="args">SNode arguments.</param>
        /// <returns></returns>
        public static object[] extract(SNode.Argument[] args)
        {
            object[] ret = new object[args.Length];
            for(int i = 0; i < args.Length; ++i)
            {
                if(args[i].data is SNode.Argument[]) {
                    ret[i] = extract((SNode.Argument[])args[i].data);
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
        public static string pack(object data)
        {
            if(data == null) {
                return null;
            }
            data = data.ToSystemObject();

            if(!data.GetType().IsArray) {
                return data.ToString();
            }
            List<object> ret = new List<object>();

            foreach(object val in (object[])data)
            {
                object sys = val.ToSystemObject();

                if(sys.GetType().IsArray) {
                    ret.Add(pack(sys));
                    continue;
                }

                if(sys is string) {
                    ret.Add(string.Format("\"{0}\"", sys));
                    continue;
                }

                if(sys is bool) {
                    ret.Add(sys.ToString().ToLower());
                    continue;
                }

                if(sys is char) {
                    ret.Add(string.Format("'{0}'", sys));
                    continue;
                }

                if(sys is Single) {
                    ret.Add(string.Format("{0}f", sys.ToString().Replace(',', '.')));
                    continue;
                }

                if(sys is Double) {
                    ret.Add(sys.ToString().Replace(',', '.'));
                    continue;
                }

                ret.Add(sys);
            }
            return string.Format("{{{0}}}", string.Join(", ", ret));
        }

        /// <summary>
        /// To pack string argument in object.
        /// </summary>
        /// <param name="arg">Argument for packing.</param>
        /// <returns></returns>
        public static object packArgument(object arg)
        {
            if(arg == null) {
                return null;
            }

            if(!(arg is string) || string.IsNullOrWhiteSpace((string)arg)) {
                return arg;
            }

            SNode.IPM pm            = new SNode.PM(string.Format("_({0})", arg));
            SNode.Argument first    = pm.FirstLevel.Args[0];

            if(first.type != SNode.ArgumentType.Object) {
                return arg;
            }
            return extract((SNode.Argument[])first.data);
        }

        /// <summary>
        /// Comparing values
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <param name="coperator">Operator of comparison</param>
        /// <returns>Result of comparison</returns>
        public static bool cmp(string left, string right = VTRUE, string coperator = "===")
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
                    return isEqual(left, right);
                }
                case "!=": {
                    return !isEqual(left, right);
                }
                case "^=": {
                    return left.StartsWith(right);
                }
                case "=^": {
                    return left.EndsWith(right);
                }
                case ">": {
                    return (toInt32(left) > toInt32(right));
                }
                case ">=": {
                    return (toInt32(left) >= toInt32(right));
                }
                case "<": {
                    return (toInt32(left) < toInt32(right));
                }
                case "<=": {
                    return (toInt32(left) <= toInt32(right));
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
        private static bool isEqual(string left, string right)
        {
            if(Int32.TryParse(left, out int lNumber) && Int32.TryParse(right, out int rNumber))
            {
                LSender.Send(typeof(Value), $"Values-isEqual: as numeric '{left}' == '{right}'", MsgLevel.Trace);
                return lNumber == rNumber;
            }

            try
            {
                bool ret = (toBoolean(left) == toBoolean(right));
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
