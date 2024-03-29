﻿/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.SobaScript.Z.Ext;
using net.r_eg.SobaScript.Z.Ext.IO;
using net.r_eg.SobaScript.Z.VS;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SobaScript.Components;

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

            IEncDetector detector = new EncDetector();

            var fc = new FileComponent(soba, detector, new Exer(Settings.WPath, detector));
            var zc = new SevenZipComponent(soba, new SzArchiver(), Settings.WPath);
            var nc = new NuGetComponent(soba, Settings.WPath);

            Settings._.WorkPathUpdated += (object sender, DataArgs<string> e) =>
            {
                fc.Exer.BasePath = e.Data;
                zc.BasePath = e.Data;
                nc.BasePath = e.Data;
            };

            //NOTE: custom order makes sense for vsSBE

            soba.Register(new TryComponent(soba));
            soba.Register(new CommentComponent());
            soba.Register(new BoxComponent(soba));
            soba.Register(new ConditionComponent(soba));
            soba.Register(new UserVariableComponent(soba));
            soba.Register(new OwpComponent(soba, new OwpEnv(env)));
            soba.Register(new DteComponent(soba, new DteEnv(env)));
            soba.Register(new InternalComponent(soba, env, fc.Exer));
            soba.Register(new EvMSBuildComponent(soba));
            soba.Register(new BuildComponent(soba, new BuildEnv(env)));
            soba.Register(new FunctionComponent(soba));
            soba.Register(fc);
            soba.Register(nc);
            soba.Register(zc);

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
