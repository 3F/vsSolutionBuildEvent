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
using System.Text.RegularExpressions;

namespace net.r_eg.SobaScript
{
    public static class RPattern
    {
        /// <summary>
        /// General container of SBE-Scripts
        /// </summary>
        public static Regex Container
        {
            get
            {
                /*
                     (
                       \#{1,2}
                     )
                     (?=
                       (
                         \[
                           (?>
                             [^\[\]]
                             |
                             (?2)
                           )*
                         \]
                       )
                     )            -> for .NET: v
                */
                if(container == null) {
                    container = new Regex(@"(?:\r?\n\x20*)?\r?\n?(
                                                \#{1,2}   #1 - # or ##
                                              )
                                              (           #2 - mixed data of SBE-Script
                                                \[
                                                  (?>
                                                    [^\[\]]
                                                    |
                                                    \[(?<R>)
                                                    |
                                                    \](?<-R>)
                                                  )*
                                                  (?(R)(?!))
                                                \]
                                              )\r?\n?",
                                              RegexOptions.IgnorePatternWhitespace | 
                                              RegexOptions.Compiled);
                }
                return container;
            }
        }
        private static Regex container;

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
            return string.Format(@"
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
                                  (withoutQuotes)? "?:" : string.Empty,
                                  (withoutQuotes)? string.Empty : "?:");
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
            return string.Format(@"\{0}
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
