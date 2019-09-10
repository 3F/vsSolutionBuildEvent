using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.VS;

namespace SobaScript.Z.VSTest.Stubs
{
    internal class DteComponentAcs: DteComponent
    {
        public NullDteEnv Env => (NullDteEnv)env;

        //public void EmulateBeforeExecute(string guid, int id, object customIn, object customOut, bool cancelDefault)
        //{
        //    mEnvCE.Raise(e => e.BeforeExecute += null, guid, id, customIn, customOut, cancelDefault);
        //}

        //public void EmulateAfterExecute(string guid, int id, object customIn, object customOut)
        //{
        //    mEnvCE.Raise(e => e.AfterExecute += null, guid, id, customIn, customOut);
        //}

        public DteComponentAcs() 
            : base(new Soba(), new NullDteEnv())
        {

        }

        //protected void initCommandEvents()
        //{
        //    this.mEnvCE = new Mock<EnvDTE.CommandEvents>();
        //    mEnv.Setup(p => p.Events.get_CommandEvents("{00000000-0000-0000-0000-000000000000}", 0)).Returns(mEnvCE.Object);
        //}
    }
}
