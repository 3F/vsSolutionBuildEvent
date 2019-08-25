using System;
using EvMSBuildTest.Stubs;
using net.r_eg.EvMSBuild;
using net.r_eg.Varhead;
using Xunit;

namespace EvMSBuildTest
{
    public class VtSignTest
    {
        [Fact]
        public void VtSignTest1()
        {
            var uvar    = new UVars();
            var target  = new EvMSBuilder(new EnvStub(), uvar);

            Assert.Equal(string.Empty, target.Eval("$(+name = 'myvar')"));
            Assert.Equal("myvar", uvar.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(+name += '12')"));
            Assert.Equal("myvar12", uvar.GetValue("name"));

            Assert.Equal(string.Empty, target.Eval("$(name += '34')"));
            Assert.Equal("myvar1234", uvar.GetValue("name"));
        }

        [Fact]
        public void VtSignTest2()
        {
            var uvar = new UVars();
            var target = new EvMSBuilder(new EnvStub(), uvar);

            Assert.Equal(string.Empty, target.Eval("$(i = 0)"));
            Assert.Equal(string.Empty, target.Eval("$(i += 1)"));
            Assert.Equal(string.Empty, target.Eval("$(i += 2)$(i += 1)"));
            Assert.Equal(string.Empty, target.Eval("$(i -= 2)"));
            Assert.Equal("2", uvar.GetValue("i"));
        }

        [Fact]
        public void VtSignTest3()
        {
            var uvar = new UVars();
            var target = new EvMSBuilder(new EnvStub(), uvar);
            
            Assert.Equal(string.Empty, target.Eval("$(i += 1)"));
            Assert.Equal("1", uvar.GetValue("i"));

            Assert.Equal(string.Empty, target.Eval("$(j -= 1)"));
            Assert.Equal("-1", uvar.GetValue("j"));
        }

        [Fact]
        public void VtSignTest4()
        {
            var uvar    = new UVars();
            var target  = new EvMSBuilder(new EnvStub(), uvar);

            Assert.Equal(string.Empty, target.Eval("$(i = \"test\")"));
            Assert.Equal(string.Empty, target.Eval("$(i += 1)"));
            Assert.Equal("test1", uvar.GetValue("i"));

            Assert.Throws<ArgumentException>(() =>
            {
                target.Eval("$(i -= 1)");
            });
        }

        [Fact]
        public void VtSignTest5()
        {
            var uvar = new UVars();
            var target = new EvMSBuilder(new EnvStub(), uvar);

            Assert.Equal(string.Empty, target.Eval("$(i = 1)"));
            Assert.Equal(string.Empty, target.Eval("$(i += $(i))"));
            Assert.Equal("2", uvar.GetValue("i"));

            //Assert.Equal(string.Empty, target.parse("$(i += '2')"));

            Assert.ThrowsAny<Exception>(() => target.Eval("$(i += 'test')"));
        }
    }
}