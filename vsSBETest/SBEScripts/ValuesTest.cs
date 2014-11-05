using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;

namespace vsSBETest.SBEScripts
{
    /// <summary>
    ///This is a test class for ValuesTest and is intended
    ///to contain all ValuesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ValuesTest
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
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        public void toBooleanTest()
        {
            Assert.AreEqual(true, Values.toBoolean("1"));
            Assert.AreEqual(true, Values.toBoolean(" 1 "));
            Assert.AreEqual(true, Values.toBoolean("true"));
            Assert.AreEqual(true, Values.toBoolean(" true "));
            Assert.AreEqual(true, Values.toBoolean("TruE"));
            Assert.AreEqual(true, Values.toBoolean("TRUE"));
            Assert.AreEqual(false, Values.toBoolean("0"));
            Assert.AreEqual(false, Values.toBoolean(" 0 "));
            Assert.AreEqual(false, Values.toBoolean("false"));
            Assert.AreEqual(false, Values.toBoolean(" false "));
            Assert.AreEqual(false, Values.toBoolean("FalsE"));
            Assert.AreEqual(false, Values.toBoolean("FALSE"));
        }

        /// <summary>
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void toBooleanTest2()
        {
            Values.toBoolean("-true");
        }

        /// <summary>
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void toBooleanTest3()
        {
            Values.toBoolean("");
        }

        /// <summary>
        ///A test for from(bool)
        ///</summary>
        [TestMethod()]
        public void fromTest()
        {
            Assert.AreEqual("false", Values.from(false));
            Assert.AreEqual("true", Values.from(true));
        }

        /// <summary>
        ///A test for cmp ===
        ///</summary>
        [TestMethod()]
        public void cmpTest()
        {
            Assert.AreEqual(true, Values.cmp(" test", " test", "==="));
            Assert.AreEqual(true, Values.cmp(" tESt ", " tESt ", "==="));
            Assert.AreEqual(true, Values.cmp("", "", "==="));
            Assert.AreEqual(true, Values.cmp("12", "12", "==="));
            Assert.AreEqual(false, Values.cmp("test", " test", "==="));
            Assert.AreEqual(false, Values.cmp("test ", " test", "==="));
            Assert.AreEqual(false, Values.cmp("tESt", " test", "==="));
            Assert.AreEqual(false, Values.cmp(" ", "", "==="));
            Assert.AreEqual(false, Values.cmp("12", "12 ", "==="));
        }

        /// <summary>
        ///A test for cmp !==
        ///</summary>
        [TestMethod()]
        public void cmpTest2()
        {
            Assert.AreEqual(false, Values.cmp(" test", " test", "!=="));
            Assert.AreEqual(false, Values.cmp(" tESt ", " tESt ", "!=="));
            Assert.AreEqual(false, Values.cmp("", "", "!=="));
            Assert.AreEqual(false, Values.cmp("12", "12", "!=="));
            Assert.AreEqual(true, Values.cmp("test", " test", "!=="));
            Assert.AreEqual(true, Values.cmp("test ", " test", "!=="));
            Assert.AreEqual(true, Values.cmp("tESt", " test", "!=="));
            Assert.AreEqual(true, Values.cmp(" ", "", "!=="));
            Assert.AreEqual(true, Values.cmp("12", "12 ", "!=="));
        }

        /// <summary>
        ///A test for cmp ~=
        ///</summary>
        [TestMethod()]
        public void cmpTest3()
        {
            Assert.AreEqual(true, Values.cmp("test-12M-word", "12M", "~="));
            Assert.AreEqual(true, Values.cmp("", "", "~="));
            Assert.AreEqual(true, Values.cmp("test ", " ", "~="));
            Assert.AreEqual(true, Values.cmp(" ", "", "~="));
            Assert.AreEqual(true, Values.cmp("123", "2", "~="));
            Assert.AreEqual(false, Values.cmp("test-12M-word", "12m", "~="));
            Assert.AreEqual(false, Values.cmp("test-12M-word", " ", "~="));
            Assert.AreEqual(false, Values.cmp("test-12M-word", "0", "~="));
        }

        /// <summary>
        ///A test for cmp ==
        ///</summary>
        [TestMethod()]
        public void cmpTest4()
        {
            Assert.AreEqual(true, Values.cmp(" test", " test", "=="));
            Assert.AreEqual(true, Values.cmp(" tESt ", " tESt ", "=="));
            Assert.AreEqual(true, Values.cmp("", "", "=="));
            Assert.AreEqual(true, Values.cmp("12", "12", "=="));
            Assert.AreEqual(true, Values.cmp(" 12", "12 ", "=="));
            Assert.AreEqual(true, Values.cmp("00012", "12", "=="));
            Assert.AreEqual(true, Values.cmp("00012", "012", "=="));
            Assert.AreEqual(false, Values.cmp("test", " test", "=="));
            Assert.AreEqual(false, Values.cmp("test ", " test", "=="));
            Assert.AreEqual(false, Values.cmp("tESt", " test", "=="));
            Assert.AreEqual(false, Values.cmp(" ", "", "=="));
            Assert.AreEqual(false, Values.cmp("120", "12 ", "=="));
            Assert.AreEqual(true, Values.cmp("true", "1", "=="));
            Assert.AreEqual(true, Values.cmp("false", "0", "=="));
        }

        /// <summary>
        ///A test for cmp !=
        ///</summary>
        [TestMethod()]
        public void cmpTest5()
        {
            Assert.AreEqual(false, Values.cmp(" test", " test", "!="));
            Assert.AreEqual(false, Values.cmp(" tESt ", " tESt ", "!="));
            Assert.AreEqual(false, Values.cmp("", "", "!="));
            Assert.AreEqual(false, Values.cmp("12", "12", "!="));
            Assert.AreEqual(false, Values.cmp(" 12", "12 ", "!="));
            Assert.AreEqual(false, Values.cmp("00012", "12", "!="));
            Assert.AreEqual(false, Values.cmp("00012", "012", "!="));
            Assert.AreEqual(true, Values.cmp("test", " test", "!="));
            Assert.AreEqual(true, Values.cmp("test ", " test", "!="));
            Assert.AreEqual(true, Values.cmp("tESt", " test", "!="));
            Assert.AreEqual(true, Values.cmp(" ", "", "!="));
            Assert.AreEqual(true, Values.cmp("120", "12 ", "!="));
            Assert.AreEqual(false, Values.cmp("true", "1", "!="));
            Assert.AreEqual(false, Values.cmp("false", "0", "!="));
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest6()
        {
            Values.cmp(" test", " test", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest7()
        {
            Values.cmp("", "", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest8()
        {
            Values.cmp("t2", "1", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        public void cmpTest9()
        {
            Assert.AreEqual(true, Values.cmp("2", "1", ">"));
            Assert.AreEqual(true, Values.cmp("2", "01", ">"));
            Assert.AreEqual(false, Values.cmp("1", "2", ">"));
            Assert.AreEqual(false, Values.cmp("2", "2", ">"));
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest10()
        {
            Values.cmp(" test", " test", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest11()
        {
            Values.cmp("", "", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest12()
        {
            Values.cmp("t2", "1", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        public void cmpTest13()
        {
            Assert.AreEqual(true, Values.cmp("2", "1", ">="));
            Assert.AreEqual(true, Values.cmp("2", "01", ">="));
            Assert.AreEqual(true, Values.cmp("2", "2", ">="));
            Assert.AreEqual(true, Values.cmp("2", "002", ">="));
            Assert.AreEqual(false, Values.cmp("1", "2", ">="));
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest14()
        {
            Values.cmp(" test", " test", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest15()
        {
            Values.cmp("", "", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest16()
        {
            Values.cmp("t2", "1", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        public void cmpTest17()
        {
            Assert.AreEqual(false, Values.cmp("2", "1", "<"));
            Assert.AreEqual(false, Values.cmp("2", "01", "<"));
            Assert.AreEqual(false, Values.cmp("2", "2", "<"));
            Assert.AreEqual(true, Values.cmp("1", "2", "<"));
            Assert.AreEqual(true, Values.cmp("001", "2", "<"));
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest18()
        {
            Values.cmp(" test", " test", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest19()
        {
            Values.cmp("", "", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest20()
        {
            Values.cmp("t2", "1", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        public void cmpTest21()
        {
            Assert.AreEqual(false, Values.cmp("2", "1", "<="));
            Assert.AreEqual(false, Values.cmp("2", "01", "<="));
            Assert.AreEqual(true, Values.cmp("2", "002", "<="));
            Assert.AreEqual(true, Values.cmp("1", "2", "<="));
            Assert.AreEqual(true, Values.cmp("2", "2", "<="));
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest22()
        {
            Values.cmp("2", "1", "test");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest23()
        {
            Values.cmp("2", "1", "");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest24()
        {
            Values.cmp("2", "1", "> =");
        }
    }
}
