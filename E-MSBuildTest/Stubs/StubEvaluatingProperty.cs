namespace EvMSBuildTest.Stubs
{
    internal class StubEvaluatingProperty: EvMSBuilderStub
    {
        public override string GetPropValue(string name, string project)
        {
            if(Variables.IsExist(name, project)) {
                return GetUVarValue(name, project);
            }
            return $"[P~{name}~{project}]";
        }

        protected override string Obtain(string unevaluated, string project)
        {
            return $"[E~{unevaluated}~{project}]";
        }
    }
}