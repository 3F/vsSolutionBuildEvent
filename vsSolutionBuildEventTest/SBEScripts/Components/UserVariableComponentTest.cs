using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.Varhead;
using net.r_eg.Varhead.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    /// <summary>
    ///This is a test class for UserVariableComponentTest and is intended
    ///to contain all UserVariableComponentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserVariableComponentTest
    {
        private IEnvironment envmock = (new Mock<IEnvironment>()).Object;

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest()
        {
            UserVariableComponent target = new UserVariableComponent(new Soba());
            target.parse("#[var name = value]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest2()
        {
            UserVariableComponent target = new UserVariableComponent(new Soba());
            target.parse("var name = value");
        }

        /// <summary>
        ///A test for parse -> '-' operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(DefinitionNotFoundException))]
        public void parseTest3()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var -name]"));
            Assert.AreEqual("[E1:value]", target.parse("[var name]"));
        }

        /// <summary>
        ///A test for parse -> '-' operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest4()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var -name = value]"));
        }

        /// <summary>
        ///A test for parse -> '+' operation
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var +name]"));
            Assert.AreEqual(String.Format("[E1:{0}]", UserVariableComponent.UVARIABLE_VALUE_DEFAULT), target.parse("[var name]"));
        }

        /// <summary>
        ///A test for parse -> '+' operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest6()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var +name = value]"));
        }

        /// <summary>
        ///A test for parse -> '-' operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest7()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var -name = value]"));
        }

        /// <summary>
        ///A test for parse -> '+' operation
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest8()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var + name]"));
        }

        /// <summary>
        ///A test for std
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(DefinitionNotFoundException))]
        public void stdTest1()
        {
            UserVariableComponent target = new UserVariableComponent(new Soba());
            target.parse("[var name]");
        }

        /// <summary>
        ///A test for std
        ///</summary>
        [TestMethod()]
        public void stdTest2()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual("[E1:value]", target.parse("[var name]"));
        }

        /// <summary>
        ///A test for std
        ///</summary>
        [TestMethod()]
        public void stdTest3()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = line1 \n line2]"));
            Assert.AreEqual("[E1:line1 \n line2]", target.parse("[var name]"));
        }

        /// <summary>
        ///A test for std
        ///</summary>
        [TestMethod()]
        public void stdTest4()
        {
            UserVariableComponentAccessor target = new UserVariableComponentAccessor(new UVars());
            Assert.AreEqual(Value.Empty, target.parse("[var name = value]"));
            Assert.AreEqual(Value.Empty, target.parse("[var name = value2]"));
            Assert.AreEqual("[E1:value2]", target.parse("[var name]"));
        }

        private class UserVariableComponentAccessor: UserVariableComponent
        {
            public UserVariableComponentAccessor(IUVars uvariable)
                : base(new Soba(uvariable))
            {

            }

            protected override void evaluate(string name, string project = null)
            {
                uvars.Evaluate(name, project, new Evaluator1(), true);
            }
        }

        private class Evaluator1: IEvaluator
        {
            public string Evaluate(string data)
            {
                return $"[E1:{data}]";
            }
        }
    }
}
