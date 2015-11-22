using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace net.r_eg.vsSBE.Test
{
    public class StubEnv: IsolatedEnv, IEnvironment
    {
        public override Project getProject(string name = null)
        {
            return new Project(properties, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <param name="properties">Solution properties.</param>
        public StubEnv(Dictionary<string, string> properties)
            : base(properties)
        {

        }

        public StubEnv()
            : this(new Dictionary<string, string>() { { "Configuration", "Debug" }, { "Platform", "x86" } })
        {

        }
    }
}
