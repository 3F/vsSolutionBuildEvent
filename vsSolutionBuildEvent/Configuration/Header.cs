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

namespace net.r_eg.vsSBE.Configuration
{
    public class Header
    {
        /// <summary>
        /// To identification of compatibility between versions
        /// </summary>
        public string Compatibility
        {
            get { return compatibility; }
            set { compatibility = value; }
        }
        /// <summary>
        /// this value used by default if current attr not found after deserialize
        /// :: v0.2.x/v0.1.x
        /// </summary>
        private string compatibility = "0.1";

        /// <summary>
        /// What application is needed for work with the .vssbe if extension not installed
        /// </summary>
        public string application = "http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/";
    }
}
