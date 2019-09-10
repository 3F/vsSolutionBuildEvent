using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.VS;

namespace SobaScript.Z.VSTest.Stubs
{
    internal class BuildComponentAcs: BuildComponent
    {
        //protected new DTEOperation DTEO
        //{
        //    get;
        //    private set;
        //}

        //public BuildComponentStub()
        //    : base(new Soba(), (IEnvironment)null)
        //{
        //    var mock = new Mock<DTEOperation>(null, SolutionEventType.General);
        //    mock.Setup(m => m.exec(It.IsAny<string[]>(), It.IsAny<bool>()));
        //    DTEO = mock.Object;
        //}

        //public BuildComponentStub(IEnvironment env)
        //    : base(new Soba(), env)
        //{

        //}

        public BuildComponentAcs()
            : base(new Soba(), new NullBuildEnv())
        {

        }
    }
}
