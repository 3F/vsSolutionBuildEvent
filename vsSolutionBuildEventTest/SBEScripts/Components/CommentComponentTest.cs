using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.vsSBE.SBEScripts.Components;

namespace net.r_eg.vsSBE.Test.SBEScripts.Components
{
    [TestClass()]
    public class CommentComponentTest
    {
        [TestMethod()]
        public void parseTest()
        {
            CommentComponent target = new CommentComponent();
            Assert.AreEqual(Value.Empty, target.parse("[\"test\"]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        public void parseTest2()
        {
            CommentComponent target = new CommentComponent();
            Assert.AreEqual(Value.Empty, target.parse("[\"line1 \n line2\"]"));
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest3()
        {
            CommentComponent target = new CommentComponent();
            target.parse("#[\"test\"]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest4()
        {
            CommentComponent target = new CommentComponent();
            target.parse("[test]");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest5()
        {
            CommentComponent target = new CommentComponent();
            target.parse("test");
        }

        /// <summary>
        ///A test for parse
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(IncorrectSyntaxException))]
        public void parseTest6()
        {
            CommentComponent target = new CommentComponent();
            target.parse("");
        }
    }
}
