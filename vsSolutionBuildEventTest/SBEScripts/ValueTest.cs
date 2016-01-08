using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.SNode;

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
            Assert.AreEqual(true, Value.toBoolean("true"));
            Assert.AreEqual(true, Value.toBoolean("TRUE"));
            Assert.AreEqual(true, Value.toBoolean("True"));

            Assert.AreEqual(false, Value.toBoolean("0"));
            Assert.AreEqual(false, Value.toBoolean("false"));
            Assert.AreEqual(false, Value.toBoolean("FALSE"));
            Assert.AreEqual(false, Value.toBoolean("False"));

            Assert.AreEqual(true, Value.toBoolean(" 1 "));
            Assert.AreEqual(true, Value.toBoolean(" true "));
            Assert.AreEqual(false, Value.toBoolean(" 0 "));
            Assert.AreEqual(false, Value.toBoolean(" false "));
        }

        /// <summary>
        ///A test for toBoolean
        ///</summary>
        [TestMethod()]
        public void toBooleanTest2()
        {
            try {
                Value.toBoolean("TruE");
                Assert.Fail("1");
            }
            catch(IncorrectSyntaxException) {
                Assert.IsTrue(true);
            }

            try {
                Value.toBoolean("FalsE");
                Assert.Fail("2");
            }
            catch(IncorrectSyntaxException) {
                Assert.IsTrue(true);
            }

            try {
                Value.toBoolean("-true");
                Assert.Fail("3");
            }
            catch(IncorrectSyntaxException) {
                Assert.IsTrue(true);
            }

            try {
                Value.toBoolean("true.");
                Assert.Fail("4");
            }
            catch(IncorrectSyntaxException) {
                Assert.IsTrue(true);
            }
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
        
        /// <summary>
        ///A test for pack
        ///</summary>
        [TestMethod()]
        public void packTest1()
        {
            object data = (object)new object[] { "str", 123, -1.4, true, "str2", new object[] { 1.2f, "str2", false }, -24.574 };
            Assert.AreEqual("{\"str\", 123, -1.4, true, \"str2\", {1.2f, \"str2\", false}, -24.574}", Value.pack(data));
        }

        /// <summary>
        ///A test for pack
        ///</summary>
        [TestMethod()]
        public void packTest2()
        {
            object data = (object)new object[] { "str", 't', new object[] { 'a', 'b', 'c' }, true };
            Assert.AreEqual("{\"str\", 't', {'a', 'b', 'c'}, true}", Value.pack(data));
        }

        /// <summary>
        ///A test for pack
        ///</summary>
        [TestMethod()]
        public void packTest3()
        {
            Assert.AreEqual(" test ", Value.pack(" test "));
            Assert.AreEqual(String.Empty, Value.pack(String.Empty));
            Assert.AreEqual(null, Value.pack(null));
        }

        /// <summary>
        ///A test for pack and SNode.Arguments - Optional compatibility
        ///</summary>
        [TestMethod()]
        public void packTest4()
        {
            object data = (object)new object[] { "str", 123, -1.4, true, 'n', new object[] { 1.2f, "str2", 'y', false }, -24.574 };

            IPM pm = new PM(String.Format("left({0})", Value.pack(data)));
            
            Argument[] raw = (Argument[])pm.Levels[0].Args;
            Assert.AreEqual(raw.Length, 1);

            Assert.AreEqual(raw[0].type, ArgumentType.Object);
            {
                Argument[] args = (Argument[])raw[0].data;
                Assert.AreEqual(args.Length, 7);

                Assert.AreEqual(args[0].type, ArgumentType.StringDouble);
                Assert.AreEqual(args[0].data, "str");

                Assert.AreEqual(args[1].type, ArgumentType.Integer);
                Assert.AreEqual(args[1].data, 123);

                Assert.AreEqual(args[2].type, ArgumentType.Double);
                Assert.AreEqual(args[2].data, -1.4);

                Assert.AreEqual(args[3].type, ArgumentType.Boolean);
                Assert.AreEqual(args[3].data, true);

                Assert.AreEqual(args[4].type, ArgumentType.Char);
                Assert.AreEqual(args[4].data, 'n');

                Assert.AreEqual(args[5].type, ArgumentType.Object);
                {
                    Argument[] args5 = (Argument[])args[5].data;
                    Assert.AreEqual(args5.Length, 4);

                    Assert.AreEqual(args5[0].type, ArgumentType.Float);
                    Assert.AreEqual(args5[0].data, 1.2f);

                    Assert.AreEqual(args5[1].type, ArgumentType.StringDouble);
                    Assert.AreEqual(args5[1].data, "str2");

                    Assert.AreEqual(args5[2].type, ArgumentType.Char);
                    Assert.AreEqual(args5[2].data, 'y');

                    Assert.AreEqual(args5[3].type, ArgumentType.Boolean);
                    Assert.AreEqual(args5[3].data, false);
                }

                Assert.AreEqual(args[6].type, ArgumentType.Double);
                Assert.AreEqual(args[6].data, -24.574);
            }            
        }

        /// <summary>
        ///A test for pack and SNode.Arguments /Float and Double - Optional compatibility
        ///</summary>
        [TestMethod()]
        public void packTest5()
        {
            object data = (object)new object[] { 1.4, 1.4f, 1.4d, -1.4, -1.4f, -1.4d };

            IPM pm = new PM(String.Format("left({0})", Value.pack(data)));

            Argument[] args = (Argument[])pm.Levels[0].Args[0].data;
            Assert.AreEqual(args.Length, 6);
            
            Assert.AreEqual(args[0].type, ArgumentType.Double);
            Assert.AreEqual(args[0].data, 1.4d);

            Assert.AreEqual(args[1].type, ArgumentType.Float);
            Assert.AreEqual(args[1].data, 1.4f);

            Assert.AreEqual(args[2].type, ArgumentType.Double);
            Assert.AreEqual(args[2].data, 1.4d);

            Assert.AreEqual(args[3].type, ArgumentType.Double);
            Assert.AreEqual(args[3].data, -1.4d);

            Assert.AreEqual(args[4].type, ArgumentType.Float);
            Assert.AreEqual(args[4].data, -1.4f);

            Assert.AreEqual(args[5].type, ArgumentType.Double);
            Assert.AreEqual(args[5].data, -1.4d);
        }

        /// <summary>
        ///A test for pack - nested
        ///</summary>
        [TestMethod()]
        public void packTest6()
        {
            object data = (object)new object[] { "str", new object[] { 1, 'y', new object[] { -12.457f, new object[] { 12 } } }, true };
            Assert.AreEqual("{\"str\", {1, 'y', {-12.457f, {12}}}, true}", Value.pack(data));
        }
    }
}
