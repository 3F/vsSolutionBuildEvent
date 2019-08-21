using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts
{
    /// <summary>
    ///This is a test class for ScriptTest and is intended
    ///to contain all ScriptTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScriptTest
    {
        private static IEnvironment env         = new net.r_eg.vsSBE.Environment((EnvDTE80.DTE2)null);
        private static IUserVariable uvariable  = new UserVariable();

        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize()]
        public static void ScriptTestInitialize(TestContext testContext)
        {

        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest()
        {
            IEnvironment env = new net.r_eg.vsSBE.Environment((EnvDTE80.DTE2)null);
            Script target = new Script(env, new UserVariable());

            string expected = "#[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]";
            string actual   = target.parse("##[( 2 > 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SelectorMismatchException))]
        public void parseTest2()
        {
            Script target = new Script(env, new UserVariable());
            target.parse("#[NotRealComponent prop.Test]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest3()
        {
            Script target = new Script(env, new UserVariable());
            Assert.AreEqual("[( 2 > 1) { body }]", target.parse("[( 2 > 1) { body }]"));
            Assert.AreEqual("( 2 > 1) { body }", target.parse("( 2 > 1) { body }"));
            Assert.AreEqual(" test ", target.parse(" test "));
            Assert.AreEqual("", target.parse(""));
            Assert.AreEqual(" \"test\" ", target.parse(" \"test\" "));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            Script target = new Script(env, new UserVariable());
            Assert.AreEqual("B4 ", target.parse("#[(true) {\n #[(1 > 2){ B3 } \n else {B4} ] } else {\n B2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            uvariable.UnsetAll();
            Script target = new Script(env, uvariable);

            target.parse("#[( 2 < 1) { #[var name = value] } else { #[var name = value2] }]");
            Assert.AreEqual(1, uvariable.Variables.Count());
            foreach(TUserVariable uvar in uvariable.Variables) {
                Assert.AreEqual(uvar.ident, "name");
                Assert.AreEqual(uvar.unevaluated, "value2");
            }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            uvariable.UnsetAll();
            Script target = new Script(env, uvariable);

            Assert.AreEqual(String.Empty, target.parse("#[\" #[var name = value] \"]"));
            Assert.AreEqual(0, uvariable.Variables.Count());
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest1()
        {
            var uvar        = new UserVariable();
            Script target   = new Script(env, uvar);

            Assert.AreEqual("ne]", target.parse("#[var name = value\nli]ne]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("value\nli", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest2()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            Assert.AreEqual(String.Empty, target.parse("#[var name = <#data>value\nli]ne</#data>]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest3()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = #[var mx]]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest4()
        {
            var uvar        = new UserVariable();
            Script target   = new Script(env, uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = <#data>#[var mx]|value\nli]ne</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne|value\nli]ne", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest5()
        {
            var uvar        = new UserVariable();
            Script target   = new Script(env, uvar);

            uvar.SetVariable("mx", null, "<#data>value\nli]ne</#data>");
            uvar.Evaluate("mx", null, new EvaluatorBlank(), true);

            Assert.AreEqual(String.Empty, target.parse("#[var name = #[var mx]]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("value\nli]ne", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest6()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            Assert.AreEqual(String.Empty, target.parse("#[var name = left [box1] right]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("left [box1] right", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest7()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            Assert.AreEqual("#[var name = left [box1 right]", target.parse("#[var name = left [box1 right]"));
            Assert.AreEqual(0, uvar.Variables.Count());

            Assert.AreEqual(String.Empty, target.parse("#[var name = \"left [box1 right\"]"));
            Assert.AreEqual(1, uvar.Variables.Count());
            Assert.AreEqual("\"left [box1 right\"", uvar.GetValue("name"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest8()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            Assert.AreEqual("test - cc", target.parse("#[var sres = <#data>Data1</#data>]test - cc#[var sres2 = <#data>Data2</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("Data1", uvar.GetValue("sres"));
            Assert.AreEqual("Data2", uvar.GetValue("sres2"));
        }

        /// <summary>
        ///A test for container
        ///</summary>
        [TestMethod()]
        public void containerTest9()
        {
            var uvar = new UserVariable();
            Script target = new Script(env, uvar);

            Assert.AreEqual("test - cc", target.parse("#[var sres = <#data>Data1\n\nEnd</#data>]test - cc#[var sres2 = <#data>Data2\n\nEnd</#data>]"));
            Assert.AreEqual(2, uvar.Variables.Count());
            Assert.AreEqual("Data1\n\nEnd", uvar.GetValue("sres"));
            Assert.AreEqual("Data2\n\nEnd", uvar.GetValue("sres2"));
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest1()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            // p4 -> p2 -> p3 -> p1 -> p4
            msbuild.parse(sbe.parse("#[var p1 = $$(p2)]#[var p2 = $$(p1)]#[var p3 = $(p2)]", true));
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest2()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            msbuild.parse(sbe.parse("#[var p1 = $$(p4)]#[var p2 = $$(p3)]#[var p3 = $$(p1)]#[var p4 = $(p2)]", true));
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        public void parseMSBuildUnloopingTest3()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            msbuild.parse(sbe.parse("#[var p2 = $$(p1)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true));
            Assert.IsTrue(true); // no problems for stack & heap
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Exceptions.LimitException))]
        public void parseMSBuildUnloopingTest4()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            msbuild.parse(sbe.parse("#[var p2 = $$(p1) to $$(p8), and new ($$(p7.Replace('1', '2'))) s$$(p9)]#[var p6 = $$(p2)]#[var p7 = $$(p5)]#[var p5 = $(p6)]", true));
            Assert.IsTrue(true); // no problems for stack & heap
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        public void parseMSBuildUnloopingTest5()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            msbuild.parse(sbe.parse("#[var test = $$(test)]#[var test = 1 $(test) 2]", true));

            uvar.UnsetAll();
            msbuild.parse(sbe.parse("#[var test = $$(test)]#[var test = 1 $(test.Replace('1', '2')) 2]", true));

            uvar.UnsetAll();
            msbuild.parse(sbe.parse("#[var test = $(test)]#[var test = 1 $(test) 2]", true));
        }

        /// <summary>
        ///A test for parse - unlooping with MSBuild
        ///</summary>
        [TestMethod()]
        public void parseMSBuildUnloopingTest6()
        {
            var env     = new StubEnv();
            var uvar    = new UserVariable();
            var msbuild = new vsSBE.MSBuild.Parser(env, uvar);
            var sbe     = new Script(env, uvar);

            string data = "#[var test = #[($(test) == \""+ vsSBE.MSBuild.Parser.PROP_VALUE_DEFAULT + "\"){0}else{$(test)}]]#[var test]";
            Assert.AreEqual("0", msbuild.parse(sbe.parse(data, true)));
            Assert.AreEqual("0", msbuild.parse(sbe.parse(data, true)));

            uvar.SetVariable("test", null, "7");
            uvar.Evaluate("test", null, new EvaluatorBlank(), true);
            Assert.AreEqual("7", msbuild.parse(sbe.parse(data, true)));
        }

        /// <summary>
        ///A test for selector
        ///</summary>
        [TestMethod()]
        public void selectorTest1()
        {
            var target  = new Script(new StubEnv(), new UserVariable());
            var dom     = new Inspector(target.Bootloader);
            
            // Compliance Test - entry point to component + CRegex flag
            foreach(var c in dom.Root)
            {
                try {
                    target.parse(String.Format("#[{0} ]", c.Name));
                }
                catch(SelectorMismatchException ex) {
                    Assert.Fail("`{0}` <- `{1}`", c.Name, ex.Message);
                }
                catch { }
            }
        }
    }
}
