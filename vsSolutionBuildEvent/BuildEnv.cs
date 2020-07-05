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
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.VS.Build;
using net.r_eg.vsSBE.Actions;

namespace net.r_eg.vsSBE
{
    internal sealed class BuildEnv: IBuildEnv
    {
        private IEnvironment env;
        private Lazy<DTEOperation> dteo;

        public string BuildType => env.BuildType.ToString();

        public bool IsCleanOperation
        {
            get
            {
                switch(env.BuildType)
                {
                    case Bridge.BuildType.Clean:
                    case Bridge.BuildType.CleanCtx:
                    case Bridge.BuildType.CleanOnlyProject:
                    case Bridge.BuildType.CleanSelection: {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsOpenedSolution => env.IsOpenedSolution;

        public string SolutionFile => env.SolutionFile;

        public void CancelBuild() => dteo.Value.exec("Build.Cancel");

        public ISolutionContext GetSolutionContext(object ident) 
            => new _SlnContext(ident.ToString(), env);

        public BuildEnv(IEnvironment env)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env));

            dteo = new Lazy<DTEOperation>(() => 
                new DTEOperation(env, Events.SolutionEventType.General)
            );
        }

        private sealed class _SlnContext: ISolutionContext
        {
            private SolutionContext context;

            public bool IsBuildable
            {
                get => context.ShouldBuild;
                set => context.ShouldBuild = value;
            }

            public bool IsDeployable
            {
                get => context.ShouldDeploy;
                set => context.ShouldDeploy = value;
            }

            public _SlnContext(string projectName, IEnvironment env)
            {
                context = getContextByProject(projectName, env);
            }

            private SolutionContext getContextByProject(string name, IEnvironment env)
            {
                if(env?.SolutionActiveCfg == null) {
                    throw new NotSupportedException("SolutionActiveCfg is disabled for current environment.");
                }

                var slnc = env.SolutionActiveCfg.SolutionContexts.GetEnumerator();

                while(slnc.MoveNext())
                {
                    var con = (SolutionContext)slnc.Current;

                    if(con.ProjectName.Contains(name)) {
                        return con;
                    }
                }

                throw new NotFoundException(name);
            }
        }
    }
}
