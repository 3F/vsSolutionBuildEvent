using System;
using net.r_eg.SobaScript.SNode;
using Xunit;

namespace SobaScriptTest
{
    public class LevelTest
    {
        [Fact]
        public void IsTest1()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2]
                {
                    new Argument() {
                        data = "abcd123",
                        type = ArgumentType.StringDouble
                    },
                    new Argument() {
                        data = "true",
                        type = ArgumentType.Boolean
                    }
                }
            };

            Assert.True(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean));
            Assert.False(level.Is(ArgumentType.StringDouble));
            Assert.False(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean));
            Assert.False(level.Is());
            Assert.False(level.Is(ArgumentType.Boolean, ArgumentType.StringDouble));
        }

        [Fact]
        public void IsTest2()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2]
                {
                    new Argument() {
                        data = "abcd123",
                        type = ArgumentType.StringDouble
                    },
                    new Argument() {
                        data = "true",
                        type = ArgumentType.Boolean
                    }
                }
            };

            Assert.True(level.Is(null, ArgumentType.StringDouble, ArgumentType.Boolean));
            Assert.False(level.Is(null, ArgumentType.StringDouble));
            Assert.False(level.Is(null, ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean));
            Assert.False(level.Is(null, null));
            Assert.False(level.Is(null, ArgumentType.Boolean, ArgumentType.StringDouble));
        }

        [Fact]
        public void IsTest3()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = new Argument[2]
                {
                    new Argument() {
                        data = "abcd123",
                        type = ArgumentType.StringDouble
                    },
                    new Argument() {
                        data = "true",
                        type = ArgumentType.Boolean
                    }
                }
            };

            Assert.True(level.Is("hash", ArgumentType.StringDouble, ArgumentType.Boolean)); //should be without exception

            Assert.Throws<ArgumentException>(() => Assert.False(level.Is("hash", ArgumentType.StringDouble)));

            Assert.Throws<ArgumentException>(() => Assert.False(level.Is("hash", ArgumentType.StringDouble, ArgumentType.Boolean, ArgumentType.Boolean)));
            Assert.Throws<ArgumentException>(() => Assert.False(level.Is("hash", null)));
            Assert.Throws<ArgumentException>(() => Assert.False(level.Is("hash", ArgumentType.Boolean, ArgumentType.StringDouble)));
        }

        [Fact]
        public void IsTest4()
        {
            ILevel level = new Level()
            {
                Data = "hash",
                Type = LevelType.Method,
                Args = null
            };

            Assert.False(level.Is());
            Assert.False(level.Is(ArgumentType.StringDouble, ArgumentType.Boolean));
        }

    }
}
