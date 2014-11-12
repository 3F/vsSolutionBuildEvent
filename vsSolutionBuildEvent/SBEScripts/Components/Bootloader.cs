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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    public static class Bootloader
    {
        /// <summary>
        /// All registered components
        /// </summary>
        public static IEnumerable<IComponent> Components
        {
            get {
                foreach(KeyValuePair<string, IComponent> component in components) {
                    yield return component.Value;
                }
            }
        }
        private static ConcurrentDictionary<string, IComponent> components = new ConcurrentDictionary<string, IComponent>();

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variable</param>
        public static void init(IEnvironment env, IUserVariable uvariable)
        {
            register(new CommentComponent());
            register(new ConditionComponent(env, uvariable));
            register(new UserVariableComponent(env, uvariable));
            register(new OWPComponent());
            register(new DTEComponent(env));
            register(new InternalComponent());
            register(new BuildComponent(env));
            register(new FileComponent());
        }

        private static void register(IComponent c)
        {
            string ident = c.Condition;
            if(String.IsNullOrEmpty(ident) || components.ContainsKey(ident)) {
                throw new ComponentException("IComponent '{0}:{1}' is empty or is already registered", ident, c.ToString());
            }
            components[ident] = c;
        }
    }
}
