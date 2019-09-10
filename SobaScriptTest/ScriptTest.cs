using System.Linq;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.SobaScript.Z.Core;
using net.r_eg.Varhead;
using SobaScriptTest.Stubs;
using Xunit;

namespace SobaScriptTest
{
    public class ScriptTest
    {
        private static IUVars uvars = new UVars();

        [Fact]
        public void ParseTest()
        {
            Soba target = new Soba();

            string expected = "#[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]";
            string actual   = target.Eval("##[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseTest2()
        {
            Assert.Throws<MismatchException>(() =>
            {
                var target = SobaAcs.MakeNewCoreOnly();
                target.Eval("#[NotRealComponent prop.Test]");
            });
        }

        [Fact]
        public void ParseTest3()
        {
            var target = SobaAcs.MakeNewCoreOnly();
            Assert.Equal("[( 2 > 1) { body }]", target.Eval("[( 2 > 1) { body }]"));
            Assert.Equal("( 2 > 1) { body }", target.Eval("( 2 > 1) { body }"));
            Assert.Equal(" test ", target.Eval(" test "));
            Assert.Equal("", target.Eval(""));
            Assert.Equal(" \"test\" ", target.Eval(" \"test\" "));
        }

        [Fact]
        public void ParseTest4()
        {
            var target = SobaAcs.MakeNewCoreOnly();
            Assert.Equal("B4 ", target.Eval("#[(true) {\n #[(1 > 2){ B3 } \n else {B4} ] } else {\n B2 }]"));
        }

        [Fact]
        public void ParseTest5()
        {
            uvars.UnsetAll();
            var target = SobaAcs.MakeNewCoreOnly(uvars);

            target.Eval("#[( 2 < 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.Single(uvars.Variables);
            foreach(TVariable uvar in uvars.Variables) {
                Assert.Equal("name", uvar.ident);
                Assert.Equal("value2", uvar.unevaluated);
            }
        }

        [Fact]
        public void ParseTest6()
        {
            uvars.UnsetAll();
            var target = SobaAcs.MakeNewCoreOnly(uvars);

            Assert.Equal(string.Empty, target.Eval("#[\" #[var name = value] \"]"));
            Assert.Empty(uvars.Variables);
        }

        [Fact]
        public void ContainerTest1()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal("ne]", target.Eval("#[var name = value\nli]ne]"));
            Assert.Single(uvar.Variables);
            Assert.Equal("value\nli", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest2()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal(string.Empty, target.Eval("#[var name = <#data>value\nli]ne</#data>]"));
            Assert.Single(uvar.Variables);
            Assert.Equal("value\nli]ne", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest3()
        {
            var uvar = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.Equal(string.Empty, target.Eval("#[var name = #[var mx]]"));
            Assert.Equal(2, uvar.Variables.Count());
            Assert.Equal("value\nli]ne", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest4()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.Equal(string.Empty, target.Eval("#[var name = <#data>#[var mx]|value\nli]ne</#data>]"));
            Assert.Equal(2, uvar.Variables.Count());
            Assert.Equal("value\nli]ne|value\nli]ne", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest5()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.Equal(string.Empty, target.Eval("#[var name = #[var mx]]"));
            Assert.Equal(2, uvar.Variables.Count());
            Assert.Equal("value\nli]ne", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest6()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal(string.Empty, target.Eval("#[var name = left [box1] right]"));
            Assert.Single(uvar.Variables);
            Assert.Equal("left [box1] right", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest7()
        {
            var uvar    = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal("#[var name = left [box1 right]", target.Eval("#[var name = left [box1 right]"));
            Assert.Empty(uvar.Variables);

            Assert.Equal(string.Empty, target.Eval("#[var name = \"left [box1 right\"]"));
            Assert.Single(uvar.Variables);
            Assert.Equal("\"left [box1 right\"", uvar.GetValue("name"));
        }

        [Fact]
        public void ContainerTest8()
        {
            var uvar = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal("test - cc", target.Eval("#[var sres = <#data>Data1</#data>]test - cc#[var sres2 = <#data>Data2</#data>]"));
            Assert.Equal(2, uvar.Variables.Count());
            Assert.Equal("Data1", uvar.GetValue("sres"));
            Assert.Equal("Data2", uvar.GetValue("sres2"));
        }

        [Fact]
        public void ContainerTest9()
        {
            var uvar = new UVars();
            var target  = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Equal("test - cc", target.Eval("#[var sres = <#data>Data1\n\nEnd</#data>]test - cc#[var sres2 = <#data>Data2\n\nEnd</#data>]"));
            Assert.Equal(2, uvar.Variables.Count());
            Assert.Equal("Data1\n\nEnd", uvar.GetValue("sres"));
            Assert.Equal("Data2\n\nEnd", uvar.GetValue("sres2"));
        }

        [Fact]
        public void ParseMSBuildUnloopingTest1()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Throws<net.r_eg.Varhead.Exceptions.LimitException>(() =>
                // p4 -> p2 -> p3 -> p1 -> p4
                msbuild.Eval(sbe.Eval("#[var p1 = $$(p2)]#[var p2 = $$(p1)]#[var p3 = $(p2)]", true))
            );
        }

        [Fact]
        public void ParseMSBuildUnloopingTest2()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Throws<net.r_eg.Varhead.Exceptions.LimitException>(() =>
                msbuild.Eval(sbe.Eval("#[var p1 = $$(p4)]#[var p2 = $$(p3)]#[var p3 = $$(p1)]#[var p4 = $(p2)]", true))
            );
        }

        [Fact]
        public void ParseMSBuildUnloopingTest3()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            msbuild.Eval(sbe.Eval("#[var p2 = $$(p1)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true));
            // shouldn't throw LimitException, ie. no problems for stack & heap
        }

        [Fact]
        public void ParseMSBuildUnloopingTest4()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            Assert.Throws<net.r_eg.Varhead.Exceptions.LimitException>(() =>
                msbuild.Eval(sbe.Eval("#[var p2 = $$(p1) to $$(p8), and new ($$(p7.Replace('1', '2'))) s$$(p9)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true))
            );
        }

        [Fact]
        public void ParseMSBuildUnloopingTest5()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            msbuild.Eval(sbe.Eval("#[var test = $$(test)]#[var test = 1 $(test) 2]", true));

            uvar.UnsetAll();
            msbuild.Eval(sbe.Eval("#[var test = $$(test)]#[var test = 1 $(test.Replace('1', '2')) 2]", true));

            uvar.UnsetAll();
            msbuild.Eval(sbe.Eval("#[var test = $(test)]#[var test = 1 $(test) 2]", true));

            // shouldn't throw LimitException, ie. no problems for stack & heap
        }

        [Fact]
        public void ParseMSBuildUnloopingTest6()
        {
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(uvar);
            var sbe     = SobaAcs.MakeNewCoreOnly(uvar);

            string data = "#[var test = #[($(test) == \""+ EvMSBuilder.UNDEF_VAL + "\"){0}else{$(test)}]]#[var test]";
            Assert.Equal("0", msbuild.Eval(sbe.Eval(data, true)));
            Assert.Equal("0", msbuild.Eval(sbe.Eval(data, true)));

            uvar.SetVariable("test", null, "7");
            uvar.Evaluate("test", null, new EvaluatorBlank(), true);
            Assert.Equal("7", msbuild.Eval(sbe.Eval(data, true)));
        }

        //[Fact]
        //public void SelectorTest1()
        //{
        //    var target  = StubSoba.MakeNewCoreOnly(new UVars());
        //    var dom     = new Inspector(target);

        //    foreach(var c in dom.Root)
        //    {
        //        //target.Eval($"#[{c.Name} ]");

        //    }
        //}
    }
}
