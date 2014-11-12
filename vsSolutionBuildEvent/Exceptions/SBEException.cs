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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.r_eg.vsSBE.Exceptions
{
    public class SBEException: NotSupportedException
    {
        public SBEException() {}
        public SBEException(string message): base(message) {}
        public SBEException(string message, Exception innerException): base(message, innerException) {}

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

    public class IncorrectSyntaxException: SBEException
    {
        public IncorrectSyntaxException(string message): base(message) {}
        public IncorrectSyntaxException(string message, params object[] args): base(message, args) {}
    }

    public class MismatchException: SBEException
    {
        public MismatchException(string message): base(message) {}
        public MismatchException(string message, params object[] args): base(message, args) {}
    }

    public class InvalidArgumentException: SBEException
    {
        public InvalidArgumentException(string message): base(message) {}
        public InvalidArgumentException(string message, params object[] args): base(message, args) {}
    }

    public class LimitException: SBEException
    {
        public LimitException(string message): base(message) {}
        public LimitException(string message, params object[] args): base(message, args) {}
    }

    public class NotFoundException: SBEException
    {
        public NotFoundException(string message): base(message) {}
        public NotFoundException(string message, params object[] args): base(message, args) {}
    }

    public class ComponentException: SBEException
    {
        public ComponentException(string message): base(message) {}
        public ComponentException(string message, Exception innerException): base(message, innerException) {}
        public ComponentException(string message, params object[] args): base(message, args) {}
        public ComponentException(string message, Exception innerException, params object[] args): base(message, innerException, args) {}
    }
}
