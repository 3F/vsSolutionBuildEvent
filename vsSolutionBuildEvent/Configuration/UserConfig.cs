/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using IUserData = net.r_eg.vsSBE.Configuration.User.IData;
using IUserDataSvc = net.r_eg.vsSBE.Configuration.User.IDataSvc;
using UserData = net.r_eg.vsSBE.Configuration.User.Data;

namespace net.r_eg.vsSBE.Configuration
{
    internal sealed class UserConfig: PackerAbstract<IUserData, UserData>, IConfig<IUserData>
    {
        public override string EntityExt { get; } = Settings.APP_CFG_USR;

        public override void save()
        {
            try
            {
                ((IUserDataSvc)Data).updateCommon(isLoad: false);
                Data.updateCache();
            }
            catch(Exception ex) when
            (ex is ArgumentException || ex is NullReferenceException)
            {
                Log.Debug($"Unable to process {nameof(Data)} to save configuration: {ex.Message}'");
            }

            base.save();
        }

        protected override bool loadFrom(string link) => loadFrom
        (
            link,
            (evt) => 
            {
                Log.Debug($"{GetType().Name} has been loaded from {link}");

                ((IUserDataSvc)Data).updateCommon(isLoad: true);
            }
        );
    }
}
