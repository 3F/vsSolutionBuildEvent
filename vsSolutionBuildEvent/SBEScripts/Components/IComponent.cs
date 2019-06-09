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

namespace net.r_eg.vsSBE.SBEScripts.Components
{
    public interface IComponent
    {
        /// <summary>
        /// Ability to work with data for component
        /// </summary>
        string Condition { get; }

        /// <summary>
        /// Using regex engine for property - condition
        /// </summary>
        bool CRegex { get; }

        /// <summary>
        /// Activation status
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Flag of post-processing with MSBuild core.
        /// In general, some components can require immediate processing with evaluation before passing control to next level.
        /// This flag allows processing if needed.
        /// </summary>
        bool PostProcessingMSBuild { get; set; }

        /// <summary>
        /// Should be located before deepening or not
        /// </summary>
        bool BeforeDeepen { get; }

        /// <summary>
        /// To force post-analysis.
        /// </summary>
        bool PostParse { get; }

        /// <summary>
        /// Handler for current data
        /// </summary>
        /// <param name="data">mixed data</param>
        /// <returns>prepared and evaluated data</returns>
        string parse(string data);
    }
}
