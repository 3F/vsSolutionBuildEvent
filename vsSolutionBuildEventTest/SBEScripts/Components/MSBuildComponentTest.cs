using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass()]
    public class MSBuildComponentTest
    {
        [TestMethod()]
        public void parseTest1()
        {
            var target = new MSBuildComponent(new Soba());
            Assert.AreEqual(EvMSBuilder.UNDEF_VAL, target.parse("[$(notRealVariablename)]"));
            Assert.AreEqual("65536", target.parse("[$([System.Math]::Pow(2, 16))]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            var target = new MSBuildComponent(new Soba());

            try {
                target.parse("[$()]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectSyntaxException), ex.GetType().ToString()); }

            try {
                target.parse("[$(]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectSyntaxException), ex.GetType().ToString()); }

            try {
                target.parse("[$(notRealVariablename]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectSyntaxException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest3()
        {
            var target = new MSBuildComponent(new Soba());
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
            var target = new MSBuildComponent(new Soba());
            Assert.AreEqual("$(name)", target.parse("[$$(name)]"));
            Assert.AreEqual("$$(name)", target.parse("[$$$(name)]"));
            Assert.AreEqual("$([System.String]::Format(\" left '{0}' ) right \", $(name)))", target.parse("[$$([System.String]::Format(\" left '{0}' ) right \", $(name)))]"));
        }
    }
}
