using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using net.r_eg.MvsSln;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.vsSBE.Test
{
    public class StubEnv: IsolatedEnv, IEnvironment
    {
        public override string SolutionFile
        {
            get => SlnParser.MEM_FILE;
        }

        public override Project getProject(string name = null)
        {
            return new Project(slnProperties, null, ProjectCollection.GlobalProjectCollection);
        }

        /// <param name="properties">Solution properties.</param>
        public StubEnv(Dictionary<string, string> properties)
            : base(properties)
        {

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public StubEnv()
            : this(new Dictionary<string, string>() { { PropertyNames.CONFIG, "Debug" }, { PropertyNames.PLATFORM, "x86" } })
        {
            using(var ms = new MemoryStream())
            using(var stream = new StreamReader(ms))
            {
                Sln = new SlnParser().Parse(stream, SlnItems.None);
            }
        }
    }
}
