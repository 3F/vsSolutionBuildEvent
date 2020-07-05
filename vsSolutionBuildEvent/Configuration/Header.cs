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

namespace net.r_eg.vsSBE.Configuration
{
    public class Header
    {
        public string[] _ = new string[]
        {
            " This requires vsSolutionBuildEvent engine.", 
            " Free plugin for Visual Studio or MSBuild Tools:",
            "  * https://github.com/3F/vsSolutionBuildEvent"
        };

        /// <summary>
        /// Compatibility of configurations.
        /// </summary>
        public string Compatibility
        {
            get { return compatibility; }
            set { compatibility = value; }
        }
        /// <summary>
        /// The version below used by default if current attr is not found.
        /// </summary>
        private string compatibility = "0.1";

    }
}
