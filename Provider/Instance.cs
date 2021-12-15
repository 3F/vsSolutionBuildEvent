/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2021  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using System.Linq;
using System.Reflection;

namespace net.r_eg.vsSBE.Provider
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
