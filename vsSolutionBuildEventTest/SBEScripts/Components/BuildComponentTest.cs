using System;
using System.Collections;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
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
                    mockEnv.SetupGet(p => p.SolutionActiveConfiguration).Returns(mockSolutionActiveConfiguration.Object);
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
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest2()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("#[Build cancel = true]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest3()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("Build cancel = true");
        }

        /// <summary>
        ///A test for parse -> stCancel
        ///#[Build cancel = true]
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            Assert.AreEqual(String.Empty, target.parse("[Build cancel = true]"));
            Assert.AreEqual(String.Empty, target.parse("[Build cancel = 1]"));
            Assert.AreEqual(String.Empty, target.parse("[Build cancel = false]"));
            Assert.AreEqual(String.Empty, target.parse("[Build cancel = 0]"));
            Assert.AreEqual(String.Empty, target.parse("[Build cancel = true ] "));
        }

        /// <summary>
        ///A test for parse -> stCancel
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void parseTest5()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("[Build cancel = 1true]");
        }

        /// <summary>
        ///A test for parse -> stProjects
        ///#[Build projects.find("name")]
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest6()
        {
            BuildComponentAccessor target = new BuildComponentAccessor();
            target.parse("[Build projects.find(name)]");
        }

        /// <summary>
        ///A test for parse -> stProjects
        ///#[Build projects.find("name")]
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotFoundException))]
        public void parseTest7()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"NotExist\").]");
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(SyntaxIncorrectException))]
        public void parseTest8()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"project1\").IsBuildable = val]");
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(OperationNotFoundException))]
        public void parseTest9()
        {
            BuildComponent target = new BuildComponent(Env);
            target.parse("[Build projects.find(\"project1\").NotExist = true]");
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        public void parseTest10()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[Build projects.find(\"project1\").IsBuildable = true]"));
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual("true", target.parse("[Build projects.find(\"project1\").IsBuildable]"));
            Assert.AreEqual("false", target.parse("[Build projects.find(\"project2\").IsBuildable]"));
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual(String.Empty, target.parse("[Build projects.find(\"project1\").IsDeployable = true]"));
        }

        /// <summary>
        ///A test for parse -> stProjectConf
        ///</summary>
        [TestMethod()]
        public void parseTest13()
        {
            BuildComponent target = new BuildComponent(Env);
            Assert.AreEqual("true", target.parse("[Build projects.find(\"project1\").IsDeployable]"));
            Assert.AreEqual("false", target.parse("[Build projects.find(\"project2\").IsDeployable]"));
        }

        private class BuildComponentAccessor: BuildComponent
        {
            public BuildComponentAccessor(): base((IEnvironment)null)
            {
                var mock = new Mock<DTEOperation>((EnvDTE80.DTE2)null, SolutionEventType.General);
                mock.Setup(m => m.exec(It.IsAny<string[]>(), It.IsAny<bool>()));
                dteo = mock.Object;
            }

            public BuildComponentAccessor(IEnvironment env): base(env)
            {

            }
        }
    }
}
