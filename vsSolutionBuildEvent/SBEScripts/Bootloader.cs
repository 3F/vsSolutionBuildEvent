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
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.SBEScripts
{
    public class Bootloader: IBootloader
    {
        /// <summary>
        /// All enabled from the registered components
        /// </summary>
        public IEnumerable<IComponent> Components
        {
            get {
                foreach(KeyValuePair<string, IComponent> component in components) {
                    if(!component.Value.Enabled) {
                        continue;
                    }
                    yield return component.Value;
                }
            }
        }

        /// <summary>
        /// All registered components
        /// </summary>
        public IEnumerable<IComponent> ComponentsAll
        {
            get {
                foreach(KeyValuePair<string, IComponent> component in components) {
                    yield return component.Value;
                }
            }
        }

        /// <summary>
        /// Main storage
        /// </summary>
        protected ConcurrentDictionary<string, IComponent> components = new ConcurrentDictionary<string, IComponent>();

        /// <summary>
        /// Provides operations with environment
        /// </summary>
        public IEnvironment Env
        {
            get;
            protected set;
        }

        /// <summary>
        /// Current container of user-variables
        /// </summary>
        public IUserVariable UVariable
        {
            get;
            protected set;
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variable</param>
        public Bootloader(IEnvironment env, IUserVariable uvariable)
        {
            Env         = env;
            UVariable   = uvariable;
            init();
        }

        protected virtual void init()
        {
            register(new CommentComponent());
            register(new ConditionComponent(Env, UVariable));
            register(new UserVariableComponent(Env, UVariable));
            register(new OWPComponent());
            register(new DTEComponent(Env));
            register(new InternalComponent());
            register(new BuildComponent(Env));
            register(new FileComponent());
        }

        protected void register(IComponent c)
        {
            string ident = c.Condition;
            if(String.IsNullOrEmpty(ident) || components.ContainsKey(ident)) {
                throw new ComponentException("IComponent '{0}:{1}' is empty or is already registered", ident, c.ToString());
            }
            components[ident] = c;
        }
    }
}
