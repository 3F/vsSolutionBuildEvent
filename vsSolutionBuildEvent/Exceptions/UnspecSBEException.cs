/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
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