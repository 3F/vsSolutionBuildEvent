using System.Collections.Generic;
using net.r_eg.SobaScript.Z.VS.Build;

namespace SobaScript.Z.VSTest.Stubs
{
    internal sealed class NullBuildEnv: IBuildEnv
    {
        private Dictionary<string, _SlnContext> data = new Dictionary<string, _SlnContext>();

        public string BuildType
        {
            get;
            set;
        }

        public bool IsCleanOperation
        {
            get;
            set;
        }

        public bool IsOpenedSolution
        {
            get;
            set;
        }

        public string SolutionFile
        {
            get;
            set;
        }

        public void CancelBuild() { }

        public void SetContext(string ident, bool isBuildable, bool isDeployable)
        {
            var x = GetContext(ident);

            x.IsBuildable   = isBuildable;
            x.IsDeployable  = isDeployable;
        }

        public ISolutionContext GetSolutionContext(object ident)
            => GetContext(ident.ToString());

        public NullBuildEnv()
        {

        }

        private _SlnContext GetContext(string id)
        {
            if(!data.ContainsKey(id)) {
                data[id] = new _SlnContext(id);
            }
            return data[id];
        }

        private sealed class _SlnContext: ISolutionContext
        {
            public bool IsBuildable
            {
                get;
                set;
            }

            public bool IsDeployable
            {
                get;
                set;
            }

            public _SlnContext(string projectName)
            {

            }
        }
    }
}
