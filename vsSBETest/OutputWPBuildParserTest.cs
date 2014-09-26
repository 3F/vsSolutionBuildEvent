using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for OutputWPBuildParserTest and is intended
    ///to contain all OutputWPBuildParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OutputWPBuildParserTest
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
        ///A test for OutputWPBuildParser Constructor
        ///</summary>
        [TestMethod()]
        public void OutputWPBuildParserConstructorTest()
        {
            string rawdata          = "warning CS1762: A reference was created to embedded interop assembly";
            string rawdataExpected  = rawdata;

            OutputWPBuildParser target = new OutputWPBuildParser(ref rawdata);
            Assert.AreEqual(rawdataExpected, rawdata);
        }
        
        /// <summary>
        ///A test for extract
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void extractTest()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToExtract target = new OutputWPBuildParserAccessor.ToExtract(ref rawdata);

            Assert.IsTrue(target.ErrorsCount < 1);
            Assert.IsTrue(target.WarningsCount < 1);

            target.rawdata = "2>  thread_clock.cpp";
            target.extract();

            Assert.IsTrue(target.ErrorsCount < 1);
            Assert.IsTrue(target.WarningsCount < 1);
        }
        
        /// <summary>
        ///A test for extract
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void extractTest2()
        {
            string rawdata = @"9>C:\VC\atlmfc\include\atlhost.h(422): warning C4505: 'ATL::CAxHostWindow::AddRef' : unreferenced local function has been removed";
            OutputWPBuildParser target = new OutputWPBuildParser(ref rawdata);
            Assert.IsTrue(target.ErrorsCount < 1);
            Assert.IsTrue(target.WarningsCount == 1);
        }

        /// <summary>
        ///A test for extract
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void extractTest3()
        {
            string rawdata = @"11>windows\Search.cpp(2246): error C4430: missing type specifier - int assumed. Note: C++ does not support default-int";
            OutputWPBuildParser target = new OutputWPBuildParser(ref rawdata);
            Assert.IsTrue(target.ErrorsCount == 1);
            Assert.IsTrue(target.WarningsCount < 1);
        }
        
        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);
            
            target.warnings = new List<string>() { "C4505" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Warnings, true, new List<string>()));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Errors, true, new List<string>()));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest2()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Warnings, true, new List<string>() { "C4506" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Errors, true, new List<string>() { "C4431" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest3()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Warnings, true, new List<string>() { "C4507" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Errors, true, new List<string>() { "C4432" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest4()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Warnings, false, new List<string>()));
            target.warnings.Clear();
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Warnings, false, new List<string>()));

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Errors, false, new List<string>()));
            target.errors.Clear();
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Errors, false, new List<string>()));
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest5()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);

            target.warnings = new List<string>() { "C4505", "C4507" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Warnings, false, new List<string>() { "C4507" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430", "C4432" };
            Assert.AreEqual(true, target.checkRule(OutputWPBuildParser.Type.Errors, false, new List<string>() { "C4432" }));
            target.errors.Clear();
        }

        /// <summary>
        ///A test for checkRule
        ///</summary>
        [TestMethod()]
        public void checkRuleTest6()
        {
            string rawdata = String.Empty;
            OutputWPBuildParserAccessor.ToCheckRule target = new OutputWPBuildParserAccessor.ToCheckRule(ref rawdata);

            target.warnings = new List<string>() { "C4505" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Warnings, false, new List<string>() { "C4505" }));
            target.warnings.Clear();

            target.errors = new List<string>() { "C4430" };
            Assert.AreEqual(false, target.checkRule(OutputWPBuildParser.Type.Errors, false, new List<string>() { "C4430" }));
            target.errors.Clear();
        }

        internal class OutputWPBuildParserAccessor
        {
            public class Accessor: OutputWPBuildParser
            {
                public Accessor(ref string rawdata): base(ref rawdata) { }
            }

            public class ToCheckRule: Accessor
            {
                public new List<string> errors
                {
                    get { return base.errors; }
                    set { base.errors = value; }
                }

                public new List<string> warnings
                {
                    get { return base.warnings; }
                    set { base.warnings = value; }
                }

                public ToCheckRule(ref string rawdata): base(ref rawdata) { }
            }

            public class ToExtract: Accessor
            {
                public new string rawdata
                {
                    get { return base.rawdata; }
                    set { base.rawdata = value; }
                }

                public new void extract()
                {
                    base.extract();
                }

                public ToExtract(ref string rawdata): base(ref rawdata) { }
            }
        }
    }
}