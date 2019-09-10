using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Z.Core;
using Xunit;

namespace SobaScript.Z.CoreTest
{
    public class CommentComponentTest
    {
        [Fact]
        public void ParseTest()
        {
            var target = new CommentComponent();
            Assert.Equal(Value.Empty, target.Eval("[\"test\"]"));
        }

        [Fact]
        public void ParseTest2()
        {
            var target = new CommentComponent();
            Assert.Equal(Value.Empty, target.Eval("[\"line1 \n line2\"]"));
        }

        [Fact]
        public void ParseTest3()
        {
            var target = new CommentComponent();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("#[\"test\"]")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("[test]")
            );
        }

        [Fact]
        public void ParseTest4()
        {
            var target = new CommentComponent();

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("test")
            );

            Assert.Throws<IncorrectSyntaxException>(() =>
                target.Eval("")
            );
        }
    }
}
