using System;
using net.r_eg.Components;
using Xunit;

namespace LSenderTest
{
    public class MessageTest
    {
        [Fact]
        public void CtorTest1()
        {
            string content = "Message1";

            var msg = new Message(content, MsgLevel.Fatal);

            Assert.Equal(content, msg.content);
            Assert.Equal(MsgLevel.Fatal, msg.level);
            Assert.Null(msg.exception);
            Assert.Null(msg.data);
            Assert.True(msg.stamp <= DateTime.Now);
        }

        [Fact]
        public void CtorTest2()
        {
            string content = "Message2";

            try
            {
                throw new ArgumentNullException();
            }
            catch(ArgumentNullException ex)
            {
                var msg = new Message(content, ex, MsgLevel.Trace);

                Assert.Equal(content, msg.content);
                Assert.Equal(MsgLevel.Trace, msg.level);
                Assert.Null(msg.data);
                Assert.True(msg.stamp <= DateTime.Now);
                Assert.NotNull(msg.exception);

                Assert.Equal(typeof(ArgumentNullException), msg.exception.GetType());
            }
        }

        [Fact]
        public void CtorTest3()
        {
            string content = "Message3";

            object data = new int[] { 2, 4, 6 };

            var msg = new Message(content, data, MsgLevel.Warn);

            Assert.Equal(content, msg.content);
            Assert.Equal(MsgLevel.Warn, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);

            Assert.NotNull(msg.data);

            Assert.Equal(2, ((int[])msg.data)[0]);
            Assert.Equal(4, ((int[])msg.data)[1]);
            Assert.Equal(6, ((int[])msg.data)[2]);
        }

        [Fact]
        public void CtorTest4()
        {
            var msg = new Message(null);

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Debug, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);
            Assert.Null(msg.data);
        }

        [Fact]
        public void CtorTest5()
        {
            var msg = new Message(null, "data");

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Debug, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.exception);
            Assert.NotNull(msg.data);
        }

        [Fact]
        public void CtorTest6()
        {
            var msg = new Message(null, new ArgumentOutOfRangeException());

            Assert.Null(msg.content);
            Assert.Equal(MsgLevel.Error, msg.level);
            Assert.True(msg.stamp <= DateTime.Now);
            Assert.Null(msg.data);
            Assert.NotNull(msg.exception);

            Assert.Equal(typeof(ArgumentOutOfRangeException), msg.exception.GetType());
        }
    }
}
