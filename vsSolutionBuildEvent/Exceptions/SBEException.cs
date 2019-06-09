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

using System;

namespace net.r_eg.vsSBE.Exceptions
{
    [Serializable]
    public class SBEException: NotSupportedException
    {
        public SBEException()
        {

        }

        public SBEException(string message)
            : base(message)
        {

        }

        public SBEException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public SBEException(string message, params object[] args)
            : base(format(ref message, args))
        {

        }

        public SBEException(string message, Exception innerException, params object[] args)
            : base(format(ref message, args), innerException)
        {

        }

        protected static string format(ref string message, params object[] args)
        {
            return String.Format(message, args);
        }
    }
}
