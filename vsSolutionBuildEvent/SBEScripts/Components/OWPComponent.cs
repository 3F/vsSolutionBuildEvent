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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// For work with OWP
    /// </summary>
    [Component("OWP", "For work with OWP (Output Window Pane)")]
    public class OWPComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "OWP "; }
        }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[OWP
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$", 
                                           RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed OWPComponent - '{0}'", data);
            }

            switch(m.Groups[2].Value) {
                case "out": {
                    Log.nlog.Debug("OWPComponent: use stOut");
                    return stOut(m.Groups[1].Value);
                }
            }
            throw new SubtypeNotFoundException("OWPComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// Getting any data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [
            Method
            (
                "out", 
                "For getting mixed data from the OWP. Returns the partial raw from all build log", 
                new string[] { "name" }, 
                new string[] { "Name of item.\n Note: The 'Build' item used by default." }, 
                CValueType.String, 
                CValueType.String
            )
        ]
        [Property("out", "Alias for: out(\"Build\").", CValueType.String)]
        [Property("All", "Alias for: out", "out", "stOut", CValueType.String)]
        [Property("Warnings", "Partial raw with warning/s:", "out", "stOut", CValueType.String)]
        [Property("Raw", "Alias for: Warnings", "Warnings", "stOut", CValueType.String)]
        [Property("Count", "Count of warnings", "Warnings", "stOut", CValueType.Integer)]
        [Property("Codes", "List of warnings as C4702,4505 ...", "Warnings", "stOut", CValueType.List)]
        [Property("Errors", "Partial raw with error/s:", "out", "stOut", CValueType.String)]
        [Property("Raw", "Alias for: Errors", "Errors", "stOut", CValueType.String)]
        [Property("Count", "Count of Errors", "Errors", "stOut", CValueType.Integer)]
        [Property("Codes", "List of Errors as C4702,4505 ...", "Errors", "stOut", CValueType.List)]
        protected string stOut(string data)
        {
            Match m = Regex.Match(data, 
                                    String.Format(@"out
                                                   (?:
                                                     \s*
                                                     \({0}\)  #1 - arguments (optional)
                                                   )?
                                                   \s*(.*)    #2 - property",
                                                   RPattern.DoubleQuotesContent
                                                 ), RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new OperationNotFoundException("Failed stOut - '{0}'", data);
            }
            
            if(m.Groups[1].Success)
            {
                string item = StringHandler.normalize(m.Groups[1].Value);
                Log.nlog.Debug("stOut: item = '{0}'", item);

                if(item == "Build") {
                    //used by default - #[OWP out("Build")] / #[OWP out]
                }
                else {
                    throw new NotSupportedOperationException("item - '{0}' not yet supported", item);
                }
            }
            string property = m.Groups[2].Value.Trim();
            Log.nlog.Debug("stOut: property = '{0}'", property);

            string raw = StringHandler.escapeQuotes(OWP.Items._.Build.Raw);

            // #[OWP out.All] / #[OWP out]
            if(property == ".All" || property == String.Empty) {
                return raw;
            }

            // #[OWP out.Warnings.Raw] / #[OWP out.Warnings]
            if(property == ".Warnings" || property == ".Warnings.Raw") {
                return (OWP.Items._.Build.IsWarnings)? raw : String.Empty;
            }

            // #[OWP out.Warnings.Count]
            if(property == ".Warnings.Count") {
                return Value.from(OWP.Items._.Build.WarningsCount);
            }

            // #[OWP out.Warnings.Codes]
            if(property == ".Warnings.Codes") {
                return Value.from(OWP.Items._.Build.Warnings);
            }

            // #[OWP out.Errors.Raw] / #[OWP out.Errors]
            if(property == ".Errors" || property == ".Errors.Raw") {
                return (OWP.Items._.Build.IsErrors)? raw : String.Empty;
            }

            // #[OWP out.Errors.Count]
            if(property == ".Errors.Count") {
                return Value.from(OWP.Items._.Build.ErrorsCount);
            }

            // #[OWP out.Errors.Codes]
            if(property == ".Errors.Codes") {
                return Value.from(OWP.Items._.Build.Errors);
            }

            throw new NotSupportedOperationException("property - '{0}' not yet supported", property);
        }
    }
}
