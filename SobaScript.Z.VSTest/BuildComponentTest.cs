using System;
using System.Collections.Generic;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.VS;
using SobaScript.Z.VSTest.Stubs;
using Xunit;

namespace SobaScript.Z.VSTest
{
    public class BuildComponentTest
    {
        const string EXIST_GUID     = BuildComponentProjectsStub.EXIST_GUID;
        const string NOTEXIST_GUID  = BuildComponentProjectsStub.NOTEXIST_GUID;

        [Fact]
        public void ParseTest()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[Build UnitTestChecking = true]")
            );
        }

        [Fact]
        public void StCancelTest1()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[Build cancel = true]")
            );
        }

        [Fact]
        public void StCancelTest2()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("Build cancel = true")
            );
        }

        [Fact]
        public void StCancelTest3()
        {
            var target = new BuildComponentAcs();
            Assert.Equal(Value.Empty, target.Eval("[Build cancel = true]"));
            Assert.Equal(Value.Empty, target.Eval("[Build cancel = 1]"));
            Assert.Equal(Value.Empty, target.Eval("[Build cancel = false]"));
            Assert.Equal(Value.Empty, target.Eval("[Build cancel = 0]"));
            Assert.Equal(Value.Empty, target.Eval("[Build cancel = true ] "));
        }

        [Fact]
        public void StCancelTest4()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[Build cancel = 1true]")
            );
        }

        [Fact]
        public void StCancelTest5()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build cancel]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build cancel : true]")
            );
        }

        [Fact]
        public void StTypeTest1()
        {
            var env = new NullBuildEnv();
            var target = new BuildComponent(new Soba(), env);

            env.BuildType = string.Empty;
            Assert.Equal(env.BuildType, target.Eval("[Build type]"));

            env.BuildType = "Rebuild";
            Assert.Equal(env.BuildType, target.Eval("[Build type]"));

            env.BuildType = "Clean";
            Assert.Equal(env.BuildType, target.Eval("[Build type]"));
        }

        [Fact]
        public void StTypeTest2()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build type = true]")
            );
        }

        [Fact]
        public void StProjectsTest1()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[Build projects.find(name)]")
            );
        }

        [Fact]
        public void StProjectsTest2()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[Build projects.find(\"NotExist\").]")
            );
        }

        [Fact]
        public void StProjectConfTest1()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[Build projects.find(\"project1\").IsBuildable = val]")
            );
        }

        [Fact]
        public void StProjectConfTest2()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build projects.find(\"project1\").NotExist = true]")
            );
        }

        [Fact]
        public void IsBuildableTest1()
        {
            var target = new BuildComponentProjectsStub();
            Assert.Equal(Value.Empty, target.Eval("[Build projects.find(\"project1\").IsBuildable = true]"));
        }

        [Fact]
        public void IsBuildableTest2()
        {
            var env     = new NullBuildEnv();
            var target  = new BuildComponent(new Soba(), env);
            env.SetContext("project1", true, false);
            env.SetContext("project2", false, false);

            Assert.Equal("true", target.Eval("[Build projects.find(\"project1\").IsBuildable]"));
            Assert.Equal("false", target.Eval("[Build projects.find(\"project2\").IsBuildable]"));
        }

        [Fact]
        public void IsDeployableTest1()
        {
            var target = new BuildComponentProjectsStub();
            Assert.Equal(Value.Empty, target.Eval("[Build projects.find(\"project1\").IsDeployable = true]"));
        }

        [Fact]
        public void IsDeployableTest2()
        {
            var env = new NullBuildEnv();
            var target = new BuildComponent(new Soba(), env);
            env.SetContext("project1", false, true);
            env.SetContext("project2", false, false);

            Assert.Equal("true", target.Eval("[Build projects.find(\"project1\").IsDeployable]"));
            Assert.Equal("false", target.Eval("[Build projects.find(\"project2\").IsDeployable]"));
        }

        [Fact]
        public void StSolutionTest1()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"path.sln\")]")
            );
        }

        [Fact]
        public void StSolutionTest2()
        {
            var target = new BuildComponentAcs();

            Assert.Throws<ArgumentException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[Build solution.path()]"))
            );
        }

        [Fact]
        public void StSolutionTest3()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[Build solution.NotRealProperty]"))
            );
        }

        [Fact]
        public void StSlnPMapTest1()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.NotExistProperty]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.First]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.Last]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.LastRaw]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.projectBy(\"" + EXIST_GUID + "\")]")
            );
        }

        [Fact]
        public void StSlnPMapTest2()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"stub.sln\").First]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"stub.sln\").Last]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"stub.sln\").LastRaw]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"stub.sln\").projectBy(\"" + EXIST_GUID + "\")]")
            );
        }

        [Fact]
        public void StSlnPMapTest3()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<KeyNotFoundException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[Build solution.current.projectBy(\"" + NOTEXIST_GUID + "\")]"))
            );
        }

        [Fact]
        public void StSlnPMapTest4()
        {
            var target = new BuildComponentProjectsStub();
            Assert.Equal(EXIST_GUID, target.Eval("[Build solution.current.GuidList]"));
            Assert.Equal(EXIST_GUID, target.Eval("[Build solution.path(\"stub.sln\").GuidList]"));
        }

        [Fact]
        public void StSlnPMapTest5()
        {
            var target = new BuildComponentProjectsStub();

            Assert.Throws<ArgumentException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[Build solution.current.projectBy()]"))
            );
        }

        [Fact]
        public void ProjectsMapTest1()
        {
            var target = new BuildComponentProjectsStub(new NullBuildEnv() {
                IsOpenedSolution = true,
                SolutionFile = "path\\to.sln",
            });

            target.SMap.SetPrj(EXIST_GUID, "Project1", "path\\to.sln", target.SMap.Project1Type);

            bool h(string l1, string l2)
            {
                Assert.Equal("Project1", target.Eval(string.Format("[Build solution.{0}.{1}.name]", l1, l2)));
                Assert.Equal("path\\to.sln", target.Eval(string.Format("[Build solution.{0}.{1}.path]", l1, l2)));
                Assert.Equal(EXIST_GUID, target.Eval(string.Format("[Build solution.{0}.{1}.guid]", l1, l2)));
                Assert.Equal(target.SMap.Project1Type, target.Eval(string.Format("[Build solution.{0}.{1}.type]", l1, l2)));
                return true;
            }

            h("current", "First");
            h("current", "Last");
            h("current", "FirstRaw");
            h("current", "LastRaw");
            h("current", "projectBy(\"" + EXIST_GUID + "\")");

            h("path(\"path\\to.sln\")", "First");
            h("path(\"path\\to.sln\")", "Last");
            h("path(\"path\\to.sln\")", "FirstRaw");
            h("path(\"path\\to.sln\")", "LastRaw");
            h("path(\"path\\to.sln\")", "projectBy(\"" + EXIST_GUID + "\")");
        }

        [Fact]
        public void ProjectsMapTest2()
        {
            var target = new BuildComponentProjectsStub(new NullBuildEnv() {
                IsOpenedSolution = true,
                SolutionFile = "stub.sln",
            });

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.path(\"stub.sln\").First.NotRealProperty]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Build solution.current.First.NotRealProperty]")
            );
        }

        [Fact]
        public void ProjectsMapTest3()
        {
            var target = new BuildComponentProjectsStub(new NullBuildEnv() {
                IsOpenedSolution = true,
                SolutionFile = "path\\to.sln",
            });

            target.SMap.SetPrj(EXIST_GUID, "Project1", "path\\to.sln", target.SMap.Project1Type);

            bool h(string l1, string l2)
            {
                Assert.Equal("Project1", target.Eval(string.Format("[Build solution.{0}.{1}.name.RightProperty]", l1, l2)));
                Assert.Equal("path\\to.sln", target.Eval(string.Format("[Build solution.{0}.{1}.path.RightProperty]", l1, l2)));
                Assert.Equal(EXIST_GUID, target.Eval(string.Format("[Build solution.{0}.{1}.guid.RightProperty]", l1, l2)));
                Assert.Equal(target.SMap.Project1Type, target.Eval(string.Format("[Build solution.{0}.{1}.type.RightProperty]", l1, l2)));
                return true;
            }

            Assert.Throws<NotSupportedOperationException>(() => h("current", "First"));
            Assert.Throws<NotSupportedOperationException>(() => h("current", "Last"));
            Assert.Throws<NotSupportedOperationException>(() => h("current", "FirstRaw"));
            Assert.Throws<NotSupportedOperationException>(() => h("current", "LastRaw"));
            Assert.Throws<NotSupportedOperationException>(() => h("current", "projectBy(\"" + EXIST_GUID + "\")"));
            Assert.Throws<NotSupportedOperationException>(() => h("path(\"path\\to.sln\")", "First"));
            Assert.Throws<NotSupportedOperationException>(() => h("path(\"path\\to.sln\")", "Last"));
            Assert.Throws<NotSupportedOperationException>(() => h("path(\"path\\to.sln\")", "FirstRaw"));
            Assert.Throws<NotSupportedOperationException>(() => h("path(\"path\\to.sln\")", "LastRaw"));
            Assert.Throws<NotSupportedOperationException>(() => h("path(\"path\\to.sln\")", "projectBy(\"" + EXIST_GUID + "\")"));
        }
    }
}
