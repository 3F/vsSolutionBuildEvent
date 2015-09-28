using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE.Exceptions;
using net.r_eg.vsSBE.Scripts;

namespace net.r_eg.vsSBE.Test.Scripts
{
    /// <summary>
    ///This is a test class for UserVariableTest and is intended
    ///to contain all UserVariableTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserVariableTest
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
        ///A test for Definitions
        ///</summary>
        [TestMethod()]
        public void DefinitionsTest()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(target.Definitions.Count(), 0);
        }

        /// <summary>
        ///A test for Variables
        ///</summary>
        [TestMethod()]
        public void VariablesTest()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(target.Variables.Count(), 0);
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        public void setTest()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "unevaluated");

            Assert.AreEqual(1, target.Definitions.Count());
            foreach(string v in target.Definitions) {
                Assert.AreEqual(v, "name");
            }
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        public void setTest2()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.set(name, project, "unevaluated");

            Assert.AreEqual(1, target.Definitions.Count());
            foreach(string v in target.Definitions) {
                Assert.AreEqual(v, target.defIndex(name, project));
            }
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        public void setTest3()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "unevaluated");
            Assert.AreEqual(1, target.Variables.Count());

            foreach(TUserVariable v in target.Variables) {
                Assert.AreEqual(v.evaluated, null);
                Assert.AreEqual(v.ident, "name");
                Assert.AreEqual(v.persistence, false);
                Assert.AreEqual(v.prev, new TUserVariable());
                Assert.AreEqual(v.status, TUserVariable.StatusType.Unevaluated);
                Assert.AreEqual(v.unevaluated, "unevaluated");
            }
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        public void setTest4()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.set(name, project, "unevaluated");
            Assert.AreEqual(1, target.Variables.Count());

            foreach(TUserVariable v in target.Variables) {
                Assert.AreEqual(v.evaluated, null);
                Assert.AreEqual(v.ident, target.defIndex(name, project));
                Assert.AreEqual(v.persistence, false);
                Assert.AreEqual(v.prev, new TUserVariable());
                Assert.AreEqual(v.status, TUserVariable.StatusType.Unevaluated);
                Assert.AreEqual(v.unevaluated, "unevaluated");
            }
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void setTest5()
        {
            UserVariable target = new UserVariable();
            target.set("na%me", "project", "unevaluated");
        }

        /// <summary>
        ///A test for set
        ///</summary>
        [TestMethod()]
        public void setTest6()
        {
            UserVariable target = new UserVariable();
            target.set("name", "project", null);

            Assert.AreEqual(1, target.Variables.Count());
            foreach(TUserVariable v in target.Variables) {
                Assert.AreEqual(v.unevaluated, String.Empty);
            }
        }

        /// <summary>
        ///A test for unsetAll
        ///</summary>
        [TestMethod()]
        public void unsetAllTest()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "unevaluated");
            target.unsetAll();
            Assert.AreEqual(target.Variables.Count(), 0);
        }

        /// <summary>
        ///A test for unset
        ///</summary>
        [TestMethod()]
        public void unsetTest()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "unevaluated");
            target.set("name2", null, "unevaluated2");

            target.unset("name", null);
            Assert.AreEqual(1, target.Definitions.Count());

            foreach(string v in target.Definitions) {
                Assert.AreEqual(v, "name2");
            }
        }

        /// <summary>
        ///A test for unset
        ///</summary>
        [TestMethod()]
        public void unsetTest3()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "unevaluated");
            target.set("name2", null, "unevaluated2");

            target.unset("name");
            Assert.AreEqual(1, target.Definitions.Count());

            foreach(string v in target.Definitions) {
                Assert.AreEqual(v, "name2");
            }
        }

        /// <summary>
        ///A test for isValidName
        ///</summary>
        [TestMethod()]
        public void isValidNameTest()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(true, target.isValidName("name"));
            Assert.AreEqual(true, target.isValidName("n"));
            Assert.AreEqual(true, target.isValidName("name0"));
            Assert.AreEqual(true, target.isValidName("name_n"));
            Assert.AreEqual(false, target.isValidName("0name"));
            Assert.AreEqual(false, target.isValidName(""));
            Assert.AreEqual(false, target.isValidName("na%me"));
            Assert.AreEqual(false, target.isValidName("name "));
        }

        /// <summary>
        ///A test for isUnevaluated
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void isUnevaluatedTest()
        {
            UserVariable target = new UserVariable();
            target.isUnevaluated("name", "project");
        }

        /// <summary>
        ///A test for isUnevaluated
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void isUnevaluatedTest2()
        {
            UserVariable target = new UserVariable();
            target.isUnevaluated("name");
        }

        /// <summary>
        ///A test for isUnevaluated
        ///</summary>
        [TestMethod()]
        public void isUnevaluatedTest3()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "val");
            Assert.AreEqual(true, target.isUnevaluated("name"));
        }

        /// <summary>
        ///A test for isUnevaluated
        ///</summary>
        [TestMethod()]
        public void isUnevaluatedTest4()
        {
            UserVariable target = new UserVariable();
            target.set("name", "project", "val");
            Assert.AreEqual(true, target.isUnevaluated("name", "project"));
        }

        /// <summary>
        ///A test for isExist
        ///</summary>
        [TestMethod()]
        public void isExistTest()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(false, target.isExist("name", "project"));
        }

        /// <summary>
        ///A test for isExist
        ///</summary>
        [TestMethod()]
        public void isExistTest2()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(false, target.isExist("name"));
        }

        /// <summary>
        ///A test for isExist
        ///</summary>
        [TestMethod()]
        public void isExistTest3()
        {
            UserVariable target = new UserVariable();
            target.set("name", "project", "val");
            Assert.AreEqual(true, target.isExist("name", "project"));
        }

        /// <summary>
        ///A test for isExist
        ///</summary>
        [TestMethod()]
        public void isExistTest4()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.set(name, project, "val");
            Assert.AreEqual(true, target.isExist(target.defIndex(name, project)));
        }

        /// <summary>
        ///A test for get
        ///</summary>
        [TestMethod()]
        public void getTest()
        {
            UserVariable target = new UserVariable();
            Assert.AreEqual(null, target.get("name"));
            Assert.AreEqual(null, target.get("name", "project"));
        }

        /// <summary>
        ///A test for get
        ///</summary>
        [TestMethod()]
        public void getTest2()
        {
            UserVariableAccessor target = new UserVariableAccessor();
            string name = "name";
            string project = "project";
            target.set(name, project, "val");
            Assert.AreEqual(String.Empty, target.get(target.defIndex(name, project)));
        }

        /// <summary>
        ///A test for get
        ///</summary>
        [TestMethod()]
        public void getTest3()
        {
            UserVariable target = new UserVariable();
            target.set("name", null, "val");
            Assert.AreEqual(String.Empty, target.get("name"));
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotFoundException))]
        public void evaluateTest()
        {
            UserVariable target = new UserVariable();
            target.evaluate("name", "project", new Evaluator1(), true);
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(NotFoundException))]
        public void evaluateTest2()
        {
            UserVariable target = new UserVariable();
            target.evaluate("name", new Evaluator1(), true);
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidArgumentException))]
        public void evaluateTest3()
        {
            UserVariable target = new UserVariable();
            string name = "name";
            string project = "project";

            target.set(name, project, "val");
            target.evaluate(name, project, null, true);
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        public void evaluateTest4()
        {
            UserVariable target = new UserVariable();
            string name = "name";
            string project = "project";

            target.set(name, project, "val");
            target.evaluate(name, project, new Evaluator1(), true);
            Assert.AreEqual("[E1:val]", target.get(name, project));
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        public void evaluateTest5()
        {
            UserVariable target = new UserVariable();
            string name = "name";
            string project = "project";

            target.set(name, project, "val");
            target.evaluate(name, project, new Evaluator1(), true);
            target.evaluate(name, project, new Evaluator2(), false);
            Assert.AreEqual("[E2:[E1:val]]", target.get(name, project));
        }

        /// <summary>
        ///A test for evaluate
        ///</summary>
        [TestMethod()]
        public void evaluateTest6()
        {
            UserVariable target = new UserVariable();
            string name = "name";
            string project = "project";

            target.set(name, project, "val");
            target.evaluate(name, project, new Evaluator1(), true);
            target.evaluate(name, project, new Evaluator2(), true);
            Assert.AreEqual("[E2:val]", target.get(name, project));
        }

        private class UserVariableAccessor: UserVariable
        {
            public new string defIndex(string name, string project)
            {
                return base.defIndex(name, project);
            }
        }

        private class Evaluator1: IEvaluator
        {
            public string evaluate(string data)
            {
                return String.Format("[E1:{0}]", data);
            }
        }

        private class Evaluator2: IEvaluator
        {
            public string evaluate(string data)
            {
                return String.Format("[E2:{0}]", data);
            }
        }
    }
}
