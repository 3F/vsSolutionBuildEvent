using System;
using System.Collections.Generic;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for MSBuildParserTest and is intended
    ///to contain all MSBuildParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MSBuildParserTest
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
        ///A test for _splitGeneralProjectAttr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void _splitGeneralProjectAttrTest()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string unevaluated  = "(name:project)";
            string expectedArg  = "name";
            string expectedRet  = "project";

            string actual = target._splitGeneralProjectAttr(ref unevaluated);
            Assert.AreEqual(expectedArg, unevaluated);
            Assert.AreEqual(expectedRet, actual);
        }

        /// <summary>
        ///A test for _splitGeneralProjectAttr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void _splitGeneralProjectAttrTest2()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string unevaluated  = "([class]::func($(path:project), $([class]::func2($(path2)):project)):project)";
            string expectedArg  = "[class]::func($(path:project), $([class]::func2($(path2)):project))";
            string expectedRet  = "project";

            string actual = target._splitGeneralProjectAttr(ref unevaluated);
            Assert.AreEqual(expectedArg, unevaluated);
            Assert.AreEqual(expectedRet, actual);
        }

        /// <summary>
        ///A test for _splitGeneralProjectAttr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void _splitGeneralProjectAttrTest3()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string unevaluated  = "(name)";
            string expectedArg  = "name";
            string expectedRet  = null;

            string actual = target._splitGeneralProjectAttr(ref unevaluated);
            Assert.AreEqual(expectedArg, unevaluated);
            Assert.AreEqual(expectedRet, actual);
        }

        /// <summary>
        ///A test for _splitGeneralProjectAttr
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void _splitGeneralProjectAttrTest4()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string unevaluated  = "([class]::func($(path:project), $([class]::func2($(path2)):project)):project))";
            string expectedArg  = "[class]::func($(path:project), $([class]::func2($(path2)):project)):project)";
            string expectedRet  = null;

            string actual = target._splitGeneralProjectAttr(ref unevaluated);
            Assert.AreEqual(expectedArg, unevaluated);
            Assert.AreEqual(expectedRet, actual);
        }

        /// <summary>
        ///A test for _getSolutionGlobalProperty
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void _getSolutionGlobalPropertyTest()
        {
            var mockDte2                    = new Mock<EnvDTE80.DTE2>();
            var mockSolution                = new Mock<EnvDTE.Solution>();
            var mockSolutionBuild           = new Mock<EnvDTE.SolutionBuild>();
            var mockSolutionConfiguration2  = new Mock<EnvDTE80.SolutionConfiguration2>();

            mockSolutionConfiguration2.SetupGet(p => p.Name).Returns("Release");
            mockSolutionConfiguration2.SetupGet(p => p.PlatformName).Returns("x86");

            mockSolutionBuild.SetupGet(p => p.ActiveConfiguration).Returns(mockSolutionConfiguration2.Object);
            mockSolution.SetupGet(p => p.SolutionBuild).Returns(mockSolutionBuild.Object);
            mockDte2.SetupGet(p => p.Solution).Returns(mockSolution.Object);

            MSBuildParser_Accessor target = new MSBuildParser_Accessor(mockDte2.Object);
            Assert.IsNotNull(target._getSolutionGlobalProperty("Configuration"));
            Assert.IsNotNull(target._getSolutionGlobalProperty("Platform"));
            Assert.IsNull(target._getSolutionGlobalProperty("Foo"));
            Assert.IsNull(target._getSolutionGlobalProperty(null));
        }

        /// <summary>
        ///A test for parseCustomVariable
        ///</summary>
        [TestMethod()]
        public void parseCustomVariableTest()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string expected = "$(name)";
            string actual   = target.parseCustomVariable("$(name)", "subname", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseCustomVariable
        ///</summary>
        [TestMethod()]
        public void parseCustomVariableTest2()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string expected = "value";
            string actual   = target.parseCustomVariable("$(name)", "name", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseCustomVariable
        ///</summary>
        [TestMethod()]
        public void parseCustomVariableTest3()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string expected = "$$(name)";
            string actual   = target.parseCustomVariable("$$(name)", "name", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseCustomVariable
        ///</summary>
        [TestMethod()]
        public void parseCustomVariableTest4()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string expected = String.Empty;
            string actual   = target.parseCustomVariable("$(name)", "name", null);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        [ExpectedException(typeof(NotSupportedException))]
        public void prepareVariablesTest()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);
            TPreparedData actual = target.prepareVariables("var=$(Path:project):project");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        [ExpectedException(typeof(NotSupportedException))]
        public void prepareVariablesTest2()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);
            TPreparedData actual = target.prepareVariables("$(var=$(Path:project):project)");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest3()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(var=$(Path:project2):project)";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = "project";
            expected.variable.isPersistence = false;
            expected.property.raw           = "$(Path:project2)";
            expected.property.escaped       = false;
            expected.property.project       = "project2";
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest4()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(#var=$(Path:project2):project)";

            TPreparedData expected          = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = "project";
            expected.variable.isPersistence = true;
            expected.property.raw           = "$(Path:project2)";
            expected.property.escaped       = false;
            expected.property.project       = "project2";
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest5()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(Path:project)";

            TPreparedData expected          = new TPreparedData();
            expected.variable.name          = null;
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "Path";
            expected.property.escaped       = false;
            expected.property.project       = "project";
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest6()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(Path)";

            TPreparedData expected          = new TPreparedData();
            expected.variable.name          = null;
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "Path";
            expected.property.escaped       = false;
            expected.property.project       = null;
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest7()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(var=$$(Path:project2):project)";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = "project";
            expected.variable.isPersistence = false;
            expected.property.raw           = "$$(Path:project2)";
            expected.property.escaped       = true;
            expected.property.project       = null;
            expected.property.completed     = true;
            expected.property.complex       = true;
            expected.property.unevaluated   = "$(Path:project2)";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest8()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(var=$(Path.Replace('\', '/')):project)";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = "project";
            expected.variable.isPersistence = false;
            expected.property.raw           = "$(Path.Replace('\', '/'))";
            expected.property.escaped       = false;
            expected.property.project       = null;
            expected.property.completed     = false;
            expected.property.complex       = true;
            expected.property.unevaluated   = "$(Path.Replace('\', '/'))";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest9()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(Path.Replace('\', '/'))";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = null;
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "Path.Replace('\', '/')";
            expected.property.escaped       = false;
            expected.property.project       = null;
            expected.property.completed     = false;
            expected.property.complex       = true;
            expected.property.unevaluated   = "$(Path.Replace('\', '/'))";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }        

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest10()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(var=$(Path:project))";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "$(Path:project)";
            expected.property.escaped       = false;
            expected.property.project       = "project";
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest11()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "($(Path:project))";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = null;
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "$(Path:project)";
            expected.property.escaped       = false;
            expected.property.project       = "project";
            expected.property.completed     = true;
            expected.property.complex       = false;
            expected.property.unevaluated   = "Path";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest12()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string raw = "(var=$$(Path:project))";

            TPreparedData expected  = new TPreparedData();
            expected.variable.name          = "var";
            expected.variable.project       = null;
            expected.variable.isPersistence = false;
            expected.property.raw           = "$$(Path:project)";
            expected.property.escaped       = true;
            expected.property.project       = null;
            expected.property.completed     = true;
            expected.property.complex       = true;
            expected.property.unevaluated = "$(Path:project)";

            TPreparedData actual = target.prepareVariables(raw);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for evaluateVariable
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void evaluateVariableTest()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            TPreparedData prepared  = new TPreparedData();
            prepared.variable.name          = "var";
            prepared.variable.project       = "project";
            prepared.variable.isPersistence = false;
            prepared.property.raw           = "$$(Path:project2)";
            prepared.property.escaped       = true;
            prepared.property.project       = null;
            prepared.property.completed     = true;
            prepared.property.complex       = true;
            prepared.property.unevaluated   = "$(Path:project2)";

            Assert.IsTrue(target.definitions.Count < 1);
            Assert.AreEqual(String.Empty, target.evaluateVariable(prepared));
            Assert.IsTrue(target.definitions.Count == 1);
        }

        /// <summary>
        ///A test for parseVariablesMSBuild
        ///</summary>
        [TestMethod()]
        public void parseVariablesMSBuildTest()
        {
            MSBuildParser target = new MSBuildParser((DTE2)null);

            string actual   = target.parseVariablesMSBuild("$$(Path:project)");
            string expected = "$(Path:project)";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesMSBuild
        ///</summary>
        [TestMethod()]
        public void parseVariablesMSBuildTest2()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            string actual   = target.parseVariablesMSBuild("FooBar");
            string expected = "FooBar";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesMSBuild
        ///</summary>
        [TestMethod()]
        public void parseVariablesMSBuildTest3()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);
                        
            string actual   = target.parseVariablesMSBuild("$$(Path.Replace('\', '/'):project)");
            string expected = "$(Path.Replace('\', '/'):project)";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesMSBuild
        ///</summary>
        [TestMethod()]
        public void parseVariablesMSBuildTest4()
        {
            MSBuildParser_Accessor target = new MSBuildParser_Accessor((DTE2)null);

            target.definitions["var:project"] = "is a Windows_NT"; //"$(var.Replace('%OS%', $(OS)):project)";

            string actual   = target.parseVariablesMSBuild("$(var:project)");
            Assert.AreEqual("is a Windows_NT", actual);
        }

        /// <summary>
        ///A test for parseVariablesMSBuild
        ///</summary>
        
        class AccessorToParseVariablesMSBuildTest5: MSBuildParser
        {
            public override string evaluateVariable(string u, string p) { return u; }
            public AccessorToParseVariablesMSBuildTest5() : base((DTE2)null) { }
        }

        [TestMethod()]
        public void parseVariablesMSBuildTest5()
        {
            string data     = "$((var.Replace('%OS%', $(OS)):project).Concat($($(Path:project2).Replace('.', '_')), ' ($()) '))";
            string actual   = (new AccessorToParseVariablesMSBuildTest5()).parseVariablesMSBuild(data);
            Assert.AreEqual(data, actual);
        }
    }
}