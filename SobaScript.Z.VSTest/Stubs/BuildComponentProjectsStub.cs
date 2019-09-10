using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.VS;
using net.r_eg.SobaScript.Z.VS.Build;

namespace SobaScript.Z.VSTest.Stubs
{
    internal class BuildComponentProjectsStub: BuildComponent
    {
        internal const string EXIST_GUID = "{11111111-1111-1111-1111-111111111111}";
        internal const string NOTEXIST_GUID = "{00000000-0000-0000-0000-000000000000}";

        internal StubProjectsMap SMap
        {
            get;
            private set;
        }

        public BuildComponentProjectsStub()
            : this(new NullBuildEnv() { IsOpenedSolution = true, SolutionFile = "stub.sln" })
        {

        }

        public BuildComponentProjectsStub(NullBuildEnv env)
            : base(new Soba(), env)
        {
            SMap = new StubProjectsMap();
        }

        internal override ProjectsMap GetProjectsMap(string sln) => SMap;

        internal class StubProjectsMap: ProjectsMap
        {
            public string Project1Guid => EXIST_GUID;
            public string Project1Type => "{F2D36D43-290E-4564-9D09-19E2E3D595FD}";

            public void SetPrj(string pGuid, string name, string path, string type)
            {
                SetProjectRecord
                (
                    pGuid,
                    name,
                    path,
                    type
                );

                order.Add(pGuid);
            }

            public StubProjectsMap()
            {
                SetPrj(Project1Guid, "Project1", "path\\to.sln", Project1Type);
            }
        }
    }
}
