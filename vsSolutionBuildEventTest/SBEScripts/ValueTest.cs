using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;

namespace net.r_eg.vsSBE.Test.SBEScripts
{
    /// <summary>
    ///This is a test class for ValuesTest and is intended
    ///to contain all ValuesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ValueTest
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
            Assert.AreEqual(true, Value.toBoolean("1"));
            Assert.AreEqual(true, Value.toBoolean(" 1 "));
            Assert.AreEqual(true, Value.toBoolean("true"));
            Assert.AreEqual(true, Value.toBoolean(" true "));
            Assert.AreEqual(true, Value.toBoolean("TruE"));
            Assert.AreEqual(true, Value.toBoolean("TRUE"));
            Assert.AreEqual(false, Value.toBoolean("0"));
            Assert.AreEqual(false, Value.toBoolean(" 0 "));
            Assert.AreEqual(false, Value.toBoolean("false"));
            Assert.AreEqual(false, Value.toBoolean(" false "));
            Assert.AreEqual(false, Value.toBoolean("FalsE"));
            Assert.AreEqual(false, Value.toBoolean("FALSE"));
        }

        /// <summary>
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void toBooleanTest2()
        {
            Value.toBoolean("-true");
        }

        /// <summary>
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void toBooleanTest3()
        {
            Value.toBoolean("");
        }

        /// <summary>
        ///A test for from(bool)
        ///</summary>
        [TestMethod()]
        public void fromTest()
        {
            Assert.AreEqual("false", Value.from(false));
            Assert.AreEqual("true", Value.from(true));
        }

        /// <summary>
        ///A test for cmp ===
        ///</summary>
        [TestMethod()]
        public void cmpTest()
        {
            Assert.AreEqual(true, Value.cmp(" test", " test", "==="));
            Assert.AreEqual(true, Value.cmp(" tESt ", " tESt ", "==="));
            Assert.AreEqual(true, Value.cmp("", "", "==="));
            Assert.AreEqual(true, Value.cmp("12", "12", "==="));
            Assert.AreEqual(false, Value.cmp("test", " test", "==="));
            Assert.AreEqual(false, Value.cmp("test ", " test", "==="));
            Assert.AreEqual(false, Value.cmp("tESt", " test", "==="));
            Assert.AreEqual(false, Value.cmp(" ", "", "==="));
            Assert.AreEqual(false, Value.cmp("12", "12 ", "==="));
        }

        /// <summary>
        ///A test for cmp !==
        ///</summary>
        [TestMethod()]
        public void cmpTest2()
        {
            Assert.AreEqual(false, Value.cmp(" test", " test", "!=="));
            Assert.AreEqual(false, Value.cmp(" tESt ", " tESt ", "!=="));
            Assert.AreEqual(false, Value.cmp("", "", "!=="));
            Assert.AreEqual(false, Value.cmp("12", "12", "!=="));
            Assert.AreEqual(true, Value.cmp("test", " test", "!=="));
            Assert.AreEqual(true, Value.cmp("test ", " test", "!=="));
            Assert.AreEqual(true, Value.cmp("tESt", " test", "!=="));
            Assert.AreEqual(true, Value.cmp(" ", "", "!=="));
            Assert.AreEqual(true, Value.cmp("12", "12 ", "!=="));
        }

        /// <summary>
        ///A test for cmp ~=
        ///</summary>
        [TestMethod()]
        public void cmpTest3()
        {
            Assert.AreEqual(true, Value.cmp("test-12M-word", "12M", "~="));
            Assert.AreEqual(true, Value.cmp("", "", "~="));
            Assert.AreEqual(true, Value.cmp("test ", " ", "~="));
            Assert.AreEqual(true, Value.cmp(" ", "", "~="));
            Assert.AreEqual(true, Value.cmp("123", "2", "~="));
            Assert.AreEqual(false, Value.cmp("test-12M-word", "12m", "~="));
            Assert.AreEqual(false, Value.cmp("test-12M-word", " ", "~="));
            Assert.AreEqual(false, Value.cmp("test-12M-word", "0", "~="));
        }

        /// <summary>
        ///A test for cmp ==
        ///</summary>
        [TestMethod()]
        public void cmpTest4()
        {
            Assert.AreEqual(true, Value.cmp(" test", " test", "=="));
            Assert.AreEqual(true, Value.cmp(" tESt ", " tESt ", "=="));
            Assert.AreEqual(true, Value.cmp("", "", "=="));
            Assert.AreEqual(true, Value.cmp("12", "12", "=="));
            Assert.AreEqual(true, Value.cmp(" 12", "12 ", "=="));
            Assert.AreEqual(true, Value.cmp("00012", "12", "=="));
            Assert.AreEqual(true, Value.cmp("00012", "012", "=="));
            Assert.AreEqual(false, Value.cmp("test", " test", "=="));
            Assert.AreEqual(false, Value.cmp("test ", " test", "=="));
            Assert.AreEqual(false, Value.cmp("tESt", " test", "=="));
            Assert.AreEqual(false, Value.cmp(" ", "", "=="));
            Assert.AreEqual(false, Value.cmp("120", "12 ", "=="));
            Assert.AreEqual(true, Value.cmp("true", "1", "=="));
            Assert.AreEqual(true, Value.cmp("false", "0", "=="));
        }

        /// <summary>
        ///A test for cmp !=
        ///</summary>
        [TestMethod()]
        public void cmpTest5()
        {
            Assert.AreEqual(false, Value.cmp(" test", " test", "!="));
            Assert.AreEqual(false, Value.cmp(" tESt ", " tESt ", "!="));
            Assert.AreEqual(false, Value.cmp("", "", "!="));
            Assert.AreEqual(false, Value.cmp("12", "12", "!="));
            Assert.AreEqual(false, Value.cmp(" 12", "12 ", "!="));
            Assert.AreEqual(false, Value.cmp("00012", "12", "!="));
            Assert.AreEqual(false, Value.cmp("00012", "012", "!="));
            Assert.AreEqual(true, Value.cmp("test", " test", "!="));
            Assert.AreEqual(true, Value.cmp("test ", " test", "!="));
            Assert.AreEqual(true, Value.cmp("tESt", " test", "!="));
            Assert.AreEqual(true, Value.cmp(" ", "", "!="));
            Assert.AreEqual(true, Value.cmp("120", "12 ", "!="));
            Assert.AreEqual(false, Value.cmp("true", "1", "!="));
            Assert.AreEqual(false, Value.cmp("false", "0", "!="));
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest6()
        {
            Value.cmp(" test", " test", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest7()
        {
            Value.cmp("", "", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest8()
        {
            Value.cmp("t2", "1", ">");
        }

        /// <summary>
        ///A test for cmp >
        ///</summary>
        [TestMethod()]
        public void cmpTest9()
        {
            Assert.AreEqual(true, Value.cmp("2", "1", ">"));
            Assert.AreEqual(true, Value.cmp("2", "01", ">"));
            Assert.AreEqual(false, Value.cmp("1", "2", ">"));
            Assert.AreEqual(false, Value.cmp("2", "2", ">"));
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest10()
        {
            Value.cmp(" test", " test", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest11()
        {
            Value.cmp("", "", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest12()
        {
            Value.cmp("t2", "1", ">=");
        }

        /// <summary>
        ///A test for cmp >=
        ///</summary>
        [TestMethod()]
        public void cmpTest13()
        {
            Assert.AreEqual(true, Value.cmp("2", "1", ">="));
            Assert.AreEqual(true, Value.cmp("2", "01", ">="));
            Assert.AreEqual(true, Value.cmp("2", "2", ">="));
            Assert.AreEqual(true, Value.cmp("2", "002", ">="));
            Assert.AreEqual(false, Value.cmp("1", "2", ">="));
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest14()
        {
            Value.cmp(" test", " test", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest15()
        {
            Value.cmp("", "", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest16()
        {
            Value.cmp("t2", "1", "<");
        }

        /// <summary>
        ///A test for cmp <
        ///</summary>
        [TestMethod()]
        public void cmpTest17()
        {
            Assert.AreEqual(false, Value.cmp("2", "1", "<"));
            Assert.AreEqual(false, Value.cmp("2", "01", "<"));
            Assert.AreEqual(false, Value.cmp("2", "2", "<"));
            Assert.AreEqual(true, Value.cmp("1", "2", "<"));
            Assert.AreEqual(true, Value.cmp("001", "2", "<"));
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest18()
        {
            Value.cmp(" test", " test", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest19()
        {
            Value.cmp("", "", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void cmpTest20()
        {
            Value.cmp("t2", "1", "<=");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        public void cmpTest21()
        {
            Assert.AreEqual(false, Value.cmp("2", "1", "<="));
            Assert.AreEqual(false, Value.cmp("2", "01", "<="));
            Assert.AreEqual(true, Value.cmp("2", "002", "<="));
            Assert.AreEqual(true, Value.cmp("1", "2", "<="));
            Assert.AreEqual(true, Value.cmp("2", "2", "<="));
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest22()
        {
            Value.cmp("2", "1", "test");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest23()
        {
            Value.cmp("2", "1", "");
        }

        /// <summary>
        ///A test for cmp <=
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void cmpTest24()
        {
            Value.cmp("2", "1", "> =");
        }
    }
}
