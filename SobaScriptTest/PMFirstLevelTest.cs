using System;
using System.Collections.Generic;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class PMFirstLevelTest
    {
        [Fact]
        public void FirstLevelTest1()
        {
            IPM pm = new PM("left.solution.right");

            Assert.Equal(pm.FirstLevel.Data, pm.Levels[0].Data);
            Assert.Equal(pm.FirstLevel.Type, pm.Levels[0].Type);

            pm.PinTo(2);
            Assert.Equal(pm.FirstLevel.Data, pm.Levels[0].Data);
            Assert.Equal(pm.FirstLevel.Type, pm.Levels[0].Type);
        }

        [Fact]
        public void FirstLevelTest2()
        {
            IPM pm = new PM(new List<ILevel>());
            Assert.Empty(pm.Levels);
            Assert.Throws<ArgumentException>(() => pm.FirstLevel);
        }

        [Fact]
        public void FirstLevelTest3()
        {
            IPM pm = new PM("left.solution.right");

            Assert.Equal("left", pm.Levels[0].Data);
            Assert.Equal("left", pm.FirstLevel.Data);
        }

        [Fact]
        public void FirstLevelTest4()
        {
            IPM pm = new PM(new List<ILevel>());
            Assert.Empty(pm.Levels);

            Assert.Throws<ArgumentException>(() => pm.FirstLevel = new Level());
        }
    }
}
