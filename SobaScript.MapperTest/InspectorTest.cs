using System.Linq;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using Xunit;

namespace SobaScript.MapperTest
{
    public class InspectorTest
    {
        [Fact]
        public void DataTest1()
        {
            var soba = new Soba(new UVars());
            var dom = new Inspector(soba);

            Assert.Empty(dom.Root);
            Assert.Empty(dom.GetBy(new EvMSBuildComponent(soba)));
        }

        [Fact]
        public void DataTest2()
        {
            var dom = new Inspector(new Soba(new UVars()));

            Assert.Empty(dom.GetBy(typeof(string)));
            Assert.Empty(dom.GetBy(new NodeIdent()));
        }

        [Fact]
        public void DataTest3()
        {
            var soba    = new Soba(new UVars());
            var evm     = new EvMSBuildComponent(soba);
            soba.Register(evm);

            var dom = new Inspector(soba);
            Assert.Single(dom.Root);

            Assert.NotNull(dom.Root.First());
        }
    }
}
