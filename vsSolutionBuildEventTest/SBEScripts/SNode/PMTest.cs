using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Exceptions;
using net.r_eg.vsSBE.SBEScripts.SNode;

namespace net.r_eg.vsSBE.Test.SBEScripts.SNode
{
    /// <summary>
    ///This is a test class for PMTest and is intended
    ///to contain all PMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PMTest
    {
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
        private TestContext testContextInstance;

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest1()
        {
            IPM pm = new PM("solution.path(\"sln file\").Last");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Data, "path");
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, "sln file");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[2].Data, "Last");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandEmpty);
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest2()
        {
            IPM pm = new PM("solution.current.FirstBy(true, 0, \"raw\")");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[1].Data, "current");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[2].Data, "FirstBy");
            Assert.AreEqual(pm.Levels[2].Args[0].type, ArgumentType.Boolean);
            Assert.AreEqual(pm.Levels[2].Args[0].data, true);
            Assert.AreEqual(pm.Levels[2].Args[1].type, ArgumentType.Integer);
            Assert.AreEqual(pm.Levels[2].Args[1].data, 0);
            Assert.AreEqual(pm.Levels[2].Args[2].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[2].Args[2].data, "raw");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandEmpty);
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest3()
        {
            IPM pm = new PM(".solution");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandEmpty);
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void detectTest4()
        {
            IPM pm = new PM(".solution.");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest5()
        {
            IPM pm = new PM(". solution . data = true");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");
            Assert.AreEqual(pm.Levels[1].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[1].Data, "data");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.RightOperandStd);
            Assert.AreEqual(pm.Levels[2].Data, " true");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest6()
        {
            IPM pm = new PM("solution.data : 123 ");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");
            Assert.AreEqual(pm.Levels[1].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[1].Data, "data");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[2].Data, " 123 ");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest7()
        {
            IPM pm = new PM("solution = ");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandStd);
            Assert.AreEqual(pm.Levels[1].Data, " ");

            pm = new PM("solution =");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandStd);
            Assert.AreEqual(pm.Levels[1].Data, "");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest8()
        {
            IPM pm = new PM("solution() : ");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[0].Args.Length, 0);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[1].Data, " ");

            pm = new PM("solution(123) :456");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[0].Args[0].type, ArgumentType.Integer);
            Assert.AreEqual(pm.Levels[0].Args[0].data, 123);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[1].Data, "456");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest9()
        {
            IPM pm = new PM("left.solution(\" test , args\", 123, true, ' n1 , n2 ').right");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "left");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Args.Length, 4);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, " test , args");
            Assert.AreEqual(pm.Levels[1].Args[1].type, ArgumentType.Integer);
            Assert.AreEqual(pm.Levels[1].Args[1].data, 123);
            Assert.AreEqual(pm.Levels[1].Args[2].type, ArgumentType.Boolean);
            Assert.AreEqual(pm.Levels[1].Args[2].data, true);
            Assert.AreEqual(pm.Levels[1].Args[3].type, ArgumentType.StringSingle);
            Assert.AreEqual(pm.Levels[1].Args[3].data, " n1 , n2 ");
            Assert.AreEqual(pm.Levels[1].Data, "solution");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[2].Data, "right");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandEmpty);
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest10()
        {
            try {
                IPM pm = new PM("solution(123, ).right");
                Assert.Fail("1");
            }
            catch(SyntaxIncorrectException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("solution(, 123).right");
                Assert.Fail("2");
            }
            catch(SyntaxIncorrectException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest11()
        {
            IPM pm = new PM("solution=left.right");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandStd);
            Assert.AreEqual(pm.Levels[1].Data, "left.right");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest12()
        {
            IPM pm = new PM("solution.path(\"D:/app/name.sln\").projectBy(\"4262A1DC\")");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Data, "path");
            Assert.AreEqual(pm.Levels[1].Args.Length, 1);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, "D:/app/name.sln");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[2].Data, "projectBy");
            Assert.AreEqual(pm.Levels[2].Args.Length, 1);
            Assert.AreEqual(pm.Levels[2].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[2].Args[0].data, "4262A1DC");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest13()
        {
            IPM pm = new PM("solution.path(\"D:/app/name.sln\", \"test\").projectBy(\"4262A1DC\")");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Data, "path");
            Assert.AreEqual(pm.Levels[1].Args.Length, 2);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, "D:/app/name.sln");
            Assert.AreEqual(pm.Levels[1].Args[1].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[1].data, "test");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[2].Data, "projectBy");
            Assert.AreEqual(pm.Levels[2].Args.Length, 1);
            Assert.AreEqual(pm.Levels[2].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[2].Args[0].data, "4262A1DC");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest14()
        {
            IPM pm = new PM("left.solution(\" test , (args)\", 123, true, ' (n1) , n2) ').right");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "left");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Args.Length, 4);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, " test , (args)");
            Assert.AreEqual(pm.Levels[1].Args[1].type, ArgumentType.Integer);
            Assert.AreEqual(pm.Levels[1].Args[1].data, 123);
            Assert.AreEqual(pm.Levels[1].Args[2].type, ArgumentType.Boolean);
            Assert.AreEqual(pm.Levels[1].Args[2].data, true);
            Assert.AreEqual(pm.Levels[1].Args[3].type, ArgumentType.StringSingle);
            Assert.AreEqual(pm.Levels[1].Args[3].data, " (n1) , n2) ");
            Assert.AreEqual(pm.Levels[1].Data, "solution");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[2].Data, "right");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandEmpty);
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest15()
        {
            IPM pm = new PM("solution.path(\"D:/app/(name)).sln\", \"test\").projectBy(\"426(2)A1DC\") : \"test 123\", 'true'");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Data, "path");
            Assert.AreEqual(pm.Levels[1].Args.Length, 2);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, "D:/app/(name)).sln");
            Assert.AreEqual(pm.Levels[1].Args[1].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[1].data, "test");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[2].Data, "projectBy");
            Assert.AreEqual(pm.Levels[2].Args.Length, 1);
            Assert.AreEqual(pm.Levels[2].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[2].Args[0].data, "426(2)A1DC");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[3].Data, " \"test 123\", 'true'");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest16()
        {
            IPM pm = new PM("solution = \"te()st 123\"), 'true'");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandStd);
            Assert.AreEqual(pm.Levels[1].Data, " \"te()st 123\"), 'true'");

            pm = new PM("solution : \"te()st 123\"), 'true'");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[1].Data, " \"te()st 123\"), 'true'");
        }

        /// <summary>
        ///A test for detect
        ///</summary>
        [TestMethod()]
        public void detectTest17()
        {
            IPM pm = new PM("solution.path(\"\\\"D:/app/(name)).sln\\\"\", \"\\\"test\\\"\", '\\\"12 34\\\"').projectBy(\"\\\"426(2)A1DC\\\"\") : \"\\\"test 123\\\"\", '\\\"true\\\"'");

            Assert.AreEqual(pm.Levels[0].Type, LevelType.Property);
            Assert.AreEqual(pm.Levels[0].Data, "solution");

            Assert.AreEqual(pm.Levels[1].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[1].Data, "path");
            Assert.AreEqual(pm.Levels[1].Args.Length, 3);
            Assert.AreEqual(pm.Levels[1].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[0].data, "\\\"D:/app/(name)).sln\\\"");
            Assert.AreEqual(pm.Levels[1].Args[1].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[1].Args[1].data, "\\\"test\\\"");
            Assert.AreEqual(pm.Levels[1].Args[2].type, ArgumentType.StringSingle);
            Assert.AreEqual(pm.Levels[1].Args[2].data, "\\\"12 34\\\"");

            Assert.AreEqual(pm.Levels[2].Type, LevelType.Method);
            Assert.AreEqual(pm.Levels[2].Data, "projectBy");
            Assert.AreEqual(pm.Levels[2].Args.Length, 1);
            Assert.AreEqual(pm.Levels[2].Args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(pm.Levels[2].Args[0].data, "\\\"426(2)A1DC\\\"");

            Assert.AreEqual(pm.Levels[3].Type, LevelType.RightOperandColon);
            Assert.AreEqual(pm.Levels[3].Data, " \"\\\"test 123\\\"\", '\\\"true\\\"'");
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest1()
        {
            IPM pm = new PM("solution=left.right");

            Assert.AreEqual(pm.Is(100, LevelType.Property, "solution"), false);
            Assert.AreEqual(pm.Is(-1, LevelType.Property, "solution"), false);
            Assert.AreEqual(pm.Is(0, LevelType.Property, "solution"), true);
            Assert.AreEqual(pm.Is(1, LevelType.RightOperandStd, "left.right"), true);
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest2()
        {
            IPM pm = new PM("solution.m1().m2().right");

            Assert.AreEqual(pm.Is(0, LevelType.Property, "solution"), true);
            Assert.AreEqual(pm.Is(1, LevelType.Method, "m1"), true);
            Assert.AreEqual(pm.Is(2, LevelType.Method, "m2"), true);
            Assert.AreEqual(pm.Is(3, LevelType.Property, "right"), true);
            Assert.AreEqual(pm.Is(4, LevelType.RightOperandEmpty), true);
        }

        /// <summary>
        ///A test for Is
        ///</summary>
        [TestMethod()]
        public void IsTest3()
        {
            IPM pm = new PM("solution.m1(')', '(', \"(\").m2(123, \" -> )\").right");

            Assert.AreEqual(pm.Is(0, LevelType.Property, "solution"), true);
            Assert.AreEqual(pm.Is(1, LevelType.Method, "m1"), true);
            Assert.AreEqual(pm.Is(2, LevelType.Method, "m2"), true);
            Assert.AreEqual(pm.Is(3, LevelType.Property, "right"), true);
            Assert.AreEqual(pm.Is(4, LevelType.RightOperandEmpty), true);
        }

        /// <summary>
        ///A test for FinalIs
        ///</summary>
        [TestMethod()]
        public void FinalIsTest1()
        {
            IPM pm = new PM("left.solution.right");
            Assert.AreEqual(pm.Levels.Count, 4);
            Assert.AreEqual(pm.FinalIs(2, LevelType.Property, "right"), true);
            Assert.AreEqual(pm.FinalIs(3, LevelType.RightOperandEmpty), true);
        }

        /// <summary>
        ///A test for FinalIs
        ///</summary>
        [TestMethod()]
        public void FinalIsTest2()
        {
            try {
                IPM pm = new PM("left.solution.right");
                Assert.AreEqual(pm.FinalIs(1, LevelType.Property, "solution"), true);
                Assert.Fail("1");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("left.solution.right");
                Assert.AreEqual(pm.FinalIs(0, LevelType.Property, "left"), true);
                Assert.Fail("2");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for FinalIs
        ///</summary>
        [TestMethod()]
        public void FinalIsTest3()
        {
            IPM pm = new PM("left.solution.right");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "right"), false);
            Assert.AreEqual(pm.FinalIs(1, LevelType.Property, "right"), false);
            Assert.AreEqual(pm.Is(2, LevelType.Property, "right"), true);
        }

        /// <summary>
        ///A test for FinalIs
        ///</summary>
        [TestMethod()]
        public void FinalIsTest4()
        {
            IPM pm = new PM("left.solution().right");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "left"), true);
            Assert.AreEqual(pm.Is(1, LevelType.Method, "solution"), true);
            Assert.AreEqual(pm.FinalIs(2, LevelType.Property, "right"), true);
        }

        /// <summary>
        ///A test for FinalIs
        ///</summary>
        [TestMethod()]
        public void FinalIsTest5()
        {
            IPM pm = new PM("left.solution(\" (a, b) \").right");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "left"), true);
            Assert.AreEqual(pm.Is(1, LevelType.Method, "solution"), true);
            Assert.AreEqual(pm.FinalIs(2, LevelType.Property, "right"), true);
        }

        /// <summary>
        ///A test for FinalEmptyIs
        ///</summary>
        [TestMethod()]
        public void FinalEmptyIsTest1()
        {
            IPM pm = new PM("solution.right ");
            Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), true);
            Assert.AreEqual(pm.FinalEmptyIs(2, LevelType.RightOperandEmpty), true);
        }

        /// <summary>
        ///A test for FinalEmptyIs
        ///</summary>
        [TestMethod()]
        public void FinalEmptyIsTest2()
        {
            try {
                IPM pm = new PM("solution.right = ");
                Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), true);
                Assert.Fail("1");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("solution.right : ");
                Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), true);
                Assert.Fail("2");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("solution.right . prop");
                Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), true);
                Assert.Fail("3");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("solution.right mixed data");
                Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), true);
                Assert.Fail("4");
            }
            catch(NotSupportedOperationException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for FinalEmptyIs
        ///</summary>
        [TestMethod()]
        public void FinalEmptyIsTest3()
        {
            IPM pm = new PM("left.solution.right");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "right"), false);
            Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Property, "right"), false);
            Assert.AreEqual(pm.Is(2, LevelType.Property, "right"), true);
        }

        /// <summary>
        ///A test for FinalEmptyIs
        ///</summary>
        [TestMethod()]
        public void FinalEmptyIsTest4()
        {
            IPM pm = new PM("left.solution(\"test m()\")");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "left"), true);
            Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Method, "solution"), true);
        }

        /// <summary>
        ///A test for FinalEmptyIs
        ///</summary>
        [TestMethod()]
        public void FinalEmptyIsTest5()
        {
            IPM pm = new PM("left.solution(\"m(1)\", \"m()\", ')', \")\", \"(\")");
            Assert.AreEqual(pm.Is(0, LevelType.Property, "left"), true);
            Assert.AreEqual(pm.FinalEmptyIs(1, LevelType.Method, "solution"), true);
        }

        /// <summary>
        ///A test for pinTo
        ///</summary>
        [TestMethod()]
        public void pinToTest1()
        {
            IPM pm = new PM("left.solution.right");

            Assert.AreEqual(4, pm.Levels.Count);

            pm.pinTo(1);
            Assert.AreEqual(3, pm.Levels.Count);

            Assert.AreEqual(pm.Is(0, LevelType.Property, "solution"), true);
            Assert.AreEqual(pm.Is(1, LevelType.Property, "right"), true);
            Assert.AreEqual(pm.Is(2, LevelType.RightOperandEmpty, null), true);

            pm.pinTo(2);
            Assert.AreEqual(pm.Is(0, LevelType.RightOperandEmpty, null), true);
        }

        /// <summary>
        ///A test for pinTo - indexes
        ///</summary>
        [TestMethod()]
        public void pinToTest2()
        {
            try {
                IPM pm = new PM("left.solution.right");
                pm.pinTo(100);
                Assert.Fail("1");
            }
            catch(InvalidArgumentException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("left.solution.right");
                pm.pinTo(-1);
                Assert.Fail("2");
            }
            catch(InvalidArgumentException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("left.solution.right"); //4
                pm.pinTo(4);
                Assert.Fail("4");
            }
            catch(InvalidArgumentException) {
                Assert.IsTrue(true);
            }

            try {
                IPM pm = new PM("left.solution.right"); //4
                pm.pinTo(1);
                pm.pinTo(2);
                pm.pinTo(1);
                Assert.Fail("5");
            }
            catch(InvalidArgumentException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for detectArgument
        ///</summary>
        [TestMethod()]
        public void detectArgumentTest1()
        {
            IPM pm = new PM("solution(\"str data\", 'str data2', 12, -12, 1.5, -1.5, STDOUT, TestEnum.SpecialType, mixed * data, true)");
            
            Assert.AreEqual(pm.Is(0, LevelType.Method, "solution"), true);
            Assert.AreEqual(pm.Levels[0].Args.Length, 10);

            Argument[] args = pm.Levels[0].Args;
            Assert.AreEqual(args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(args[0].data, "str data");

            Assert.AreEqual(args[1].type, ArgumentType.StringSingle);
            Assert.AreEqual(args[1].data, "str data2");

            Assert.AreEqual(args[2].type, ArgumentType.Integer);
            Assert.AreEqual(args[2].data, 12);

            Assert.AreEqual(args[3].type, ArgumentType.Integer);
            Assert.AreEqual(args[3].data, -12);

            Assert.AreEqual(args[4].type, ArgumentType.Double);
            Assert.AreEqual(args[4].data, 1.5);

            Assert.AreEqual(args[5].type, ArgumentType.Double);
            Assert.AreEqual(args[5].data, -1.5);

            Assert.AreEqual(args[6].type, ArgumentType.EnumOrConst);
            Assert.AreEqual(args[6].data, "STDOUT");

            Assert.AreEqual(args[7].type, ArgumentType.EnumOrConst);
            Assert.AreEqual(args[7].data, "TestEnum.SpecialType");

            Assert.AreEqual(args[8].type, ArgumentType.Mixed);
            Assert.AreEqual(args[8].data, "mixed * data");

            Assert.AreEqual(args[9].type, ArgumentType.Boolean);
            Assert.AreEqual(args[9].data, true);
        }

        /// <summary>
        ///A test for detectArgument - floating-point numbers 
        ///</summary>
        [TestMethod()]
        public void detectArgumentTest2()
        {
            IPM pm = new PM(" solution (1.5, -1.5, 1.5f, -1.5f, 1.5d, -1.5d) ");
            
            Assert.AreEqual(pm.Is(0, LevelType.Method, "solution"), true);
            Assert.AreEqual(pm.Levels[0].Args.Length, 6);

            Argument[] args = pm.Levels[0].Args;
            Assert.AreEqual(args[0].type, ArgumentType.Double);
            Assert.AreEqual(args[0].data, 1.5d);

            Assert.AreEqual(args[1].type, ArgumentType.Double);
            Assert.AreEqual(args[1].data, -1.5d);

            Assert.AreEqual(args[2].type, ArgumentType.Float);
            Assert.AreEqual(args[2].data, 1.5f);

            Assert.AreEqual(args[3].type, ArgumentType.Float);
            Assert.AreEqual(args[3].data, -1.5f);

            Assert.AreEqual(args[4].type, ArgumentType.Double);
            Assert.AreEqual(args[4].data, 1.5d);

            Assert.AreEqual(args[5].type, ArgumentType.Double);
            Assert.AreEqual(args[5].data, -1.5d);
        }

        /// <summary>
        ///A test for detectArgument - Object data 
        ///</summary>
        [TestMethod()]
        public void detectArgumentTest3()
        {
            IPM pm = new PM(" m77(\"guid\", 12, {\"p1\", {4, \"test\", 8, 'y'}, true}, {false, 'p2'}) ");

            Assert.AreEqual(pm.Is(0, LevelType.Method, "m77"), true);

            Argument[] args = pm.Levels[0].Args;
            Assert.AreEqual(args.Length, 4);

            Assert.AreEqual(args[0].type, ArgumentType.StringDouble);
            Assert.AreEqual(args[0].data, "guid");

            Assert.AreEqual(args[1].type, ArgumentType.Integer);
            Assert.AreEqual(args[1].data, 12);

            Assert.AreEqual(args[2].type, ArgumentType.Object);
            {
                Argument[] args2 = (Argument[])args[2].data;
                Assert.AreEqual(args2.Length, 3);

                Assert.AreEqual(args2[0].type, ArgumentType.StringDouble);
                Assert.AreEqual(args2[0].data, "p1");

                Assert.AreEqual(args2[1].type, ArgumentType.Object);
                {
                    Argument[] args21 = (Argument[])args2[1].data;
                    Assert.AreEqual(args21.Length, 4);

                    Assert.AreEqual(args21[0].type, ArgumentType.Integer);
                    Assert.AreEqual(args21[0].data, 4);

                    Assert.AreEqual(args21[1].type, ArgumentType.StringDouble);
                    Assert.AreEqual(args21[1].data, "test");

                    Assert.AreEqual(args21[2].type, ArgumentType.Integer);
                    Assert.AreEqual(args21[2].data, 8);

                    Assert.AreEqual(args21[3].type, ArgumentType.Char);
                    Assert.AreEqual(args21[3].data, 'y');
                }

                Assert.AreEqual(args2[2].type, ArgumentType.Boolean);
                Assert.AreEqual(args2[2].data, true);
            }

            Assert.AreEqual(args[3].type, ArgumentType.Object);
            {
                Argument[] args3 = (Argument[])args[3].data;
                Assert.AreEqual(args3.Length, 2);

                Assert.AreEqual(args3[0].type, ArgumentType.Boolean);
                Assert.AreEqual(args3[0].data, false);

                Assert.AreEqual(args3[1].type, ArgumentType.StringSingle);
                Assert.AreEqual(args3[1].data, "p2");
            }
        }

    }
}
