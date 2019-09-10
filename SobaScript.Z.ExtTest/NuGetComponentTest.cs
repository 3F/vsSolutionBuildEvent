using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext;
using Xunit;

namespace SobaScript.Z.ExtTest
{
    public class NuGetComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new NuGetComponent(new Soba(), ".");

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[NuGet NotRealSubtype.check]")
            );
        }

        [Fact]
        public void GntTest1()
        {
            var target = new NuGetComponent(new Soba(), ".");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[NuGet gnt]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[NuGet gnt()]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[NuGet gnt.NotRealNode]")
            );
        }

        [Fact]
        public void GntRawTest1()
        {
            var target = new NuGetComponent(new Soba(), ".");

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[NuGet gnt.raw()]")
            );

            Assert.ThrowsAny<Exception>(() =>
                // should be any exception from gnt.core as normal behavior
                target.Eval("[NuGet gnt.raw(\"the is not a correct command\")]")
            );
        }
    }
}