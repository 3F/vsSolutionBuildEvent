/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.CodeDom.Compiler;

namespace net.r_eg.vsSBE.Exceptions
{
    [Serializable]
    public class CompilerException: UnspecSBEException
    {
        public CompilerException(CompilerError error)
            : base(error.ToString(), error)
        {

        }

        public CompilerException(string message)
            : base(message)
        {

        }
    }
}
