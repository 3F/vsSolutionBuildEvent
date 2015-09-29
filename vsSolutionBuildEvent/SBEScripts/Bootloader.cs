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
        public IEnumerable<IComponent> Registered
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

        /// <summary>
        /// Register new component.
        /// </summary>
        /// <param name="c">component</param>
        public void register(IComponent c)
        {
            if(String.IsNullOrEmpty(c.Condition)) {
                throw new ComponentException("Condition for '{0}' is null or empty.", c.ToString());
            }

            Type ident = c.GetType();
            if(components.ContainsKey(ident)) {
                throw new ComponentException("IComponent '{0}:{1}' is already registered.", ident, c.ToString());
            }
            components[ident] = c;
        }

        /// <summary>
        /// Register the all default components.
        /// </summary>
        public virtual void register()
        {
            register(new CommentComponent());
            register(new ConditionComponent(this));
            register(new UserVariableComponent(this));
            register(new OWPComponent(Env));
            register(new DTEComponent(Env));
            register(new InternalComponent());
            register(new BuildComponent(Env));
            register(new FunctionComponent());
            register(new FileComponent());
        }

        /// <summary>
        /// Unregister specific component.
        /// </summary>
        /// <param name="c">component</param>
        public void unregister(IComponent c)
        {
            IComponent v;
            if(!components.TryRemove(c.GetType(), out v)) {
                throw new SBEException("Cannot remove component '{0}'", c.ToString());
            }
        }

        /// <summary>
        /// Unregister all available components.
        /// </summary>
        public void unregister()
        {
            components.Clear();
        }

        /// <param name="env">Used environment</param>
        /// <param name="uvariable">Used instance of user-variable</param>
        public Bootloader(IEnvironment env, IUserVariable uvariable)
        {
            Env         = env;
            UVariable   = uvariable;
        }
    }
}
