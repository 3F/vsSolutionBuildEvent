using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Components.Build;
using net.r_eg.vsSBE.SBEScripts.Exceptions;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for BuildComponentTest and is intended
    ///to contain all BuildComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BuildComponentTest
    {
        private const string EXIST_GUID     = "{11111111-1111-1111-1111-111111111111}";
        private const string NOTEXIST_GUID  = "{00000000-0000-0000-0000-000000000000}";

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
                    var mockEnv = new Mock<IEnvironment>();
                    var mockSolutionActiveConfiguration = new Mock<EnvDTE80.SolutionConfiguration2>();
                    var mockSolutionContexts  = new Mock<SolutionContexts>();

                    mockSolutionContexts.Setup(m => m.GetEnumerator()).Returns(SolutionContexts);
                    mockSolutionActiveConfiguration.SetupGet(p => p.SolutionContexts).Returns(mockSolutionContexts.Object);
                    mockEnv.SetupGet(p => p.SolutionActiveCfg).Returns(mockSolutionActiveConfiguration.Object);
                    mockEnv.SetupGet(p => p.SolutionFile).Returns("stub.sln");
                    mockEnv.SetupGet(p => p.IsOpenedSolution).Returns(true);
                    env = mockEnv.Object;
                }
                return env;
            }
        }
        protected IEnvironment env;

        /// <summary>
        /// SolutionContexts for tests
        /// </summary>
        /// <returns></returns>
        public IEnumerator SolutionContexts
        {
            get{
                yield return solutionContext("project1", "platform1", true, true);
                yield return solutionContext("project2", "platform1", false, false);
                yield return solutionContext("project3", "platform2", false, false);
            }
        }

        /// <summary>
        /// Mock of SolutionContext
        /// </summary>
        /// <param name="project">project name</param>
        /// <param name="platform">platform name</param>
        /// <param name="shouldBuild">IsBuildable</param>
        /// <param name="shouldDeploy"IsDeployable></param>
        public SolutionContext solutionContext(string project, string platform, bool shouldBuild, bool shouldDeploy)
        {
            var mock  = new Mock<SolutionContext>();
            mock.SetupGet(p => p.ProjectName).Returns(project);
            mock.SetupGet(p => p.PlatformName).Returns(platform);
            mock.SetupGet(p => p.ShouldBuild).Returns(shouldBuild);
            mock.SetupGet(p => p.ShouldDeploy).Returns(shouldDeploy);
            return mock.Object;
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SubtypeNotFoundException))]
        public void parseTest()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("[Build UnitTestChecking = true]");
        }

        /// <summary>
        ///A test for parse - stCancel
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stCancelTest1()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("#[Build cancel = true]");
        }

        /// <summary>
        ///A test for parse - stCancel
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stCancelTest2()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("Build cancel = true");
        }

        /// <summary>
        ///A test for parse -> stCancel
        ///#[Build cancel = true]
        ///</summary>
        [TestMethod()]
        public void stCancelTest3()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            Assert.AreEqual(Value.Empty, target.parse("[Build cancel = true]"));
            Assert.AreEqual(Value.Empty, target.parse("[Build cancel = 1]"));
            Assert.AreEqual(Value.Empty, target.parse("[Build cancel = false]"));
            Assert.AreEqual(Value.Empty, target.parse("[Build cancel = 0]"));
            Assert.AreEqual(Value.Empty, target.parse("[Build cancel = true ] "));
        }

        /// <summary>
        ///A test for parse -> stCancel
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void stCancelTest4()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("[Build cancel = 1true]");
        }

        /// <summary>
        ///A test for parse -> stCancel
        ///</summary>
        [TestMethod()]
        public void stCancelTest5()
        {
            var target = new BuildComponentAccessor();

            try {
                target.parse("[Build cancel]");
                Assert.Fail("1");
            }
            catch(IncorrectNodeException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build cancel : true]");
                Assert.Fail("2");
            }
            catch(IncorrectNodeException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse -> stType
        ///</summary>
        [TestMethod()]
        public void stTypeTest1()
        {
            IEnvironment _env       = new Environment((DTE2)(new Mock<DTE2>()).Object);
            BuildComponent target   = new BuildComponent(_env);

            Assert.AreEqual(BuildType.Common.ToString(), target.parse("[Build type]"));

            _env.BuildType = BuildType.Compile;
            Assert.AreEqual(BuildType.Compile.ToString(), target.parse("[Build type]"));

            _env.BuildType = BuildType.Clean;
            Assert.AreEqual(BuildType.Clean, (BuildType)Enum.Parse(typeof(BuildType), target.parse("[Build type]")));
        }

        /// <summary>
        ///A test for parse -> stType
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void stTypeTest2()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build type = true]");
        }

        /// <summary>
        ///A test for parse -> stProjects
        ///#[Build projects.find("name")]
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentPMException))]
        public void stProjectsTest1()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("[Build projects.find(name)]");
        }

        /// <summary>
        ///A test for parse -> stProjects
        ///#[Build projects.find("name")]
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void stProjectsTest2()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"NotExist\").]");
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void stProjectConfTest1()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"project1\").IsBuildable = val]");
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void stProjectConfTest2()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"project1\").NotExist = true]");
        }

        /// <summary>
        ///A test for parse -> IsBuildable
        ///</summary>
        [TestMethod()]
        public void isBuildableTest1()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build projects.find(\"project1\").IsBuildable = true]"));
        }

        /// <summary>
        ///A test for parse -> IsBuildable
        ///</summary>
        [TestMethod()]
        public void isBuildableTest2()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual("true", target.parse("[Build projects.find(\"project1\").IsBuildable]"));
            Assert.AreEqual("false", target.parse("[Build projects.find(\"project2\").IsBuildable]"));
        }

        /// <summary>
        ///A test for parse -> IsDeployable
        ///</summary>
        [TestMethod()]
        public void isDeployableTest1()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build projects.find(\"project1\").IsDeployable = true]"));
        }

        /// <summary>
        ///A test for parse -> IsDeployable
        ///</summary>
        [TestMethod()]
        public void isDeployableTest2()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual("true", target.parse("[Build projects.find(\"project1\").IsDeployable]"));
            Assert.AreEqual("false", target.parse("[Build projects.find(\"project2\").IsDeployable]"));
        }

        /// <summary>
        ///A test for parse -> stSolution
        ///</summary>
        [TestMethod()]
        public void stSolutionTest1()
        {
            var target = new BuildComponentAccessor(Env);

            try {
                target.parse("[Build solution.current]");
                Assert.Fail("1");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.path(\"path.sln\")]");
                Assert.Fail("2");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse - stSolution
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void stSolutionTest2()
        {
            BuildComponent target = new BuildComponentAccessor(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build solution.path()]"));
        }

        /// <summary>
        ///A test for parse - stSolution
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectNodeException))]
        public void stSolutionTest3()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build solution.NotRealProperty]"));
        }

        /// <summary>
        ///A test for parse - stSolution - stSlnPMap
        ///</summary>
        [TestMethod()]
        public void stSlnPMapTest1()
        {
            var target = new BuildComponentAccessor(Env);

            try {
                target.parse("[Build solution.current.NotExistProperty]");
                Assert.Fail("1");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.current.First]");
                Assert.Fail("2");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.current.Last]");
                Assert.Fail("3");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.current.LastRaw]");
                Assert.Fail("4");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.current.projectBy(\"" + EXIST_GUID + "\")]");
                Assert.Fail("5");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse - stSolution - stSlnPMap
        ///</summary>
        [TestMethod()]
        public void stSlnPMapTest2()
        {
            var target = new BuildComponentAccessor(Env);

            try {
                target.parse("[Build solution.path(\"stub.sln\").First]");
                Assert.Fail("1");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.path(\"stub.sln\").Last]");
                Assert.Fail("2");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.path(\"stub.sln\").LastRaw]");
                Assert.Fail("3");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.path(\"stub.sln\").projectBy(\"" + EXIST_GUID + "\")]");
                Assert.Fail("4");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void stSlnPMapTest3()
        {
            BuildComponent target = new BuildComponentAccessor(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build solution.current.projectBy(\"" + NOTEXIST_GUID + "\")]"));
        }

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap
        ///</summary>
        [TestMethod()]
        public void stSlnPMapTest4()
        {
            BuildComponent target = new BuildComponentAccessor(Env);
            Assert.AreEqual(EXIST_GUID, target.parse("[Build solution.current.GuidList]"));
            Assert.AreEqual(EXIST_GUID, target.parse("[Build solution.path(\"stub.sln\").GuidList]"));
        }

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void stSlnPMapTest5()
        {
            BuildComponent target = new BuildComponentAccessor(Env);
            Assert.AreEqual(Value.Empty, target.parse("[Build solution.current.projectBy()]"));
        }

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap - projectsMap
        ///</summary>
        [TestMethod()]
        public void projectsMapTest1()
        {
            BuildComponent target = new BuildComponentAccessor(Env);

            Func<string, string, bool> h = delegate(string l1, string l2)
            {
                Assert.AreEqual("Project1", target.parse(String.Format("[Build solution.{0}.{1}.name]", l1, l2)));
                Assert.AreEqual("path\\to.sln", target.parse(String.Format("[Build solution.{0}.{1}.path]", l1, l2)));
                Assert.AreEqual(EXIST_GUID, target.parse(String.Format("[Build solution.{0}.{1}.guid]", l1, l2)));
                Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.parse(String.Format("[Build solution.{0}.{1}.type]", l1, l2)));
                return true;
            };

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

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap - projectsMap
        ///</summary>
        [TestMethod()]
        public void projectsMapTest2()
        {
            var target = new BuildComponentAccessor(Env);

            try {
                target.parse("[Build solution.path(\"stub.sln\").First.NotRealProperty]");
                Assert.Fail("1");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }

            try {
                target.parse("[Build solution.current.First.NotRealProperty]");
                Assert.Fail("2");
            }
            catch(OperationNotFoundException) {
                Assert.IsTrue(true);
            }
        }

        /// <summary>
        ///A test for parse - stSolution  - stSlnPMap - projectsMap
        ///</summary>
        [TestMethod()]
        public void projectsMapTest3()
        {
            BuildComponent target = new BuildComponentAccessor(Env);

            Func<string, string, bool> h = delegate(string l1, string l2) {
                Assert.AreEqual("Project1", target.parse(String.Format("[Build solution.{0}.{1}.name.RightProperty]", l1, l2)));
                Assert.AreEqual("path\\to.sln", target.parse(String.Format("[Build solution.{0}.{1}.path.RightProperty]", l1, l2)));
                Assert.AreEqual(EXIST_GUID, target.parse(String.Format("[Build solution.{0}.{1}.guid.RightProperty]", l1, l2)));
                Assert.AreEqual("{22222222-2222-2222-2222-222222222222}", target.parse(String.Format("[Build solution.{0}.{1}.type.RightProperty]", l1, l2)));
                return true;
            };

            try { h("current", "First"); Assert.Fail("1"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("current", "Last"); Assert.Fail("2"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("current", "FirstRaw"); Assert.Fail("3"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("current", "LastRaw"); Assert.Fail("4"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("current", "projectBy(\"" + EXIST_GUID + "\")"); Assert.Fail("5"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }

            try { h("path(\"path\\to.sln\")", "First"); Assert.Fail("6"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("path(\"path\\to.sln\")", "Last"); Assert.Fail("7"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("path(\"path\\to.sln\")", "FirstRaw"); Assert.Fail("8"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("path(\"path\\to.sln\")", "LastRaw"); Assert.Fail("9"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
            try { h("path(\"path\\to.sln\")", "projectBy(\"" + EXIST_GUID + "\")"); Assert.Fail("10"); } catch(NotSupportedOperationException) { Assert.IsTrue(true); }
        }

        private class BuildComponentAccessor: BuildComponent
        {
            public BuildComponentAccessor()
                : base((IEnvironment)null)
            {
                var mock = new Mock<DTEOperation>((IEnvironment)null, SolutionEventType.General);
                mock.Setup(m => m.exec(It.IsAny<string[]>(), It.IsAny<bool>()));
                dteo = mock.Object;
            }

            public BuildComponentAccessor(IEnvironment env)
                : base(env)
            {

            }

            protected class StubProjectsMap: ProjectsMap
            {
                public StubProjectsMap()
                {
                    projects[EXIST_GUID] = new Project()
                    {
                        guid = EXIST_GUID,
                        name = "Project1",
                        path = "path\\to.sln",
                        type = "{22222222-2222-2222-2222-222222222222}",
                    };
                    order.Add(EXIST_GUID);
                }
            }

            protected override ProjectsMap getProjectsMap(string sln)
            {
                return new StubProjectsMap();
            }
        }
    }
}
