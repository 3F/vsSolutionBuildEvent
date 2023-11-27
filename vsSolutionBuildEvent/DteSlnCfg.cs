/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using EnvDTE;
using EnvDTE80;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.vsSBE
{
    internal sealed class DteSlnCfg: SolutionConfiguration2
    {
        public string Name { get; }

        public string PlatformName { get; }

        internal DteSlnCfg(string name, string platform)
        {
            Name = name;
            PlatformName = platform;
        }

        internal DteSlnCfg(IConfPlatform def)
            : this(def.Configuration, def.Platform)
        {

        }

        #region NotSupported

        public DTE DTE => throw new NotSupportedException();
        public SolutionConfigurations Collection => throw new NotSupportedException();
        public SolutionContexts SolutionContexts => throw new NotSupportedException();
        public void Delete() => throw new NotSupportedException();
        public void Activate() => throw new NotSupportedException();

        #endregion
    }
}
