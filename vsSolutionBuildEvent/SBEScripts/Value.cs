/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts
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
        /// Getting boolean value
        /// Boolean.Parse() - converts only true/false value from string
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool toBoolean(string val)
        {
            val = val.Trim().ToLower();
            switch(val) {
                case "1":
                case VTRUE: {
                    return true;
                }
                case "0":
                case VFALSE: {
                    return false;
                }
            }
            throw new IncorrectSyntaxException("Values: incorrect boolean value - '{0}'", val);
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
            return String.Join(ARRAY_SEPARATOR, val);
        }

        /// <param name="val"></param>
        /// <returns></returns>
        public static string from(int val)
        {
            return val.ToString();
        }

        /// <param name="val">Including array of data</param>
        /// <returns></returns>
        public static string from(object val)
        {
            if(val == null) {
                return String.Empty;
            }

            if(val.GetType().IsArray) {
                string[] arr = Array.ConvertAll((object[])val, i => i.ToString());
                return String.Join(ARRAY_SEPARATOR, arr);
            }
            return val.ToString();
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
            throw new IncorrectSyntaxException("Values-comparison: incorrect operator - '{0}'", coperator);
        }

        /// <summary>
        /// Comparing values by chain: Int32 -> Boolean -> String
        /// </summary>
        /// <param name="left">left operand</param>
        /// <param name="right">right operand</param>
        /// <returns></returns>
        private static bool isEqual(string left, string right)
        {
            int lNumber, rNumber;
            if(Int32.TryParse(left, out lNumber) && Int32.TryParse(right, out rNumber)) {
                Log.nlog.Trace("Values-isEqual: as numeric '{0}' == '{1}'", left, right);
                return (lNumber == rNumber);
            }

            try {
                bool ret = (toBoolean(left) == toBoolean(right));
                Log.nlog.Trace("Values-isEqual: as boolean '{0}' == '{1}'", left, right);
                return ret;
            }
            catch(IncorrectSyntaxException) {
                Log.nlog.Trace("Values-isEqual: as string '{0}' == '{1}'", left, right);
            }
            return (left == right);
        }
    }
}
