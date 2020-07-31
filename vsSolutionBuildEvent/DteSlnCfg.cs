/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
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
