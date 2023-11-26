/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using net.r_eg.MvsSln.Extensions;
using SysVersion = System.Version;

namespace net.r_eg.vsSBE.Configuration
{
    internal sealed class SlnConfig: PackerAbstract<ISolutionEvents, SolutionEvents>, IConfig<ISolutionEvents>
    {
        /// <summary>
        /// Current version for <see cref="SlnConfig"/> configuration.
        /// </summary>
        /// <remarks>Version of app managed separately via vsixmanifest etc.</remarks>
        internal static readonly SysVersion ConfigVersion = new(0, 12, 4);

        public override bool load(string link)
            => throw new NotSupportedException();

        protected override bool loadFrom(string link) => loadFrom
        (
            link,
            (evt) => 
            {
                Log.Info($"Loaded {evt.Header.Compatibility} {EntityName}  {Settings.WPath}");
                WarnAboutJsonConfig(SysVersion.Parse(evt.Header.Compatibility));

                // now we have the latest version, anyway
                Data.Header.Compatibility = ConfigVersion.ToString();
            },
            importance: true,
            (jsonEx) =>
            {
                WarnAboutXmlConfig();
                Log.Error($"Incorrect configuration data: {jsonEx.Message}");
            }
        );

        private static void WarnAboutJsonConfig(SysVersion cfgVer)
        {
            if(cfgVer > ConfigVersion)
            {
                Log.Warn
                (
                    $"Configuration file v{cfgVer} is higher than supported v{ConfigVersion}. Update app for best known behavior."
                );
            }
        }

        private static void WarnAboutXmlConfig()
        {
            const string _MSG = "Please use any version from `{0}` for auto-upgrading configuration `{1}`.";

            Log.Warn(_MSG, "0.4.x - 0.8.x", "<= v0.3");
            Log.Warn(_MSG, "0.9.x - 1.14.0", "<= v0.8");
        }
    }
}
