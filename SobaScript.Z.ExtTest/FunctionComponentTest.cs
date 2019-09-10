using System;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Ext;
using Xunit;

namespace SobaScript.Z.ExtTest
{
    public class FunctionComponentTest
    {
        [Fact]
        public void ParseTest1()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<SubtypeNotFoundException>(() =>
                target.Eval("[Func NotRealSubtype.check]")
            );
        }

        [Fact]
        public void HashTest1()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Func hash]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Func hash = 1]")
            );
        }

        [Fact]
        public void HashTest2()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Func hash.MD5]")
            );

            Assert.Throws<IncorrectNodeException>(() =>
                target.Eval("[Func hash.SHA1]")
            );
        }

        [Fact]
        public void HashTest3()
        {
            var target = new FunctionComponent(new Soba());
            Assert.Equal("ED076287532E86365E841E92BFC50D8C", target.Eval("[Func hash.MD5(\"Hello World!\")]"));
            Assert.Equal("2EF7BDE608CE5404E97D5F042F95F89F1C232871", target.Eval("[Func hash.SHA1(\"Hello World!\")]"));
        }

        [Fact]
        public void HashTest4()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[Func hash.MD5(\"Hello World!\").right]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[Func hash.SHA1(\"Hello World!\").right]")
            );
        }

        [Fact]
        public void HashTest5()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[Func hash.MD5(\"Hello World!\") = true]")
            );

            Assert.Throws<NotSupportedOperationException>(() =>
                target.Eval("[Func hash.SHA1(\"Hello World!\") = true]")
            );
        }

        [Fact]
        public void HashTest6()
        {
            var target = new FunctionComponent(new Soba());

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.MD5()]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.SHA1()]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.MD5(test)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.SHA1(test)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.MD5(\"test\", true)]")
            );

            Assert.Throws<ArgumentException>(() =>
                target.Eval("[Func hash.SHA1(\"test\", true)]")
            );
        }

        [Fact]
        public void HashTest7()
        {
            var target = new FunctionComponent(new Soba());
            Assert.Equal("D41D8CD98F00B204E9800998ECF8427E", target.Eval("[Func hash.MD5(\"\")]"));
            Assert.Equal("DA39A3EE5E6B4B0D3255BFEF95601890AFD80709", target.Eval("[Func hash.SHA1(\"\")]"));
        }
    }
}
