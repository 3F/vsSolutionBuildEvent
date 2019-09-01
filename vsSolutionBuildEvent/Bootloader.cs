/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using net.r_eg.SobaScript;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE
{
    //TODO: new arch for SobaScript + E-MSBuild
    internal sealed class Bootloader
    {
        public static Bootloader _
        {
            get;
            private set;
        }

        public IEnvironment Env
        {
            get;
            private set;
        }

        public IUVars UVars
        {
            get;
            private set;
        }

        public ISobaScript Soba
        {
            get;
            private set;
        }

        public static Bootloader Init(IEnvironment env)
        {
            if(_ == null) {
                _ = new Bootloader(env);
            }
            return _;
        }

        public static ISobaScript Configure(ISobaScript soba, IEnvironment env)
        {
            if(soba == null) {
                throw new ArgumentNullException(nameof(soba));
            }

            //NOTE: custom order makes sense for vsSBE

            soba.Register(new TryComponent(soba));
            soba.Register(new CommentComponent());
            soba.Register(new BoxComponent(soba));
            soba.Register(new ConditionComponent(soba));
            soba.Register(new UserVariableComponent(soba));
            soba.Register(new OWPComponent(soba, env));
            soba.Register(new DTEComponent(soba, env));
            soba.Register(new InternalComponent(soba, env));
            soba.Register(new MSBuildComponent(soba));
            soba.Register(new BuildComponent(soba, env));
            soba.Register(new FunctionComponent(soba));
            soba.Register(new FileComponent(soba));
            soba.Register(new NuGetComponent(soba));
            soba.Register(new SevenZipComponent(soba));

            return soba;
        }

        public static ISobaScript Reset(ISobaScript soba, bool unsetUVars)
        {
            if(soba == null) {
                throw new ArgumentNullException(nameof(soba));
            }

            soba.Unregister();

            if(unsetUVars) {
                soba.UVars.UnsetAll();
            }

            return soba;
        }

        public ISobaScript Configure(ISobaScript soba)
            => Configure(soba, Env);

        private Bootloader(IEnvironment env)
        {
            Env = env ?? throw new ArgumentNullException(nameof(env));

            UVars = new UVars();

            Soba = Configure(
                new Soba(MSBuild.MakeEvaluator(env, UVars), UVars)
            );
        }
    }
}
