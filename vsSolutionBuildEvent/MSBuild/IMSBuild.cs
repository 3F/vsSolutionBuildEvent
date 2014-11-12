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

namespace net.r_eg.vsSBE.MSBuild
{
    public interface IMSBuild
    {
        /// <summary>
        /// MSBuild Property from default Project
        /// </summary>
        /// <param name="name">key property</param>
        /// <returns>evaluated value</returns>
        string getProperty(string name);

        /// <summary>
        /// MSBuild Property from specific project
        /// </summary>
        /// <param name="name">key property</param>
        /// <param name="projectName">project name</param>
        /// <returns>evaluated value</returns>
        string getProperty(string name, string projectName);

        /// <summary>
        /// Evaluate data with the MSBuild engine.
        /// Property Function Syntax: 
        ///   http://msdn.microsoft.com/en-us/library/vstudio/dd633440%28v=vs.120%29.aspx
        /// </summary>
        /// <param name="unevaluated">raw unevaluated data</param>
        /// <param name="projectName">Specific project for evaluating</param>
        /// <returns>Evaluated end value</returns>
        string evaluate(string unevaluated, string projectName);

        /// <summary>
        /// Handler of variables/properties MSBuild
        /// </summary>
        /// <param name="data">string with $(ident) data</param>
        /// <returns>All evaluated values for data</returns>
        string parse(string data);
    }
}
