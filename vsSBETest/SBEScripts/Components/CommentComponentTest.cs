using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for CommentComponentTest and is intended
    ///to contain all CommentComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommentComponentTest
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
        public void parseTest()
        {
            CommentComponent target = new CommentComponent();
            Assert.AreEqual(String.Empty, target.parse("[\"test\"]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            CommentComponent target = new CommentComponent();
            Assert.AreEqual(String.Empty, target.parse("[\"line1 \n line2\"]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest3()
        {
            CommentComponent target = new CommentComponent();
            target.parse("#[\"test\"]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest4()
        {
            CommentComponent target = new CommentComponent();
            target.parse("[test]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest5()
        {
            CommentComponent target = new CommentComponent();
            target.parse("test");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest6()
        {
            CommentComponent target = new CommentComponent();
            target.parse("");
        }
    }
}
