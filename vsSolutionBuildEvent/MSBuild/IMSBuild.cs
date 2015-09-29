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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.MSBuild
{
    [Guid("958B9A32-BE6F-4B74-A98A-AC99099A63A5")]
    public interface IMSBuild
    {
        /// <summary>
        /// Used environment.
        /// </summary>
        IEnvironment Env { get; }

        /// <summary>
        /// Container of user-variables.
        /// </summary>
        IUserVariable UVariable { get; }

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
        /// Gets all properties from specific project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        List<PropertyItem> listProperties(string projectName = null);

        /// <summary>
        /// Evaluate data with MSBuild engine.
        /// Property Functions Syntax: 
        ///   http://msdn.microsoft.com/en-us/library/vstudio/dd633440%28v=vs.120%29.aspx
        /// </summary>
        /// <param name="unevaluated">raw unevaluated data</param>
        /// <param name="projectName">specific project or null value for default</param>
        /// <returns>Evaluated value</returns>
        string evaluate(string unevaluated, string projectName = null);

        /// <summary>
        /// Entry point to evaluating MSBuild data.
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>All evaluated values for data</returns>
        string parse(string data);
    }
}
