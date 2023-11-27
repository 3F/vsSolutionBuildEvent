/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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
