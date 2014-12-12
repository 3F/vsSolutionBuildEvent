/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Text;
using System.Text.RegularExpressions;

namespace net.r_eg.vsSBE.SBEScripts
{
    public static class RPattern
    {
        /// <summary>
        /// Captures content from double quotes
        /// </summary>
        public static string DoubleQuotesContent
        {
            get {
                return quotesContent('"');
            }
        }

        /// <summary>
        /// Captures content from single quotes
        /// </summary>
        public static string SingleQuotesContent
        {
            get {
                return quotesContent('\'');
            }
        }

        /// <summary>
        /// Captures content from Square Brackets
        /// [ ... ]
        /// </summary>
        public static string SquareBracketsContent
        {
            get { return bracketsContent('[', ']'); }
        }

        /// <summary>
        /// Captures content from Parentheses (Round Brackets)
        /// ( ... )
        /// </summary>
        public static string RoundBracketsContent
        {
            get { return bracketsContent('(', ')'); }
        }

        /// <summary>
        /// Captures content from Curly Brackets
        /// { ... }
        /// </summary>
        public static string CurlyBracketsContent
        {
            get { return bracketsContent('{', '}'); }
        }

        /// <summary>
        /// Captures boolean value from allowed syntax
        /// </summary>
        public static string BooleanContent
        {
            get { return @"\s*(true|false|True|False|TRUE|FALSE)\s*"; }
        }

        /// <summary>
        /// Captures content for present symbol of quotes
        /// Escaping is a "\" for used symbol
        /// e.g.: \', \"
        /// </summary>
        /// <param name="symbol">' or "</param>
        private static string quotesContent(char symbol)
        {
            return String.Format(@"
                                  \s*(?<!\\){0}
                                  (
                                     (?:
                                        [^{0}\\]
                                      |
                                        \\{0}?
                                     )*
                                  )
                                  {0}\s*", symbol);
        }


        /// <summary>
        /// Captures content for present symbol of brackets
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
