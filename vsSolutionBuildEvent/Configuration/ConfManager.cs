/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using SysData = net.r_eg.vsSBE.Configuration.Sys.Data;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;

namespace net.r_eg.vsSBE.Configuration
{
    public sealed class ConfManager
    {
        public IConfig<ISolutionEvents> Sln { get; } = new SlnConfig();

        public IConfig<IUserData> Usr { get; } = new UserConfig();

        public IConfig<SysData> Sys { get; } = new SysConfig();

        public bool IsLoadedConfigs => IsLoadedSln && IsLoadedUsr && IsLoadedSys;

        public bool IsLoadedSln => Sln.Data != null;

        public bool IsLoadedUsr => Usr.Data != null;

        public bool IsLoadedSys => Sys.Data != null;

        public void save()
        {
            Sys.save();
            Usr.save();
            Sln.save();
        }

        public override string ToString()
            => $"Sln: {IsLoadedSln}, Usr: {IsLoadedUsr}, Sys: {IsLoadedSys}";
    }
}
