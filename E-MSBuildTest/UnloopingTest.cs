using EvMSBuildTest.Stubs;
using net.r_eg.EvMSBuild;
using net.r_eg.Varhead.Exceptions;
using Xunit;

namespace EvMSBuildTest
{
    public class UnloopingTest
    {
        [Fact]
        public void ParseUnloopingTest1()
        {
            var target = new EvMSBuilderStub();

            string data = "$(myVar = $$(myVar))$(myVar)";
            Assert.Equal("$(myVar)", target.Eval(data));
        }

        [Fact]
        public void ParseUnloopingTest2()
        {
            var target = new EvMSBuilderStub();

            string data = "$(myVar = $(myVar))$(myVar)";
            Assert.Equal(EvMSBuilder.UNDEF_VAL, target.Eval(data));
        }

        [Fact]
        public void ParseUnloopingTest3()
        {
            var target = new EvMSBuilderStub();

            Assert.Throws<LimitException>(() => {
                // p1 -> p2 -> p1 ] p3 -> p2
                target.Eval("$(p1 = $$(p2))$(p2 = $$(p1))$(p3 = $(p2))");
            });
        }

        [Fact]
        public void ParseUnloopingTest4()
        {
            var target = new EvMSBuilderStub();

            Assert.Throws<LimitException>(() => {
                // p4 -> p2 -> p3 -> p1 -> p4
                target.Eval("$(p4 = $$(p2))$(p3 = $$(p1))$(p2 = $$(p3))$(p1 = $$(p4))$(p4)");
            });
        }

        [Fact]
        public void ParseUnloopingTest5()
        {
            var target = new EvMSBuilderStub();
            Assert.Equal(EvMSBuilder.UNDEF_VAL, target.Eval("$(p2 = $$(p1))$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)"));
        }

        [Fact]
        public void ParseUnloopingTest6()
        {
            var target = new EvMSBuilderStub();
            target.Eval("$(p2 = \"$$(p1) to $$(p8),  and new ($$( p7.Replace('1', '2'))) s$$(p9)\")$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)");
            target.Eval("$(p2 = \"$$(p1) to $$(p8),  and new ($$(p7.Replace('1', '2'))) s$$(p9)\")$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)");
            Assert.True(true); // no problems for stack & heap
        }

        [Fact]
        public void ParseUnloopingTest7()
        {
            var target = new EvMSBuilderStub();
            Assert.Equal(string.Format("2 {0} 2", EvMSBuilder.UNDEF_VAL), 
                            target.Eval("$(test = \"1 $(test) 2\")$(test = $(test.Replace('1', '2')))$(test)"));
        }

        [Fact]
        public void ParseUnloopingTest8()
        {
            var target = new EvMSBuilderStub();
            Assert.Equal("7", target.Eval("$(test = 7)$(test = $(test))$(test)"));
        }
    }
}