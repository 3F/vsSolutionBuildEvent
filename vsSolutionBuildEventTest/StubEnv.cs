using System.Collections.Generic;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln;

namespace net.r_eg.vsSBE.Test
{
    public class StubEnv: IsolatedEnv, IEnvironment
    {
        public override Project getProject(string name = null)
        {
            return new Project(slnProperties, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <param name="properties">Solution properties.</param>
        public StubEnv(Dictionary<string, string> properties)
            : base(properties)
        {

        }

        public StubEnv()
            : this(new Dictionary<string, string>() { { PropertyNames.CONFIG, "Debug" }, { PropertyNames.PLATFORM, "x86" } })
        {

        }
    }
}
