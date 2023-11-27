/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using net.r_eg.EvMSBuild;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.Bridge.CoreCommand;

namespace net.r_eg.vsSBE
{
    internal static class MSBuild
    {
        internal static IEvMSBuild MakeEvaluator(IEnvironment env, IUVars uvars)
            => PostAction(env, new EvMSBuilder(env, uvars));

        internal static IEvMSBuild MakeEvaluator(IEnvironment env)
            => PostAction(env, new EvMSBuilder(env));

        private static IEvMSBuild PostAction(IEnvironment env, IEvMSBuild instance)
        {
            //TODO: to CoreCommandType

            instance.GlobalPropertyChanged += (object sender, PropertyArgs e)
                => env?.CoreCmdSender?.fire(GetRawCommand(new[] 
                {
                    e.Removed ? "property.del" : "property.set",
                    e.name,
                    e.value
                }));

            return instance;
        }

        private static CoreCommandArgs GetRawCommand(object[] cmd)
            => new CoreCommandArgs() { Type = CoreCommandType.RawCommand, Args = cmd };
    }
}
