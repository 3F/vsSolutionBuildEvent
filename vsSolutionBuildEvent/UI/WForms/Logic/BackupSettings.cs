/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.vsSBE.Configuration;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.Extensions;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE.UI.WForms.Logic
{
    internal sealed class BackupSettings
    {
        private ISolutionEvents slnConfig;

        private IUserData userConfig;

        public void update(IConfig<IUserData> usr, IConfig<ISolutionEvents> sln = null)
        {
            slnConfig = sln?.Data?.CloneBySerializationWithType<ISolutionEvents, SolutionEvents>();
            userConfig = usr?.Data?.CloneBySerializationWithType<IData, Data>();
        }

        public void restore(IConfig<IUserData> usr, IConfig<ISolutionEvents> sln)
        {
            usr?.load(userConfig?.CloneBySerializationWithType<IData, Data>() ?? new Data());
            sln?.load(slnConfig?.CloneBySerializationWithType<ISolutionEvents, SolutionEvents>() ?? new SolutionEvents());
        }
    }
}