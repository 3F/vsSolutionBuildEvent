using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMTest
    {
        [Fact]
        public void IsMethodWithArgsTest1()
        {
            IPM pm = new PM("solution(\"str data\", 12, true).data.right(false, 1).end()");

            Assert.True(pm.IsMethodWithArgs("solution", ArgumentType.StringDouble, ArgumentType.Integer, ArgumentType.Boolean));
            Assert.False(pm.IsMethodWithArgs(1, "data"));
            Assert.True(pm.IsMethodWithArgs(2, "right", ArgumentType.Boolean, ArgumentType.Integer));
            Assert.True(pm.IsMethodWithArgs(3, "end"));
        }

        [Fact]
        public void IsRightTest1()
        {
            IPM pm = new PM("pname = true");

            Assert.True(pm.It(LevelType.Property, "pname"));
            Assert.False(pm.IsRight(LevelType.RightOperandEmpty));
            Assert.False(pm.IsRight(LevelType.RightOperandColon));
            Assert.True(pm.IsRight(LevelType.RightOperandStd));
        }

        [Fact]
        public void IsRightTest2()
        {
            IPM pm = new PM("pname.m1(): mixed data");

            Assert.True(pm.It(LevelType.Property, "pname"));
            Assert.True(pm.It(LevelType.Method, "m1"));
            Assert.False(pm.IsRight(LevelType.RightOperandEmpty));
            Assert.True(pm.IsRight(LevelType.RightOperandColon));
            Assert.False(pm.IsRight(LevelType.RightOperandStd));
        }

        [Fact]
        public void IsDataTest1()
        {
            IPM pm = new PM("pname . m1(true, 123).right = true");

            Assert.True(pm.IsData("pname"));
            Assert.False(pm.IsData("pname "));
            pm.pinTo(1);

            Assert.True(pm.IsData("m1"));
            pm.pinTo(1);

            Assert.True(pm.IsData("right"));
            pm.pinTo(1);

            Assert.True(pm.IsData(" true"));
        }

        [Fact]
        public void IsDataTest2()
        {
            IPM pm = new PM("pname = true");

            Assert.False(pm.IsData("property1", "property2", "property2", "property3"));
            Assert.True(pm.IsData("property1", "property2", "pname", "property3"));
        } 

        [Fact]
        public void GetRightOperandTest1()
        {
            Assert.Throws<IncorrectSyntaxException>(() => new PM("pname.... = true"));
            Assert.Throws<IncorrectSyntaxException>(() => new PM("pname @ = false"));
        }

        [Fact]
        public void GetRightOperandTest2()
        {
            IPM pm = new PM("pname = true ");

            Assert.True(pm.It(LevelType.Property, "pname"));
            Assert.True(pm.IsRight(LevelType.RightOperandStd));
            Assert.Equal(" true ", pm.Levels[0].Data);

            pm = new PM("m(): mixed\ndata ");

            Assert.True(pm.It(LevelType.Method, "m"));
            Assert.True(pm.IsRight(LevelType.RightOperandColon));
            Assert.Equal(" mixed\ndata ", pm.Levels[0].Data);
        }
    }
}
