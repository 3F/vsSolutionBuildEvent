using net.r_eg.EvMSBuild;

namespace EvMSBuildTest.Stubs
{
    internal class EvMSBuilderAcs: EvMSBuilder
    {
        public EvMSBuilderAcs()
            : base(new EnvStub())
        {

        }
    }
}