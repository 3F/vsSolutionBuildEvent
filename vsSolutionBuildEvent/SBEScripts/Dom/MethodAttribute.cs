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
using System.Linq;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.SBEScripts.Dom
{
    /// <summary>
    /// Specification of the hierarchy of methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MethodAttribute: Attribute, IAttrDomLevelB
    {
        public struct TArguments
        {
            public CValueType type;
            public string name;
            public string description;

            public TArguments(CValueType type, string name = "", string description = "")
            {
                this.type           = type;
                this.name           = name;
                this.description    = description;
            }
        }

        /// <summary>
        /// Method name
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Description for current method
        /// </summary>
        public string Description
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Return value
        /// </summary>
        public CValueType Return
        {
            get;
            protected set;
        }
        
        /// <summary>
        /// Arguments of method
        /// </summary>
        public TArguments[] Arguments
        {
            get;
            protected set;
        }

        /// <summary>
        /// Name of the parent specification (property/method/etc.) if exist or null
        /// </summary>
        public string Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Actual/real method name of the parent specification if exist or null
        /// </summary>
        public string Method
        {
            get;
            protected set;
        }

        /// <param name="name">Method name</param>
        /// <param name="description">Description for current method</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string description, CValueType ret = CValueType.Void, params CValueType[] args)
        {
            Name        = name;
            Description = description;
            Return      = ret;
            Arguments   = args.Select(arg => new TArguments(arg)).ToArray();
        }

        /// <param name="name">Method name</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, CValueType ret = CValueType.Void, params CValueType[] args)
            : this(name, String.Empty, ret, args)
        {

        }

        /// <param name="name">Method name</param>
        /// <param name="description">Description for current method</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string description, string parent, string method, CValueType ret = CValueType.Void, params CValueType[] args)
            : this(name, description, ret, args)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Method name</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string parent, string method, CValueType ret = CValueType.Void, params CValueType[] args)
            : this(name, String.Empty, parent, method, ret, args)
        {

        }

        /// <param name="name">Method name</param>
        /// <param name="description">Description for current method</param>
        /// <param name="argsName">Arguments of method by name</param>
        /// <param name="argsDesc">Description for arguments</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string description, string[] argsName, string[] argsDesc, CValueType ret, params CValueType[] args)
            : this(name, description, ret, args)
        {
            if(argsName == null || argsDesc == null) {
                throw new InvalidArgumentException("null value is not valid for argsName/argsDesc");
            }

            if(args.Length != argsName.Length || args.Length != argsDesc.Length) {
                throw new MismatchException("CValueType[] is not equal by count with argsName/argsDesc :: {0}", name);
            }

            Arguments = args.Select((arg, i) => new TArguments(arg, argsName[i], argsDesc[i])).ToArray();
        }

        /// <param name="name">Method name</param>
        /// <param name="argsName">Arguments of method by name</param>
        /// <param name="argsDesc">Description for arguments</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string[] argsName, string[] argsDesc, CValueType ret, params CValueType[] args)
            : this(name, String.Empty, argsName, argsDesc, ret, args)
        {

        }

        /// <param name="name">Method name</param>
        /// <param name="description">Description for current method</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="argsName">Arguments of method by name</param>
        /// <param name="argsDesc">Description for arguments</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string description, string parent, string method, string[] argsName, string[] argsDesc, CValueType ret, params CValueType[] args)
            : this(name, description, argsName, argsDesc, ret, args)
        {
            Parent = parent;
            Method = method;
        }

        /// <param name="name">Method name</param>
        /// <param name="parent">Name of the parent specification (property/method/etc.) if exist or null</param>
        /// <param name="method">Actual/real method name of the parent specification if exist or null</param>
        /// <param name="argsName">Arguments of method by name</param>
        /// <param name="argsDesc">Description for arguments</param>
        /// <param name="ret">Return value</param>
        /// <param name="args">Arguments of method</param>
        public MethodAttribute(string name, string parent, string method, string[] argsName, string[] argsDesc, CValueType ret, params CValueType[] args)
            : this(name, String.Empty, parent, method, argsName, argsDesc, ret, args)
        {

        }
    }
}
