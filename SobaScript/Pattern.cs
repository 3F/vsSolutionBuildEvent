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
    public static class Pattern
    {
        private static readonly Lazy<Regex> _container = new Lazy<Regex>(() => new Regex
        (
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
            @"(?:\r?\n\x20*)?\r?\n?(
                \#{1,2}     #1 - # or ##
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
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
        ));

        private static readonly Lazy<string> _doubleQuotesContent = new Lazy<string>(() =>
            QuotesContent('"', true)
        );

        private static readonly Lazy<string> _singleQuotesContent = new Lazy<string>(() =>
            QuotesContent('\'', true)
        );

        private static readonly Lazy<string> _doubleQuotesContentFull = new Lazy<string>(() =>
            QuotesContent('"', false)
        );

        private static readonly Lazy<string> _singleQuotesContentFull = new Lazy<string>(() =>
           QuotesContent('\'', false)
        );

        private static readonly Lazy<string> _squareBracketsContent = new Lazy<string>(() =>
           BracketsContent('[', ']')
        );

        private static readonly Lazy<string> _roundBracketsContent = new Lazy<string>(() =>
            BracketsContent('(', ')')
        );

        private static readonly Lazy<string> _curlyBracketsContent = new Lazy<string>(() =>
            BracketsContent('{', '}')
        );

        /// <summary>
        /// Content from double quotes according to SobaScript.
        /// </summary>
        public static string DoubleQuotesContent => _doubleQuotesContent.Value;

        /// <summary>
        /// Content from single quotes according to SobaScript.
        /// </summary>
        public static string SingleQuotesContent => _singleQuotesContent.Value;

        /// <summary>
        /// Double quotes with content according to SobaScript.
        /// </summary>
        public static string DoubleQuotesContentFull => _doubleQuotesContentFull.Value;

        /// <summary>
        /// Single quotes with content according to SobaScript.
        /// </summary>
        public static string SingleQuotesContentFull => _singleQuotesContentFull.Value;

        /// <summary>
        /// Content from Square Brackets according to SobaScript.
        /// [ ... ]
        /// </summary>
        public static string SquareBracketsContent => _squareBracketsContent.Value;

        /// <summary>
        /// Content from Parentheses (Round Brackets) according to SobaScript.
        /// ( ... )
        /// </summary>
        public static string RoundBracketsContent => _roundBracketsContent.Value;

        /// <summary>
        /// Content from Curly Brackets according to SobaScript.
        /// { ... }
        /// </summary>
        public static string CurlyBracketsContent => _curlyBracketsContent.Value;

        /// <summary>
        /// Boolean value from allowed syntax
        /// </summary>
        internal static string BooleanContent => @"\s*(true|false|True|False|TRUE|FALSE)\s*";

        /// <summary>
        /// Integer value from allowed syntax
        /// </summary>
        internal static string IntegerContent => @"\s*(-?\d+)\s*";

        /// <summary>
        /// Unsigned Integer value from allowed syntax
        /// </summary>
        internal static string UnsignedIntegerContent => @"\s*(\d+)\s*";

        /// <summary>
        /// Signed floating-point number with single-precision from allowed syntax
        /// </summary>
        internal static string FloatContent => @"\s*(-?\d+(?:\.\d+)?)f\s*";

        /// <summary>
        /// Unsigned floating-point number with single-precision from allowed syntax
        /// </summary>
        internal static string UnsignedFloatContent => @"\s*(\d+(?:\.\d+)?)f\s*";

        /// <summary>
        /// Signed floating-point number with double-precision from allowed syntax
        /// </summary>
        internal static string DoubleContent => @"\s*(-?\d+(?:\.\d+)?)d?\s*";

        /// <summary>
        /// Unsigned floating-point number with double-precision from allowed syntax
        /// </summary>
        internal static string UnsignedDoubleContent => @"\s*(\d+(?:\.\d+)?)d?\s*";

        /// <summary>
        /// Mixed Enum or Const value from allowed syntax
        /// </summary>
        internal static string EnumOrConstContent => @"\s*([A-Za-z_0-9.]+)\s*";

        /// <summary>
        /// Object data. Similar as array with mixed data.
        /// Format: { "p1", true, { 12, 'n', -4.5f }, 12d }
        /// </summary>
        internal static string ObjectContent => CurlyBracketsContent;

        /// <summary>
        /// Char symbol value from allowed syntax
        /// </summary>
        internal static string CharContent => @"\s*'(\S{1})'\s*";

        /// <summary>
        /// Container according to SobaScript.
        /// </summary>
        internal static Regex Container => _container.Value;

        /// <summary>
        /// Content for present symbol of quotes
        /// Escaping is a "\" for used symbol
        /// e.g.: \', \"
        /// </summary>
        /// <param name="symbol">' or "</param>
        /// <param name="withoutQuotes"></param>
        private static string QuotesContent(char symbol, bool withoutQuotes = true) => string.Format
        (@"
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
            withoutQuotes ? "?:" : string.Empty,
            withoutQuotes ? string.Empty : "?:"
        );

        /// <summary>
        /// Contents for the specified brackets character.
        /// 
        /// Note: A balancing group definition deletes the definition of a previously defined group, 
        ///       therefore allowed some intersection with name of the balancing group.. don't worry., be happy
        /// </summary>
        /// <param name="open">left symbol of bracket: [, {, ( etc.</param>
        /// <param name="close">right symbol of bracket: ], }, ) etc.</param>
        private static string BracketsContent(char open, char close) => string.Format
        (@"
           \{0}
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
            \{1}", 
            open, 
            close
        );
    }
}
