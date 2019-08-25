using net.r_eg.Varhead;

namespace EvMSBuildTest.Stubs
{
    internal class ToUserVariables: StubEvaluatingProperty
    {
        public IUVars AccessToVariables
        {
            get => Variables;
            set => Variables = value;
        }
    }
}