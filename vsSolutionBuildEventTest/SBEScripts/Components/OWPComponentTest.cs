using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test
{
    /// <summary>
    ///This is a test class for OWPComponentTest and is intended
    ///to contain all OWPComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OWPComponentTest
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

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest()
        {
            OWPComponent target = new OWPComponent();
            target.parse("#[OWP out.Warnings.Count]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            OWPComponent target = new OWPComponent();
            target.parse("OWP out.Warnings.Count");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest3()
        {
            OWPComponent target = new OWPComponent();
            target.parse("[OWP NotFound.Test]");
        }

        /// <summary>
        ///A test for parse - stOut
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            OWPComponent target = new OWPComponent();
            Assert.AreEqual(String.Empty, target.parse("[OWP out.All]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings.Raw]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings]"));
            Assert.AreEqual("0", target.parse("[OWP out.Warnings.Count]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings.Codes]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors.Raw]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors]"));
            Assert.AreEqual("0", target.parse("[OWP out.Errors.Count]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors.Codes]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void parseTest5()
        {
            OWPComponent target = new OWPComponent();
            target.parse("[OWP out.NotSupportedTest]");
        }
    }
}
