/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.SBEScripts
{
    public interface IBootloader
    {
        /// <summary>
        /// All enabled from the registered components
        /// </summary>
        IEnumerable<IComponent> Components { get; }

        /// <summary>
        /// All registered components
        /// </summary>
        IEnumerable<IComponent> Registered { get; }

        /// <summary>
        /// Operations with environment
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Container for user-variables
        /// </summary>
        IUserVariable UVariable { get; }

        /// <summary>
        /// Getting component for selected type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IComponent getComponentByType(Type type);

        /// <summary>
        /// To register new component.
        /// </summary>
        /// <param name="c">component</param>
        void register(IComponent c);

        /// <summary>
        /// To register the all default components.
        /// </summary>
        void register();

        /// <summary>
        /// To unregister specific component.
        /// </summary>
        /// <param name="c">component</param>
        void unregister(IComponent c);

        /// <summary>
        /// To unregister all available components.
        /// </summary>
        void unregister();

        /// <summary>
        /// Activation of components with ISolutionEvents.
        /// </summary>
        /// <param name="data"></param>
        void updateActivation(ISolutionEvents data);
    }
}
