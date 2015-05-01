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

namespace net.r_eg.vsSBE.MSBuild
{
    public static class RPattern
    {
        /// <summary>
        /// State of the container
        /// </summary>
        public enum ContainerType
        {
            /// <summary>
            /// $(..)
            /// </summary>
            Normal,
            /// <summary>
            /// $$(..)
            /// </summary>
            Escaped,
            /// <summary>
            /// $(..) / $$(..)
            /// </summary>
            Unclear
        }

        /// <summary>
        /// Escaped outer container, e.g.: -} $$(.. $(..) ...) {-
        /// </summary>
        public static string ContainerEscOuter
        {
            get {
                if(containerEscOuter == null) {
                    containerEscOuter = container(ContainerType.Escaped, false);
                }
                return containerEscOuter;
            }
        }
        private static string containerEscOuter;

        /// <summary>
        /// Deepest container, e.g.: $(.. -} $(..) {- ...)
        /// </summary>
        public static string ContainerIn
        {
            get {
                if(containerIn == null) {
                    containerIn = container(ContainerType.Normal, true);
                }
                return containerIn;
            }
        }
        private static string containerIn;

        /// <summary>
        /// Outer container, e.g.: -} $(.. $(..) ...) {-
        /// </summary>
        public static string ContainerOuter
        {
            get {
                if(containerOuter == null) {
                    containerOuter = container(ContainerType.Normal, false);
                }
                return containerOuter;
            }
        }
        private static string containerOuter;

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
        /// Content for present symbol of quotes
        /// Escaping is a \ for used symbol
        /// e.g.: \', \"
        /// However! '\' and "\" used 'as is' for compatibility with MSBuild
        /// </summary>
        /// <param name="symbol">' or "</param>
        /// <param name="withoutQuotes"></param>
        private static string quotesContent(char symbol, bool withoutQuotes = true)
        {
            return String.Format(@"
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
                                  (withoutQuotes)? "?:" : String.Empty,
                                  (withoutQuotes)? String.Empty : "?:");
        }

        /// <summary>
        /// Container variants
        /// </summary>
        /// <param name="type"></param>
        /// <param name="upward">all or only internal(deepest)</param>
        /// <returns></returns>
        private static string container(ContainerType type, bool upward)
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
            return String.Format(@"({0}{1}
                                     \${{{2}}}
                                   )
                                   (
                                     \(
                                       (?>
                                         {3}[^()]
                                         |
                                         \((?<R>)
                                         |
                                         \)(?<-R>)
                                       )*
                                       (?(R)(?!))
                                     \)
                                   )",
                                   (type == ContainerType.Unclear)? String.Empty : "?:",
                                   (type == ContainerType.Normal)? @"(?<!\$)" : String.Empty,
                                   (type == ContainerType.Unclear)? "1,2" : (type == ContainerType.Normal)? "1" : "2",
                                   (upward)? @"(?!\$\()" : String.Empty);
        }
    }
}
