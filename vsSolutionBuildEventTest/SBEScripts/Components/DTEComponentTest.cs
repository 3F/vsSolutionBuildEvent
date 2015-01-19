using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test
{
    /// <summary>
    ///This is a test class for DTEComponentTest and is intended
    ///to contain all DTEComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DTEComponentTest
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
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("#[DTE exec: command(arg)]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("DTE exec: command(arg)");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest3()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("[DTE NotExist.test]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void parseTest4()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("[DTE exec:]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            Assert.AreEqual(String.Empty, target.parse("[DTE exec: command]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            Assert.AreEqual(String.Empty, target.parse("[DTE exec: command(args)]"));
        }

        private class DTEComponentAccessor: DTEComponent
        {
            public static IEnvironment MockOfIEnvironment
            {
                get {
                    return new Environment((EnvDTE80.DTE2)null);
                    //var mockEnv = new Mock<IEnvironment>();
                    //mockEnv.SetupGet(p => p.Dte2).Returns((EnvDTE80.DTE2)null);
                    //return mockEnv.Object;
                }
            }

            public DTEComponentAccessor(): base(MockOfIEnvironment)
            {
                var mock = new Mock<DTEOperation>((IEnvironment)null, SolutionEventType.General);
                mock.Setup(m => m.exec(It.IsAny<string[]>(), It.IsAny<bool>()));
                dteo = mock.Object;
            }

            public DTEComponentAccessor(IEnvironment env): base(env)
            {

            }
        }
    }
}
