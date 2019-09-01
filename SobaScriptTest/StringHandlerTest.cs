using System;
using System.Text.RegularExpressions;
using net.r_eg.SobaScript;
using Xunit;

namespace SobaScriptTest
{
    public class StringHandlerTest
    {
        [Fact]
        public void NormalizeTest()
        {
            Assert.Equal("\"test\"", StringHandler.normalize("\\\"test\\\""));
        }

        [Fact]
        public void EscapeQuotesTest()
        {
            Assert.Equal(" \\\"test\\\" ", StringHandler.escapeQuotes(" \"test\" "));
        }

        [Fact]
        public void NormalizeTest2()
        {
            Assert.Equal(String.Empty, StringHandler.normalize(null));
        }

        [Fact]
        public void ProtectTest()
        {
            StringHandler target = new StringHandler();
            string actual = target.ProtectMixedQuotes("test \"str1\" - 'str2' data");
            Assert.False(Regex.IsMatch(actual, RPattern.DoubleQuotesContent, RegexOptions.IgnorePatternWhitespace));
            Assert.False(Regex.IsMatch(actual, RPattern.SingleQuotesContent, RegexOptions.IgnorePatternWhitespace));
        }

        [Fact]
        public void RecoveryTest()
        {
            StringHandler target = new StringHandler();
            string str = target.ProtectMixedQuotes("test \"str1\" - 'str2' data");
            Assert.Equal("test \"str1\" - 'str2' data", target.Recovery(str));
        }
    }
}
