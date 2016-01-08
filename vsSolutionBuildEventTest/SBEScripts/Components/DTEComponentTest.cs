using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
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
        ///A test for parse - exec
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseExecTest1()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("#[DTE exec: command(arg)]");
        }

        /// <summary>
        ///A test for parse - exec
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseExecTest2()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("DTE exec: command(arg)");
        }

        /// <summary>
        ///A test for parse - exec
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void parseExecTest3()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("[DTE exec:]");
        }

        /// <summary>
        ///A test for parse - exec
        ///</summary>
        [TestMethod()]
        public void parseExecTest4()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            Assert.AreEqual(Value.Empty, target.parse("[DTE exec: command]"));
        }

        /// <summary>
        ///A test for parse - exec
        ///</summary>
        [TestMethod()]
        public void parseExecTest5()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            Assert.AreEqual(Value.Empty, target.parse("[DTE exec: command(args)]"));
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void parseLastCommandTest1()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.emulateAfterExecute("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 264, (object)"In", (object)"Out");
            target.parse("[DTE events.LastCommand]");
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotSupportedOperationException))]
        public void parseLastCommandTest2()
        {
            DTEComponent target = new DTEComponent((IEnvironment)null);
            target.parse("[DTE events.LastCommand]");
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        public void parseLastCommandTest3()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();

            string guid         = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            int id              = 264;
            object customIn     = (object)"In";
            object customOut    = (object)"Out";
            bool pre            = true;
            target.emulateBeforeExecute(guid, id, customIn, customOut, false);

            Assert.AreEqual(guid, target.parse("[DTE events.LastCommand.Guid]"));
            Assert.AreEqual(Value.from(id), target.parse("[DTE events.LastCommand.Id]"));
            Assert.AreEqual(customIn, target.parse("[DTE events.LastCommand.CustomIn]"));
            Assert.AreEqual(customOut, target.parse("[DTE events.LastCommand.CustomOut]"));
            Assert.AreEqual(Value.from(pre), target.parse("[DTE events.LastCommand.Pre]"));
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        public void parseLastCommandTest4()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();

            string guid         = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            int id              = 264;
            object customIn     = (object)"In";
            object customOut    = (object)"Out";
            bool pre            = false;
            target.emulateAfterExecute(guid, id, customIn, customOut);

            Assert.AreEqual(guid, target.parse("[DTE events.LastCommand.Guid]"));
            Assert.AreEqual(Value.from(id), target.parse("[DTE events.LastCommand.Id]"));
            Assert.AreEqual(customIn, target.parse("[DTE events.LastCommand.CustomIn]"));
            Assert.AreEqual(customOut, target.parse("[DTE events.LastCommand.CustomOut]"));
            Assert.AreEqual(Value.from(pre), target.parse("[DTE events.LastCommand.Pre]"));
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void parseLastCommandTest5()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.emulateAfterExecute("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 264, (object)"In", (object)"Out");

            target.parse("[DTE events.LastCommand.NotRealPropStub]");
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        public void parseLastCommandTest6()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();

            string guid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            target.emulateBeforeExecute(guid, 264, (object)"", (object)"", false);
            target.emulateAfterExecute(guid, 264, (object)"", (object)"");

            Assert.AreEqual(guid, target.parse("[DTE events . LastCommand . Guid]"));
        }

        /// <summary>
        ///A test for parse - events.LastCommand
        ///</summary>
        [TestMethod()]
        public void parseLastCommandTest7()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();

            string expectedGuid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            string otherGuid    = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";

            target.emulateBeforeExecute(otherGuid, 1627, (object)"", (object)"", false);
            target.emulateAfterExecute(expectedGuid, 264, (object)"", (object)"");

            Assert.AreEqual(expectedGuid, target.parse("[DTE events.LastCommand.Guid]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest1()
        {
            DTEComponentAccessor target = new DTEComponentAccessor();
            target.parse("[DTE NotExist.test]");
        }


        private class DTEComponentAccessor: DTEComponent
        {
            protected Mock<IEnvironment> mEnv;
            protected Mock<EnvDTE.CommandEvents> mEnvCE;

            public void emulateBeforeExecute(string guid, int id, object customIn, object customOut, bool cancelDefault)
            {
                mEnvCE.Raise(e => e.BeforeExecute += null, guid, id, customIn, customOut, cancelDefault);
            }

            public void emulateAfterExecute(string guid, int id, object customIn, object customOut)
            {
                mEnvCE.Raise(e => e.AfterExecute += null, guid, id, customIn, customOut);
            }

            public DTEComponentAccessor() 
                : base((IEnvironment)null)
            {
                init();
                attachCommandEvents();
            }

            protected void init()
            {
                this.mEnv   = new Mock<IEnvironment>();
                this.env    = mEnv.Object;

                initCommandEvents();
                initDTEO();
            }

            protected void initCommandEvents()
            {
                this.mEnvCE = new Mock<EnvDTE.CommandEvents>();
                mEnv.Setup(p => p.Events.get_CommandEvents("{00000000-0000-0000-0000-000000000000}", 0)).Returns(mEnvCE.Object);
            }

            protected void initDTEO()
            {
                var mDTEO = new Mock<DTEOperation>(env, SolutionEventType.General);
                mDTEO.Setup(m => m.exec(It.IsAny<string[]>(), It.IsAny<bool>()));
                this.dteo = mDTEO.Object;
            }

            //protected override void raise(string guid, int id, ref object customIn, ref object customOut)
            //{
            //    // ...
            //}
        }
    }
}
