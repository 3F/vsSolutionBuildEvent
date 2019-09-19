using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.SobaScript.Components;

namespace net.r_eg.vsSBE.Test.SobaScript.Components
{
    [TestClass]
    public class InternalComponentTest
    {
        private IEnvironment env = new StubEnv();
        private IUVars uvariable = new UVars();

        [TestMethod]
        public void eventsItemRunTest1()
        {
            var target = new InternalComponentAccessor();

            try {
                target.Eval("[Core events.Pre.item(1).run]");
                Assert.Fail("1");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(IncorrectNodeException), ex.GetType().ToString()); }

            try {
                target.Eval("[Core events.Pre.item(1).run() = true]");
                Assert.Fail("2");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.Eval("[Core events.Pre.item(1).run(): true]");
                Assert.Fail("3");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }

            try {
                target.Eval("[Core events.Pre.item(1).run().m]");
                Assert.Fail("4");
            }
            catch(Exception ex) { Assert.IsTrue(ex.GetType() == typeof(NotSupportedOperationException), ex.GetType().ToString()); }
        }

        [TestMethod]
        public void eventsItemRunTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreEqual(Value.From(true), target.Eval("[Core events.Pre.item(1).run()]"));
            Assert.AreEqual(Value.From(true), target.Eval("[Core events.Pre.item(1).run(Common)]"));
            Assert.AreEqual(Value.From(false), target.Eval("[Core events.Pre.item(2).run()]"));
            Assert.AreEqual(Value.From(false), target.Eval("[Core events.Pre.item(3).run()]"));
            Assert.AreEqual(Value.From(false), target.Eval("[Core events.Pre.item(3).run(Common)]"));
            Assert.AreEqual(Value.From(true), target.Eval("[Core events.Pre.item(3).run(Rebuild)]"));
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest()
        {
            InternalComponent target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("#[vsSBE events.Type.item(1)]");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest2()
        {
            InternalComponent target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("vsSBE events.Type.item(1)");
        }

        [TestMethod]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest3()
        {
            InternalComponent target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("[vsSBE NoExist.Type]");
        }

        [TestMethod]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void stEventsParseTest1()
        {
            InternalComponent target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("[vsSBE events.Type.item(name)]");
        }

        [TestMethod()]
        [ExpectedException(typeof(OperandNotFoundException))]
        public void stEventsParseTest2()
        {
            InternalComponent target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("[vsSBE events.Type.item(1).test]");
        }

        [TestMethod()]
        public void pEnabledParseTest1()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual(Value.From(true), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.AreEqual(Value.From(true), target.Eval("[vsSBE events.Pre.item(\"Name1\").Enabled]"));
            Assert.AreEqual(Value.From(false), target.Eval("[vsSBE events.Pre.item(2).Enabled]"));
            Assert.AreEqual(Value.From(false), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void pEnabledParseTest2()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.Eval("[vsSBE events.Pre.item(1).Enabled = 1true]");
        }

        [TestMethod()]
        public void pEnabledParseTest3()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual(Value.From(true), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.AreEqual(Value.Empty, target.Eval("[vsSBE events.Pre.item(1).Enabled = false]"));
            Assert.AreEqual(Value.From(false), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));

            Assert.AreEqual(Value.From(false), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
            Assert.AreEqual(Value.Empty, target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled = true]"));
            Assert.AreEqual(Value.From(true), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStatusParseTest1()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.Eval("[vsSBE events.Pre.item(1).Status.Has Errors]");
        }

        [TestMethod()]
        public void pStatusParseTest2()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            Assert.AreEqual("false", target.Eval("[vsSBE events.Pre.item(1).Status.HasErrors]"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStatusParseTest3()
        {
            InternalComponentAccessor target = new InternalComponentAccessor();
            target.Eval("[vsSBE events.Pre.item(1).Status.NotExistProp]");
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void startUpProjectTest1()
        {
            var target = new InternalComponent(new Soba(uvariable), new StubEnv());
            target.Eval("[Core StartUpProject: test]");
        }

        [TestMethod]
        public void startUpProjectTest2()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(new Soba(), env);
            string defProject   = env.StartupProjectString;

            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.Eval("[Core StartUpProject = project1]"));
            Assert.AreEqual("project1", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.Eval("[Core StartUpProject = \"project2\"]"));
            Assert.AreEqual("project2", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.Eval("[Core StartUpProject = \"\"]"));
            Assert.AreEqual(defProject, env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));
        }

        [TestMethod]
        public void startUpProjectTest3()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(new Soba(), env);
            string defProject   = env.StartupProjectString;

            Assert.AreEqual(String.Empty, target.Eval("[Core StartUpProject = project1]"));
            Assert.AreEqual("project1", env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.AreEqual(String.Empty, target.Eval("[Core StartUpProject =]"));
            Assert.AreEqual(defProject, env.StartupProjectString);
            Assert.AreEqual(env.StartupProjectString, target.Eval("[Core StartUpProject]"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStdoutTest1()
        {
            var target = new InternalComponentAccessor();
            target.Eval("[Core events.Pre.item(1).stdout = true]");
        }

        [TestMethod()]
        public void pStdoutTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreNotEqual(null, target.Eval("[Core events.Pre.item(1).stdout]"));
        }

        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void pStderrTest1()
        {
            var target = new InternalComponentAccessor();
            target.Eval("[Core events.Pre.item(1).stderr = true]");
        }

        [TestMethod()]
        public void pStderrTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.AreNotEqual(null, target.Eval("[Core events.Pre.item(1).stderr]"));
        }

        private class InternalComponentAccessor: InternalComponent
        {
            private SBEEvent[] evt = null;

            public InternalComponentAccessor()
                : base(new Soba(), new StubEnv())
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
