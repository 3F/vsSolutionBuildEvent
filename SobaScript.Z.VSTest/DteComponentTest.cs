using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using SobaScript.Z.VSTest.Stubs;
using Xunit;

namespace SobaScript.Z.VSTest
{
    public class DteComponentTest
    {
        [Fact]
        public void ParseExecTest1()
        {
            var target = new DteComponentAcs();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[DTE exec: command(arg)]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("DTE exec: command(arg)")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[DTE exec:]")
            );
        }

        [Fact]
        public void ParseExecTest2()
        {
            var target = new DteComponentAcs();
            Assert.Equal(Value.Empty, target.Eval("[DTE exec: command]"));
        }

        [Fact]
        public void ParseExecTest3()
        {
            var target = new DteComponentAcs();
            Assert.Equal(Value.Empty, target.Eval("[DTE exec: command(args)]"));
        }

        [Fact]
        public void ParseLastCommandTest1()
        {
            var target = new DteComponentAcs();
            target.Env.EmulateAfterExecute("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 264, "In", "Out");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[DTE events.LastCommand]")
            );
        }

        [Fact]
        public void ParseLastCommandTest2()
        {
            var target = new DteComponentAcs();
            target.Env.IsAvaialbleDteCmd = false;

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[DTE events.LastCommand]")
            );
        }

        [Fact]
        public void ParseLastCommandTest3()
        {
            var target = new DteComponentAcs();

            string guid         = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            int id              = 264;
            object customIn     = "In";
            object customOut    = "Out";
            bool pre            = true;
            target.Env.EmulateBeforeExecute(guid, id, customIn, customOut, false);

            Assert.Equal(guid, target.Eval("[DTE events.LastCommand.Guid]"));
            Assert.Equal(Value.From(id), target.Eval("[DTE events.LastCommand.Id]"));
            Assert.Equal(customIn, target.Eval("[DTE events.LastCommand.CustomIn]"));
            Assert.Equal(customOut, target.Eval("[DTE events.LastCommand.CustomOut]"));
            Assert.Equal(Value.From(pre), target.Eval("[DTE events.LastCommand.Pre]"));
        }

        [Fact]
        public void ParseLastCommandTest4()
        {
            var target = new DteComponentAcs();

            string guid         = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            int id              = 264;
            object customIn     = "In";
            object customOut    = "Out";
            bool pre            = false;
            target.Env.EmulateAfterExecute(guid, id, customIn, customOut);

            Assert.Equal(guid, target.Eval("[DTE events.LastCommand.Guid]"));
            Assert.Equal(Value.From(id), target.Eval("[DTE events.LastCommand.Id]"));
            Assert.Equal(customIn, target.Eval("[DTE events.LastCommand.CustomIn]"));
            Assert.Equal(customOut, target.Eval("[DTE events.LastCommand.CustomOut]"));
            Assert.Equal(Value.From(pre), target.Eval("[DTE events.LastCommand.Pre]"));
        }

        [Fact]
        public void ParseLastCommandTest5()
        {
            var target = new DteComponentAcs();
            target.Env.EmulateAfterExecute("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 264, "In", "Out");

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[DTE events.LastCommand.NotRealPropStub]")
            );
        }

        [Fact]
        public void ParseLastCommandTest6()
        {
            var target = new DteComponentAcs();

            string guid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            target.Env.EmulateBeforeExecute(guid, 264, "", "", false);
            target.Env.EmulateAfterExecute(guid, 264, "", "");

            Assert.Equal(guid, target.Eval("[DTE events . LastCommand . Guid]"));
        }

        [Fact]
        public void ParseLastCommandTest7()
        {
            var target = new DteComponentAcs();

            string expectedGuid = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
            string otherGuid    = "{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}";

            target.Env.EmulateBeforeExecute(otherGuid, 1627, "", "", false);
            target.Env.EmulateAfterExecute(expectedGuid, 264, "", "");

            Assert.Equal(expectedGuid, target.Eval("[DTE events.LastCommand.Guid]"));
        }

        [Fact]
        public void ParseTest1()
        {
            var target = new DteComponentAcs();

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[DTE NotExist.test]")
            );
        }
    }
}
