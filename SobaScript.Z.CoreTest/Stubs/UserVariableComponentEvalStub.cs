using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;

namespace SobaScript.Z.CoreTest.Stubs
{
    internal class UserVariableComponentEvalStub: UserVariableComponent
    {
        private class Evaluator1: IEvaluator
        {
            public string Evaluate(string data)
            {
                return $"[E1:{data}]";
            }
        }

        public UserVariableComponentEvalStub(IUVars uvariable)
            : base(new Soba(uvariable))
        {

        }

        protected override void Evaluate(string name, string project = null)
        {
            uvars.Evaluate(name, project, new Evaluator1(), true);
        }
    }
}