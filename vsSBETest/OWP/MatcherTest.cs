using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.OWP;

namespace vsSBETest.OWP
{
    /// <summary>
    ///This is a test class for MatcherTest and is intended
    ///to contain all MatcherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MatcherTest
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
            MatcherAccessor.ToWildcards target = new MatcherAccessor.ToWildcards();
            
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
            MatcherAccessor.ToWildcards target = new MatcherAccessor.ToWildcards();

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

        internal class MatcherAccessor
        {
            public class Accessor: Matcher {}

            public class ToWildcards: Accessor
            {
                public new bool mWildcards(string term, ref string raw)
                {
                    return base.mWildcards(term, ref raw);
                }
            }
        }
    }
}