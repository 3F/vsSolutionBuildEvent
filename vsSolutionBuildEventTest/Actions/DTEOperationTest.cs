using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using net.r_eg.EvMSBuild.Exceptions;
using net.r_eg.vsSBE.Actions;
using net.r_eg.vsSBE.Events;
using Xunit;

namespace net.r_eg.vsSBE.Test.Actions
{
    public class DTEOperationTest
    {
        [Fact]
        public void dtePreparedTest()
        {
            DTEOperation target = new(null, SolutionEventType.General);

            string line = "File.OpenProject(\"c:\\path\\app.sln\")";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.Equal("File.OpenProject", actual.name);
            Assert.Equal("\"c:\\path\\app.sln\"", actual.args);
        }

        [Fact]
        public void dtePreparedTest2()
        {
            DTEOperation target = new(null, SolutionEventType.General);

            string line = "Debug.StartWithoutDebugging";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.Equal("Debug.StartWithoutDebugging", actual.name);
            Assert.Equal(string.Empty, actual.args);
        }

        [Theory]
        [InlineData("Debug StartWithoutDebugging")]
        [InlineData("")]
        public void dtePreparedTheory1(string line)
        {
            DTEOperation target = new(null, SolutionEventType.General);

            Assert.Throws<IncorrectSyntaxException>(() => target.parse(line));
        }

        [Fact]
        public void dtePreparedTest5()
        {
            DTEOperation target = new DTEOperation((IEnvironment)null, SolutionEventType.General);

            string line = "  OpenProject(arg)  ";
            DTEOperation.DTEPrepared actual = target.parse(line);

            Assert.Equal("OpenProject", actual.name);
            Assert.Equal("arg", actual.args);
        }

        [Fact]
        public void execTest()
        {
            DTEOperationAccessor.ToExec target = new(-1);

            Queue<DTEOperation.DTEPrepared> commands = new();
            commands.Enqueue(new DTEOperation.DTEPrepared("Build.Cancel", ""));
            commands.Enqueue(new DTEOperation.DTEPrepared("File.OpenProject", "app.sln"));
            commands.Enqueue(new DTEOperation.DTEPrepared("Debug.Start", ""));

            DTEOperation.DTEPrepared[] expected = new DTEOperation.DTEPrepared[commands.Count];
            commands.CopyTo(expected, 0);

            target.exec(commands, false);

            Queue<DTEOperation.DTEPrepared> actual = target.getExecuted();
            Assert.True(actual.Count == expected.Length);

            foreach(DTEOperation.DTEPrepared obj in expected) {
                Assert.Equal(actual.Dequeue(), obj);
            }
        }

        [Fact]
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
            Assert.True(actual.Count == expected.Length);

            foreach(DTEOperation.DTEPrepared obj in expected) {
                Assert.Equal(actual.Dequeue(), obj);
            }
        }

        [Fact]
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
            Assert.True(actual.Count != expected.Length);
            Assert.True(actual.Count == 2);

            int idx = 0;
            foreach(DTEOperation.DTEPrepared obj in actual) {
                Assert.Equal(expected[idx++], obj);
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
