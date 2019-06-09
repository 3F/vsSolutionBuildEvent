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

using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.SNode
{
    public struct Level: ILevel
    {
        /// <summary>
        /// Type of level.
        /// </summary>
        public LevelType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Data of level.
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// Arguments of level if used.
        /// </summary>
        public Argument[] Args
        {
            get;
            set;
        }

        /// <summary>
        /// Type of data.
        /// </summary>
        public CValueType DataType
        {
            get;
            set;
        }

        /// <summary>
        /// Checks the argument types.
        /// </summary>
        /// <param name="types">The types that should be for this level.</param>
        /// <returns>True value if the Args contains arguments with specified types.</returns>
        public bool Is(params ArgumentType[] types)
        {
            if(Args == null || types == null || Args.Length != types.Length) {
                return false;
            }

            for(int i = 0; i < Args.Length; ++i)
            {
                if(Args[i].type != types[i]) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks the argument types.
        /// </summary>
        /// <param name="exception">Use string for exception instead of boolean result.</param>
        /// <param name="types">The types that should be for this level.</param>
        /// <returns>True value if the Args contains arguments with specified types.</returns>
        public bool Is(string exception, params ArgumentType[] types)
        {
            bool val = Is(types);

            if(exception != null && !val) {
                throw new InvalidArgumentException("Incorrect arguments to `{0}`", exception);
            }

            return val;
        }
    }
}
