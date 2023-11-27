/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

namespace net.r_eg.vsSBE.Events.Types
{
    /// <summary>
    /// Struct of EnvDTE Command.
    /// </summary>
    public struct CommandDte
    {
        /// <summary>
        /// Scope by GUID.
        /// </summary>
        public string Guid
        {
            get;
            set;
        }

        /// <summary>
        /// For work with command ID.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Input parameters.
        /// </summary>
        public object CustomIn
        {
            get;
            set;
        }

        /// <summary>
        /// Output parameters.
        /// </summary>
        public object CustomOut
        {
            get;
            set;
        }
    }
}
