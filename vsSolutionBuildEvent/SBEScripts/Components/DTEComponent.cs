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
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    /// <summary>
    /// For work with DTE
    /// </summary>
    public class DTEComponent: Component, IComponent
    {
        /// <summary>
        /// Ability to work with data for current component
        /// </summary>
        public override string Condition
        {
            get { return "[DTE "; }
        }

        /// <summary>
        /// Work with DTE-Commands
        /// </summary>
        protected DTEOperation dteo;

        /// <param name="env">Used environment</param>
        public DTEComponent(IEnvironment env): base(env)
        {
            dteo = new DTEOperation((EnvDTE.DTE)env.DTE2, Events.SolutionEventType.General);
        }

        /// <summary>
        /// Handling with current type
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        public override string parse(string data)
        {
            Match m = Regex.Match(data, @"^\[DTE
                                              \s+
                                              (                  #1 - full ident
                                                ([A-Za-z_0-9]+)  #2 - subtype
                                                .*
                                              )
                                           \]$", 
                                           RegexOptions.IgnorePatternWhitespace);

            if(!m.Success) {
                throw new SyntaxIncorrectException("Failed DTEComponent - '{0}'", data);
            }

            switch(m.Groups[2].Value) {
                case "exec": {
                    Log.nlog.Debug("DTEComponent: use stExec");
                    return stExec(m.Groups[1].Value);
                }
            }
            throw new SubtypeNotFoundException("DTEComponent: not found subtype - '{0}'", m.Groups[2].Value);
        }

        /// <summary>
        /// DTE-command to execution
        /// e.g: #[DTE exec: command(arg)]
        /// </summary>
        /// <param name="data"></param>
        /// <returns>found command</returns>
        protected string stExec(string data)
        {
            Match m = Regex.Match(data, @"exec\s*:(.+)");
            if(!m.Success) {
                throw new OperandNotFoundException("Failed stExec - '{0}'", data);
            }
            string cmd = m.Groups[1].Value.Trim();
            Log.nlog.Debug("Found '{0}' to execution", cmd);

            dteo.exec(new string[] { cmd }, false);
            return String.Empty;
        }
    }
}
