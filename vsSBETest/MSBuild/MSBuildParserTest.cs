using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.MSBuild;

namespace vsSBETest.MSBuild
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
        ///A test for getProperty
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void getPropertyTest()
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

            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment(mockDte2.Object));
            Assert.IsNotNull(target.getProperty("Configuration"));
            Assert.IsNotNull(target.getProperty("Platform"));
        }

        /// <summary>
        ///A test for parseVariablesSBE
        ///</summary>
        [TestMethod()]
        public void parseVariablesSBETest()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string expected = "$(name)";
            string actual   = target.parseVariablesSBE("$(name)", "subname", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesSBE
        ///</summary>
        [TestMethod()]
        public void parseVariablesSBETest2()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string expected = "value";
            string actual   = target.parseVariablesSBE("$(name)", "name", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesSBE
        ///</summary>
        [TestMethod()]
        public void parseVariablesSBETest3()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string expected = "$$(name)";
            string actual   = target.parseVariablesSBE("$$(name)", "name", "value");
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parseVariablesSBE
        ///</summary>
        [TestMethod()]
        public void parseVariablesSBETest4()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string expected = String.Empty;
            string actual   = target.parseVariablesSBE("$(name)", "name", null);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void prepareVariablesTest()
        {
            (new MSBuildParserAccessor.ToPrepareVariables()).prepareVariables("var=$(Path:project2):project");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void prepareVariablesTest2()
        {
            (new MSBuildParserAccessor.ToPrepareVariables()).prepareVariables("$(var=$(Path:project2):project)");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest3()
        {
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

            string raw              = "(var=$(Path.Replace('\', '/')):project)";
            TPreparedData actual    = target.prepareVariables(raw);

            Assert.AreEqual(actual.variable.name,           "var");
            Assert.AreEqual(actual.variable.project,        "project");
            Assert.AreEqual(actual.variable.isPersistence,  false);
            Assert.AreEqual(actual.property.raw,            "$(Path.Replace('\', '/'))");
            Assert.AreEqual(actual.property.escaped,        false);
            Assert.AreEqual(actual.property.project,        null);
            //Assert.AreEqual(actual.property.completed,      true);
            Assert.AreEqual(actual.property.complex,        true);
            Assert.AreEqual(actual.property.unevaluated,    "$(Path.Replace('\', '/'))");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest9()
        {
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

            string raw              = "(Path.Replace('\', '/'))";
            TPreparedData actual    = target.prepareVariables(raw);
            
            Assert.AreEqual(actual.variable.name,           null);
            Assert.AreEqual(actual.variable.project,        null);
            Assert.AreEqual(actual.variable.isPersistence,  false);
            Assert.AreEqual(actual.property.raw,            "Path.Replace('\', '/')");
            Assert.AreEqual(actual.property.escaped,        false);
            Assert.AreEqual(actual.property.project,        null);
            //Assert.AreEqual(actual.property.completed,      true);
            Assert.AreEqual(actual.property.complex,        true);
            Assert.AreEqual(actual.property.unevaluated,    "$(Path.Replace('\', '/'))");
        }

        /// <summary>
        ///A test for prepareVariables
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void prepareVariablesTest10()
        {
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToPrepareVariables target = new MSBuildParserAccessor.ToPrepareVariables();

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
            MSBuildParserAccessor.ToEvaluateVariable target = new MSBuildParserAccessor.ToEvaluateVariable();

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

            Assert.IsTrue(target.uvariable.Variables.Count() < 1);
            Assert.AreEqual(String.Empty, target.evaluateVariable(prepared));
            Assert.IsTrue(target.uvariable.Variables.Count() == 1);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string actual   = target.parse("$$(Path:project)");
            string expected = "$(Path:project)";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));

            string actual   = target.parse("FooBar");
            string expected = "FooBar";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest3()
        {
            MSBuildParser target = new MSBuildParser(new net.r_eg.vsSBE.Environment((DTE2)null));
                        
            string actual   = target.parse("$$(Path.Replace('\', '/'):project)");
            string expected = "$(Path.Replace('\', '/'):project)";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            MSBuildParserAccessor.ToUserVariables target = new MSBuildParserAccessor.ToUserVariables();

            target.uvariable.set("var", "project", "is a Windows_NT"); //"$(var.Replace('%OS%', $(OS)):project)";

            string actual   = target.parse("$(var:project)");
            Assert.AreEqual("is a Windows_NT", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            string data     = "$(ProjectDir.Replace(\"str1\", \"str2\"))";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace(\"str1\", \"str2\")~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            string data     = "$($(ProjectDir.Replace('\\', '/'):client))";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('\\', '/')~client]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest7()
        {
            string data     = "$(ProjectDir.Replace('\\', '/'):client)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('\\', '/')~client]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest8()
        {
            string data     = "$(ProjectDir.Replace('\\', '/'))";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('\\', '/')~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest9()
        {
            string data     = "$($(ProjectDir.Replace('\\', '/')))";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('\\', '/')~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest10()
        {
            string data     = "$(var.Method('~str~', $(OS:$($(data):project2)), \"~str2~\"):project)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~var.Method('~str~', [P~OS~[E~[P~data~]~project2]], \"~str2~\")~project]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            string data     = "$($(var.Method('str', $(OS))):$(var.Method('str2', $(SO))))";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~var.Method('str', [P~OS~])~]:[E~var.Method('str2', [P~SO~])~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            string data     = "$(Path)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[P~Path~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest13()
        {
            string data     = "$(Path:project)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[P~Path~project]", actual);
        }
    }

    internal class MSBuildParserAccessor
    {
        public class Accessor: MSBuildParser
        {
            public Accessor(): base(new net.r_eg.vsSBE.Environment((DTE2)null)) {}
            public Accessor(net.r_eg.vsSBE.Environment env): base(env) { }
        }

        public class StubEvaluatingProperty: Accessor
        {
            public override string evaluate(string unevaluated, string project)
            {
                return String.Format("[E~{0}~{1}]", unevaluated, project);
            }

            public override string getProperty(string name, string project)
            {
                string def = uvariable.get(name, project);
                return (def == null)? String.Format("[P~{0}~{1}]", name, project) : def;
            }
        }

        public class ToParse: StubEvaluatingProperty
        {

        }

        public class ToUserVariables: StubEvaluatingProperty
        {
            public new net.r_eg.vsSBE.SBEScripts.IUserVariable uvariable
            {
                get { return base.uvariable; }
                set { base.uvariable = value; }
            }
        }

        public class ToPrepareVariables: StubEvaluatingProperty
        {
            public new TPreparedData prepareVariables(string raw)
            {
                return base.prepareVariables(raw);
            }
        }

        public class ToEvaluateVariable: ToUserVariables
        {
            public new string evaluateVariable(TPreparedData prepared)
            {
                return base.evaluateVariable(prepared);
            }
        }
    }
}