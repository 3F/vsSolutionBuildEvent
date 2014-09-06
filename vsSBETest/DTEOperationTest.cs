using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.vsSBE;
using net.r_eg.vsSBE.Exceptions;

namespace vsSBETest
{
    /// <summary>
    ///This is a test class for DTEOperationTest and is intended
    ///to contain all DTEOperationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DTEOperationTest
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
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest()
        {
            DTEOperation target = new DTEOperation((DTE)null);

            string line = "File.OpenProject(\"c:\\path\\app.sln\")";
            DTEOperation.TPrepared actual = target.parse(line);

            Assert.AreEqual("File.OpenProject", actual.name);
            Assert.AreEqual("\"c:\\path\\app.sln\"", actual.args);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            DTEOperation target = new DTEOperation((DTE)null);

            string line = "Debug.StartWithoutDebugging";
            DTEOperation.TPrepared actual = target.parse(line);

            Assert.AreEqual("Debug.StartWithoutDebugging", actual.name);
            Assert.AreEqual("", actual.args);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest3()
        {
            DTEOperation target = new DTEOperation((DTE)null);

            string line = "Debug StartWithoutDebugging";
            DTEOperation.TPrepared actual = target.parse(line);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest4()
        {
            DTEOperation target = new DTEOperation((DTE)null);

            string line = "";
            DTEOperation.TPrepared actual = target.parse(line);
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest5()
        {
            DTEOperation target = new DTEOperation((DTE)null);

            string line = "  OpenProject(arg)  ";
            DTEOperation.TPrepared actual = target.parse(line);

            Assert.AreEqual("OpenProject", actual.name);
            Assert.AreEqual("arg", actual.args);
        }

        /// <summary>
        ///A test for exec
        ///</summary>
        [TestMethod()]
        public void execTest()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(-1);

            Queue<DTEOperation.TPrepared> commands = new Queue<DTEOperation.TPrepared>();
            commands.Enqueue(new DTEOperation.TPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.TPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.TPrepared("Debug.Start", ""));

            DTEOperation.TPrepared[] expected = new DTEOperation.TPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            target.exec(commands, false);

            Queue<DTEOperation.TPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count == expected.Length);

            foreach(DTEOperation.TPrepared obj in expected) {
                Assert.AreEqual(actual.Dequeue(), obj);                
            }
        }

        /// <summary>
        ///A test for exec
        ///</summary>
        [TestMethod()]
        public void execTest2()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(1);

            Queue<DTEOperation.TPrepared> commands = new Queue<DTEOperation.TPrepared>();
            commands.Enqueue(new DTEOperation.TPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.TPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.TPrepared("Debug.Start", ""));

            DTEOperation.TPrepared[] expected = new DTEOperation.TPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            try {
                target.exec(commands, false);
            }
            catch(ComponentException) {
                // other type should fail the current test
            }

            Queue<DTEOperation.TPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count == expected.Length);

            foreach(DTEOperation.TPrepared obj in expected) {
                Assert.AreEqual(actual.Dequeue(), obj);
            }
        }

        /// <summary>
        ///A test for exec
        ///</summary>
        [TestMethod()]
        public void execTest3()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(2);

            Queue<DTEOperation.TPrepared> commands = new Queue<DTEOperation.TPrepared>();
            commands.Enqueue(new DTEOperation.TPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.TPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.TPrepared("Debug.Start", ""));
            commands.Enqueue(new DTEOperation.TPrepared("Debug.StartWithoutDebugging", ""));

            DTEOperation.TPrepared[] expected = new DTEOperation.TPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            try {
                target.exec(commands, true);
            }
            catch(ComponentException) {
                // other type should fail the current test
            }

            Queue<DTEOperation.TPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count != expected.Length);
            Assert.IsTrue(actual.Count == 2);

            int idx = 0;
            foreach(DTEOperation.TPrepared obj in actual) {
                Assert.AreEqual(expected[idx++], obj);
            }
        }


        internal class DTEOperationAccessor
        {
            public class Accessor: DTEOperation
            {
                public Accessor(): base((DTE)null) {}
                public Accessor(DTE dte): base(dte) {}
            }

            public class ToExec: Accessor
            {
                /// <summary>
                /// Simulation of the executed commands
                /// </summary>
                protected Queue<TPrepared> executed = new Queue<TPrepared>();
                /// <summary>
                /// when to generate an error
                /// -1 if never
                /// </summary>
                protected int simulateFailAfter     = -1;

                public override void exec(string name, string args)
                {
                    executed.Enqueue(new TPrepared(name, args));
                    if(simulateFailAfter != -1 && executed.Count >= simulateFailAfter) {
                        throw new Exception("simulate DTE error");
                    }
                }

                public Queue<TPrepared> getExecuted()
                {
                    return executed;
                }

                public ToExec(int simulateFailAfter)
                {
                    this.simulateFailAfter = simulateFailAfter;
                }
            }
        }
    }
}
