/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
using System.Linq;
using System.Reflection;

namespace net.r_eg.vsSBE.Clients
{
    /// <summary>
    /// Helper for getting instance
    /// </summary>
    /// <typeparam name="T">Type of the instance</typeparam>
    public struct Instance<T>
    {
        public static T from(Assembly asm, params object[] args)
        {
            foreach(Type type in asm.GetTypes()) {
                if(type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T))) {
                    return (T)Activator.CreateInstance(type, args);
                }
            }
            throw new DllNotFoundException(String.Format("Incorrect Assembly('{0}') for type '{1}'", asm.FullName, typeof(T)));
        }
    }
}
