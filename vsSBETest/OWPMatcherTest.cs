using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for OWPMatcherTest and is intended
    ///to contain all OWPMatcherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OWPMatcherTest
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
        ///A test for mWildcards
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void mWildcardsTest()
        {
            OWPMatcher_Accessor target = new OWPMatcher_Accessor();
            
            string raw          = "new tes;ted project-12, and 75_protection of various systems.";
            string rawExpected  = raw;
            
            bool actual = target.mWildcards("pro*system", ref raw);
            Assert.AreEqual(rawExpected, raw);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for mWildcards
        ///</summary>
        [TestMethod()]
        [DeploymentItem("vsSolutionBuildEvent.dll")]
        public void mWildcardsTest1()
        {
            OWPMatcher_Accessor target = new OWPMatcher_Accessor();

            string raw = "new tes;ted project-12, and 75_protection of various systems";
            
            Assert.AreEqual(true, target.mWildcards("new*7*systems", ref raw));
            Assert.AreEqual(true, target.mWildcards("", ref raw));
            Assert.AreEqual(true, target.mWildcards("*", ref raw));
            Assert.AreEqual(false, target.mWildcards("new*express", ref raw));
            Assert.AreEqual(false, target.mWildcards("tes*ting*project", ref raw));

            Assert.AreEqual(true, target.mWildcards("?", ref raw));
            Assert.AreEqual(true, target.mWildcards("project?12", ref raw));
            Assert.AreEqual(false, target.mWildcards("pro?tect", ref raw));

            Assert.AreEqual(true, target.mWildcards("+", ref raw));
            Assert.AreEqual(true, target.mWildcards("new+systems", ref raw));
            Assert.AreEqual(false, target.mWildcards("systems+", ref raw));
        }
    }
}