using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.SBEScripts;

namespace vsSBETest.SBEScripts
{
    /// <summary>
    ///This is a test class for RPatternTest and is intended
    ///to contain all RPatternTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RPatternTest
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
        ///A test for SquareBracketsContent
        ///</summary>
        [TestMethod()]
        public void SquareBracketsContentTest()
        {
            string data = " #[var name] ";
            Match actual = Regex.Match(data, RPattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual("var name", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for SquareBracketsContent
        ///</summary>
        [TestMethod()]
        public void SquareBracketsContentTest2()
        {
            string data = " [ test [name [ data]  ]] ";
            Match actual = Regex.Match(data, RPattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test [name [ data]  ]", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for SquareBracketsContent
        ///</summary>
        [TestMethod()]
        public void SquareBracketsContentTest3()
        {
            string data = " [ test name [ data]  p]] ";
            Match actual = Regex.Match(data, RPattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test name [ data]  p", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for SquareBracketsContent
        ///</summary>
        [TestMethod()]
        public void SquareBracketsContentTest4()
        {
            string data = " data] [test ";
            bool actual = Regex.IsMatch(data, RPattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for SquareBracketsContent
        ///</summary>
        [TestMethod()]
        public void SquareBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, RPattern.SquareBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for RoundBracketsContent
        ///</summary>
        [TestMethod()]
        public void RoundBracketsContentTest()
        {
            string data = " $(var name) ";
            Match actual = Regex.Match(data, RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual("var name", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for RoundBracketsContent
        ///</summary>
        [TestMethod()]
        public void RoundBracketsContentTest2()
        {
            string data = " ( test (name ( data)  )) ";
            Match actual = Regex.Match(data, RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test (name ( data)  )", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for RoundBracketsContent
        ///</summary>
        [TestMethod()]
        public void RoundBracketsContentTest3()
        {
            string data = " ( test name ( data)  p)) ";
            Match actual = Regex.Match(data, RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test name ( data)  p", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for RoundBracketsContent
        ///</summary>
        [TestMethod()]
        public void RoundBracketsContentTest4()
        {
            string data = " data) (test ";
            bool actual = Regex.IsMatch(data, RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for RoundBracketsContent
        ///</summary>
        [TestMethod()]
        public void RoundBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, RPattern.RoundBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for CurlyBracketsContent
        ///</summary>
        [TestMethod()]
        public void CurlyBracketsContentTest()
        {
            string data = " { body1 } ";
            Match actual = Regex.Match(data, RPattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" body1 ", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for CurlyBracketsContent
        ///</summary>
        [TestMethod()]
        public void CurlyBracketsContentTest2()
        {
            string data = " { test {name { data}  }} ";
            Match actual = Regex.Match(data, RPattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test {name { data}  }", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for CurlyBracketsContent
        ///</summary>
        [TestMethod()]
        public void CurlyBracketsContentTest3()
        {
            string data = " { test name { data}  p}} ";
            Match actual = Regex.Match(data, RPattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual(" test name { data}  p", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for CurlyBracketsContent
        ///</summary>
        [TestMethod()]
        public void CurlyBracketsContentTest4()
        {
            string data = " data} {test ";
            bool actual = Regex.IsMatch(data, RPattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for CurlyBracketsContent
        ///</summary>
        [TestMethod()]
        public void CurlyBracketsContentTest5()
        {
            string data = " data ";
            bool actual = Regex.IsMatch(data, RPattern.CurlyBracketsContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for DoubleQuotesContent
        ///</summary>
        [TestMethod()]
        public void DoubleQuotesContentTest()
        {
            string data = " test \"123\" ";
            Match actual = Regex.Match(data, RPattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual("123", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for DoubleQuotesContent
        ///</summary>
        [TestMethod()]
        public void DoubleQuotesContentTest2()
        {
            string data = " test \\\"123\\\" "; // \"data\"
            bool actual = Regex.IsMatch(data, RPattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for DoubleQuotesContent
        ///</summary>
        [TestMethod()]
        public void DoubleQuotesContentTest3()
        {
            string data = " test 123\" ";
            Assert.AreEqual(false, Regex.IsMatch(data, RPattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace));
        }

        /// <summary>
        ///A test for SingleQuotesContent
        ///</summary>
        [TestMethod()]
        public void SingleQuotesContentTest()
        {
            string data = " test '123' ";
            Match actual = Regex.Match(data, RPattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(true, actual.Success);
            Assert.AreEqual("123", actual.Groups[1].Value);
        }

        /// <summary>
        ///A test for SingleQuotesContent
        ///</summary>
        [TestMethod()]
        public void SingleQuotesContentTest2()
        {
            string data = " test \\'123\\' "; // \'data\'
            bool actual = Regex.IsMatch(data, RPattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for SingleQuotesContent
        ///</summary>
        [TestMethod()]
        public void SingleQuotesContentTest3()
        {
            string data = " test 123' ";
            bool actual = Regex.IsMatch(data, RPattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace);
            Assert.AreEqual(false, actual);
        }
    }
}
