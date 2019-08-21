using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.Test.SBEScripts
{
    /// <summary>
    ///This is a test class for StringHandlerTest and is intended
    ///to contain all StringHandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringHandlerTest
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
        ///A test for normalize
        ///</summary>
        [TestMethod()]
        public void normalizeTest()
        {
            Assert.AreEqual("\"test\"", StringHandler.normalize("\\\"test\\\""));
        }

        /// <summary>
        ///A test for escapeQuotes
        ///</summary>
        [TestMethod()]
        public void escapeQuotesTest()
        {
            Assert.AreEqual(" \\\"test\\\" ", StringHandler.escapeQuotes(" \"test\" "));
        }

        /// <summary>
        ///A test for normalize
        ///</summary>
        [TestMethod()]
        public void normalizeTest2()
        {
            Assert.AreEqual(String.Empty, StringHandler.normalize(null));
        }

        /// <summary>
        ///A test for protect
        ///</summary>
        [TestMethod()]
        public void protectTest()
        {
            StringHandler target = new StringHandler();
            string actual = target.ProtectMixedQuotes("test \"str1\" - 'str2' data");
            Assert.AreEqual(false, Regex.IsMatch(actual, RPattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace));
            Assert.AreEqual(false, Regex.IsMatch(actual, RPattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace));
        }

        /// <summary>
        ///A test for recovery
        ///</summary>
        [TestMethod()]
        public void recoveryTest()
        {
            StringHandler target = new StringHandler();
            string str = target.ProtectMixedQuotes("test \"str1\" - 'str2' data");
            Assert.AreEqual("test \"str1\" - 'str2' data", target.Recovery(str));
        }
    }
}
