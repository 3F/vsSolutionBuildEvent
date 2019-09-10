using System.Collections.Generic;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.VS;
using SobaScript.Z.VSTest.Stubs;
using Xunit;

namespace SobaScript.Z.VSTest
{
    public class OwpComponentTest
    {
        //public IEnvironment Env
        //{
        //    get
        //    {
        //        var owp = new Mock<EnvDTE.OutputWindowPane>();
        //        owp.Setup(m => m.OutputString(It.IsAny<string>()));
        //        owp.Setup(m => m.Activate());
        //        owp.Setup(m => m.Clear());

        //        var ow = new Mock<IOW>();
        //        ow.Setup(m => m.getByName(It.IsAny<string>(), It.IsAny<bool>())).Returns(owp.Object);
        //        ow.Setup(m => m.deleteByName(It.IsAny<string>()));

        //        var mockEnv = new Mock<IEnvironment>();
        //        mockEnv.SetupGet(p => p.OutputWindowPane).Returns(ow.Object);

        //        return mockEnv.Object;
        //    }
        //}

        [Fact]
        public void ParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[OWP out.Warnings.Count]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("OWP out.Warnings.Count")
            );

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[OWP NotFound.Test]")
            );
        }

        [Fact]
        public void StOutParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.Empty, target.Eval("[OWP out.All]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Warnings.Raw]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Warnings]"));
            Assert.Equal("0", target.Eval("[OWP out.Warnings.Count]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Warnings.Codes]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors.Raw]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors]"));
            Assert.Equal("0", target.Eval("[OWP out.Errors.Count]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors.Codes]"));
        }

        [Fact]
        public void StOutParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv
            (
                "raw",
                new List<string>() { "err1" },
                new List<string>() { "warn1", "warn2" }
            ));

            Assert.Equal("raw", target.Eval("[OWP out.All]"));
            Assert.Equal("raw", target.Eval("[OWP out]"));
            Assert.Equal("raw", target.Eval("[OWP out.Warnings.Raw]"));
            Assert.Equal("raw", target.Eval("[OWP out.Warnings]"));
            Assert.Equal("2", target.Eval("[OWP out.Warnings.Count]"));
            Assert.Equal("warn1,warn2", target.Eval("[OWP out.Warnings.Codes]"));
            Assert.Equal("raw", target.Eval("[OWP out.Errors.Raw]"));
            Assert.Equal("raw", target.Eval("[OWP out.Errors]"));
            Assert.Equal("1", target.Eval("[OWP out.Errors.Count]"));
            Assert.Equal("err1", target.Eval("[OWP out.Errors.Codes]"));
        }

        [Fact]
        public void StOutParseTest3()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv
            (
                "raw",
                new List<string>(),
                new List<string>() { "warn1", "warn2" }
            ));

            Assert.Equal("raw", target.Eval("[OWP out.All]"));
            Assert.Equal("raw", target.Eval("[OWP out]"));
            Assert.Equal("raw", target.Eval("[OWP out.Warnings.Raw]"));
            Assert.Equal("raw", target.Eval("[OWP out.Warnings]"));
            Assert.Equal("2", target.Eval("[OWP out.Warnings.Count]"));
            Assert.Equal("warn1,warn2", target.Eval("[OWP out.Warnings.Codes]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors.Raw]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors]"));
            Assert.Equal("0", target.Eval("[OWP out.Errors.Count]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP out.Errors.Codes]"));
        }

        [Fact]
        public void StOutParseTest4()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP out.NotRealPropertyTest]")
            );
        }

        [Fact]
        public void StOutParseTest5()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[OWP out()]")
            );

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[OWP out().All]")
            );
        }

        [Fact]
        public void StOutParseTest6()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[OWP out.All.NotRealProperty]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[OWP out.Warnings.NotRealProperty]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[OWP out.Warnings.Codes.NotRealProperty]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP out.NotRealProperty]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[OWP out.Warnings.Count = 12]")
            );
        }

        [Fact]
        public void StOutParseTest7()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<NotFoundException>(() =>
                target.Eval($"[OWP out(\"{NullOwpEnv.MOCK_ITEM_NAME}\").Warnings.Raw]")
            );
        }

        [Fact]
        public void StLogParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP log]")
            );
        }

        [Fact]
        public void StLogParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP log.NotSupportedTest]")
            );
        }

        [Fact]
        public void StLogParseTest3()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.Empty, target.Eval("[OWP log.Message]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP log.Level]"));
        }

        [Fact]
        public void StItemParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP item(\"name\")]")
            );
        }

        [Fact]
        public void StItemParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP item(\"name\").NotSupportedTest]")
            );
        }

        [Fact]
        public void StItemParseTest3()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[OWP item(\"\").write(false): ]")
            );
        }

        [Fact]
        public void StItemParseTest4()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<PMLevelException>(() =>
                target.Eval("[OWP item(name).write(false): ]")
            );
        }

        [Fact]
        public void StItemWriteParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[OWP item(\"name\").write(\"false\"): ]")
            );
        }

        [Fact]
        public void StItemWriteParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").write(false): data]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").write(true): data]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").writeLine(false): data]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").writeLine(true): data]"));
        }

        [Fact]
        public void StItemWriteParseTest3()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").write(false): multi\nline\" \n 'data'.]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").write(true): multi\nline\" \n 'data'.]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").writeLine(false): multi\nline\" \n 'data'.]"));
            Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").writeLine(true): multi\nline\" \n 'data'.]"));
        }

        [Fact]
        public void StItemDeleteParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").delete]"))
            );
        }

        [Fact]
        public void StItemDeleteParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.From(false), target.Eval("[OWP item(\"name\").delete = false]"));
            Assert.Equal(Value.From(true), target.Eval("[OWP item(\"name\").delete = true]"));
        }

        [Fact]
        public void StItemClearParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").clear]"))
            );
        }

        [Fact]
        public void StItemClearParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.From(false), target.Eval("[OWP item(\"name\").clear = false]"));
            Assert.Equal(Value.From(true), target.Eval("[OWP item(\"name\").clear = true]"));
        }

        [Fact]
        public void StItemActivateParseTest1()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());

            Assert.Throws<IncorrectNodeException>(() =>
                Assert.Equal(Value.Empty, target.Eval("[OWP item(\"name\").activate]"))
            );
        }

        [Fact]
        public void StItemActivateParseTest2()
        {
            var target = new OwpComponent(new Soba(), new NullOwpEnv());
            Assert.Equal(Value.From(false), target.Eval("[OWP item(\"name\").activate = false]"));
            Assert.Equal(Value.From(true), target.Eval("[OWP item(\"name\").activate = true]"));
        }
    }
}
