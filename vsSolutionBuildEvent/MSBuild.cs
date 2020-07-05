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
