using net.r_eg.EvMSBuild;

namespace EvMSBuildTest.Stubs
{
    internal class EvMSBuilderStub: EvMSBuilder
    {
        public EvMSBuilderStub()
            : base(new EnvStub())
        {

        }
    }
}