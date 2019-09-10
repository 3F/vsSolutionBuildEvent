namespace EvMSBuildTest.Stubs
{
    internal class StubEvaluatingProperty: EvMSBuilderAcs
    {
        public override string GetPropValue(string name, string project)
        {
            if(UVars.IsExist(name, project)) {
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