using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass()]
    public class MSBuildComponentTest
    {
        private IBootloader bootloader;
        private IEnvironment env = new StubEnv();
        private IUserVariable uvariable = new UserVariable();

        private IBootloader Loader
        {
            get {
                if(bootloader == null) {
                    bootloader = new Bootloader(env, uvariable);
                    bootloader.register();
                }
                return bootloader;
            }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest1()
        {
            var target = new MSBuildComponent(Loader);
            Assert.AreEqual(vsSBE.MSBuild.Parser.PROP_VALUE_DEFAULT, target.parse("[$(notRealVariablename)]"));
            Assert.AreEqual("65536", target.parse("[$([System.Math]::Pow(2, 16))]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            var target = new MSBuildComponent(Loader);

            try {
                target.parse("[$()]");
                Assert.Fail("1");
            }
            catch(SyntaxIncorrectException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[$(]");
                Assert.Fail("2");
            }
            catch(SyntaxIncorrectException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[$(notRealVariablename]");
                Assert.Fail("3");
            }
            catch(SyntaxIncorrectException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest3()
        {
            var target = new MSBuildComponent(Loader);
            Assert.AreEqual(Value.Empty, target.parse("[$(vParseTest3 = \"string123\")]"));
            Assert.AreEqual(" left 'string123' ) right ", target.parse("[$([System.String]::Format(\" left '{0}' ) right \", $(vParseTest3)))]"));
            Assert.AreEqual(" left \"string123\" ) right ", target.parse("[$([System.String]::Format(' left \"{0}\" ) right ', $(vParseTest3)))]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            var target = new MSBuildComponent(Loader);
            Assert.AreEqual("$(name)", target.parse("[$$(name)]"));
            Assert.AreEqual("$$(name)", target.parse("[$$$(name)]"));
            Assert.AreEqual("$([System.String]::Format(\" left '{0}' ) right \", $(name)))", target.parse("[$$([System.String]::Format(\" left '{0}' ) right \", $(name)))]"));
        }
    }
}
