using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Dom;

namespace net.r_eg.vsSBE.Test.SBEScripts
{
    [TestClass]
    public class ScriptTest
    {
        private static readonly IEnvironment env = new Environment((EnvDTE80.DTE2)null);
        private static IUVars uvariable = new UVars();

        [TestMethod]
        public void parseTest()
        {
            Soba target = new Soba();

            string expected = "#[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]";
            string actual   = target.parse("##[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(MismatchException))]
        public void parseTest2()
        {
            var target = StubSoba.MakeNew();
            target.parse("#[NotRealComponent prop.Test]");
        }

        [TestMethod]
        public void parseTest3()
        {
            var target = StubSoba.MakeNew();
            Assert.AreEqual("[( 2 > 1) { body }]", target.parse("[( 2 > 1) { body }]"));
            Assert.AreEqual("( 2 > 1) { body }", target.parse("( 2 > 1) { body }"));
            Assert.AreEqual(" test ", target.parse(" test "));
            Assert.AreEqual("", target.parse(""));
            Assert.AreEqual(" \"test\" ", target.parse(" \"test\" "));
        }

        [TestMethod]
        public void parseTest4()
        {
            var target = StubSoba.MakeNew();
            Assert.AreEqual("B4 ", target.parse("#[(true) {\n #[(1 > 2){ B3 } \n else {B4} ] } else {\n B2 }]"));
        }

        [TestMethod]
        public void parseTest5()
        {
            uvariable.UnsetAll();
            var target = StubSoba.MakeNew(uvariable);

            target.parse("#[( 2 < 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.AreEqual(1, uvariable.Variables.Count());
            foreach(TVariable uvar in uvariable.Variables) {
                Assert.AreEqual(uvar.ident, "name");
                Assert.AreEqual(uvar.unevaluated, "value2");
            }
        }

        [TestMethod]
        public void parseTest6()
        {
            uvariable.UnsetAll();
            var target = StubSoba.MakeNew(uvariable);

            Assert.AreEqual(String.Empty, target.parse("#[\" #[var name = value] \"]"));
            Assert.AreEqual(0, uvariable.Variables.Count());
        }

        [TestMethod]
        public void containerTest1()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual("ne]", target.parse("#[var name = value\nli]ne]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("value\nli", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest2()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(string.Empty, target.parse("#[var name = <#data>value\nli]ne</#data>]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest3()
        {
            var uvar = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = #[var mx]]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest4()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = <#data>#[var mx]|value\nli]ne</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne|value\nli]ne", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest5()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = #[var mx]]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest6()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual(String.Empty, target.parse("#[var name = left [box1] right]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("left [box1] right", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest7()
        {
            var uvar    = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual("#[var name = left [box1 right]", target.parse("#[var name = left [box1 right]"));
            Assert.AreEqual(0, uvar.Variables.Count());

            Assert.AreEqual(String.Empty, target.parse("#[var name = \"left [box1 right\"]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("\"left [box1 right\"", uvar.GetValue("name"));
        }

        [TestMethod]
        public void containerTest8()
        {
            var uvar = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual("test - cc", target.parse("#[var sres = <#data>Data1</#data>]test - cc#[var sres2 = <#data>Data2</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("Data1", uvar.GetValue("sres"));
            Assert.AreEqual("Data2", uvar.GetValue("sres2"));
        }

        [TestMethod]
        public void containerTest9()
        {
            var uvar = new UVars();
            var target  = StubSoba.MakeNew(uvar);

            Assert.AreEqual("test - cc", target.parse("#[var sres = <#data>Data1\n\nEnd</#data>]test - cc#[var sres2 = <#data>Data2\n\nEnd</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("Data1\n\nEnd", uvar.GetValue("sres"));
            Assert.AreEqual("Data2\n\nEnd", uvar.GetValue("sres2"));
        }

        [TestMethod]
        [ExpectedException(typeof(Varhead.Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest1()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            // p4 -> p2 -> p3 -> p1 -> p4
            msbuild.Eval(sbe.parse("#[var p1 = $$(p2)]#[var p2 = $$(p1)]#[var p3 = $(p2)]", true));
        }

        [TestMethod]
        [ExpectedException(typeof(Varhead.Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest2()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            msbuild.Eval(sbe.parse("#[var p1 = $$(p4)]#[var p2 = $$(p3)]#[var p3 = $$(p1)]#[var p4 = $(p2)]", true));
        }

        [TestMethod]
        public void parseMSBuildUnloopingTest3()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            msbuild.Eval(sbe.parse("#[var p2 = $$(p1)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true));
            Assert.IsTrue(true); // no problems for stack & heap
        }

        [TestMethod]
        [ExpectedException(typeof(Varhead.Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest4()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            msbuild.Eval(sbe.parse("#[var p2 = $$(p1) to $$(p8), and new ($$(p7.Replace('1', '2'))) s$$(p9)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true));
            Assert.IsTrue(true); // no problems for stack & heap
        }

        [TestMethod]
        public void parseMSBuildUnloopingTest5()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            msbuild.Eval(sbe.parse("#[var test = $$(test)]#[var test = 1 $(test) 2]", true));

            uvar.UnsetAll();
            msbuild.Eval(sbe.parse("#[var test = $$(test)]#[var test = 1 $(test.Replace('1', '2')) 2]", true));

            uvar.UnsetAll();
            msbuild.Eval(sbe.parse("#[var test = $(test)]#[var test = 1 $(test) 2]", true));
        }

        [TestMethod]
        public void parseMSBuildUnloopingTest6()
        {
            var env     = new StubEnv();
            var uvar    = new UVars();
            var msbuild = new EvMSBuilder(env, uvar);
            var sbe     = StubSoba.MakeNew(uvar);

            string data = "#[var test = #[($(test) == \""+ EvMSBuilder.UNDEF_VAL + "\"){0}else{$(test)}]]#[var test]";
            Assert.AreEqual("0", msbuild.Eval(sbe.parse(data, true)));
            Assert.AreEqual("0", msbuild.Eval(sbe.parse(data, true)));

            uvar.SetVariable("test", null, "7");
            uvar.Evaluate("test", null, new EvaluatorBlank(), true);
            Assert.AreEqual("7", msbuild.Eval(sbe.parse(data, true)));
        }

        [TestMethod]
        public void selectorTest1()
        {
            var target  = new Soba(new UVars());
            var dom     = new Inspector(target);
            
            // Compliance Test - entry point to component + CRegex flag
            foreach(var c in dom.Root)
            {
                try {
                    target.parse($"#[{c.Name} ]");
                }
                catch(MismatchException ex) {
                    Assert.Fail("`{0}` <- `{1}`", c.Name, ex.Message);
                }
                catch { }
            }
        }
    }
}
