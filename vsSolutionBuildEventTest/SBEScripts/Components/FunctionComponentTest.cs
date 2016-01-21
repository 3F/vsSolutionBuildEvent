using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for FunctionComponentTest and is intended
    ///to contain all FunctionComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FunctionComponentTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest1()
        {
            FunctionComponent target = new FunctionComponent();
            target.parse("[Func NotRealSubtype.check]");
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest1()
        {
            var target = new FunctionComponent();

            try {
                target.parse("[Func hash]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash = 1]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest2()
        {
            var target = new FunctionComponent();

            try {
                target.parse("[Func hash.MD5]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest3()
        {
            FunctionComponent target = new FunctionComponent();
            Assert.AreEqual("ED076287532E86365E841E92BFC50D8C", target.parse("[Func hash.MD5(\"Hello World!\")]"));
            Assert.AreEqual("2EF7BDE608CE5404E97D5F042F95F89F1C232871", target.parse("[Func hash.SHA1(\"Hello World!\")]"));
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest4()
        {
            var target = new FunctionComponent();

            try {
                target.parse("[Func hash.MD5(\"Hello World!\").right]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1(\"Hello World!\").right]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest5()
        {
            var target = new FunctionComponent();

            try {
                target.parse("[Func hash.MD5(\"Hello World!\") = true]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1(\"Hello World!\") = true]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest6()
        {
            var target = new FunctionComponent();

            try {
                target.parse("[Func hash.MD5()]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1()]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.MD5(test)]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1(test)]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.MD5(\"test\", true)]");
                Assert.Fail("5");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }

            try {
                target.parse("[Func hash.SHA1(\"test\", true)]");
                Assert.Fail("6");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(InvalidArgumentException), ex.GetType().ToString()); }
        }

        /// <summary>
        ///A test for parse - hash
        ///</summary>
        [TestMethod()]
        public void hashTest7()
        {
            FunctionComponent target = new FunctionComponent();
            Assert.AreEqual("D41D8CD98F00B204E9800998ECF8427E", target.parse("[Func hash.MD5(\"\")]"));
            Assert.AreEqual("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709", target.parse("[Func hash.SHA1(\"\")]"));
        }
    }
}
