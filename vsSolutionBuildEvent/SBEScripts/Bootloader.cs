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
using System.Collections.Concurrent;
using System.Collections.Generic;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.Scripts;

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
                foreach(KeyValuePair<Type, IComponent> component in components) {
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
                foreach(KeyValuePair<Type, IComponent> component in components) {
                    yield return component.Value;
                }
            }
        }

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

        /// <summary>
        /// Main storage
        /// </summary>
        protected ConcurrentDictionary<Type, IComponent> components = new ConcurrentDictionary<Type, IComponent>();

        /// <summary>
        /// Gets component for selected type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Instance of the IComponent or null value if the type not registered in collection</returns>
        public IComponent getComponentByType(Type type)
        {
            if(components.ContainsKey(type)) {
                return components[type];
            }
            return null;
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
            register(new OWPComponent(Env));
            register(new DTEComponent(Env));
            register(new InternalComponent());
            register(new BuildComponent(Env));
            register(new FileComponent());
        }

        protected void register(IComponent c)
        {
            Type ident = c.GetType();
            if(String.IsNullOrEmpty(c.Condition) || components.ContainsKey(ident)) {
                throw new ComponentException("IComponent '{0}:{1}' is empty or is already registered", ident, c.ToString());
            }
            components[ident] = c;
        }
    }
}
