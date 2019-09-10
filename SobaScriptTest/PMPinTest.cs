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

            pm.PinTo(1);
            Assert.Equal(3, pm.Levels.Count);

            Assert.True(pm.Is(0, LevelType.Property, "solution"));
            Assert.True(pm.Is(1, LevelType.Property, "right"));
            Assert.True(pm.Is(2, LevelType.RightOperandEmpty, null));

            pm.PinTo(2);
            Assert.True(pm.Is(0, LevelType.RightOperandEmpty, null));
        }

        [Fact]
        public void PinToTest2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                pm.PinTo(100);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right");
                pm.PinTo(-1);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right"); //4
                pm.PinTo(4);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                IPM pm = new PM("left.solution.right"); //4
                pm.PinTo(1);
                pm.PinTo(2);
                pm.PinTo(1);
            });
        }
    }
}
