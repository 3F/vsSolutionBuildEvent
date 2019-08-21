using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass]
    public class InternalComponentTest
    {
        private IBootloader bootloader;
        private IEnvironment env = new StubEnv();
        private IUserVariable uvariable = new UserVariable();

        private IBootloader Loader
        {
            get {
                if(bootloader == null) {
                    bootloader = new Bootloader(env, uvariable);
                    bootloader.register();
                }
                return bootloader;
            }
        }

        [TestMethod]
        public void eventsItemRunTest1()
        {
            var target = new InternalComponentAccessor();

            try {
                target.parse("[Core events.Pre.item(1).run]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.parse("[Core events.Pre.item(1).run() = true]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[Core events.Pre.item(1).run(): true]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.parse("[Core events.Pre.item(1).run().m]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }
        }

        [TestMethod]
        public void eventsItemRunTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreEqual(Value.from(true), target.parse("[Core events.Pre.item(1).run()]"));
            Assert.AreEqual(Value.from(true), target.parse("[Core events.Pre.item(1).run(Common)]"));
            Assert.AreEqual(Value.from(false), target.parse("[Core events.Pre.item(2).run()]"));
            Assert.AreEqual(Value.from(false), target.parse("[Core events.Pre.item(3).run()]"));
            Assert.AreEqual(Value.from(false), target.parse("[Core events.Pre.item(3).run(Common)]"));
            Assert.AreEqual(Value.from(true), target.parse("[Core events.Pre.item(3).run(Rebuild)]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest()
        {
            InternalComponent target = new InternalComponent(Loader);
            target.parse("#[vsSBE events.Type.item(1)]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            InternalComponent target = new InternalComponent(Loader);
            target.parse("vsSBE events.Type.item(1)");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest3()
        {
            InternalComponent target = new InternalComponent(Loader);
            target.parse("[vsSBE NoExist.Type]");
        }

        /// <summary>
        ///A test for parse - stEvents
        ///</summary>
        [TestMethod]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void stEventsParseTest1()
        {
            InternalComponent target = new InternalComponent(Loader);
            target.parse("[vsSBE events.Type.item(name)]");
        }

        /// <summary>
        ///A test for parse - stEvents
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void stEventsParseTest2()
        {
            InternalComponent target = new InternalComponent(Loader);
            target.parse("[vsSBE events.Type.item(1).test]");
        }

        /// <summary>
        ///A test for parse - stEventItem - pEnabled
        ///</summary>
        [TestMethod()]
        public void pEnabledParseTest1()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual(Value.from(true), target.parse("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.AreEqual(Value.from(true), target.parse("[vsSBE events.Pre.item(\"Name1\").Enabled]"));
            Assert.AreEqual(Value.from(false), target.parse("[vsSBE events.Pre.item(2).Enabled]"));
            Assert.AreEqual(Value.from(false), target.parse("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        /// <summary>
        ///A test for parse - stEventItem - pEnabled
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void pEnabledParseTest2()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.parse("[vsSBE events.Pre.item(1).Enabled = 1true]");
        }

        /// <summary>
        ///A test for parse - stEventItem - pEnabled
        ///</summary>
        [TestMethod()]
        public void pEnabledParseTest3()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual(Value.from(true), target.parse("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.AreEqual(Value.Empty, target.parse("[vsSBE events.Pre.item(1).Enabled = false]"));
            Assert.AreEqual(Value.from(false), target.parse("[vsSBE events.Pre.item(1).Enabled]"));

            Assert.AreEqual(Value.from(false), target.parse("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
            Assert.AreEqual(Value.Empty, target.parse("[vsSBE events.Pre.item(\"Name2\").Enabled = true]"));
            Assert.AreEqual(Value.from(true), target.parse("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        /// <summary>
        ///A test for parse - stEventItem - pStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStatusParseTest1()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.parse("[vsSBE events.Pre.item(1).Status.Has Errors]");
        }

        /// <summary>
        ///A test for parse - stEventItem - pStatus
        ///</summary>
        [TestMethod()]
        public void pStatusParseTest2()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual("false", target.parse("[vsSBE events.Pre.item(1).Status.HasErrors]"));
        }

        /// <summary>
        ///A test for parse - stEventItem - pStatus
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStatusParseTest3()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.parse("[vsSBE events.Pre.item(1).Status.NotExistProp]");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void startUpProjectTest1()
        {
            var target = new InternalComponent(Loader);
            target.parse("[Core StartUpProject: test]");
        }

        [TestMethod]
        public void startUpProjectTest2()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(env);
            string defProject   = env.StartupProjectString;

            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.parse("[Core StartUpProject = project1]"));
            Assert.AreEqual("project1", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.parse("[Core StartUpProject = \"project2\"]"));
            Assert.AreEqual("project2", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.parse("[Core StartUpProject = \"\"]"));
            Assert.AreEqual(defProject, env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));
        }

        [TestMethod]
        public void startUpProjectTest3()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(env);
            string defProject   = env.StartupProjectString;

            Assert.AreEqual(String.Empty, target.parse("[Core StartUpProject = project1]"));
            Assert.AreEqual("project1", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.parse("[Core StartUpProject =]"));
            Assert.AreEqual(defProject, env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.parse("[Core StartUpProject]"));
        }

        /// <summary>
        ///A test for parse - stEventItem - pStdout
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStdoutTest1()
        {
            var target = new InternalComponentAccessor();
            target.parse("[Core events.Pre.item(1).stdout = true]");
        }

        /// <summary>
        ///A test for parse - stEventItem - pStdout
        ///</summary>
        [TestMethod()]
        public void pStdoutTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreNotEqual(null, target.parse("[Core events.Pre.item(1).stdout]"));
        }

        /// <summary>
        ///A test for parse - stEventItem - pStderr
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStderrTest1()
        {
            var target = new InternalComponentAccessor();
            target.parse("[Core events.Pre.item(1).stderr = true]");
        }

        /// <summary>
        ///A test for parse - stEventItem - pStderr
        ///</summary>
        [TestMethod()]
        public void pStderrTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreNotEqual(null, target.parse("[Core events.Pre.item(1).stderr]"));
        }

        private class InternalComponentAccessor: InternalComponent
        {
            private SBEEvent[] evt = null;

            public InternalComponentAccessor()
                : base(new StubEnv())
            {

            }

            protected override ISolutionEvent[] getEvent(SolutionEventType type)
            {
                if(evt != null) {
                    return evt;
                }

                evt = new SBEEvent[3]{ 
                    new SBEEvent(){
                        Name = "Name1",
                        SupportMSBuild = false,
                        SupportSBEScripts = false,
                        Mode = new ModeFile() { Command = "" },
                        Enabled = true
                    },
                    new SBEEvent(){
                        Name = "Name2",
                        Mode = new ModeFile() { Command = "" },
                        Enabled = false
                    },
                    new SBEEvent(){
                        Name = "Name3",
                        SupportMSBuild = false,
                        SupportSBEScripts = false,
                        BuildType = Bridge.BuildType.Rebuild,
                        Mode = new ModeFile() { Command = "" },
                        Enabled = true
                    }
                };
                return evt;
            }
        }
    }
}
