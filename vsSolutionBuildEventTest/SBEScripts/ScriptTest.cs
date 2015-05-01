using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.SBEScripts
{
    /// <summary>
    ///This is a test class for ScriptTest and is intended
    ///to contain all ScriptTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScriptTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
            uvariable.unsetAll();
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
            uvariable.unsetAll();
            Script target = new Script(env, uvariable);

            Assert.AreEqual(String.Empty, target.parse("#[\" #[var name = value] \"]"));
            Assert.AreEqual(0, uvariable.Variables.Count());
        }
    }
}
