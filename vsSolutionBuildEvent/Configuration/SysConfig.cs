/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.MvsSln.Extensions;
using NLog;
using SysData = net.r_eg.vsSBE.Configuration.Sys.Data;

namespace net.r_eg.vsSBE.Configuration
{
    internal sealed class SysConfig: PackerAbstract<SysData, SysData>, IConfig<SysData>
    {
        public override string EntityExt { get; } = Settings.APP_CFG_USR;

        public SysConfig() => loadPath(Settings.GetCommonPath());

        protected override bool loadFrom(string link) => loadFrom
        (
            link,
            (evt) =>
            {
                // read more about the condition in Log.Fallback
                if(evt.DebugMode && !evt.LogIgnoreLevels.GetOrDefault(LogLevel.Debug.Name))
                {
                    Log.Debug($"{GetType().Name} has been loaded from {link}");
                }
            }
        );
    }
}
