﻿/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2013-2019  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) E-MSBuild contributors: https://github.com/3F/E-MSBuild/graphs/contributors
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

namespace net.r_eg.EvMSBuild
{
    public static class Pattern
    {
        private static readonly Lazy<Regex> _containerInCompiled = new Lazy<Regex>(() => new Regex
        (
            ContainerIn,
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
        ));

        private static readonly Lazy<Regex> _containerInNamedCompiled = new Lazy<Regex>(() => new Regex
        (
            GetContainerIn(@"\s*(?'name'[^$\s).:]+)"), // $( name ... 
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
        ));

        private static readonly Lazy<Regex> _isNumber = new Lazy<Regex>(() => new Regex
        (
            @"^\d+([.,]\d+)?$", RegexOptions.Compiled
        ));

        private static readonly Lazy<Regex> _pItem = new Lazy<Regex>(() => new Regex
        (
            string.Format
            (@"^\(  
                    (?:
                        \s*
                        (?'tsign'-|\+)?
                        ([A-Za-z_0-9]+) # 1 -> variable name (optional)
                        \s*
                        (?'vsign'-|\+)?
                        =\s*
                        (?: {0}         # 2 -> string data inside double quotes
                            |
                            {1}         # 3 -> string data inside single quotes
                        )? 
                    )?
                    (?:
                        (.+)           # 4 -> unevaluated data
                        (?<!:):
                        ([^:)]+)       # 5 -> scope for variable (if #1 is true) or for unevaluated data
                    |                # or:
                        (.+)           # 6 -> unevaluated data
                    )?
                \)$",
                DoubleQuotesContent,
                SingleQuotesContent
            ),
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
        ));

        private static readonly Lazy<string> _containerEscOuter = new Lazy<string>(() => 
            Container(ContainerType.Escaped, false)
        );

        private static readonly Lazy<string> _containerIn = new Lazy<string>(() =>
            Container(ContainerType.Normal, true)
        );

        private static readonly Lazy<string> _containerOuter = new Lazy<string>(() =>
            Container(ContainerType.Normal, false)
        );

        private static readonly Lazy<string> _doubleQuotesContentFull = new Lazy<string>(() =>
            QuotesContent('"', false)
        );

        private static readonly Lazy<string> _singleQuotesContentFull = new Lazy<string>(() =>
            QuotesContent('\'', false)
        );

        private static readonly Lazy<string> _doubleQuotesContent = new Lazy<string>(() =>
            QuotesContent('"', true)
        );

        private static readonly Lazy<string> _singleQuotesContent = new Lazy<string>(() =>
            QuotesContent('\'', true)
        );

        /// <summary>
        /// Checks the numeric format.
        /// </summary>
        public static Regex IsNumber => _isNumber.Value;

        /// <summary>
        /// Escaped outer container, eg.: -} $$(.. $(..) ...) {-
        /// </summary>
        public static string ContainerEscOuter => _containerEscOuter.Value;

        /// <summary>
        /// Outer container, eg.: -} $(.. $(..) ...) {-
        /// </summary>
        public static string ContainerOuter => _containerOuter.Value;

        /// <summary>
        /// Content from double quotes according to E-MSBuild.
        /// </summary>
        public static string DoubleQuotesContent => _doubleQuotesContent.Value;

        /// <summary>
        /// Content from single quotes according to E-MSBuild.
        /// </summary>
        public static string SingleQuotesContent => _singleQuotesContent.Value;

        /// <summary>
        /// Double quotes with content according to E-MSBuild.
        /// </summary>
        public static string DoubleQuotesContentFull => _doubleQuotesContentFull.Value;

        /// <summary>
        /// Single quotes with content according to E-MSBuild.
        /// </summary>
        public static string SingleQuotesContentFull => _singleQuotesContentFull.Value;

        /// <summary>
        /// Compiled ContainerIn.
        /// </summary>
        internal static Regex ContainerInCompiled => _containerInCompiled.Value;

        /// <summary>
        /// Compiled ContainerIn with naming the left definitions.
        /// </summary>
        internal static Regex ContainerInNamedCompiled => _containerInNamedCompiled.Value;

        /// <summary>
        /// An expression of property item.
        /// </summary>
        internal static Regex PItem => _pItem.Value;

        /// <summary>
        /// Deepest container, eg.: $(.. -} $(..) {- ...)
        /// </summary>
        internal static string ContainerIn => _containerIn.Value;

        /// <summary>
        /// Deepest container, eg.: $(.. -} $(..) {- ...)
        /// </summary>
        /// <param name="left">Condition of left bracket - `$(left`</param>
        internal static string GetContainerIn(string left)
        {
            return Container(ContainerType.Normal, true, left);
        }

        /// <summary>
        /// Content for presented symbol of quotes.
        /// Escaping is a \ for used symbol.
        /// eg.: \', \"
        /// However! '\' and "\" used 'as is' for compatibility with MSBuild
        /// </summary>
        /// <param name="symbol">' or "</param>
        /// <param name="withoutQuotes"></param>
        private static string QuotesContent(char symbol, bool withoutQuotes = true) => string.Format
        (@"
            (?<!\\)
            ({1}
            {0}({2}
                    \\  # for compatibility with MSBuild -_-
                |
                    (?:
                    [^{0}\\]
                    |
                    \\\\
                    |
                    \\{0}?
                    )*
                ){0}
            )", 
            symbol,
            withoutQuotes ? "?:" : string.Empty,
            withoutQuotes ? string.Empty : "?:"
        );

        /// <summary>
        /// Configurable Container.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="upward">all or only internal(deepest)</param>
        /// <param name="left">Condition of left bracket - `$(left`</param>
        /// <returns></returns>
        private static string Container(ContainerType type, bool upward, string left = "")
        {
            /* PCRE:
                 (
                   \${1,2}
                 )
                 (?=
                   (
                     \(
                       (?>
                         [^()]
                         |
                         (?2)
                       )*
                     \)
                   )
                 )           ~-> for .NET: v
             */
            return string.Format
            (@"({0}{1}
                    \${{{2}}}
                )
                (
                    \({3}
                    (?>
                        {4}[^()]
                        |
                        \((?<R>)
                        |
                        \)(?<-R>)
                    )*
                    (?(R)(?!))
                    \)
                )",
                (type == ContainerType.Unclear)? string.Empty : "?:",
                (type == ContainerType.Normal)? @"(?<!\$)" : string.Empty,
                (type == ContainerType.Unclear)? "1,2" : (type == ContainerType.Normal)? "1" : "2",
                left,
                upward ? @"(?!\$\()" : string.Empty
            );
        }
    }
}
