/*
 * The MIT License (MIT)
 * 
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using net.r_eg.vsSBE.Provider.Services;

namespace net.r_eg.vsSBE.Provider
{
    public class Service: IService
    {
        /// <summary>
        /// Gets the DTE2 object instance from project file.
        /// </summary>
        /// <param name="file">Project file. Full path to the *.csproj, *.vcxproj, etc.</param>
        /// <returns></returns>
        public DTE2 dte2FromProject(string file)
        {
            throw new NotImplementedException("Not yet supported. Use the 'dte2FromSolution'");
        }

        /// <summary>
        /// Gets the DTE2 object instance from solution file.
        /// </summary>
        /// <param name="file">Project file. Full path to the *.sln</param>
        /// <returns></returns>
        public DTE2 dte2FromSolution(string file)
        {
            return new _DTE2() {
                Solution = new _Solution() {
                    FullName        = file,
                    Properties      = null,
                    Projects        = null,
                    SolutionBuild   = null // ActiveConfiguration, SolutionConfigurations, StartupProjects
                }
            };
        }
    }
}
