using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMFinalIsTest
    {
        [Fact]
        public void FinalIsTest1()
        {
            IPM pm = new PM("left.solution.right");
            Assert.Equal(4, pm.Levels.Count);
            Assert.True(pm.FinalIs(2, LevelType.Property, "right"));
            Assert.True(pm.FinalIs(3, LevelType.RightOperandEmpty));
        }

        [Fact]
        public void FinalIsTest2()
        {
            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                Assert.True(pm.FinalIs(1, LevelType.Property, "solution"));
            });

            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                Assert.True(pm.FinalIs(LevelType.Property, "left"));
            });
        }

        [Fact]
        public void FinalIsTest3()
        {
            IPM pm = new PM("left.solution.right");
            Assert.False(pm.Is(0, LevelType.Property, "right"));
            Assert.False(pm.FinalIs(1, LevelType.Property, "right"));
            Assert.True(pm.Is(2, LevelType.Property, "right"));
        }

        [Fact]
        public void FinalIsTest4()
        {
            IPM pm = new PM("left.solution().right");
            Assert.True(pm.Is(0, LevelType.Property, "left"));
            Assert.True(pm.Is(1, LevelType.Method, "solution"));
            Assert.True(pm.FinalIs(2, LevelType.Property, "right"));
        }

        [Fact]
        public void FinalIsTest5()
        {
            IPM pm = new PM("left.solution(\" (a, b) \").right");
            Assert.True(pm.Is(0, LevelType.Property, "left"));
            Assert.True(pm.Is(1, LevelType.Method, "solution"));
            Assert.True(pm.FinalIs(2, LevelType.Property, "right"));
        }

        [Fact]
        public void FinalEmptyIsTest1()
        {
            IPM pm = new PM("solution.right ");
            Assert.True(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            Assert.True(pm.FinalEmptyIs(2, LevelType.RightOperandEmpty));
        }

        [Fact]
        public void FinalEmptyIsTest2()
        {
            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("solution.right = ");
                Assert.True(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            });

            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("solution.right : ");
                Assert.True(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            });

            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("solution.right . prop");
                Assert.True(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            });

            Assert.Throws<NotSupportedOperationException>(() =>
            {
                IPM pm = new PM("solution.right mixed data");
                Assert.True(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            });
        }

        [Fact]
        public void FinalEmptyIsTest3()
        {
            IPM pm = new PM("left.solution.right");
            Assert.False(pm.Is(0, LevelType.Property, "right"));
            Assert.False(pm.FinalEmptyIs(1, LevelType.Property, "right"));
            Assert.True(pm.Is(2, LevelType.Property, "right"));
        }

        [Fact]
        public void FinalEmptyIsTest4()
        {
            IPM pm = new PM("left.solution(\"test m()\")");
            Assert.True(pm.Is(0, LevelType.Property, "left"));
            Assert.True(pm.FinalEmptyIs(1, LevelType.Method, "solution"));
        }

        [Fact]
        public void FinalEmptyIsTest5()
        {
            IPM pm = new PM("left.solution(\"m(1)\", \"m()\", ')', \")\", \"(\")");
            Assert.True(pm.Is(0, LevelType.Property, "left"));
            Assert.True(pm.FinalEmptyIs(1, LevelType.Method, "solution"));
        }
    }
}
