using System;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMPinTest
    {
        [Fact]
        public void PinToTest1()
        {
            IPM pm = new PM("left.solution.right");

            Assert.Equal(4, pm.Levels.Count);

            pm.pinTo(1);
            Assert.Equal(3, pm.Levels.Count);

            Assert.True(pm.Is(0, LevelType.Property, "solution"));
            Assert.True(pm.Is(1, LevelType.Property, "right"));
            Assert.True(pm.Is(2, LevelType.RightOperandEmpty, null));

            pm.pinTo(2);
            Assert.True(pm.Is(0, LevelType.RightOperandEmpty, null));
        }

        [Fact]
        public void PinToTest2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                pm.pinTo(100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                pm.pinTo(-1);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right"); //4
                pm.pinTo(4);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right"); //4
                pm.pinTo(1);
                pm.pinTo(2);
                pm.pinTo(1);
            });
        }
    }
}
