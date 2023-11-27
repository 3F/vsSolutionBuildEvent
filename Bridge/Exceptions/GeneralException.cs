/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * This component (part of vsSolutionBuildEvent) is licensed under the MIT License (MIT).
 * See accompanying License.txt file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;

namespace net.r_eg.vsSBE.Bridge.Exceptions
{
    [Serializable]
    public class GeneralException: NotSupportedException
    {
        public GeneralException()
        {

        }

        public GeneralException(string message)
            : base(message)
        {

        }

        public GeneralException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public GeneralException(string message, params object[] args)
            : base(format(ref message, args))
        {

        }

        public GeneralException(string message, Exception innerException, params object[] args)
            : base(format(ref message, args), innerException)
        {

        }

        protected static string format(ref string message, params object[] args)
        {
            return String.Format(message, args);
        }
    }
}
