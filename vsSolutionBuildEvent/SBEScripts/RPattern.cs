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

namespace net.r_eg.vsSBE.SBEScripts
{
    public static class RPattern
    {
        /// <summary>
        /// Content from double quotes
        /// </summary>
        public static string DoubleQuotesContent
        {
            get {
                if(doubleQuotesContent == null) {
                    doubleQuotesContent = quotesContent('"', true);
                }
                return doubleQuotesContent;
            }
        }
        private static string doubleQuotesContent;

        /// <summary>
        /// Content from single quotes
        /// </summary>
        public static string SingleQuotesContent
        {
            get {
                if(singleQuotesContent == null) {
                    singleQuotesContent = quotesContent('\'', true);
                }
                return singleQuotesContent;
            }
        }
        private static string singleQuotesContent;

        /// <summary>
        /// Double quotes with content
        /// </summary>
        public static string DoubleQuotesContentFull
        {
            get {
                if(doubleQuotesContentFull == null) {
                    doubleQuotesContentFull = quotesContent('"', false);
                }
                return doubleQuotesContentFull;
            }
        }
        private static string doubleQuotesContentFull;

        /// <summary>
        /// Single quotes with content
        /// </summary>
        public static string SingleQuotesContentFull
        {
            get {
                if(singleQuotesContentFull == null) {
                    singleQuotesContentFull = quotesContent('\'', false);
                }
                return singleQuotesContentFull;
            }
        }
        private static string singleQuotesContentFull;

        /// <summary>
        /// Content from Square Brackets
        /// [ ... ]
        /// </summary>
        public static string SquareBracketsContent
        {
            get {
                if(squareBracketsContent == null) {
                    squareBracketsContent = bracketsContent('[', ']');
                }
                return squareBracketsContent;
            }
        }
        private static string squareBracketsContent;

        /// <summary>
        /// Content from Parentheses (Round Brackets)
        /// ( ... )
        /// </summary>
        public static string RoundBracketsContent
        {
            get {
                if(roundBracketsContent == null) {
                    roundBracketsContent = bracketsContent('(', ')');
                }
                return roundBracketsContent;
            }
        }
        private static string roundBracketsContent;

        /// <summary>
        /// Content from Curly Brackets
        /// { ... }
        /// </summary>
        public static string CurlyBracketsContent
        {
            get {
                if(curlyBracketsContent == null) {
                    curlyBracketsContent = bracketsContent('{', '}');
                }
                return curlyBracketsContent;
            }
        }
        private static string curlyBracketsContent;

        /// <summary>
        /// Boolean value from allowed syntax
        /// </summary>
        public static string BooleanContent
        {
            get { return @"\s*(true|false|True|False|TRUE|FALSE)\s*"; }
        }

        /// <summary>
        /// Integer value from allowed syntax
        /// </summary>
        public static string IntegerContent
        {
            get { return @"\s*(-?\d+)\s*"; }
        }

        /// <summary>
        /// Unsigned Integer value from allowed syntax
        /// </summary>
        public static string UnsignedIntegerContent
        {
            get { return @"\s*(\d+)\s*"; }
        }

        /// <summary>
        /// Signed floating-point number with single-precision from allowed syntax
        /// </summary>
        public static string FloatContent
        {
            get { return @"\s*(-?\d+(?:\.\d+)?)f\s*"; }
        }

        /// <summary>
        /// Unsigned floating-point number with single-precision from allowed syntax
        /// </summary>
        public static string UnsignedFloatContent
        {
            get { return @"\s*(\d+(?:\.\d+)?)f\s*"; }
        }

        /// <summary>
        /// Signed floating-point number with double-precision from allowed syntax
        /// </summary>
        public static string DoubleContent
        {
            get { return @"\s*(-?\d+(?:\.\d+)?)d?\s*"; }
        }

        /// <summary>
        /// Unsigned floating-point number with double-precision from allowed syntax
        /// </summary>
        public static string UnsignedDoubleContent
        {
            get { return @"\s*(\d+(?:\.\d+)?)d?\s*"; }
        }

        /// <summary>
        /// Mixed Enum or Const value from allowed syntax
        /// </summary>
        public static string EnumOrConstContent
        {
            get { return @"\s*([A-Za-z_0-9.]+)\s*"; }
        }

        /// <summary>
        /// Object data. Similar as array with mixed data.
        /// Format: { "p1", true, { 12, 'n', -4.5f }, 12d }
        /// </summary>
        public static string ObjectContent
        {
            get { return CurlyBracketsContent; }
        }

        /// <summary>
        /// Char symbol value from allowed syntax
        /// </summary>
        public static string CharContent
        {
            get { return @"\s*'(\S{1})'\s*"; }
        }

        /// <summary>
        /// Content for present symbol of quotes
        /// Escaping is a "\" for used symbol
        /// e.g.: \', \"
        /// </summary>
        /// <param name="symbol">' or "</param>
        /// <param name="withoutQuotes"></param>
        private static string quotesContent(char symbol, bool withoutQuotes = true)
        {
            return String.Format(@"
                                  \s*(?<!\\)
                                  ({1}
                                   {0}({2}
                                         (?:
                                            [^{0}\\]
                                          |
                                            \\\\
                                          |
                                            \\{0}?
                                         )*
                                      ){0}
                                  )\s*", 
                                  symbol,
                                  (withoutQuotes)? "?:" : String.Empty,
                                  (withoutQuotes)? String.Empty : "?:");
        }


        /// <summary>
        /// Content for present symbol of brackets
        /// 
        /// Note: A balancing group definition deletes the definition of a previously defined group, 
        ///       therefore allowed some intersection with name of the balancing group.. don't worry., be happy
        /// </summary>
        /// <param name="open">left symbol of bracket: [, {, ( etc.</param>
        /// <param name="close">right symbol of bracket: ], }, ) etc.</param>
        private static string bracketsContent(char open, char close)
        {
            return String.Format(@"\{0}
                                   (
                                     (?>
                                       [^\{0}\{1}]
                                       |
                                       \{0}(?<R>)
                                       |
                                       \{1}(?<-R>)
                                     )*
                                     (?(R)(?!))
                                   )
                                   \{1}", open, close);
        }
    }
}
