using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for OWPComponentTest and is intended
    ///to contain all OWPComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OWPComponentTest
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
        /// Mock of IEnvironment for current tests
        /// </summary>
        public IEnvironment Env
        {
            get
            {
                if(env == null)
                {
                    var owp = new Mock<EnvDTE.OutputWindowPane>();
                    owp.Setup(m => m.OutputString(It.IsAny<string>()));
                    owp.Setup(m => m.Activate());
                    owp.Setup(m => m.Clear());

                    var ow = new Mock<IOW>();
                    ow.Setup(m => m.getByName(It.IsAny<string>(), It.IsAny<bool>())).Returns(owp.Object);
                    ow.Setup(m => m.deleteByName(It.IsAny<string>()));

                    var mockEnv = new Mock<IEnvironment>();
                    mockEnv.SetupGet(p => p.OutputWindowPane).Returns(ow.Object);

                    env = mockEnv.Object;
                }
                return env;
            }
        }
        protected IEnvironment env;

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("#[OWP out.Warnings.Count]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("OWP out.Warnings.Count");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest3()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP NotFound.Test]");
        }

        /// <summary>
        ///A test for parse - stOut
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            Assert.AreEqual(String.Empty, target.parse("[OWP out.All]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings.Raw]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings]"));
            Assert.AreEqual("0", target.parse("[OWP out.Warnings.Count]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Warnings.Codes]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors.Raw]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors]"));
            Assert.AreEqual("0", target.parse("[OWP out.Errors.Count]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP out.Errors.Codes]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void parseTest5()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP out.NotSupportedTest]");
        }

        /// <summary>
        ///A test for stLog
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void stLogTest1()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP log]");
        }

        /// <summary>
        ///A test for stLog
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void stLogTest2()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP log.NotSupportedTest]");
        }

        /// <summary>
        ///A test for stLog
        ///</summary>
        [TestMethod()]
        public void stLogTest3()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            Assert.AreEqual(null, target.parse("[OWP log.Message]"));
            Assert.AreEqual(null, target.parse("[OWP log.Level]"));
        }

        /// <summary>
        ///A test for stItem
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stItemTest1()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP item(\"name\")]");
        }

        /// <summary>
        ///A test for stItem
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void stItemTest2()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP item(\"name\").NotSupportedTest]");
        }

        /// <summary>
        ///A test for stItem
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void stItemTest3()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP item(\"\").write(false): ]");
        }

        /// <summary>
        ///A test for stItem
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stItemTest4()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP item(name).write(false): ]");
        }

        /// <summary>
        ///A test for stItemWrite
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stItemWriteTest1()
        {
            OWPComponent target = new OWPComponent((IEnvironment)null);
            target.parse("[OWP item(\"name\").write(\"false\"): ]");
        }

        /// <summary>
        ///A test for stItemWrite
        ///</summary>
        [TestMethod()]
        public void stItemWriteTest2()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").write(false): data]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").write(true): data]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").writeLine(false): data]"));
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").writeLine(true): data]"));
        }

        /// <summary>
        ///A test for stItemDelete
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void stItemDeleteTest1()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").delete]"));
        }

        /// <summary>
        ///A test for stItemDelete
        ///</summary>
        [TestMethod()]
        public void stItemDeleteTest2()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(Value.from(false), target.parse("[OWP item(\"name\").delete = false]"));
            Assert.AreEqual(Value.from(true), target.parse("[OWP item(\"name\").delete = true]"));
        }

        /// <summary>
        ///A test for stItemClear
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void stItemClearTest1()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").clear]"));
        }

        /// <summary>
        ///A test for stItemClear
        ///</summary>
        [TestMethod()]
        public void stItemClearTest2()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(Value.from(false), target.parse("[OWP item(\"name\").clear = false]"));
            Assert.AreEqual(Value.from(true), target.parse("[OWP item(\"name\").clear = true]"));
        }

        /// <summary>
        ///A test for stItemActivate
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void stItemActivateTest1()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[OWP item(\"name\").activate]"));
        }

        /// <summary>
        ///A test for stItemActivate
        ///</summary>
        [TestMethod()]
        public void stItemActivateTest2()
        {
            OWPComponent target = new OWPComponent(Env);
            Assert.AreEqual(Value.from(false), target.parse("[OWP item(\"name\").activate = false]"));
            Assert.AreEqual(Value.from(true), target.parse("[OWP item(\"name\").activate = true]"));
        }
    }
}
