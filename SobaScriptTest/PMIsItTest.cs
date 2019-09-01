using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMIsItTest
    {
        [Fact]
        public void IsTest1()
        {
            IPM pm = new PM("solution=left.right");

            Assert.False(pm.Is(100, LevelType.Property, "solution"));
            Assert.False(pm.Is(-1, LevelType.Property, "solution"));
            Assert.True(pm.Is(0, LevelType.Property, "solution"));
            Assert.True(pm.Is(1, LevelType.RightOperandStd, "left.right"));
        }

        [Fact]
        public void IsTest2()
        {
            IPM pm = new PM("solution.m1().m2().right");

            Assert.True(pm.Is(0, LevelType.Property, "solution"));
            Assert.True(pm.Is(1, LevelType.Method, "m1"));
            Assert.True(pm.Is(2, LevelType.Method, "m2"));
            Assert.True(pm.Is(3, LevelType.Property, "right"));
            Assert.True(pm.Is(4, LevelType.RightOperandEmpty));
        }

        [Fact]
        public void IsTest3()
        {
            IPM pm = new PM("solution.m1(')', '(', \"(\").m2(123, \" -> )\").right");

            Assert.True(pm.Is(0, LevelType.Property, "solution"));
            Assert.True(pm.Is(1, LevelType.Method, "m1"));
            Assert.True(pm.Is(2, LevelType.Method, "m2"));
            Assert.True(pm.Is(3, LevelType.Property, "right"));
            Assert.True(pm.Is(4, LevelType.RightOperandEmpty));
        }

        [Fact]
        public void ItTest1()
        {
            IPM pm = new PM("solution.m1().right");

            Assert.True(pm.It(LevelType.Property, "solution"));
            Assert.True(pm.It(LevelType.Method, "m1"));
            Assert.False(pm.It(LevelType.Property, "notRealProperty"));
            Assert.True(pm.It(LevelType.Property, "right"));
            Assert.True(pm.It(LevelType.RightOperandEmpty));

            Assert.Empty(pm.Levels);
            Assert.False(pm.It(LevelType.RightOperandEmpty));
            Assert.False(pm.Is(0, LevelType.RightOperandEmpty));
        }

        [Fact]
        public void ItTest2()
        {
            IPM pm = new PM("solution.m1().Prop1.right");

            Assert.True(pm.It(LevelType.Property, "solution"));
            Assert.True( pm.Is(LevelType.Method, "m1"));
            Assert.False(pm.It(LevelType.Property, "Prop1"));
            Assert.True(pm.It(1, LevelType.Property, "Prop1"));
            Assert.True( pm.It(LevelType.Property, "right"));
        }
    }
}
