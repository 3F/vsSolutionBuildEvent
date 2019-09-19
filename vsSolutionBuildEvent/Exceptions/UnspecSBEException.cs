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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace net.r_eg.vsSBE.Exceptions
{
    [Serializable]
    public class UnspecSBEException: Exception
    {
        public readonly object value;

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(value), value);
        }

        public UnspecSBEException()
        {

        }

        public UnspecSBEException(string message) 
            : base(message)
        {

        }

        public UnspecSBEException(string message, object value)
            : base(message)
        {
            this.value = value;
        }

        public UnspecSBEException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }
}