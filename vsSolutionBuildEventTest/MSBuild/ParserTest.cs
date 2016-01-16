using System;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.MSBuild
{
    /// <summary>
    ///This is a test class for MSBuildParserTest and is intended
    ///to contain all MSBuildParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ParserTest
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

            Parser target = new Parser(new net.r_eg.vsSBE.Environment(mockDte2.Object));
            Assert.IsNotNull(target.getProperty("Configuration"));
            Assert.IsNotNull(target.getProperty("Platform"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest1()
        {
            Parser target = new Parser(new StubEnv());

            string actual   = target.parse("FooBar");
            string expected = "FooBar";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            string data     = "$(Path)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[P~Path~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest3()
        {
            string data     = "$$(Path)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("$(Path)", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest4()
        {
            string data     = "$$$(Path)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("$$(Path)", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            string data     = "(Path)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("(Path)", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest6()
        {
            string data     = "$(Path:project)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[P~Path~project]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest7()
        {
            string data = "$$(Path:project)";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("$(Path:project)", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest8()
        {
            string data     = "$([System.DateTime]::UtcNow.Ticks)";
            string actual   = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~[System.DateTime]::UtcNow.Ticks~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest9()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(Path.Replace('\\\\', '/'))";
            Assert.AreEqual("[E~Path.Replace('\\\\', '/')~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest10()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(Path.Replace('\\', '/'))";
            Assert.AreEqual("[E~Path.Replace('\\', '/')~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest11()
        {
            string data = "$(ProjectDir.Replace('\\', '/'):client)";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('\\', '/')~client]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest12()
        {
            Parser target = new Parser(new StubEnv());

            string actual = target.parse("$$(Path.Replace('\', '/'):project)");
            string expected = "$(Path.Replace('\', '/'):project)";

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest13()
        {
            string data = "$(ProjectDir.Replace(\"str1\", \"str2\"))";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace(\"str1\", \"str2\")~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest14()
        {
            string data = "$(ProjectDir.Replace('str1', 'str2'))";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~ProjectDir.Replace('str1', 'str2')~]", actual);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest15()
        {
            string data = "$(var.Method('~str~', $(OS:$($(data):project2)), \"~str2~\"):project)";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~var.Method('~str~', [P~OS~[E~[P~data~]~project2]], \"~str2~\")~project]", actual);
        }

        /// <summary>
        ///A test for parse - Wrapping
        ///</summary>
        [TestMethod()]
        public void parseWrappingTest1()
        {
            string data = "$($(ProjectDir.Replace('\\', '/'):client))";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~[E~ProjectDir.Replace('\\', '/')~client]~]", actual);
        }

        /// <summary>
        ///A test for parse - Wrapping
        ///</summary>
        [TestMethod()]
        public void parseWrappingTest2()
        {
            string data = "$($(ProjectDir.Replace('\\', '/')))";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~[E~ProjectDir.Replace('\\', '/')~]~]", actual);
        }

        /// <summary>
        ///A test for parse - Wrapping
        ///</summary>
        [TestMethod()]
        public void parseWrappingTest3()
        {
            string data = "$($(var.Method('str', $(OS))):$(var.Method('str2', $(SO))))";
            string actual = (new MSBuildParserAccessor.ToParse()).parse(data);
            Assert.AreEqual("[E~[E~var.Method('str', [P~OS~])~]:[E~var.Method('str2', [P~SO~])~]~]", actual);
        }

        /// <summary>
        ///A test for parse - Wrapping
        ///</summary>
        [TestMethod()]
        public void parseWrappingTest4()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$($(Path:project))";
            string actual = target.parse(data);
            Assert.IsTrue("[E~[P~Path~project]~]" == actual || "[P~[P~Path~project]~]" == actual);
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest1()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var=$(Path.Replace('\\', '/')):project)";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual(null, target.uvariable.get("var"));
            Assert.AreEqual("[E~Path.Replace('\\', '/')~]", target.uvariable.get("var", "project"));
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest2()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = $$(Path:project))";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$(Path:project)", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest3()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$$(var = $$(Path:project))";
            Assert.AreEqual("$(var = $$(Path:project))", target.parse(data));
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest4()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$$(var = $(Path:project))";
            Assert.AreEqual("$(var = $(Path:project))", target.parse(data));
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest5()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = $(Path:project))";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[P~Path~project]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest6()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var=$(Path:project2):project)";

            Assert.IsTrue(target.uvariable.Variables.Count() < 1);
            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[P~Path~project2]", target.uvariable.get("var", "project"));
            Assert.IsTrue(target.uvariable.Variables.Count() == 1);
        }

        /// <summary>
        ///A test for parse - Variable & data
        ///</summary>
        [TestMethod()]
        public void parseVarAndDataTest7()
        {
            MSBuildParserAccessor.ToUserVariables target = new MSBuildParserAccessor.ToUserVariables();

            target.uvariable.set("var", "project", "is a Windows_NT"); //"$(var.Replace('%OS%', $(OS)):project)";

            string actual = target.parse("$(var:project)");
            Assert.AreEqual("is a Windows_NT", actual);
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest1()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $(p1))$(p2)";
            Assert.AreEqual("[P~Platform~]", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest2()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$(p1))$(p2)";
            Assert.AreEqual("[P~Platform~]", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
            Assert.AreEqual("$(p1)", target.uvariable.get("p2"));
        }


        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest3()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$$(p1))$(p2)";
            Assert.AreEqual("$(p1)", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
            Assert.AreEqual("$$(p1)", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest4()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $(p2 = $$$$(p1))
                $(p2)
             */
            string data = "$(p1 = $(Platform))$(p2 = $$$$(p1))$(p2)";
            Assert.AreEqual("$$(p1)", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
            Assert.AreEqual("$$$(p1)", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest5()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(p2 = $(Platform))";
            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest6()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(p2 = $$(Platform))";
            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$(Platform)", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest7()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(p2 = $$$(Platform))";
            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$$(Platform)", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest8()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(p2 = $$$$(Platform))";
            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$$$(Platform)", target.uvariable.get("p2"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest9()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $(p1)
             */
            string data = "$(p1 = $(Platform))$(p1)";
            Assert.AreEqual("[P~Platform~]", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
        }

        /// <summary>
        ///A test for parse - Escape & Variable
        ///</summary>
        [TestMethod()]
        public void parseEscapeAndVarTest10()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            /*
                $(p1 = $(Platform))
                $$(p1)
             */
            string data = "$(p1 = $(Platform))$$(p1)";
            Assert.AreEqual("$(p1)", target.parse(data));
            Assert.AreEqual("[P~Platform~]", target.uvariable.get("p1"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest1()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = \"$(Path:project)\")";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[P~Path~project]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest2()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = \" mixed $(Path:project) \" )";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual(" mixed [P~Path~project] ", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest3()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            Assert.AreEqual(String.Empty, target.parse("$(var = \" $$(Path:project) \")"));
            Assert.AreEqual(" $(Path:project) ", target.uvariable.get("var"));

            Assert.AreEqual(String.Empty, target.parse("$(var = ' $$(Path:project) ')"));
            Assert.AreEqual(" $$(Path:project) ", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest4()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = ' $(Path:project) ')";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual(" $(Path:project) ", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest5()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = @"$(var = '$(Path.Replace(\'1\', \'2\'))')";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$(Path.Replace('1', '2'))", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest6()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = '$(Path.Replace(\\\"1\\\", \\\"2\\\"))')";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("$(Path.Replace(\\\"1\\\", \\\"2\\\"))", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest7()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = \"$(Path.Replace(\\'1\\', \\'2\\'))\")";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[E~Path.Replace(\\'1\\', \\'2\\')~]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest8()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = \"$(Path.Replace(\\\"1\\\", \\\"2\\\"))\")";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[E~Path.Replace(\"1\", \"2\")~]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest9()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = @"$(var = $(Path.Replace(\'1\', '2')))";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual(@"[E~Path.Replace(\'1\', '2')~]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest10()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var = $(Path.Replace(\\\"1\\\", \"2\")))";

            Assert.AreEqual("", target.parse(data));
            Assert.AreEqual("[E~Path.Replace(\\\"1\\\", \"2\")~]", target.uvariable.get("var"));
        }

        /// <summary>
        ///A test for parse - string
        ///</summary>
        [TestMethod()]
        public void parseStringTest11()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();
            
            Assert.AreEqual("", target.parse("$(name = ' test 12345  -_*~!@#$%^&= :) ')"));
            Assert.AreEqual(" test 12345  -_*~!@#$%^&= :) ", target.uvariable.get("name"));

            Assert.AreEqual("", target.parse("$(name = ' $( -_*~!@#$%^&= :) ')"));
            Assert.AreEqual(" $( -_*~!@#$%^&= :) ", target.uvariable.get("name"));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest1()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = @"$(var.Method('str1', $(OS:$($(data.Replace('\', '/')):project2)), 'str2'):project)";
            Assert.AreEqual(@"[E~var.Method('str1', [E~OS:[E~[E~data.Replace('\', '/')~]~project2]~], 'str2')~project]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest2()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = @"$(var.Method('str1', $(OS:$($(data.Method2):project2)), 'str2'):project)";
            Assert.AreEqual(@"[E~var.Method('str1', [P~OS~[E~[E~data.Method2~]~project2]], 'str2')~project]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest3()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var.Method('str1', $(OS:$($([System.DateTime]::Parse(\"27.03.2015\").ToBinary()):project2)), 'str2'):project)";
            Assert.AreEqual("[E~var.Method('str1', [E~OS:[E~[E~[System.DateTime]::Parse(\"27.03.2015\").ToBinary()~]~project2]~], 'str2')~project]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest4()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$(var.Method('str1', $(OS:$($(test.Ticks):project2)), \\\"str2\\\"):project)";
            Assert.AreEqual("[E~var.Method('str1', [P~OS~[E~[E~test.Ticks~]~project2]], \\\"str2\\\")~project]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest5()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data     = "$(var.Method('str1', $(OS:$($(data):project2)), \\\"str2\\\"):project)";
            string actual   = target.parse(data);
            Assert.IsTrue("[E~var.Method('str1', [P~OS~[E~[P~data~]~project2]], \\\"str2\\\")~project]" == actual
                            || "[E~var.Method('str1', [P~OS~[P~[P~data~]~project2]], \\\"str2\\\")~project]" == actual);
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest6()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$([System.DateTime]::Parse(\"01.01.2000\").ToBinary())";
            Assert.AreEqual("[E~[System.DateTime]::Parse(\"01.01.2000\").ToBinary()~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest7()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$([System.DateTime]::Parse(\" left $([System.DateTime]::UtcNow.Ticks) right\").ToBinary())";
            Assert.AreEqual("[E~[System.DateTime]::Parse(\" left [E~[System.DateTime]::UtcNow.Ticks~] right\").ToBinary()~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest8()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$([System.DateTime]::Parse(' left $([System.DateTime]::UtcNow.Ticks) right').ToBinary())";
            Assert.AreEqual("[E~[System.DateTime]::Parse(' left [E~[System.DateTime]::UtcNow.Ticks~] right').ToBinary()~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - nested
        ///</summary>
        [TestMethod()]
        public void parseNestedTest9()
        {
            MSBuildParserAccessor.ToParse target = new MSBuildParserAccessor.ToParse();

            string data = "$([System.TimeSpan]::FromTicks($([MSBuild]::Subtract($([System.DateTime]::UtcNow.Ticks), $([System.DateTime]::Parse('27.03.2015').ToBinary())))).TotalMinutes.ToString(\"0\"))";
            Assert.AreEqual("[E~[System.TimeSpan]::FromTicks([E~[MSBuild]::Subtract([E~[System.DateTime]::UtcNow.Ticks~], [E~[System.DateTime]::Parse('27.03.2015').ToBinary()~])~]).TotalMinutes.ToString(\"0\")~]", target.parse(data));
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest1()
        {
            var target = new Parser(new StubEnv());

            string data = "$(myVar = $$(myVar))$(myVar)";
            Assert.AreEqual("$(myVar)", target.parse(data));
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest2()
        {
            var target = new Parser(new StubEnv());

            string data = "$(myVar = $(myVar))$(myVar)";
            Assert.AreEqual(Parser.PROP_VALUE_DEFAULT, target.parse(data));
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Exceptions.LimitException))]
        public void parseUnloopingTest3()
        {
            var target = new Parser(new StubEnv());
            // p1 -> p2 -> p1 ] p3 -> p2
            target.parse("$(p1 = $$(p2))$(p2 = $$(p1))$(p3 = $(p2))");
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(Exceptions.LimitException))]
        public void parseUnloopingTest4()
        {
            var target = new Parser(new StubEnv());
            // p4 -> p2 -> p3 -> p1 -> p4
            target.parse("$(p4 = $$(p2))$(p3 = $$(p1))$(p2 = $$(p3))$(p1 = $$(p4))$(p4)");
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest5()
        {
            var target = new Parser(new StubEnv());
            Assert.AreEqual(Parser.PROP_VALUE_DEFAULT, target.parse("$(p2 = $$(p1))$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)"));
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest6()
        {
            var target = new Parser(new StubEnv());
            target.parse("$(p2 = \"$$(p1) to $$(p8),  and new ($$( p7.Replace('1', '2'))) s$$(p9)\")$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)");
            target.parse("$(p2 = \"$$(p1) to $$(p8),  and new ($$(p7.Replace('1', '2'))) s$$(p9)\")$(p6 = $$(p2))$(p7 = $$(p5))$(p5 = $(p6))$(p5)");
            Assert.IsTrue(true);
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest7()
        {
            var target = new Parser(new StubEnv());
            Assert.AreEqual(String.Format("2 {0} 2", Parser.PROP_VALUE_DEFAULT), 
                            target.parse("$(test = \"1 $(test) 2\")$(test = $(test.Replace('1', '2')))$(test)"));
        }

        /// <summary>
        ///A test for parse - unlooping
        ///</summary>
        [TestMethod()]
        public void parseUnloopingTest8()
        {
            var target = new Parser(new StubEnv());
            Assert.AreEqual("7", target.parse("$(test = 7)$(test = $(test))$(test)"));
        }

        /// <summary>
        ///A test for parse - quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest1()
        {
            var target = new Parser(new StubEnv());

            Assert.AreEqual(String.Empty, target.parse("$(name = \" $([System.Math]::Pow(2, 16)) \")"));
            Assert.AreEqual(" 65536 ", target.UVariable.get("name", null));

            Assert.AreEqual(String.Empty, target.parse("$(name = ' $([System.Math]::Pow(2, 16)) ')"));
            Assert.AreEqual(" $([System.Math]::Pow(2, 16)) ", target.UVariable.get("name", null));
        }

        /// <summary>
        ///A test for parse - quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest2()
        {
            var target = new Parser(new StubEnv());
            Assert.AreEqual(" left '123' right ", target.parse("$([System.String]::Format(\" left '{0}' right \", \"123\"))"));
            Assert.AreEqual(" left '123' ) right ", target.parse("$([System.String]::Format(\" left '{0}' ) right \", \"123\"))"));

            Assert.AreEqual(" left \"123\" right ", target.parse("$([System.String]::Format(' left \"{0}\" right ', '123'))"));
            Assert.AreEqual(" left \"123\" ) right ", target.parse("$([System.String]::Format(' left \"{0}\" ) right ', '123'))"));
        }

        /// <summary>
        ///A test for parse - quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest3()
        {
            var target = new Parser(new StubEnv());

            Assert.AreEqual(String.Empty, target.parse("$(tpl = \"My version - '%Ver%'\")"));
            Assert.AreEqual("My version - '%Ver%'", target.UVariable.get("tpl", null));

            Assert.AreEqual(String.Empty, target.parse("$(ver = '1.2.3')"));
            Assert.AreEqual("1.2.3", target.UVariable.get("ver", null));

            Assert.AreEqual(String.Empty, target.parse("$(rev = '2417')"));
            Assert.AreEqual("2417", target.UVariable.get("rev", null));

            Assert.AreEqual("My version - '1, 2, 3, 2417'", target.parse("$(tpl.Replace(\"%Ver%\", \"$(ver.Replace('.', ', ')), $(rev)\"))"));
            Assert.AreEqual("1.2.3 version - '1.2.3.2417'", target.parse("$(tpl.Replace(\"%Ver%\", \"$(ver).$(rev)\").Replace(\"My\", \"$(ver)\"))"));
        }

        /// <summary>
        ///A test for parse - quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest4()
        {
            var target = new Parser(new StubEnv());

            target.UVariable.set("name", "project", "test123");
            target.UVariable.evaluate("name", "project", new EvaluatorBlank(), true);

            Assert.AreEqual("test123", target.parse("$([System.String]::Concat('$(name:project)'))"));
            Assert.AreEqual("test123", target.parse("$([System.String]::Concat(\"$(name:project)\"))"));

            //target.parse("$([System.DateTime]::Parse(\"$([System.DateTime]::UtcNow.Ticks)\").ToBinary())");
        }

        /// <summary>
        ///A test for parse - quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest5()
        {
            var target = new Parser(new StubEnv());

            target.UVariable.set("name", null, "test123");
            target.UVariable.evaluate("name", null, new EvaluatorBlank(), true);

            string data = "$([System.String]::Concat(\"$(name)\"))";
            Assert.AreEqual("test123", target.parse(data));
        }

        /// <summary>
        /// Evaluation from quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest6()
        {
            var target = new Parser(new StubEnv());

            Assert.AreEqual(String.Empty, target.parse("$(version = \"1.2.3\")"));
            Assert.AreEqual(String.Empty, target.parse("$(tpl = \"My version - $(version), \\\"$(version)\\\", '$(version)' end.\")"));
            Assert.AreEqual("My version - 1.2.3, \"1.2.3\", '1.2.3' end.", target.parse("$(tpl)"));
        }

        /// <summary>
        /// Evaluation from quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest7()
        {
            var uvar    = new UserVariable();
            var target  = new Parser(new StubEnv(), uvar);

            uvar.set("lp", null, "s1\\dir");
            uvar.evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.AreEqual("s1\\dir", uvar.get("lp", null));

            Assert.AreEqual("\"s1\\dir\\p1.exe\"", target.parse("\"$(lp)\\p1.exe\""));
            Assert.AreEqual("'s1\\dir\\p2.exe'", target.parse("'$(lp)\\p2.exe'"));
            Assert.AreEqual("s1\\dir\\p3.exe", target.parse("$(lp)\\p3.exe"));
        }

        /// <summary>
        /// Evaluation from quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest8()
        {
            var uvar    = new UserVariable();
            var target  = new Parser(new StubEnv(), uvar);

            uvar.set("lp", null, "'s2\\dir'");
            uvar.evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.AreEqual("'s2\\dir'", uvar.get("lp", null));

            Assert.AreEqual("\"'s2\\dir'\\p1.exe\"", target.parse("\"$(lp)\\p1.exe\""));
            //Assert.AreEqual("''s2\\dir'\\p2.exe'", target.parse("'$(lp)\\p2.exe'")); // ? TODO: unspecified for current time
            Assert.AreEqual("'s2\\dir'\\p3.exe", target.parse("$(lp)\\p3.exe"));
        }

        /// <summary>
        /// Evaluation from quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest9()
        {
            var uvar    = new UserVariable();
            var target  = new Parser(new StubEnv(), uvar);

            uvar.set("lp", null, "\"s3\\dir\"");
            uvar.evaluate("lp", null, new EvaluatorBlank(), true);
            Assert.AreEqual("\"s3\\dir\"", uvar.get("lp", null));

            //Assert.AreEqual("\"\"s3\\dir\"\\p1.exe\"", target.parse("\"$(lp)\\p1.exe\"")); // ? TODO: unspecified for current time
            Assert.AreEqual("'\"s3\\dir\"\\p2.exe'", target.parse("'$(lp)\\p2.exe'"));
            Assert.AreEqual("\"s3\\dir\"\\p3.exe", target.parse("$(lp)\\p3.exe"));
        }

        /// <summary>
        /// Evaluation from quotes
        ///</summary>
        [TestMethod()]
        public void quotesTest10()
        {
            var target = new Parser(new StubEnv());

            target.UVariable.set("name", null, "test123");
            target.UVariable.evaluate("name", null, new EvaluatorBlank(), true);

            Assert.AreEqual("test123)", target.parse("$([System.String]::Concat(\"$(name))\"))"));
            Assert.AreEqual("(test123", target.parse("$([System.String]::Concat(\"($(name)\"))"));

            Assert.AreEqual("(test123", target.parse("$([System.String]::Concat('($(name)'))"));
            Assert.AreEqual("test123)", target.parse("$([System.String]::Concat('$(name))'))"));

            Assert.AreEqual(" left ) test123 ", target.parse("$([System.String]::Concat(\" left ) $(name) \"))"));
            Assert.AreEqual(" left ) test123 ", target.parse("$([System.String]::Concat(' left ) $(name) '))"));

            Assert.AreEqual(" left () test123 ", target.parse("$([System.String]::Concat(\" left () $(name) \"))"));
            Assert.AreEqual(" left () test123 ", target.parse("$([System.String]::Concat(' left () $(name) '))"));
        }

        /// <summary>
        /// 
        /// </summary>
        private class MSBuildParserAccessor
        {
            public class Accessor: vsSBE.MSBuild.Parser
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
                    if(uvariable.isExist(name, project)) {
                        return getUVariableValue(name, project);
                    }
                    return String.Format("[P~{0}~{1}]", name, project);
                }
            }

            public class ToUserVariables: StubEvaluatingProperty
            {
                public new IUserVariable uvariable
                {
                    get { return base.uvariable; }
                    set { base.uvariable = value; }
                }
            }

            public class ToParse: ToUserVariables
            {

            }

            public class ToPrepareVariables: StubEvaluatingProperty
            {
                public new PreparedData prepare(string raw)
                {
                    return base.prepare(raw);
                }
            }

            public class ToEvaluateVariable: ToUserVariables
            {
                public new string evaluate(PreparedData prepared)
                {
                    return base.evaluate(prepared);
                }
            }
        }
    }
}