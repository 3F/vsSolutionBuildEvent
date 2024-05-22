using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.SobaScript.Components;
using Xunit;

namespace net.r_eg.vsSBE.Test.SobaScript.Components
{
    public class InternalComponentTest
    {
        private readonly IUVars uvariable = new UVars();

        [Theory]
        [InlineData("[Core events.Pre.item(1).run() = true]")]
        [InlineData("[Core events.Pre.item(1).run(): true]")]
        [InlineData("[Core events.Pre.item(1).run().m]")]
        public void evalNotSupportedOperationTheory(string data)
        {
            InternalComponentAccessor target = new();
            Assert.Throws<NotSupportedOperationException>(() => target.Eval(data));
        }

        [Theory]
        [InlineData("[Core events.Pre.item(1).run]")]
        [InlineData("[vsSBE events.Pre.item(1).Status.Has Errors]")]
        [InlineData("[vsSBE events.Pre.item(1).Status.NotExistProp]")]
        [InlineData("[Core events.Pre.item(1).stdout = true]")]
        [InlineData("[Core events.Pre.item(1).stderr = true]")]
        public void evalIncorrectNodeTheory(string data)
        {
            InternalComponentAccessor target = new();
            Assert.Throws<IncorrectNodeException>(() => target.Eval(data));
        }

        [Theory]
        [InlineData("[vsSBE events.Type.item(name)]")]
        [InlineData("[vsSBE events.Type.item(1).test]")]
        public void evalOperandNotFoundTheory(string data)
        {
            InternalComponent target = new(new Soba(uvariable), new StubEnv());
            Assert.Throws<OperandNotFoundException>(() => target.Eval(data));
        }

        [Theory]
        [InlineData("#[vsSBE events.Type.item(1)]")]
        [InlineData("vsSBE events.Type.item(1)")]
        [InlineData("[vsSBE events.Pre.item(1).Enabled = 1true]")]
        public void parseIncorrectSyntaxTheory(string data)
        {
            InternalComponentAccessor target = new(new Soba(uvariable), new StubEnv());
            Assert.Throws<IncorrectSyntaxException>(() => target.Eval(data));
        }

        [Theory]
        [InlineData("[vsSBE NoExist.Type]")]
        public void evalSubtypeNotFoundTheory(string data)
        {
            InternalComponent target = new(new Soba(uvariable), new StubEnv());
            Assert.Throws<SubtypeNotFoundException>(() => target.Eval(data));
        }

        [Fact]
        public void pStatusParseTest2()
        {
            InternalComponentAccessor target = new();
            Assert.Equal("false", target.Eval("[vsSBE events.Pre.item(1).Status.HasErrors]"));
        }

        [Theory]
        [InlineData("[Core StartUpProject: test]")]
        public void evalIncorrectNodeUVarTheory(string data)
        {
            InternalComponent target = new(new Soba(uvariable), new StubEnv());
            Assert.Throws<IncorrectNodeException>(() => target.Eval(data));
        }

        [Fact]
        public void eventsItemRunTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.Equal(Value.From(true), target.Eval("[Core events.Pre.item(1).run()]"));
            Assert.Equal(Value.From(true), target.Eval("[Core events.Pre.item(1).run(Common)]"));
            Assert.Equal(Value.From(true), target.Eval("[Core events.Pre.item(2).run()]"));
            Assert.Equal(Value.From(false), target.Eval("[Core events.Pre.item(3).run()]"));
            Assert.Equal(Value.From(false), target.Eval("[Core events.Pre.item(3).run(Common)]"));
            Assert.Equal(Value.From(true), target.Eval("[Core events.Pre.item(3).run(Rebuild)]"));
        }

        [Fact]
        public void pEnabledParseTest1()
        {
            InternalComponentAccessor target = new();
            Assert.Equal(Value.From(true), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.Equal(Value.From(true), target.Eval("[vsSBE events.Pre.item(\"Name1\").Enabled]"));
            Assert.Equal(Value.From(false), target.Eval("[vsSBE events.Pre.item(2).Enabled]"));
            Assert.Equal(Value.From(false), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        [Fact]
        public void pEnabledParseTest3()
        {
            InternalComponentAccessor target = new();
            Assert.Equal(Value.From(true), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));
            Assert.Equal(Value.Empty, target.Eval("[vsSBE events.Pre.item(1).Enabled = false]"));
            Assert.Equal(Value.From(false), target.Eval("[vsSBE events.Pre.item(1).Enabled]"));

            Assert.Equal(Value.From(false), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
            Assert.Equal(Value.Empty, target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled = true]"));
            Assert.Equal(Value.From(true), target.Eval("[vsSBE events.Pre.item(\"Name2\").Enabled]"));
        }

        [Fact]
        public void startUpProjectTest2()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(new Soba(), env);
            string defProject   = env.StartupProjectString;

            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.Equal(String.Empty, target.Eval("[Core StartUpProject = project1]"));
            Assert.Equal("project1", env.StartupProjectString);
            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.Equal(String.Empty, target.Eval("[Core StartUpProject = \"project2\"]"));
            Assert.Equal("project2", env.StartupProjectString);
            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.Equal(String.Empty, target.Eval("[Core StartUpProject = \"\"]"));
            Assert.Equal(defProject, env.StartupProjectString);
            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));
        }

        [Fact]
        public void startUpProjectTest3()
        {
            IEnvironment env    = new StubEnv();
            var target          = new InternalComponent(new Soba(), env);
            string defProject   = env.StartupProjectString;

            Assert.Equal(String.Empty, target.Eval("[Core StartUpProject = project1]"));
            Assert.Equal("project1", env.StartupProjectString);
            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));

            Assert.Equal(String.Empty, target.Eval("[Core StartUpProject =]"));
            Assert.Equal(defProject, env.StartupProjectString);
            Assert.Equal(env.StartupProjectString, target.Eval("[Core StartUpProject]"));
        }

        [Fact]
        public void pStdoutTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.NotNull(target.Eval("[Core events.Pre.item(1).stdout]"));
        }

        [Fact]
        public void pStderrTest2()
        {
            var target = new InternalComponentAccessor();
            Assert.NotNull(target.Eval("[Core events.Pre.item(1).stderr]"));
        }

        private class InternalComponentAccessor: InternalComponent
        {
            private SBEEvent[] evt = null;

            public InternalComponentAccessor()
                : base(new Soba(), new StubEnv())
            {

            }

            public InternalComponentAccessor(ISobaScript soba, IEnvironment env)
                : base(soba, env)
            {

            }

            protected override ISolutionEvent[] getEvent(SolutionEventType type)
            {
                if(evt != null) {
                    return evt;
                }

                evt = new SBEEvent[3]
                { 
                    new SBEEvent()
                    {
                        Name = "Name1",
                        SupportMSBuild = false,
                        SupportSBEScripts = false,
                        Mode = new ModeFile() { Command = "" },
                        Enabled = true
                    },
                    new SBEEvent()
                    {
                        Name = "Name2",
                        Mode = new ModeFile() { Command = "" },
                        Enabled = false
                    },
                    new SBEEvent()
                    {
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
