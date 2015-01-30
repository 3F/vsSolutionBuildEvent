using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for ConditionComponentTest and is intended
    ///to contain all ConditionComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConditionComponentTest
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
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("#[(true){body1}]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[(true)]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            Assert.AreEqual(String.Empty, target.parse("[ (true * 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (!true * 1) { body1 } ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest5()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[ (!) { body1 } ]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            Assert.AreEqual(" body1 ", target.parse("[ (true) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (true == 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (true == true) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[(true){ body1 }]"));
            Assert.AreEqual("\n body1 \n", target.parse("[(true){\n body1 \n}]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"true\" == 1) { body1 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"true\") { body1 } ]"));
            Assert.AreEqual(String.Empty, target.parse("[ (false) { body1 } ]"));
            Assert.AreEqual(String.Empty, target.parse("[ (true == 0) { body1 } ]"));
            Assert.AreEqual(String.Empty, target.parse("[ (true == false) { body1 } ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest7()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            Assert.AreEqual(" body2 ", target.parse("[ (false) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (false == 0) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (false == false) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body1 ", target.parse("[ (\"false\" == 0) { body1 } else { body2 } ]"));
            Assert.AreEqual(" body2 ", target.parse("[ (\"false\") { body1 } else { body2 } ]"));
            Assert.AreEqual(" body2 ", target.parse("[(false){ body1 }else{ body2 }]"));
            Assert.AreEqual("\n body2 \n", target.parse("[ (false) {\n body1 \n}\nelse {\n body2 \n} ]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest8()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            Assert.AreEqual(" body1 ", target.parse("[(!false){ body1 }else{ body2 }]"));
            Assert.AreEqual(String.Empty, target.parse("[(!true){ body1 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest9()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);

            Assert.AreEqual(" body1 ", target.parse("[(str1 === str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(str1 == str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(\"str1\"==\" str1 \"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(str1==\"str1 \"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(02 == 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(02 === 2){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest10()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);

            Assert.AreEqual(" body2 ", target.parse("[(str1 !== str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(str1 != str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(02 != 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(02 !== 2){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);

            Assert.AreEqual(" body1 ", target.parse("[(Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(Test 123 Data ~= Data){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);

            Assert.AreEqual(" body2 ", target.parse("[(1 > 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(1 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 > 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(01 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 < 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(1 <= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(1 < 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(01 <= 1){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest13()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);

            Assert.AreEqual(" body2 ", target.parse("[(!str1 === str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!\"str1 \"===\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! str1 == str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! \"str1 \"==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!02 == 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!02 === 2){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body1 ", target.parse("[(!str1 !== str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!\"str1 \"!==\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! str1 != str1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!\"str1 \"!=\" str1\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!02 != 2){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 02 !== 2){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body2 ", target.parse("[(!Test123Data ~= 12){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! Test123Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! Test 123 Data ~= \" 12\"){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!Test 123 Data ~= Data){ body1 }else{ body2 }]"));

            Assert.AreEqual(" body1 ", target.parse("[(!1 > 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!1 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! 1 > 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(!01 >= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(! 1 < 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 1 <= 1){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body1 ", target.parse("[(!1 < 01){ body1 }else{ body2 }]"));
            Assert.AreEqual(" body2 ", target.parse("[(! 01 <= 1){ body1 }else{ body2 }]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest14()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[(1 > ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest15()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[(1 === ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest16()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[(1 >= ){ body1 }else{ body2 }]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.FormatException))]
        public void parseTest17()
        {
            var mockEnv = new Mock<IEnvironment>();
            var mockUVar = new Mock<IUserVariable>();
            ConditionComponent target = new ConditionComponent(mockEnv.Object, mockUVar.Object);
            target.parse("[(2 > 1test ){ body1 }else{ body2 }]");
        }
    }
}
