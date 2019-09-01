using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.EvMSBuild.Exceptions;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.Test.Actions
{
    [TestClass]
    public class DTEOperationTest
    {
        [TestMethod]
        public void dtePreparedTest()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "File.OpenProject(\"c:\\path\\app.sln\")";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.AreEqual("File.OpenProject", actual.name);
            Assert.AreEqual("\"c:\\path\\app.sln\"", actual.args);
        }

        [TestMethod]
        public void dtePreparedTest2()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "Debug.StartWithoutDebugging";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.AreEqual("Debug.StartWithoutDebugging", actual.name);
            Assert.AreEqual("", actual.args);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void dtePreparedTest3()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "Debug StartWithoutDebugging";
            DTEOperation.DTEPrepared actual = target.parse(line);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void dtePreparedTest4()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "";
            DTEOperation.DTEPrepared actual = target.parse(line);
        }

        [TestMethod]
        public void dtePreparedTest5()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "  OpenProject(arg)  ";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.AreEqual("OpenProject", actual.name);
            Assert.AreEqual("arg", actual.args);
        }

        [TestMethod]
        public void execTest()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(-1);

            Queue<DTEOperation.DTEPrepared> commands = new Queue<DTEOperation.DTEPrepared>();
            commands.Enqueue(new DTEOperation.DTEPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.DTEPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.DTEPrepared("Debug.Start", ""));

            DTEOperation.DTEPrepared[] expected = new DTEOperation.DTEPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            target.exec(commands, false);

            Queue<DTEOperation.DTEPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count == expected.Length);

            foreach(DTEOperation.DTEPrepared obj in expected) {
                Assert.AreEqual(actual.Dequeue(), obj);
            }
        }

        [TestMethod]
        public void execTest2()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(1);

            Queue<DTEOperation.DTEPrepared> commands = new Queue<DTEOperation.DTEPrepared>();
            commands.Enqueue(new DTEOperation.DTEPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.DTEPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.DTEPrepared("Debug.Start", ""));

            DTEOperation.DTEPrepared[] expected = new DTEOperation.DTEPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            try {
                target.exec(commands, false);
            }
            catch(ExternalException) {
                // other type should fail the current test
            }

            Queue<DTEOperation.DTEPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count == expected.Length);

            foreach(DTEOperation.DTEPrepared obj in expected) {
                Assert.AreEqual(actual.Dequeue(), obj);
            }
        }

        [TestMethod]
        public void execTest3()
        {
            DTEOperationAccessor.ToExec target = new DTEOperationAccessor.ToExec(2);

            Queue<DTEOperation.DTEPrepared> commands = new Queue<DTEOperation.DTEPrepared>();
            commands.Enqueue(new DTEOperation.DTEPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.DTEPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.DTEPrepared("Debug.Start", ""));
            commands.Enqueue(new DTEOperation.DTEPrepared("Debug.StartWithoutDebugging", ""));

            DTEOperation.DTEPrepared[] expected = new DTEOperation.DTEPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            try {
                target.exec(commands, true);
            }
            catch(ExternalException) {
                // other type should fail the current test
            }

            Queue<DTEOperation.DTEPrepared> actual = target.getExecuted();
            Assert.IsTrue(actual.Count != expected.Length);
            Assert.IsTrue(actual.Count == 2);

            int idx = 0;
            foreach(DTEOperation.DTEPrepared obj in actual) {
                Assert.AreEqual(expected[idx++], obj);
            }
        }

        private class DTEOperationAccessor
        {
            public class Accessor: DTEOperation
            {
                public Accessor(): base((IEnvironment)null, SolutionEventType.General) {}
                public Accessor(IEnvironment env, SolutionEventType type): base(env, type) {}
            }

            public class ToExec: Accessor
            {
                /// <summary>
                /// Simulation of the executed commands
                /// </summary>
                protected Queue<DTEPrepared> executed = new Queue<DTEPrepared>();
                /// <summary>
                /// when to generate an error
                /// -1 if never
                /// </summary>
                protected int simulateFailAfter     = -1;

                public override void exec(string name, string args)
                {
                    executed.Enqueue(new DTEPrepared(name, args));
                    if(simulateFailAfter != -1 && executed.Count >= simulateFailAfter) {
                        throw new Exception("simulate DTE error");
                    }
                }

                public Queue<DTEPrepared> getExecuted()
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
